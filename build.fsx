//  __  __       _   _       _   _ ______ _______
// |  \/  |     | | | |     | \ | |  ____|__   __|
// | \  / | __ _| |_| |__   |  \| | |__     | |
// | |\/| |/ _` | __| '_ \  | . ` |  __|    | |
// | |  | | (_| | |_| | | |_| |\  | |____   | |
// |_|  |_|\__,_|\__|_| |_(_)_| \_|______|  |_|
//
// Math.NET Spatial - http://spatial.mathdotnet.com
// Copyright (c) Math.NET - Open Source MIT/X11 License
//
// FAKE build script, see http://fsharp.github.io/FAKE
//

// --------------------------------------------------------------------------------------
// PRELUDE
// --------------------------------------------------------------------------------------

#I "packages/FAKE/tools"
#r "packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.DocuHelper
open Fake.AssemblyInfoFile
open Fake.ReleaseNotesHelper
open Fake.StringHelper
open System

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
let header = ReadFile(__SOURCE_DIRECTORY__ @@ "build.fsx") |> Seq.take 10 |> Seq.map (fun s -> s.Substring(2)) |> toLines
trace header


// --------------------------------------------------------------------------------------
// PROJECT INFO
// --------------------------------------------------------------------------------------

// CORE PACKAGES

type Package =
    { Id: string
      Version: string
      Title: string
      Summary: string
      Description: string
      ReleaseNotes: string
      Tags: string
      Authors: string list
      Dependencies: (string*string) list
      Files: (string * string option * string option) list }

type Bundle =
    { Id: string
      Version: string
      Title: string
      ReleaseNotesFile: string
      FsLoader: bool
      Packages: Package list }

let release = LoadReleaseNotes "RELEASENOTES.md"
let buildPart = "0"
let assemblyVersion = release.AssemblyVersion + "." + buildPart
let packageVersion = release.NugetVersion
let releaseNotes = release.Notes |> List.map (fun l -> l.Replace("*","").Replace("`","")) |> toLines
trace (sprintf " Math.NET Spatial  v%s" packageVersion)
trace ""

let summary = "Math.NET Spatial, providing methods and algorithms for geometry computations in science, engineering and every day use."
let description = "Math.NET Spatial. "
let support = "Supports .Net 4.0, .Net 3.5 and Mono on Windows, Linux and Mac; Silverlight 5, WindowsPhone/SL 8, WindowsPhone 8.1 and Windows 8 with PCL Portable Profiles 47 and 344; Android/iOS with Xamarin."
let tags = "math spatial geometry"

let libnet35 = "lib/net35"
let libnet40 = "lib/net40"
let libnet45 = "lib/net45"
let libpcl47 = "lib/portable-net45+sl5+netcore45+MonoAndroid1+MonoTouch1"
let libpcl344 = "lib/portable-net45+sl5+netcore45+wpa81+wp8+MonoAndroid1+MonoTouch1"

let spatialPack =
    { Id = "MathNet.Spatial"
      Version = packageVersion
      Title = "Math.NET Spatial"
      Summary = summary
      Description = description + support
      ReleaseNotes = releaseNotes
      Tags = tags
      Authors = [ "Christoph Ruegg"; "Johan Larsson" ]
      Dependencies = []
      Files = [ @"..\..\out\lib\Net35\MathNet.Spatial.*", Some libnet35, None;
                @"..\..\out\lib\Net45\MathNet.Spatial.*", Some libnet45, None;
                @"..\..\out\lib\Profile47\MathNet.Spatial.*", Some libpcl47, None;
                @"..\..\out\lib\Profile344\MathNet.Spatial.*", Some libpcl344, None;
                @"..\..\src\Spatial\**\*.cs", Some "src/Common", None ] }

let coreBundle =
    { Id = spatialPack.Id
      Version = packageVersion
      Title = spatialPack.Title
      ReleaseNotesFile = "RELEASENOTES.md"
      FsLoader = false
      Packages = [ spatialPack ] }


// --------------------------------------------------------------------------------------
// PREPARE
// --------------------------------------------------------------------------------------

