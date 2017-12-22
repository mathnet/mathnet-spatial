// ReSharper disable UnusedMember.Local
namespace Spatial.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Reports;
    using BenchmarkDotNet.Running;

    internal class Program
    {
        private static string ArtifactsDirectory { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "BenchmarkDotNet.Artifacts", "results");

        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            var config = new SpatialConfig();

            if (args.Length > 0)
            {
                RunAll(config, args);
            }
            else
            {
                RunAll(config);
            }
        }

        private static IEnumerable<Summary> RunAll(IConfig config)
        {
            var switcher = new BenchmarkSwitcher(typeof(Program).Assembly);
            var summaries = switcher.Run(new[] { "*" }, config);
            return summaries;
        }

        private static IEnumerable<Summary> RunAll(IConfig config, string[] args)
        {
            var switcher = new BenchmarkSwitcher(typeof(Program).Assembly);
            var summaries = switcher.Run(args, config);
            return summaries;
        }

        private static IEnumerable<Summary> RunSingle<T>()
        {
            var summaries = new[] { BenchmarkRunner.Run<T>() };
            return summaries;
        }
    }
}
