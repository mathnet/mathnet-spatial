``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                     Method |        Mean |      Error |     StdDev |      Median |  Gen 0 | Allocated |
|--------------------------- |------------:|-----------:|-----------:|------------:|-------:|----------:|
| OperatorEqualityRay3DRay3D |    23.79 ns |  0.5716 ns |  0.7020 ns |    23.80 ns |      - |       0 B |
|             IntersectionOf | 1,247.02 ns | 24.5310 ns | 52.2775 ns | 1,266.49 ns | 0.2747 |    1440 B |
|                     LineTo |    47.48 ns |  0.9767 ns |  1.8584 ns |    46.97 ns |      - |       0 B |
|           IntersectionWith |    69.86 ns |  1.4199 ns |  3.2339 ns |    67.56 ns |      - |       0 B |
|                IsCollinear |    30.17 ns |  0.6470 ns |  1.0073 ns |    29.63 ns |      - |       0 B |
|                     Equals |    24.26 ns |  0.5172 ns |  1.1244 ns |    24.01 ns |      - |       0 B |
|        EqualsWithTolerance |    48.77 ns |  1.2742 ns |  1.1919 ns |    48.13 ns |      - |       0 B |
