//  __  __       _   _       _   _ ______ _______
// |  \/  |     | | | |     | \ | |  ____|__   __|
// | \  / | __ _| |_| |__   |  \| | |__     | |
// | |\/| |/ _` | __| '_ \  | . ` |  __|    | |
// | |  | | (_| | |_| | | |_| |\  | |____   | |
// |_|  |_|\__,_|\__|_| |_(_)_| \_|______|  |_|
//
// Math.NET Spatial - https://spatial.mathdotnet.com
// Copyright (c) Math.NET - Open Source MIT/X11 License
//
// FAKE build script, see http://fsharp.github.io/FAKE
//

// --------------------------------------------------------------------------------------
// PRELUDE
// --------------------------------------------------------------------------------------

#I "packages/build/FAKE/tools"
#r "packages/build/FAKE/tools/FakeLib.dll"

open Fake
open Fake.DocuHelper
open System
open System.IO

#load "build/build-framework.fsx"
open BuildFramework

let dotnetbuild = environVarOrDefault "DOTNET" "0"

// --------------------------------------------------------------------------------------
// PROJECT INFO
// --------------------------------------------------------------------------------------

// VERSION OVERVIEW

let spatialRelease = release "Math.NET Spatial" "RELEASENOTES.md"
let releases = [ spatialRelease ]
traceHeader releases


// CORE PACKAGES

let summary = "Math.NET Spatial, providing methods and algorithms for geometry computations in science, engineering and every day use."
let description = "Math.NET Spatial. "
let support = "Supports .Net 4.0 and Mono on Windows, Linux and Mac."
let supportSigned = "Supports .Net 4.0. This package contains strong-named assemblies for legacy use cases."
let tags = "math spatial geometry 2D 3D"

let spatialPack =
    { Id = "MathNet.Spatial"
      Release = spatialRelease
      Title = "Math.NET Spatial"
      Summary = summary
      Description = description + support
      Tags = tags
      Authors = [ "Christoph Ruegg"; "Johan Larsson" ]
      FsLoader = false
      Dependencies =
        [ { FrameworkVersion=""
            Dependencies=[ "MathNet.Numerics", GetPackageVersion "./packages/mathnet/" "MathNet.Numerics"] } ]
      Files =
        [ @"..\..\out\lib\Net40\MathNet.Spatial.*", Some libnet40, None;
          @"..\..\out\lib\netstandard2.0\MathNet.Spatial.*", Some netstandard20, None;
          @"..\..\src\Spatial\**\*.cs", Some "src/Common", None ] }

let spatialSignedPack =
  { spatialPack with
      Id = spatialPack.Id + ".Signed"
      Title = spatialPack.Title + " - Signed Edition"
      Description = description + supportSigned
      Tags = spatialPack.Tags + " signed"
      Dependencies =
        [ { FrameworkVersion=""
            Dependencies=[ "MathNet.Numerics.Signed", GetPackageVersion "./packages/mathnet/" "MathNet.Numerics.Signed" ] } ]
      Files =
        [ @"..\..\out\lib-signed\Net40\MathNet.Spatial.*", Some libnet40, None;
          @"..\..\src\Spatial\**\*.cs", Some "src/Common", None ] }

let coreBundle =
    { Id = spatialPack.Id
      Release = spatialRelease
      Title = spatialPack.Title
      Packages = [ spatialPack ] }

let coreSignedBundle =
    { Id = spatialSignedPack.Id
      Release = spatialRelease
      Title = spatialSignedPack.Title
      Packages = [ spatialSignedPack ] }


// --------------------------------------------------------------------------------------
// PREPARE
// --------------------------------------------------------------------------------------

Target "Start" DoNothing

Target "Clean" (fun _ ->
    DotNetCli.RunCommand id "clean MathNet.SpatialMinimal.sln"
    CleanDirs [ "obj" ]
    CleanDirs [ "out/api"; "out/docs"; "out/packages" ]
    CleanDirs [ "out/lib/Net40" ]
    CleanDirs [ "out/lib/netstandard2.0" ]
    CleanDirs [ "out/lib/netstandard1.3" ]
    CleanDirs [ "out/test/Net40" ]
    CleanDirs [ "out/lib-signed/Net40" ])

Target "CleanDotnet" (fun _ -> DotNetCli.RunCommand id "clean MathNet.Spatial.Benchmarks.sln")

