using System.Linq;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

namespace Spatial.Benchmarks
{
    internal class SpatialConfig : ManualConfig
    {
        public SpatialConfig()
        {
            this.AddLogger(DefaultConfig.Instance.GetLoggers().ToArray());
            this.AddValidator(DefaultConfig.Instance.GetValidators().ToArray());
            this.AddHardwareCounters(DefaultConfig.Instance.GetHardwareCounters().ToArray());
            this.AddDiagnoser(DefaultConfig.Instance.GetDiagnosers().ToArray());
            this.AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray());
            this.AddExporter(MarkdownExporter.GitHub);
#if NET461
            this.AddJob(Job.Default
                .WithPlatform(Platform.X86)
                .WithJit(Jit.LegacyJit)
                .WithRuntime(ClrRuntime.Net461));
            this.AddJob(Job.Default
                .WithPlatform(Platform.X64)
                .WithJit(Jit.LegacyJit)
                .WithRuntime(ClrRuntime.Net461));
            this.AddJob(Job.Default
                .WithPlatform(Platform.X64)
                .WithJit(Jit.RyuJit)
                .WithRuntime(ClrRuntime.Net461));
#endif
#if NETCOREAPP3_1
            this.AddJob(Job.Default
                .WithPlatform(Platform.X64)
                .WithJit(Jit.RyuJit)
                .WithRuntime(CoreRuntime.Core31));
#endif
        }
    }
}
