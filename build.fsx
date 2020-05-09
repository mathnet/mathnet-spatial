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

let spatialRelease = release "spatial" "Math.NET Spatial" "RELEASENOTES.md"
let releases = [ spatialRelease ]
traceHeader releases


// SPATIAL PACKAGES

let spatialZipPackage = zipPackage "MathNet.Spatial" "Math.NET Spatial" spatialRelease
let spatialStrongNameZipPackage = zipPackage "MathNet.Spatial.Signed" "Math.NET Spatial" spatialRelease

let spatialNuGetPackage = nugetPackage "MathNet.Spatial" spatialRelease
let spatialStrongNameNuGetPackage = nugetPackage "MathNet.Spatial.Signed" spatialRelease

let spatialProject = project "MathNet.Spatial" "src/Spatial/Spatial.csproj" [spatialNuGetPackage]
let spatialStrongNameProject = project "MathNet.Spatial" "src/Spatial/Spatial.Signed.csproj" [spatialStrongNameNuGetPackage]

let spatialSolution = solution "Spatial" "MathNet.Spatial.sln" [spatialProject] [spatialZipPackage]
let spatialStrongNameSolution = solution "Spatial" "MathNet.Spatial.Signed.sln" [spatialStrongNameProject] [spatialStrongNameZipPackage]


// ALL

let allSolutions = [spatialSolution; spatialStrongNameSolution]
let allProjects = allSolutions |> List.collect (fun s -> s.Projects) |> List.distinct


// --------------------------------------------------------------------------------------
// PREPARE
// --------------------------------------------------------------------------------------

Target "Start" DoNothing

Target "Clean" (fun _ ->
    DeleteDirs (!! "src/**/obj/" ++ "src/**/bin/" )
    CleanDirs [ "out/api"; "out/docs" ]
    allSolutions |> List.iter (fun solution -> CleanDirs [ solution.OutputZipDir; solution.OutputNuGetDir; solution.OutputLibDir; solution.OutputLibStrongNameDir ]))

Target "ApplyVersion" (fun _ ->
    allProjects |> List.iter patchVersionInProjectFile
    patchVersionInAssemblyInfo "src/Spatial.Tests" spatialRelease)

Target "Restore" (fun _ -> allSolutions |> List.iter restoreWeak)
"Start"
  =?> ("Clean", not (hasBuildParam "incremental"))
  ==> "Restore"

Target "Prepare" DoNothing
"Start"
  =?> ("Clean", not (hasBuildParam "incremental"))
  ==> "ApplyVersion"
  ==> "Prepare"


// --------------------------------------------------------------------------------------
// BUILD, SIGN, COLLECT
// --------------------------------------------------------------------------------------

let fingerprint = "490408de3618bed0a28e68dc5face46e5a3a97dd"
let timeserver = "http://time.certum.pl/"

Target "Build" (fun _ ->

    // Strong Name Build (with strong name, without certificate signature)
    if hasBuildParam "strongname" then
        CleanDirs (!! "src/**/obj/" ++ "src/**/bin/" )
        restoreStrong spatialStrongNameSolution
        buildStrong spatialStrongNameSolution
        if isWindows && hasBuildParam "sign" then sign fingerprint timeserver spatialStrongNameSolution
        collectBinariesSN spatialStrongNameSolution
        zip spatialStrongNameZipPackage spatialStrongNameSolution.OutputZipDir spatialStrongNameSolution.OutputLibStrongNameDir (fun f -> f.Contains("MathNet.Spatial.") || f.Contains("MathNet.Numerics."))
        if isWindows then
            packStrong spatialStrongNameSolution
            collectNuGetPackages spatialStrongNameSolution

    // Normal Build (without strong name, with certificate signature)
    CleanDirs (!! "src/**/obj/" ++ "src/**/bin/" )
    restoreWeak spatialSolution
    buildWeak spatialSolution
    if isWindows && hasBuildParam "sign" then sign fingerprint timeserver spatialSolution
    collectBinaries spatialSolution
    zip spatialZipPackage spatialSolution.OutputZipDir spatialSolution.OutputLibDir (fun f -> f.Contains("MathNet.Spatial.") || f.Contains("MathNet.Numerics."))
    if isWindows then
        packWeak spatialSolution
        collectNuGetPackages spatialSolution

    // NuGet Sign (all or nothing)
    if isWindows && hasBuildParam "sign" then signNuGet fingerprint timeserver [spatialSolution]

    )
