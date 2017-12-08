``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410097 Hz, Resolution=293.2468 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                     Method |     Mean |     Error |   StdDev | Allocated |
|--------------------------- |---------:|----------:|---------:|----------:|
| OperatorEqualityRay3DRay3D | 25.23 ns | 0.5581 ns | 1.451 ns |       0 B |
|             IntersectionOf |       NA |        NA |       NA |       N/A |
|                     LineTo | 50.67 ns | 1.1823 ns | 3.486 ns |       0 B |
|           IntersectionWith | 78.13 ns | 2.1312 ns | 6.250 ns |       0 B |
|                IsCollinear | 33.45 ns | 0.8394 ns | 2.449 ns |       0 B |
|                     Equals | 25.67 ns | 0.5567 ns | 1.515 ns |       0 B |
|        EqualsWithTolerance | 53.67 ns | 1.1162 ns | 2.999 ns |       0 B |

Benchmarks with issues:
  Ray3DBenchmarks.IntersectionOf: DefaultJob
