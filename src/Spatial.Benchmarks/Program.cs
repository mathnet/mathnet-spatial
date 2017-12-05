// ReSharper disable UnusedMember.Local
namespace Spatial.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using BenchmarkDotNet.Reports;
    using BenchmarkDotNet.Running;

    internal class Program
    {
        private static readonly string BenchmarksDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\");

        private static string ArtifactsDirectory { get; } = Path.Combine(Directory.GetCurrentDirectory(), "BenchmarkDotNet.Artifacts", "results");

        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            var file = Path.Combine(BenchmarksDirectory, "Spatial.Benchmarks.csproj");
            if (!File.Exists(file))
            {
                throw new FileNotFoundException(file);
            }

            foreach (var summary in RunSingle<Vector3DBenchmarks>())
            {
                CopyResult(summary);
            }
        }

        private static IEnumerable<Summary> RunAll()
        {
            var switcher = new BenchmarkSwitcher(typeof(Program).Assembly);
            var summaries = switcher.Run(new[] { "*" });
            return summaries;
        }

        private static IEnumerable<Summary> RunSingle<T>()
        {
            var summaries = new[] { BenchmarkRunner.Run<T>() };
            return summaries;
        }

        private static void CopyResult(Summary summary)
        {
            var sourceFileName = Directory.EnumerateFiles(summary.ResultsDirectoryPath)
                                          .Single(x => x.EndsWith(summary.Title + "-report-github.md"));
            var destinationFileName = Path.Combine(BenchmarksDirectory, summary.Title + ".md");
            Console.WriteLine($"Copy: {sourceFileName} -> {destinationFileName}");
            File.Copy(sourceFileName, destinationFileName, overwrite: true);
        }
    }
}
