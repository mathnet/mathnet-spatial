using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;

namespace Spatial.Benchmarks.Properties
{
    public class MemoryDiagnoserConfig : ManualConfig
    {
        public MemoryDiagnoserConfig()
        {
            this.AddDiagnoser(MemoryDiagnoser.Default);
        }
    }
}
