``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                          Method |         Mean |      Error |     StdDev |       Median |  Gen 0 | Allocated |
|-------------------------------- |-------------:|-----------:|-----------:|-------------:|-------:|----------:|
|        OperatorAdditionVector3D |     2.366 ns |  0.3587 ns |  1.0576 ns |     2.142 ns |      - |       0 B |
|    OperatorAdditionUnitVector3D |     1.744 ns |  0.3257 ns |  0.4566 ns |     1.392 ns |      - |       0 B |
|     OperatorSubtractionVector3D |     2.119 ns |  0.3392 ns |  0.7587 ns |     2.171 ns |      - |       0 B |
| OperatorSubtractionUnitVector3D |     1.846 ns |  0.3359 ns |  0.7717 ns |     1.887 ns |      - |       0 B |
|      OperatorSubtractionPoint3D |     2.156 ns |  0.3469 ns |  0.8703 ns |     2.024 ns |      - |       0 B |
|                OperatorEquality |     2.547 ns |  0.0872 ns |  0.2154 ns |     2.543 ns |      - |       0 B |
|                           Parse | 1,456.256 ns | 29.3019 ns | 46.4759 ns | 1,466.342 ns | 0.0916 |     488 B |
|                        OfVector |     4.460 ns |  0.3857 ns |  0.9887 ns |     4.362 ns |      - |       0 B |
|                        Centroid |   343.062 ns |  7.2169 ns | 16.2897 ns |   343.295 ns | 0.0226 |     120 B |
|                        MidPoint |   184.003 ns |  4.0278 ns |  6.3884 ns |   184.629 ns | 0.0341 |     180 B |
|            IntersectionOfPlanes |           NA |         NA |         NA |           NA |    N/A |       N/A |
|       IntersectionOfPlaneAndRay |    62.726 ns |  1.5452 ns |  3.3262 ns |    62.512 ns |      - |       0 B |
|                     MirrorAbout |    52.409 ns |  1.5498 ns |  2.0152 ns |    51.805 ns |      - |       0 B |
|                       ProjectOn |    22.586 ns |  0.7342 ns |  1.2063 ns |    22.675 ns |      - |       0 B |
|                          Rotate | 1,276.310 ns |  0.9905 ns |  0.5895 ns | 1,276.050 ns | 0.4215 |    2216 B |
|          RotateAroundUnitVector | 1,246.201 ns | 23.5669 ns | 37.3795 ns | 1,255.381 ns | 0.4215 |    2216 B |
|                        VectorTo |     1.543 ns |  0.3398 ns |  0.6547 ns |     1.679 ns |      - |       0 B |
|                      DistanceTo |     1.510 ns |  0.0071 ns |  0.0063 ns |     1.508 ns |      - |       0 B |
|                      ToVector3D |     1.352 ns |  0.3486 ns |  0.5323 ns |     1.258 ns |      - |       0 B |
|                        ToVector |    32.906 ns |  0.7249 ns |  1.6213 ns |    33.308 ns | 0.0167 |      88 B |
|                          Equals |     2.488 ns |  0.0865 ns |  0.1708 ns |     2.504 ns |      - |       0 B |
|             EqualsWIthTolerance |    16.264 ns |  0.0135 ns |  0.0105 ns |    16.263 ns |      - |       0 B |

Benchmarks with issues:
  Point3DBenchmarks.IntersectionOfPlanes: DefaultJob
