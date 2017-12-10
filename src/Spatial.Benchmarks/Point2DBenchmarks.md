``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                    Method |          Mean |      Error |     StdDev |        Median |  Gen 0 | Allocated |
|-------------------------- |--------------:|-----------:|-----------:|--------------:|-------:|----------:|
|          OperatorAddition |     0.4012 ns |  0.2204 ns |  0.5528 ns |     0.1162 ns |      - |       0 B |
| OperatorSubtractionVector |     0.0865 ns |  0.1046 ns |  0.1938 ns |     0.0000 ns |      - |       0 B |
|  OperatorSubtractionPoint |     0.4196 ns |  0.2198 ns |  0.5674 ns |     0.1653 ns |      - |       0 B |
|          OperatorEquality |     1.6827 ns |  0.0700 ns |  0.1245 ns |     1.6627 ns |      - |       0 B |
|                 FromPolar |    57.9601 ns |  0.0427 ns |  0.0379 ns |    57.9544 ns |      - |       0 B |
|                     Parse | 1,057.9093 ns | 21.3735 ns | 51.6193 ns | 1,057.9487 ns | 0.0725 |     384 B |
|                  Centroid |   109.4807 ns |  2.4524 ns |  5.1192 ns |   105.4646 ns | 0.0150 |      80 B |
|                  MidPoint |   114.1860 ns |  2.6749 ns |  5.5242 ns |   110.6827 ns | 0.0236 |     124 B |
|                  VectorTo |     0.3390 ns |  0.2256 ns |  0.3891 ns |     0.1854 ns |      - |       0 B |
|                DistanceTo |     1.5763 ns |  0.3275 ns |  0.6979 ns |     1.1386 ns |      - |       0 B |
|                ToVector2D |     0.2160 ns |  0.3088 ns |  0.4015 ns |     0.0000 ns |      - |       0 B |
|                 ToPoint3D |     1.2829 ns |  0.3247 ns |  0.5425 ns |     1.3707 ns |      - |       0 B |
|                  ToVector |    22.6533 ns |  0.5237 ns |  1.2032 ns |    22.6013 ns | 0.0152 |      80 B |
|                    Equals |     1.6559 ns |  0.0696 ns |  0.1883 ns |     1.6670 ns |      - |       0 B |
|       EqualsWithTolerance |     3.6720 ns |  0.1067 ns |  0.2319 ns |     3.6611 ns |      - |       0 B |
