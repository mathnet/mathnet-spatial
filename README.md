Math.NET Spatial
================

[![Join the chat at https://gitter.im/mathnet/mathnet-spatial](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/mathnet/mathnet-spatial?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Math.NET Spatial is an opensource **geometry library for .Net, Silverlight and Mono**.

Supports Mono and .Net Framework 4.6.1 or higher and .Net Standard 2.0 or higher on Linux, Mac and Windows.

Math.NET Spatial is covered under the terms of the [MIT/X11](LICENSE.md) license. You may therefore link to it and use it in both opensource and proprietary software projects.

**[Release Notes & Changes](RELEASENOTES.md)**

Installation Instructions
-------------------------

The recommended way to get Math.NET Spatial is to use NuGet. The following packages are provided and maintained in the public [NuGet Gallery](https://nuget.org/profiles/mathnet/):

- **MathNet.Spatial** - core package.

Supported Platforms:

- .Net 4.0 and Mono: Windows, Linux and Mac.

Building Math.NET Spatial
-------------------------

Windows (.Net): [![AppVeyor build status](https://ci.appveyor.com/api/projects/status/b0v856pd85i6a3hg/branch/master)](https://ci.appveyor.com/project/cdrnet/mathnet-spatial)

If you do not want to use the official binaries, or if you like to modify, debug or contribute, you can compile Math.NET Spatial locally either using Visual Studio or manually with the build scripts.

* The Visual Studio solutions should build out of the box, without any preparation steps or package restores.
* Instead of a compatible IDE you can also build the solutions with `msbuild`, or on Mono with `xbuild`.
* The full build including unit tests, docs, NuGet and Zip packages is using [FAKE](https://fsharp.github.io/FAKE/).

### How to build with MSBuild/XBuild

    restore.cmd (or restore.sh)
    dotnet build MathNet.Spatial.sln       # build with .Net SDK tools
    msbuild MathNet.Spatial.sln            # build with MsBuild
    xbuild MathNet.Spatial.sln             # build with Mono, e.g. on Linux or Mac

### How to build with FAKE

    build.cmd    # normal build (.Net 4.0), run unit tests
    ./build.sh   # normal build (.Net 4.0), run unit tests - on Linux or Mac
    ./buildn.sh  # normal build (.Net 4.0), run unit tests - bash on Windows (.Net instead of mono)

    build.cmd Build              # normal build
    build.cmd Build incremental  # normal build, incremental

    build.cmd Test        # normal build, run unit tests
    build.cmd Test quick  # normal build, run unit tests except long running ones

    build.cmd Clean  # cleanup build artifacts
    build.cmd Docs   # generate documentation
    build.cmd Api    # generate api reference

    build.cmd All          # build, test, docs, api reference

FAKE itself is not included in the repository but it will download and bootstrap itself automatically when build.cmd is run the first time. Note that this step is *not* required when using Visual Studio or `msbuild` directly.

Quick Links
-----------

* [**Project Website**](https://spatial.mathdotnet.com)
* [Source Code](https://github.com/mathnet/mathnet-spatial)
* [Documentation](https://spatial.mathdotnet.com/docs/)
* [API Reference](https://spatial.mathdotnet.com/api/)
* [Work Items and Bug Tracker](https://github.com/mathnet/mathnet-spatial/issues)

Math.NET on other sites:

* [Twitter @MathDotNet](https://twitter.com/MathDotNet)
* [Google+](https://plus.google.com/112484567926928665204)
* [Ohloh](https://www.ohloh.net/p/mathnet)
* [Stack Overflow](https://stackoverflow.com/questions/tagged/mathdotnet)