Target "ApplyVersion" (fun _ ->
    patchVersionInAssemblyInfo "src/Spatial" spatialRelease
    patchVersionInAssemblyInfo "src/SpatialUnitTests" spatialRelease)

let dotnetRestore project = DotNetCli.Restore (fun c ->
       { c with 
           Project = project 
           NoCache = true })

Target "DotnetRestore" (fun _ -> dotnetRestore "MathNet.SpatialMinimal.sln")

Target "DotnetRestoreBenchmark" (fun _ -> dotnetRestore "MathNet.Spatial.Benchmarks.sln")

Target "Prepare" DoNothing
"Start"
  =?> ("Clean", not (hasBuildParam "incremental"))
  =?> ("CleanDotnet",  dotnetbuild = "1")
  ==> "DotnetRestore"
  =?> ("DotnetRestoreBenchmark",  dotnetbuild = "1")
  ==> "ApplyVersion"
  ==> "Prepare"

// --------------------------------------------------------------------------------------
// BUILD
// --------------------------------------------------------------------------------------

let dotnetBuild configuration solution = DotNetCli.Build (fun p ->
    let defaultArgs = ["--no-restore"]
    { p with
        Project = solution
        Configuration = configuration
        AdditionalArgs = defaultArgs})

Target "BuildMain" (fun _ -> dotnetBuild "Release" "MathNet.SpatialMinimal.sln")

Target "BuildBenchmarks" (fun _ -> dotnetBuild "Release" "MathNet.Spatial.Benchmarks.sln")

//Target "BuildMain" (fun _ -> build !! "MathNet.Spatial.sln")
//Target "BuildAll" (fun _ -> build !! "MathNet.Spatial.All.sln")

Target "Build" DoNothing
"Prepare"
  =?> ("BuildMain", hasBuildParam "release")
  =?> ("BuildBenchmarks",  dotnetbuild = "1")
  ==> "Build"


// --------------------------------------------------------------------------------------
// TEST
// --------------------------------------------------------------------------------------

let testLibrary testsDir testsProj framework =
    DotNetCli.RunCommand
        (fun c -> { c with WorkingDir = testsDir})
        (sprintf "run -p %s --configuration Release --framework %s"
            testsProj
            framework)

let testLibraryCsharp framework = testLibrary "src/SpatialUnitTests" "SpatialUnitTests.csproj" framework

Target "Test" DoNothing
Target "TestC#" DoNothing

Target "TestC#Core1.1" (fun _ -> testLibraryCsharp "netcoreapp1.1")
Target "TestC#Core2.0" (fun _ -> testLibraryCsharp "netcoreapp2.0")
Target "TestC#NET40" (fun _ -> testLibraryCsharp "net40")
Target "TestC#NET45" (fun _ -> testLibraryCsharp "net45")
Target "TestC#NET46" (fun _ -> testLibraryCsharp "net46")
Target "TestC#NET47"  (fun _ -> testLibraryCsharp "net47")

"Build" ==> "TestC#Core1.1" ==> "TestC#"
"Build" ==> "TestC#Core2.0" ==> "TestC#"
"Build"
=?> ("TestC#NET45", dotnetbuild = "1")
==> "TestC#"

"TestC#" ==> "Test"

//Target "Test" (fun _ -> test !! "out/test/**/*UnitTests*.dll")
//"Build" ==> "Test"

// --------------------------------------------------------------------------------------
// BENCHMARKS
// --------------------------------------------------------------------------------------

let benchmarkLibrary framework = testLibrary "" "src/Spatial.Benchmarks/Spatial.Benchmarks.csproj" framework

Target "Benchmarks" DoNothing
Target "RunBenchmarks" DoNothing
Target "CleanBenchmarks" (fun _ -> CleanDirs [ "BenchmarkDotNet.Artifacts" ])

Target "Benchmark#Core" (fun _ ->
        benchmarkLibrary "netcoreapp2.0"
        Rename "BenchmarkDotNet.Artifacts/netcoreapp2.0" "BenchmarkDotNet.Artifacts/results")
Target "Benchmark#Net47" (fun _ ->
        benchmarkLibrary "net47"
        Rename "BenchmarkDotNet.Artifacts/net47" "BenchmarkDotNet.Artifacts/results")

"BuildBenchmarks" ==> "Benchmark#Core" ==> "RunBenchmarks"
"BuildBenchmarks" =?> ("Benchmark#Net47", dotnetbuild = "1") ==> "RunBenchmarks"

