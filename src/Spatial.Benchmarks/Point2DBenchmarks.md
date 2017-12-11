``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                    Method |          Mean |      Error |     StdDev |        Median |  Gen 0 | Allocated |
|-------------------------- |--------------:|-----------:|-----------:|--------------:|-------:|----------:|
|          OperatorAddition |     0.0672 ns |  0.1312 ns |  0.1228 ns |     0.0000 ns |      - |       0 B |
| OperatorSubtractionVector |     0.0846 ns |  0.1033 ns |  0.1697 ns |     0.0000 ns |      - |       0 B |
|  OperatorSubtractionPoint |     0.1446 ns |  0.2050 ns |  0.1917 ns |     0.0000 ns |      - |       0 B |
|          OperatorEquality |     1.5401 ns |  0.0626 ns |  0.0555 ns |     1.5248 ns |      - |       0 B |
|                 FromPolar |    60.8038 ns |  1.5441 ns |  4.0948 ns |    60.8176 ns |      - |       0 B |
|                     Parse | 1,095.8234 ns | 16.4040 ns | 14.5417 ns | 1,096.7946 ns | 0.0725 |     384 B |
|                  Centroid |   111.1149 ns |  2.9465 ns |  2.6120 ns |   109.9865 ns | 0.0150 |      80 B |
|                  MidPoint |   111.9316 ns |  3.5598 ns |  4.8727 ns |   109.2888 ns | 0.0236 |     124 B |
|                  VectorTo |     0.0944 ns |  0.1341 ns |  0.1047 ns |     0.0673 ns |      - |       0 B |
|                DistanceTo |     0.0847 ns |  0.0047 ns |  0.0039 ns |     0.0851 ns |      - |       0 B |
|                ToVector2D |     0.8018 ns |  0.0134 ns |  0.0104 ns |     0.7976 ns |      - |       0 B |
|                 ToPoint3D |     1.2343 ns |  0.3478 ns |  0.4005 ns |     1.1617 ns |      - |       0 B |
|                  ToVector |    21.4499 ns |  0.0182 ns |  0.0152 ns |    21.4475 ns | 0.0152 |      80 B |
|                    Equals |     1.5775 ns |  0.0007 ns |  0.0005 ns |     1.5774 ns |      - |       0 B |
|       EqualsWithTolerance |     3.2272 ns |  0.1258 ns |  0.1115 ns |     3.1842 ns |      - |       0 B |