"Prepare" ==> "Build"


// --------------------------------------------------------------------------------------
// TEST
// --------------------------------------------------------------------------------------
let testSpatial framework = test "src/Spatial.Tests" "Spatial.Tests.csproj" framework
Target "TestSpatial" DoNothing
Target "TestSpatialCore3.1" (fun _ -> testSpatial "netcoreapp3.1")
Target "TestSpatialNET461" (fun _ -> testSpatial "net461")
Target "TestSpatialNET47"  (fun _ -> testSpatial "net47")
"Build" ==> "TestSpatialCore3.1" ==> "TestSpatial"
"Build" =?> ("TestSpatialNET461", isWindows) ==> "TestSpatial"
"Build" =?> ("TestSpatialNET47", isWindows)
Target "Test" DoNothing
"TestSpatial" ==> "Test"


// --------------------------------------------------------------------------------------
// BENCHMARKS
// --------------------------------------------------------------------------------------

//let dotnetBuild configuration solution = DotNetCli.Build (fun p ->
//    let defaultArgs = ["--no-restore"]
//    { p with
//        Project = solution
//        Configuration = configuration
//        AdditionalArgs = defaultArgs})

//let CopyBenchmarks target source framework =
//    Directory.GetFiles(source, "*.md", SearchOption.TopDirectoryOnly)
//    |> Seq.iter (fun file ->
//           let fi =
//               file
//               |> replaceFirst source ""
//               |> replaceFirst "\Spatial.Benchmarks." ""
//               |> replaceFirst "Benchmarks-report-github" (sprintf ".%s" framework)
//               |> trimSeparator
//           trace fi
//           let newFile = target @@ fi
//           logVerbosefn "%s => %s" file newFile
//           DirectoryName newFile |> ensureDirectory
//           File.Copy(file, newFile, true))
//    |> ignore

//let benchmarkLibrary framework = testLibrary "src/Spatial.Benchmarks/" "Spatial.Benchmarks.csproj" framework
//let benchmarkRootDir = currentDirectory </> "src" </> "Spatial.Benchmarks"
//let benchmarkSourceFiles = benchmarkRootDir </> "BenchmarkDotNet.Artifacts" </> "results"
//let benchmarkDestination = benchmarkRootDir

//Target "Benchmarks" DoNothing
//Target "RunBenchmarks" DoNothing
//Target "CleanBenchmarks" (fun _ -> CleanDirs [ benchmarkRootDir </> "BenchmarkDotNet.Artifacts" ])

//Target "Benchmark#Core" (fun _ ->
//        benchmarkLibrary "netcoreapp2.0"
//        CopyBenchmarks benchmarkDestination benchmarkSourceFiles "netstandard20")
//Target "Benchmark#Net471" (fun _ ->
//        benchmarkLibrary "net471"
//        CopyBenchmarks benchmarkDestination benchmarkSourceFiles "net471")

//"BuildBenchmarks" ==> "Benchmark#Core" ==> "RunBenchmarks"
//"BuildBenchmarks" =?> ("Benchmark#Net471", isWindows) ==> "RunBenchmarks"

//"RestoreBenchmark"
//==> "CleanBenchmarks"
//==> "BuildBenchmarks"
//==> "RunBenchmarks"
//==> "Benchmarks"


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
    !! "src/Spatial/bin/Release/net461/MathNet.Spatial.dll"
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

Target "PublishDocs" (fun _ -> publishDocs spatialRelease)
Target "PublishApi" (fun _ -> publishApi spatialRelease)

Target "PublishArchive" (fun _ -> publishArchives [spatialSolution; spatialStrongNameSolution])

Target "PublishNuGet" (fun _ -> publishNuGet [spatialSolution; spatialStrongNameSolution])

Target "Publish" DoNothing
Dependencies "Publish" [ "PublishTag"; "PublishDocs"; "PublishApi"; "PublishArchive"; "PublishNuGet" ]


// --------------------------------------------------------------------------------------
// Default Targets
// --------------------------------------------------------------------------------------

Target "All" DoNothing
Dependencies "All" [ "Build"; "Docs"; "Api"; "Test" ]

RunTargetOrDefault "Test"
