``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                     Method |        Mean |      Error |      StdDev |      Median |  Gen 0 | Allocated |
|--------------------------- |------------:|-----------:|------------:|------------:|-------:|----------:|
|                          A |    11.29 ns |  0.5167 ns |   0.8633 ns |    11.12 ns |      - |       0 B |
|                          B |    12.14 ns |  0.5453 ns |   0.8649 ns |    12.22 ns |      - |       0 B |
|                          C |    11.81 ns |  0.5819 ns |   1.0191 ns |    11.40 ns |      - |       0 B |
|                  RootPoint |    15.01 ns |  0.6121 ns |   0.9529 ns |    14.92 ns |      - |       0 B |
| OperatorEqualityPlanePlane |    28.51 ns |  0.8069 ns |   1.1045 ns |    28.31 ns |      - |       0 B |
|                 FromPoints |   250.63 ns |  5.2778 ns |  14.1784 ns |   248.78 ns |      - |       0 B |
|            PointFromPlanes |          NA |         NA |          NA |          NA |    N/A |       N/A |
|    SignedDistanceToPoint3D |    36.48 ns |  1.0380 ns |   2.1666 ns |    36.56 ns |      - |       0 B |
|      SignedDistanceToPlane |          NA |         NA |          NA |          NA |    N/A |       N/A |
|      SignedDistanceToRay3D |    44.87 ns |  1.2111 ns |   2.5012 ns |    44.73 ns |      - |       0 B |
|  AbsoluteDistanceToPoint3D |    34.25 ns |  0.9749 ns |   1.6817 ns |    34.16 ns |      - |       0 B |
|             ProjectPoint3D |    22.46 ns |  0.7273 ns |   1.2738 ns |    22.70 ns |      - |       0 B |
|              ProjectLine3D |    60.29 ns |  1.2382 ns |   2.2009 ns |    59.08 ns |      - |       0 B |
|               ProjectRay3D |   259.03 ns |  5.1787 ns |  14.6066 ns |   258.34 ns |      - |       0 B |
|            ProjectVector3D |   214.78 ns |  4.2378 ns |   7.4222 ns |   213.74 ns |      - |       0 B |
|        ProjectUnitVector3D |   210.19 ns |  4.2057 ns |   6.0317 ns |   211.63 ns |      - |       0 B |
|      IntersectionWithPlane |          NA |         NA |          NA |          NA |    N/A |       N/A |
|     IntersectionWithLine3D |   457.33 ns |  9.1671 ns |  15.3161 ns |   455.26 ns |      - |       0 B |
|           IntersectionWith |          NA |         NA |          NA |          NA |    N/A |       N/A |
|                MirrorAbout |    58.04 ns |  1.5983 ns |   1.5697 ns |    57.82 ns |      - |       0 B |
|                     Rotate | 2,693.65 ns | 53.8517 ns | 110.0047 ns | 2,699.94 ns | 0.8621 |    4536 B |
|                     Equals |    30.11 ns |  0.8538 ns |   1.0485 ns |    29.93 ns |      - |       0 B |

Benchmarks with issues:
  PlaneBenchmarks.PointFromPlanes: DefaultJob
  PlaneBenchmarks.SignedDistanceToPlane: DefaultJob
  PlaneBenchmarks.IntersectionWithPlane: DefaultJob
  PlaneBenchmarks.IntersectionWith: DefaultJob
