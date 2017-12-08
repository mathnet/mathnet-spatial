``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410097 Hz, Resolution=293.2468 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                    Method |          Mean |      Error |     StdDev |        Median |  Gen 0 | Allocated |
|-------------------------- |--------------:|-----------:|-----------:|--------------:|-------:|----------:|
|          OperatorAddition |     0.2470 ns |  0.1665 ns |  0.4909 ns |     0.0000 ns |      - |       0 B |
| OperatorSubtractionVector |     0.8389 ns |  0.3440 ns |  1.0089 ns |     0.5801 ns |      - |       0 B |
|  OperatorSubtractionPoint |     0.6326 ns |  0.2942 ns |  0.8675 ns |     0.0186 ns |      - |       0 B |
|          OperatorEquality |     1.7217 ns |  0.0842 ns |  0.2430 ns |     1.7179 ns |      - |       0 B |
|                 FromPolar |    66.6750 ns |  1.8176 ns |  5.3308 ns |    65.8863 ns |      - |       0 B |
|                     Parse | 1,100.7367 ns | 28.4113 ns | 83.3254 ns | 1,103.2367 ns | 0.0725 |     384 B |
|                  Centroid |   122.8273 ns |  3.7304 ns | 10.9406 ns |   120.1131 ns | 0.0150 |      80 B |
|                  MidPoint |   123.5018 ns |  3.1025 ns |  9.0992 ns |   121.9977 ns | 0.0236 |     124 B |
|                  VectorTo |     0.5193 ns |  0.2645 ns |  0.7674 ns |     0.0000 ns |      - |       0 B |
|                DistanceTo |     1.7846 ns |  0.3824 ns |  1.1214 ns |     1.7140 ns |      - |       0 B |
|                ToVector2D |     1.2802 ns |  0.4375 ns |  1.2831 ns |     0.9547 ns |      - |       0 B |
|                 ToPoint3D |     1.5735 ns |  0.5713 ns |  1.6846 ns |     1.1676 ns |      - |       0 B |
|                  ToVector |    25.4011 ns |  0.9072 ns |  2.6748 ns |    25.3036 ns | 0.0152 |      80 B |
|                    Equals |     1.7380 ns |  0.0737 ns |  0.2173 ns |     1.7380 ns |      - |       0 B |
|       EqualsWithTolerance |     3.7969 ns |  0.1459 ns |  0.4303 ns |     3.7729 ns |      - |       0 B |
