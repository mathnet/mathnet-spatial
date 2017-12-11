``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                     Method |        Mean |      Error |     StdDev |      Median |  Gen 0 | Allocated |
|--------------------------- |------------:|-----------:|-----------:|------------:|-------:|----------:|
| OperatorEqualityRay3DRay3D |    25.39 ns |  0.5301 ns |  0.5672 ns |    25.21 ns |      - |       0 B |
|             IntersectionOf | 1,222.63 ns | 24.2224 ns | 56.6191 ns | 1,223.89 ns | 0.2747 |    1440 B |
|                     LineTo |    46.20 ns |  0.9477 ns |  1.8926 ns |    44.97 ns |      - |       0 B |
|           IntersectionWith |    71.98 ns |  1.4763 ns |  3.9407 ns |    71.69 ns |      - |       0 B |
|                IsCollinear |    30.35 ns |  0.6577 ns |  1.8116 ns |    30.36 ns |      - |       0 B |
|                     Equals |    23.57 ns |  0.5035 ns |  0.6891 ns |    23.65 ns |      - |       0 B |
|        EqualsWithTolerance |    49.01 ns |  1.0139 ns |  1.8539 ns |    48.22 ns |      - |       0 B |
