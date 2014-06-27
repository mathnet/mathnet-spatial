(*** hide ***)
#I "../../out/lib/net40"

(**
Getting Started
===============

NuGet Packages
--------------

The recommended way to get Math.NET Spatial is to use NuGet. The following packages are provided and maintained in the public [NuGet Gallery](https://nuget.org/profiles/mathnet/):

- **MathNet.Spatial** - core package, including .Net 4, .Net 3.5 and portable/PCL builds.

Supported Platforms:

- .Net 4.0, .Net 3.5 and Mono: Windows, Linux and Mac.
- PCL Portable Profiles 47 and 344: Windows 8, Silverlight 5, Windows Phone/SL 8, Windows Phone 8.1.
- Xamarin: Android, iOS


Using Math.NET Spatial with C#
------------------------------

Building Math.NET Spatial
-------------------------

If you do not want to use the official binaries, or if you like to modify, debug or contribute, you can compile Math.NET Spatial locally either using Visual Studio or manually with the build scripts.

* The Visual Studio solutions should build out of the box, without any preparation steps or package restores.
* Instead of a compatible IDE you can also build the solutions with `msbuild`, or on Mono with `xbuild`.
* The full build including unit tests, docs, NuGet and Zip packages is using [FAKE](http://fsharp.github.io/FAKE/).

### How to build with MSBuild/XBuild

    [lang=sh]
    msbuild MathNet.Spatial.sln             # only build for .Net 4 (main solution)
    msbuild MathNet.Spatial.Net35Only.sln  # only build for .Net 3.5
    msbuild MathNet.Spatial.All.sln        # full build with .Net 4, 3.5 and PCL profiles
    xbuild MathNet.Spatial.sln             # build with Mono, e.g. on Linux or Mac

### How to build with FAKE

    [lang=sh]
    build.cmd    # normal build (.Net 4.0), run unit tests
    ./build.sh   # normal build (.Net 4.0), run unit tests - on Linux or Mac
    ./buildn.sh  # normal build (.Net 4.0), run unit tests - bash on Windows (.Net instead of mono)
    
    build.cmd Build              # normal build (.Net 4.0)
    build.cmd Build incremental  # normal build, incremental (.Net 4.0)
    build.cmd Build all          # full build (.Net 4.0, 3.5, PCL)
    build.cmd Build net35        # compatibility build (.Net 3.5
    
    build.cmd Test        # normal build (.Net 4.0), run unit tests
    build.cmd Test quick  # normal build (.Net 4.0), run unit tests except long running ones
    build.cmd Test all    # full build (.Net 4.0, 3.5, PCL), run all unit tests
    build.cmd Test net35  # compatibility build (.Net 3.5), run unit tests
    
    build.cmd Clean  # cleanup build artifacts
    build.cmd Docs   # generate documentation
    build.cmd Api    # generate api reference
    
    build.cmd NuGet all     # generate normal NuGet packages (.Net 4.0, 3.5, PCL)
    build.cmd NuGet signed  # generate signed/strong named NuGet packages (.Net 4.0)

    build.cmd All          # build, test, docs, api reference (.Net 4.0)
    build.cmd All release  # release build

FAKE itself is not included in the repository but it will download and bootstrap itself automatically when build.cmd is run the first time. Note that this step is *not* required when using Visual Studio or `msbuild` directly.

If the build or tests fail claiming that FSharp.Core was not be found, see [fsharp.org](http://fsharp.org/use/windows/) or install the [Visual F# 3.0 Tools](http://go.microsoft.com/fwlink/?LinkId=261286) directly.

*)
