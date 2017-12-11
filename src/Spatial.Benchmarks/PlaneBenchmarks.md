``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                     Method |         Mean |      Error |     StdDev |       Median |  Gen 0 | Allocated |
|--------------------------- |-------------:|-----------:|-----------:|-------------:|-------:|----------:|
|                          A |    10.603 ns |  0.5136 ns |  0.5708 ns |    10.676 ns |      - |       0 B |
|                          B |     9.544 ns |  0.4702 ns |  0.5031 ns |     9.130 ns |      - |       0 B |
|                          C |    12.260 ns |  0.5364 ns |  0.8192 ns |    12.306 ns |      - |       0 B |
|                  RootPoint |    16.032 ns |  0.6311 ns |  1.1379 ns |    16.050 ns |      - |       0 B |
| OperatorEqualityPlanePlane |    29.303 ns |  0.6805 ns |  0.8606 ns |    29.216 ns |      - |       0 B |
|                 FromPoints |   241.648 ns |  5.4472 ns | 12.2952 ns |   240.768 ns |      - |       0 B |
|            PointFromPlanes |           NA |         NA |         NA |           NA |    N/A |       N/A |
|    SignedDistanceToPoint3D |    32.880 ns |  0.0262 ns |  0.0204 ns |    32.877 ns |      - |       0 B |
|      SignedDistanceToPlane |           NA |         NA |         NA |           NA |    N/A |       N/A |
|      SignedDistanceToRay3D |    43.878 ns |  1.1507 ns |  1.2790 ns |    44.209 ns |      - |       0 B |
|  AbsoluteDistanceToPoint3D |    33.025 ns |  0.9553 ns |  1.3392 ns |    32.145 ns |      - |       0 B |
|             ProjectPoint3D |    21.824 ns |  0.7630 ns |  0.7137 ns |    21.797 ns |      - |       0 B |
|              ProjectLine3D |    60.282 ns |  1.2275 ns |  2.1819 ns |    58.631 ns |      - |       0 B |
|               ProjectRay3D |   267.730 ns |  5.1431 ns |  4.2947 ns |   268.298 ns |      - |       0 B |
|            ProjectVector3D |   211.316 ns |  4.2216 ns |  5.4892 ns |   211.326 ns |      - |       0 B |
|        ProjectUnitVector3D |   211.533 ns |  4.7270 ns |  7.4975 ns |   210.895 ns |      - |       0 B |
|      IntersectionWithPlane |           NA |         NA |         NA |           NA |    N/A |       N/A |
|     IntersectionWithLine3D |   449.041 ns |  8.8854 ns | 13.2993 ns |   450.186 ns |      - |       0 B |
|           IntersectionWith |           NA |         NA |         NA |           NA |    N/A |       N/A |
|                MirrorAbout |    55.997 ns |  0.0217 ns |  0.0192 ns |    55.994 ns |      - |       0 B |
|                     Rotate | 2,607.358 ns | 56.1719 ns | 62.4349 ns | 2,569.116 ns | 0.8621 |    4536 B |
|                     Equals |    29.494 ns |  0.6045 ns |  0.6961 ns |    29.826 ns |      - |       0 B |

Benchmarks with issues:
  PlaneBenchmarks.PointFromPlanes: DefaultJob
  PlaneBenchmarks.SignedDistanceToPlane: DefaultJob
  PlaneBenchmarks.IntersectionWithPlane: DefaultJob
  PlaneBenchmarks.IntersectionWith: DefaultJob