"DotnetRestoreBenchmark"
==> "CleanBenchmarks"
==> "BuildBenchmarks"
==> "RunBenchmarks"
==> "Benchmarks"

// --------------------------------------------------------------------------------------
// PACKAGES
// --------------------------------------------------------------------------------------

Target "Pack" DoNothing

// ZIP

Target "Zip" (fun _ ->
    CleanDir "out/packages/Zip"
    coreBundle |> zip "out/packages/Zip" "out/lib" (fun f -> f.Contains("MathNet.Spatial.") || f.Contains("MathNet.Numerics."))
    coreSignedBundle |> zip "out/packages/Zip" "out/lib-signed" (fun f -> f.Contains("MathNet.Spatial.") || f.Contains("MathNet.Numerics.")))
"Build" ==> "Zip" ==> "Pack"

// NUGET

let dotnetPack solution = DotNetCli.Pack (fun p ->
    let defaultArgs = ["--no-restore"; "--no-build" ]
    { p with
        Project = solution
        Configuration = "Release"
        AdditionalArgs = defaultArgs})

Target "NuGet" (fun _ ->
    dotnetPack "MathNet.Spatial.sln")
"Build" ==> "NuGet" ==> "Pack"


// --------------------------------------------------------------------------------------
// Documentation
// --------------------------------------------------------------------------------------

// DOCS

Target "CleanDocs" (fun _ -> CleanDirs ["out/docs"])

let extraDocs =
    [ "LICENSE.md", "License.md"
      "CONTRIBUTING.md", "Contributing.md"
      "CONTRIBUTORS.md", "Contributors.md" ]

Target "Docs" (fun _ ->
    provideDocExtraFiles extraDocs releases
    generateDocs true false)
Target "DocsDev" (fun _ ->
    provideDocExtraFiles extraDocs releases
    generateDocs true true)
Target "DocsWatch" (fun _ ->
    provideDocExtraFiles extraDocs releases
    use watcher = new FileSystemWatcher(DirectoryInfo("docs/content").FullName, "*.*")
    watcher.EnableRaisingEvents <- true
    watcher.Changed.Add(fun e -> generateDocs false true)
    watcher.Created.Add(fun e -> generateDocs false true)
    watcher.Renamed.Add(fun e -> generateDocs false true)
    watcher.Deleted.Add(fun e -> generateDocs false true)
    traceImportant "Waiting for docs edits. Press any key to stop."
    System.Console.ReadKey() |> ignore
    watcher.EnableRaisingEvents <- false
    watcher.Dispose())

"Build" ==> "CleanDocs" ==> "Docs"

"Start"
  =?> ("CleanDocs", not (hasBuildParam "incremental"))
  ==> "DocsDev"
  ==> "DocsWatch"


// API REFERENCE

Target "CleanApi" (fun _ -> CleanDirs ["out/api"])

Target "Api" (fun _ ->
    !! "out/lib/Net40/MathNet.Spatial.dll"
    |> Docu (fun p ->
        { p with
            ToolPath = "tools/docu/docu.exe"
            TemplatesPath = "tools/docu/templates/"
            TimeOut = TimeSpan.FromMinutes 10.
            OutputPath = "out/api/" }))

"Build" ==> "CleanApi" ==> "Api"


// --------------------------------------------------------------------------------------
// Publishing
// Requires permissions; intended only for maintainers
// --------------------------------------------------------------------------------------

Target "PublishTag" (fun _ -> publishReleaseTag "Math.NET Spatial" "" spatialRelease)

Target "PublishMirrors" (fun _ -> publishMirrors ())
Target "PublishDocs" (fun _ -> publishDocs spatialRelease)
Target "PublishApi" (fun _ -> publishApi spatialRelease)

Target "PublishArchive" (fun _ -> publishArchive "out/packages/Zip" "out/packages/NuGet" [coreBundle; coreSignedBundle])

Target "PublishNuGet" (fun _ -> !! "out/packages/NuGet/*.nupkg" -- "out/packages/NuGet/*.symbols.nupkg" |> publishNuGet)

Target "Publish" DoNothing
Dependencies "Publish" [ "PublishTag"; "PublishDocs"; "PublishApi"; "PublishArchive"; "PublishNuGet" ]


// --------------------------------------------------------------------------------------
// Default Targets
// --------------------------------------------------------------------------------------

Target "All" DoNothing
Dependencies "All" [ "Pack"; "Docs"; "Api"; "Test" ]

RunTargetOrDefault "Test"