Target "Start" DoNothing

Target "Clean" (fun _ ->
    CleanDirs [ "obj" ]
    CleanDirs [ "out/api"; "out/docs"; "out/packages" ]
    CleanDirs [ "out/lib/Net35"; "out/lib/Net45"; "out/lib/Profile47"; "out/lib/Profile344" ]
    CleanDirs [ "out/test/Net35"; "out/test/Net45"; "out/test/Profile47"; "out/test/Profile344" ])

Target "RestorePackages" RestorePackages

Target "ApplyVersion" (fun _ ->
    let patchAssemblyInfo path assemblyVersion packageVersion =
        BulkReplaceAssemblyInfoVersions path (fun f ->
            { f with
                AssemblyVersion = assemblyVersion
                AssemblyFileVersion = assemblyVersion
                AssemblyInformationalVersion = packageVersion })
    patchAssemblyInfo "src/Spatial" assemblyVersion packageVersion
    patchAssemblyInfo "src/SpatialUnitTests" assemblyVersion packageVersion)

Target "Prepare" DoNothing
"Start"
  =?> ("Clean", not (hasBuildParam "incremental"))
  ==> "RestorePackages"
  ==> "ApplyVersion"
  ==> "Prepare"


// --------------------------------------------------------------------------------------
// BUILD
// --------------------------------------------------------------------------------------

let buildConfig config subject = MSBuild "" (if hasBuildParam "incremental" then "Build" else "Rebuild") [ "Configuration", config ] subject |> ignore
let build subject = buildConfig "Release" subject

Target "BuildMain" (fun _ -> build !! "MathNet.Spatial.sln")
Target "BuildNet35" (fun _ -> build !! "MathNet.Spatial.Net35Only.sln")
Target "BuildAll" (fun _ -> build !! "MathNet.Spatial.All.sln")

Target "Build" DoNothing
"Prepare"
  =?> ("BuildNet35", hasBuildParam "net35")
  =?> ("BuildAll", hasBuildParam "all" || hasBuildParam "release")
  =?> ("BuildMain", not (hasBuildParam "all" || hasBuildParam "release" || hasBuildParam "net35" || hasBuildParam "signed"))
  ==> "Build"


// --------------------------------------------------------------------------------------
// TEST
// --------------------------------------------------------------------------------------

let test target =
    let quick p = if hasBuildParam "quick" then { p with ExcludeCategory="LongRunning" } else p
    NUnit (fun p ->
        { p with
            DisableShadowCopy = true
            TimeOut = TimeSpan.FromMinutes 30.
            OutputFile = "TestResults.xml" } |> quick) target

Target "Test" (fun _ -> test !! "out/test/**/*UnitTests*.dll")
"Build" ==> "Test"


// --------------------------------------------------------------------------------------
// PACKAGES
// --------------------------------------------------------------------------------------

let provideLicense path =
    ReadFileAsString "LICENSE.md"
    |> ConvertTextToWindowsLineBreaks
    |> ReplaceFile (path @@ "license.txt")

let provideReadme title releasenotes path =
    String.concat Environment.NewLine [header; " " + title; ""; ReadFileAsString releasenotes]
    |> ConvertTextToWindowsLineBreaks
    |> ReplaceFile (path @@ "readme.txt")

let provideFsLoader includes path =
    // inspired by FsLab/tpetricek
    let fullScript = ReadFile "src/FSharp/MathNet.Spatial.fsx" |> Array.ofSeq
    let startIndex = fullScript |> Seq.findIndex (fun s -> s.Contains "***MathNet.Spatial.fsx***")
    let extraScript = fullScript .[startIndex + 1 ..] |> List.ofSeq
    let assemblies = [ "MathNet.Spatial.dll" ]
    let nowarn = ["#nowarn \"211\""]
    let references = [ for assembly in assemblies -> sprintf "#r \"%s\"" assembly ]
    ReplaceFile (path @@ "MathNet.Spatial.fsx") (nowarn @ includes @ references @ extraScript |> toLines)

