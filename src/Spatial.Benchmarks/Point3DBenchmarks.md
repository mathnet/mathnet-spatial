``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                          Method |         Mean |      Error |     StdDev |       Median |  Gen 0 | Allocated |
|-------------------------------- |-------------:|-----------:|-----------:|-------------:|-------:|----------:|
|        OperatorAdditionVector3D |     1.659 ns |  0.3353 ns |  0.3136 ns |     1.745 ns |      - |       0 B |
|    OperatorAdditionUnitVector3D |     1.922 ns |  0.3323 ns |  0.5074 ns |     1.640 ns |      - |       0 B |
|     OperatorSubtractionVector3D |     1.810 ns |  0.3311 ns |  0.6299 ns |     1.409 ns |      - |       0 B |
| OperatorSubtractionUnitVector3D |     2.000 ns |  0.3673 ns |  0.4776 ns |     1.716 ns |      - |       0 B |
|      OperatorSubtractionPoint3D |     1.590 ns |  0.0059 ns |  0.0042 ns |     1.592 ns |      - |       0 B |
|                OperatorEquality |     2.524 ns |  0.0851 ns |  0.1578 ns |     2.548 ns |      - |       0 B |
|                           Parse | 1,459.640 ns | 29.0139 ns | 53.7791 ns | 1,469.470 ns | 0.0916 |     488 B |
|                        OfVector |     3.479 ns |  0.0066 ns |  0.0051 ns |     3.481 ns |      - |       0 B |
|                        Centroid |   182.365 ns |  2.5259 ns |  2.2391 ns |   182.689 ns | 0.0226 |     120 B |
|                        MidPoint |   182.574 ns |  3.8891 ns |  5.1919 ns |   178.451 ns | 0.0341 |     180 B |
|            IntersectionOfPlanes |           NA |         NA |         NA |           NA |    N/A |       N/A |
|       IntersectionOfPlaneAndRay |    57.896 ns |  0.0675 ns |  0.0563 ns |    57.877 ns |      - |       0 B |
|                     MirrorAbout |    51.500 ns |  0.0524 ns |  0.0464 ns |    51.485 ns |      - |       0 B |
|                       ProjectOn |    21.399 ns |  0.0095 ns |  0.0079 ns |    21.401 ns |      - |       0 B |
|                          Rotate | 1,315.777 ns | 26.6338 ns | 45.9419 ns | 1,328.955 ns | 0.4215 |    2216 B |
|          RotateAroundUnitVector | 1,168.676 ns | 23.6314 ns | 50.8691 ns | 1,164.455 ns | 0.4215 |    2216 B |
|                        VectorTo |     1.846 ns |  0.3360 ns |  0.4599 ns |     1.995 ns |      - |       0 B |
|                      DistanceTo |     1.664 ns |  0.0062 ns |  0.0052 ns |     1.664 ns |      - |       0 B |
|                      ToVector3D |     1.781 ns |  0.3296 ns |  0.7703 ns |     1.637 ns |      - |       0 B |
|                        ToVector |    30.564 ns |  0.0068 ns |  0.0053 ns |    30.562 ns | 0.0167 |      88 B |
|                          Equals |     2.392 ns |  0.0005 ns |  0.0004 ns |     2.392 ns |      - |       0 B |
|             EqualsWIthTolerance |    16.099 ns |  0.0112 ns |  0.0105 ns |    16.098 ns |      - |       0 B |

Benchmarks with issues:
  Point3DBenchmarks.IntersectionOfPlanes: DefaultJob
