``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410097 Hz, Resolution=293.2468 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                          Method |         Mean |      Error |      StdDev |        Median |  Gen 0 | Allocated |
|-------------------------------- |-------------:|-----------:|------------:|--------------:|-------:|----------:|
|        OperatorAdditionVector3D |     2.789 ns |  0.5248 ns |   1.5308 ns |     2.2251 ns |      - |       0 B |
|    OperatorAdditionUnitVector3D |     1.398 ns |  0.4543 ns |   1.3396 ns |     1.4076 ns |      - |       0 B |
|     OperatorSubtractionVector3D |     1.604 ns |  0.4762 ns |   1.3966 ns |     1.4100 ns |      - |       0 B |
| OperatorSubtractionUnitVector3D |     1.628 ns |  0.3930 ns |   1.1586 ns |     1.5104 ns |      - |       0 B |
|      OperatorSubtractionPoint3D |     1.738 ns |  0.4038 ns |   1.1907 ns |     1.6982 ns |      - |       0 B |
|                OperatorEquality |     2.570 ns |  0.1162 ns |   0.3426 ns |     2.5703 ns |      - |       0 B |
|                           Parse | 1,648.356 ns | 45.2549 ns | 132.0107 ns | 1,638.4033 ns | 0.0916 |     488 B |
|                        OfVector |     4.801 ns |  0.5013 ns |   1.4781 ns |     4.5396 ns |      - |       0 B |
|                        Centroid |   197.059 ns |  4.5331 ns |  13.2948 ns |   196.7110 ns | 0.0224 |     120 B |
|                        MidPoint |   205.989 ns |  6.0465 ns |  17.6378 ns |   204.3920 ns | 0.0341 |     180 B |
|            IntersectionOfPlanes |           NA |         NA |          NA |            NA |    N/A |       N/A |
|       IntersectionOfPlaneAndRay |    66.973 ns |  2.0545 ns |   6.0254 ns |    65.9950 ns |      - |       0 B |
|                     MirrorAbout |    55.598 ns |  2.1763 ns |   6.3828 ns |    54.7076 ns |      - |       0 B |
|                       ProjectOn |    22.026 ns |  0.7744 ns |   1.8702 ns |    22.2268 ns |      - |       0 B |
|                          Rotate | 1,387.218 ns | 30.5836 ns |  89.2138 ns | 1,376.1153 ns | 0.4215 |    2216 B |
|          RotateAroundUnitVector | 1,221.696 ns | 33.3636 ns |  98.3734 ns | 1,194.1924 ns | 0.4215 |    2216 B |
|                        VectorTo |     2.651 ns |  0.4167 ns |   1.2156 ns |     2.4543 ns |      - |       0 B |
|                      DistanceTo |     1.931 ns |  0.3650 ns |   1.0294 ns |     1.9995 ns |      - |       0 B |
|                      ToVector3D |     1.075 ns |  0.3726 ns |   1.0985 ns |     0.8523 ns |      - |       0 B |
|                        ToVector |    33.991 ns |  1.0443 ns |   3.0792 ns |    33.6502 ns | 0.0167 |      88 B |
|                          Equals |     2.488 ns |  0.1092 ns |   0.3219 ns |     2.3960 ns |      - |       0 B |
|             EqualsWIthTolerance |    18.831 ns |  0.4640 ns |   1.3461 ns |    18.8056 ns |      - |       0 B |

Benchmarks with issues:
  Point3DBenchmarks.IntersectionOfPlanes: DefaultJob