let provideZipExtraFiles path (bundle:Bundle) =
    provideLicense path
    provideReadme (sprintf "%s v%s" bundle.Title bundle.Version) bundle.ReleaseNotesFile path
    if bundle.FsLoader then
        let includes = [ for root in [ ""; "../"; "../../" ] -> sprintf "#I \"%sNet40\"" root ]
        provideFsLoader includes path

let provideNuGetExtraFiles path (bundle:Bundle) (pack:Package) =
    provideLicense path
    provideReadme (sprintf "%s v%s" pack.Title pack.Version) bundle.ReleaseNotesFile path

// ZIP

let zip zipDir filesDir filesFilter bundle =
    CleanDir "obj/Zip"
    let workPath = "obj/Zip/" + bundle.Id
    CopyDir workPath filesDir filesFilter
    provideZipExtraFiles workPath bundle
    Zip "obj/Zip/" (zipDir @@ sprintf "%s-%s.zip" bundle.Id bundle.Version) !! (workPath + "/**/*.*")
    CleanDir "obj/Zip"

Target "Zip" (fun _ ->
    CleanDir "out/packages/Zip"
    if not (hasBuildParam "signed") || hasBuildParam "release" then
        coreBundle |> zip "out/packages/Zip" "out/lib" (fun f -> f.Contains("MathNet.Spatial.")))
"Build" ==> "Zip"


// NUGET

let updateNuspec (pack:Package) outPath symbols updateFiles spec =
    { spec with ToolPath = "tools/NuGet/NuGet.exe"
                OutputPath = outPath
                WorkingDir = "obj/NuGet"
                Version = pack.Version
                ReleaseNotes = pack.ReleaseNotes
                Project = pack.Id
                Title = pack.Title
                Summary = pack.Summary
                Description = pack.Description
                Tags = pack.Tags
                Authors = pack.Authors
                Dependencies = pack.Dependencies
                SymbolPackage = symbols
                Files = updateFiles pack.Files
                Publish = false }

let nugetPack bundle outPath =
    CleanDir "obj/NuGet"
    for pack in bundle.Packages do
        provideNuGetExtraFiles "obj/NuGet" bundle pack
        let withLicenseReadme f = [ "license.txt", None, None; "readme.txt", None, None; ] @ f
        let withoutSymbolsSources f =
            List.choose (function | (_, Some (target:string), _) when target.StartsWith("src") -> None
                                  | (s, t, None) -> Some (s, t, Some ("**/*.pdb"))
                                  | (s, t, Some e) -> Some (s, t, Some (e + ";**/*.pdb"))) f
        // first pass - generates symbol + normal package. NuGet does drop the symbols from the normal package, but unfortunately not the sources.
        NuGet (updateNuspec pack outPath NugetSymbolPackage.Nuspec withLicenseReadme) "build/MathNet.Spatial.nuspec"
        // second pass - generate only normal package, again, but this time explicitly drop the sources (and the debug symbols)
        NuGet (updateNuspec pack outPath NugetSymbolPackage.None (withLicenseReadme >> withoutSymbolsSources)) "build/MathNet.Spatial.nuspec"
        CleanDir "obj/NuGet"

let nugetPackExtension bundle outPath =
    CleanDir "obj/NuGet"
    for pack in bundle.Packages do
        provideNuGetExtraFiles "obj/NuGet" bundle pack
        let withLicenseReadme f = [ "license.txt", None, None; "readme.txt", None, None; ] @ f
        NuGet (updateNuspec pack outPath NugetSymbolPackage.None withLicenseReadme) "build/MathNet.Spatial.Extension.nuspec"
        CleanDir "obj/NuGet"

Target "NuGet" (fun _ ->
    CleanDir "out/packages/NuGet"
    if hasBuildParam "all" || hasBuildParam "release" then
        nugetPack coreBundle "out/packages/NuGet")
"Build" ==> "NuGet"


// --------------------------------------------------------------------------------------
// Documentation
// --------------------------------------------------------------------------------------

// DOCS

