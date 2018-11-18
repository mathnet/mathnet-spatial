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
        [ { FrameworkVersion="net40"
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
        [ { FrameworkVersion="net40"
            Dependencies=[ "MathNet.Numerics.Signed", GetPackageVersion "./packages/mathnet/" "MathNet.Numerics.Signed" ] } ]
      Files =
        [ @"..\..\out\lib\Net40\MathNet.Spatial.*", Some libnet40, None;
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
    CleanDirs [ "src/Spatial/bin"; "src/Spatial.Tests/bin" ]
    CleanDirs [ "src/Spatial/obj"; "src/Spatial.Tests/obj" ]
    CleanDirs [ "obj" ]
    CleanDirs [ "out/api"; "out/docs"; "out/packages" ]
    CleanDirs [ "out/lib" ]
    CleanDirs [ "out/test/Net40" ]
    clean "MathNet.SpatialMinimal.sln")


Target "ApplyVersion" (fun _ ->
    patchVersionInProjectFile "src/Spatial/Spatial.csproj" spatialRelease
    patchVersionInProjectFile "src/Spatial.Tests/Spatial.Tests.csproj" spatialRelease)

Target "Restore" (fun _ ->
    restore "MathNet.SpatialMinimal.sln")

Target "RestoreBenchmark" (fun _ ->
    clean "MathNet.Spatial.Benchmarks.sln"
    restore "MathNet.Spatial.Benchmarks.sln")

Target "Prepare" DoNothing
"Start"
  =?> ("Clean", not (hasBuildParam "incremental"))
  ==> "Restore"
  ==> "ApplyVersion"
  ==> "Prepare"


// --------------------------------------------------------------------------------------
// BUILD
// --------------------------------------------------------------------------------------

Target "Build" (fun _ -> build "MathNet.SpatialMinimal.sln")
"Prepare" ==> "Build"

Target "BuildBenchmarks" (fun _ -> build "MathNet.Spatial.Benchmarks.sln")



// --------------------------------------------------------------------------------------
// TEST
// --------------------------------------------------------------------------------------

let testLibrary testsDir testsProj framework =
    DotNetCli.RunCommand
        (fun c -> { c with WorkingDir = testsDir})
        (sprintf "run -p %s --configuration Release --framework %s --no-restore --no-build"
            testsProj
            framework)

let testSpatial framework = testLibrary "src/Spatial.Tests" "Spatial.Tests.csproj" framework
Target "TestSpatial" DoNothing
Target "TestSpatialCore2.1" (fun _ -> testSpatial "netcoreapp2.1")
Target "TestSpatialNET40" (fun _ -> testSpatial "net40")
Target "TestSpatialNET45" (fun _ -> testSpatial "net45")
Target "TestSpatialNET461" (fun _ -> testSpatial "net461")
Target "TestSpatialNET47"  (fun _ -> testSpatial "net47")

"Build" ==> "TestSpatialCore2.1" ==> "TestSpatial"
"Build" =?> ("TestSpatialNET40", isWindows)
"Build" =?> ("TestSpatialNET45", isWindows)
"Build" =?> ("TestSpatialNET461", isWindows) ==> "TestSpatial"
"Build" =?> ("TestSpatialNET47", isWindows)
Target "Test" DoNothing
"TestSpatial" ==> "Test"

// --------------------------------------------------------------------------------------
// BENCHMARKS
// --------------------------------------------------------------------------------------

let dotnetBuild configuration solution = DotNetCli.Build (fun p ->
    let defaultArgs = ["--no-restore"]
    { p with
        Project = solution
        Configuration = configuration
        AdditionalArgs = defaultArgs})

let CopyBenchmarks target source framework =
    Directory.GetFiles(source, "*.md", SearchOption.TopDirectoryOnly)
    |> Seq.iter (fun file ->
           let fi =
               file
               |> replaceFirst source ""
               |> replaceFirst "\Spatial.Benchmarks." ""
               |> replaceFirst "Benchmarks-report-github" (sprintf ".%s" framework)
               |> trimSeparator
           trace fi
           let newFile = target @@ fi
           logVerbosefn "%s => %s" file newFile
           DirectoryName newFile |> ensureDirectory
           File.Copy(file, newFile, true))
    |> ignore

let benchmarkLibrary framework = testLibrary "src/Spatial.Benchmarks/" "Spatial.Benchmarks.csproj" framework
let benchmarkRootDir = currentDirectory </> "src" </> "Spatial.Benchmarks"
let benchmarkSourceFiles = benchmarkRootDir </> "BenchmarkDotNet.Artifacts" </> "results"
let benchmarkDestination = benchmarkRootDir

Target "Benchmarks" DoNothing
Target "RunBenchmarks" DoNothing
Target "CleanBenchmarks" (fun _ -> CleanDirs [ benchmarkRootDir </> "BenchmarkDotNet.Artifacts" ])

Target "Benchmark#Core" (fun _ ->
        benchmarkLibrary "netcoreapp2.0"
        CopyBenchmarks benchmarkDestination benchmarkSourceFiles "netstandard20")
Target "Benchmark#Net471" (fun _ ->
        benchmarkLibrary "net471"
        CopyBenchmarks benchmarkDestination benchmarkSourceFiles "net471")

"BuildBenchmarks" ==> "Benchmark#Core" ==> "RunBenchmarks"
"BuildBenchmarks" =?> ("Benchmark#Net471", isWindows) ==> "RunBenchmarks"

"RestoreBenchmark"
==> "CleanBenchmarks"
==> "BuildBenchmarks"
==> "RunBenchmarks"
==> "Benchmarks"


// --------------------------------------------------------------------------------------
// CODE SIGN
// --------------------------------------------------------------------------------------

Target "Sign" (fun _ ->
    let fingerprint = "490408de3618bed0a28e68dc5face46e5a3a97dd"
    let timeserver = "http://time.certum.pl/"
    sign fingerprint timeserver (!! "src/Spatial/bin/Release/**/MathNet.Spatial.dll"))

// --------------------------------------------------------------------------------------
// PACKAGES
// --------------------------------------------------------------------------------------

Target "Pack" DoNothing

// COLLECT

Target "Collect" (fun _ ->
    // It is important that the libs have been signed before we collect them (that's why we cannot copy them right after the build)
    CopyDir "out/lib" "src/Spatial/bin/Release" (fun n -> n.Contains("MathNet.Spatial.dll") || n.Contains("MathNet.Spatial.pdb") || n.Contains("MathNet.Spatial.xml")))
"Build" =?> ("Sign", hasBuildParam "sign") ==> "Collect"


// ZIP

Target "Zip" (fun _ ->
    CleanDir "out/packages/Zip"
    coreBundle |> zip "out/packages/Zip" "out/lib" (fun f -> f.Contains("MathNet.Spatial.") || f.Contains("MathNet.Numerics.")))
"Collect" ==> "Zip" ==> "Pack"


// NUGET

Target "NuGet" (fun _ ->
    pack "MathNet.SpatialMinimal.sln"
    CopyDir "out/packages/NuGet" "src/Spatial/bin/Release/" (fun n -> n.EndsWith(".nupkg")))
"Collect" ==> "NuGet" ==> "Pack"


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
    provideDocExtraFiles  extraDocs releases
    generateDocs true true)
Target "DocsWatch" (fun _ ->
    provideDocExtraFiles  extraDocs releases
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
    !! "src/Spatial/bin/Release/net40/MathNet.Spatial.dll"
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
