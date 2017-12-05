``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410117 Hz, Resolution=293.2451 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|     Method |        Mean |     Error |    StdDev |      Median | Allocated |
|----------- |------------:|----------:|----------:|------------:|----------:|
|  Normalize | 143.9735 ns | 3.1835 ns | 7.8687 ns | 138.1000 ns |       0 B |
| DotProduct |   0.9593 ns | 0.3162 ns | 0.5621 ns |   0.5660 ns |       0 B |
