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
            this.Add(DefaultConfig.Instance.GetLoggers().ToArray());
            this.Add(DefaultConfig.Instance.GetValidators().ToArray());
            this.Add(DefaultConfig.Instance.GetHardwareCounters().ToArray());
            this.Add(DefaultConfig.Instance.GetDiagnosers().ToArray());
            this.Add(DefaultConfig.Instance.GetColumnProviders().ToArray());
            this.Add(MarkdownExporter.GitHub);
#if NET461
            this.Add(Job.Default
                .With(Platform.X86)
                .With(Jit.LegacyJit)
                .With(Runtime.Clr));
            this.Add(Job.Default
                .With(Platform.X64)
                .With(Jit.LegacyJit)
                .With(Runtime.Clr));
            this.Add(Job.Default
                .With(Platform.X64)
                .With(Jit.RyuJit)
                .With(Runtime.Clr));
#endif
#if NETCOREAPP2_2
            this.Add(Job.Default
                .With(Platform.X64)
                .With(Jit.RyuJit)
                .With(Runtime.Core));
#endif
        }
    }
}
