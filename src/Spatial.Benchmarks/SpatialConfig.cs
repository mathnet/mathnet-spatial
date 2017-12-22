namespace Spatial.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Analysers;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Environments;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Loggers;
    using BenchmarkDotNet.Toolchains.CsProj;

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
#if NET47 == true
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
#if NETCOREAPP2_0 == true
            this.Add(Job.Default
                .With(Platform.X64)
                .With(Jit.RyuJit)
                .With(Runtime.Core));
#endif
        }
    }
}