Target "CleanDocs" (fun _ -> CleanDirs ["out/docs"])

Target "Docs" (fun _ ->
    executeFSIWithArgs "docs/tools" "build-docs.fsx" ["--define:RELEASE"] [] |> ignore
)

Target "DocsDev" (fun _ ->
    executeFSIWithArgs "docs/tools" "build-docs.fsx" [] [] |> ignore
)

"Build" ==> "CleanDocs" ==> "Docs"

"Start"
  =?> ("CleanDocs", not (hasBuildParam "incremental"))
  ==> "DocsDev"


// API REFERENCE

Target "CleanApi" (fun _ -> CleanDirs ["out/api"])

Target "Api" (fun _ ->
    !! "out/lib/Net45/MathNet.Spatial.dll"
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

let publishReleaseTag title prefix version notes =
    // inspired by Deedle/tpetricek
    let tagName = prefix + "v" + version
    let tagMessage = String.concat Environment.NewLine [title + " v" + version; ""; notes ]
    let cmd = sprintf """tag -a %s -m "%s" """ tagName tagMessage
    Git.CommandHelper.runSimpleGitCommand "." cmd |> printfn "%s"
    let _, remotes, _ = Git.CommandHelper.runGitCommand "." "remote -v"
    let main = remotes |> Seq.find (fun s -> s.Contains("(push)") && s.Contains("mathnet/mathnet-spatial"))
    let remoteName = main.Split('\t').[0]
    Git.Branches.pushTag "." remoteName tagName

Target "PublishTag" (fun _ -> publishReleaseTag "Math.NET Spatial" "" packageVersion releaseNotes)

Target "PublishDocs" (fun _ ->
    let repo = "../mathnet-websites"
    Git.Branches.pull repo "origin" "master"
    CleanDir "../mathnet-websites/spatial/docs"
    CopyRecursive "out/docs" "../mathnet-websites/spatial/docs" true |> printfn "%A"
    Git.Staging.StageAll repo
    Git.Commit.Commit repo (sprintf "Spatial: %s docs update" packageVersion)
    Git.Branches.pushBranch repo "origin" "master")

Target "PublishApi" (fun _ ->
    let repo = "../mathnet-websites"
    Git.Branches.pull repo "origin" "master"
    CleanDir "../mathnet-websites/spatial/api"
    CopyRecursive "out/api" "../mathnet-websites/spatial/api" true |> printfn "%A"
    Git.Staging.StageAll repo
    Git.Commit.Commit repo (sprintf "Spatial: %s api update" packageVersion)
    Git.Branches.pushBranch repo "origin" "master")

let publishNuGet packageFiles =
    // TODO: Migrate to NuGet helper once it supports direct (non-integrated) operations
    let rec impl trials file =
        trace ("NuGet Push: " + System.IO.Path.GetFileName(file) + ".")
        try
            let args = sprintf "push \"%s\"" (FullName file)
            let result =
                ExecProcess (fun info ->
                    info.FileName <- "tools/NuGet/NuGet.exe"
                    info.WorkingDirectory <- FullName "obj/NuGet"
                    info.Arguments <- args) (TimeSpan.FromMinutes 10.)
            if result <> 0 then failwith "Error during NuGet push."
        with exn ->
            if trials > 0 then impl (trials-1) file
            else ()
    Seq.iter (impl 3) packageFiles

Target "PublishNuGet" (fun _ -> !! "out/packages/NuGet/*.nupkg" -- "out/packages/NuGet/*.symbols.nupkg" |> publishNuGet)

Target "Publish" DoNothing
"PublishTag" ==> "Publish"
"PublishNuGet" ==> "Publish"
"PublishDocs" ==> "Publish"
"PublishApi" ==> "Publish"


// --------------------------------------------------------------------------------------
// Default Targets
// --------------------------------------------------------------------------------------

Target "All" DoNothing
"Build" ==> "All"
"Zip" ==> "All"
"NuGet" ==> "All"
"Docs" ==> "All"
"Api" ==> "All"
"Test" ==> "All"

RunTargetOrDefault "Test"
