``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                           Method |          Mean |      Error |     StdDev |        Median |  Gen 0 | Allocated |
|--------------------------------- |--------------:|-----------:|-----------:|--------------:|-------:|----------:|
|                           Length |     0.0071 ns |  0.0273 ns |  0.0325 ns |     0.0000 ns |      - |       0 B |
|                 OperatorEquality |     1.8343 ns |  0.0705 ns |  0.0866 ns |     1.7914 ns |      - |       0 B |
|                 OperatorAddition |     0.0000 ns |  0.0000 ns |  0.0000 ns |     0.0000 ns |      - |       0 B |
|              OperatorSubtraction |     0.3853 ns |  0.7451 ns |  0.7318 ns |     0.0388 ns |      - |       0 B |
|            OperatorUnaryNegation |     2.6817 ns |  0.3170 ns |  0.2965 ns |     2.5649 ns |      - |       0 B |
|                 OperatorMultiply |     1.6860 ns |  0.2035 ns |  0.1700 ns |     1.6790 ns |      - |       0 B |
|                 OperatorDivision |     1.1174 ns |  0.0048 ns |  0.0040 ns |     1.1156 ns |      - |       0 B |
|                        FromPolar |    62.6317 ns |  1.5841 ns |  3.4099 ns |    62.5096 ns |      - |       0 B |
|                            Parse | 1,130.5049 ns | 22.4818 ns | 39.3751 ns | 1,119.7084 ns | 0.0725 |     384 B |
|                         OfVector |     3.6681 ns |  0.0046 ns |  0.0038 ns |     3.6686 ns |      - |       0 B |
|      IsParallelToDoubleTolerance |    20.9150 ns |  0.4805 ns |  0.7481 ns |    20.9280 ns |      - |       0 B |
|       IsParallelToAngleTolerance |    40.0116 ns |  0.8346 ns |  1.7785 ns |    40.0119 ns |      - |       0 B |
| IsPerpendicularToDoubleTolerance |    17.7490 ns |  0.3874 ns |  0.6786 ns |    17.2174 ns |      - |       0 B |
|  IsPerpendicularToAngleTolerance |    39.6027 ns |  0.8067 ns |  1.0770 ns |    38.7673 ns |      - |       0 B |
|                    SignedAngleTo |    85.5176 ns |  2.0506 ns |  3.2524 ns |    82.8495 ns |      - |       0 B |
|                          AngleTo |    30.8177 ns |  0.9288 ns |  1.6510 ns |    31.5175 ns |      - |       0 B |
|                           Rotate |    59.9624 ns |  1.5976 ns |  3.1161 ns |    60.6947 ns |      - |       0 B |
|                       DotProduct |     0.0053 ns |  0.0193 ns |  0.0311 ns |     0.0000 ns |      - |       0 B |
|                     CrossProduct |     0.1749 ns |  0.4085 ns |  0.3621 ns |     0.0000 ns |      - |       0 B |
|                        ProjectOn |     2.2216 ns |  0.3638 ns |  0.4331 ns |     2.2910 ns |      - |       0 B |
|                        Normalize |     4.3468 ns |  0.3776 ns |  0.7964 ns |     4.3522 ns |      - |       0 B |
|                          ScaleBy |     1.1005 ns |  0.3694 ns |  0.3275 ns |     0.9396 ns |      - |       0 B |
|                           Negate |     3.3408 ns |  0.2339 ns |  0.1691 ns |     3.3207 ns |      - |       0 B |
|                         Subtract |     0.1291 ns |  0.1765 ns |  0.2232 ns |     0.0000 ns |      - |       0 B |
|                              Add |     0.0310 ns |  0.0923 ns |  0.0863 ns |     0.0000 ns |      - |       0 B |
|                         ToVector |    25.6090 ns |  0.5828 ns |  0.6236 ns |    25.5752 ns | 0.0152 |      80 B |
|                           Equals |     1.7094 ns |  0.0109 ns |  0.0085 ns |     1.7067 ns |      - |       0 B |
|              EqualsWithTolerance |     2.7370 ns |  0.0895 ns |  0.1745 ns |     2.8127 ns |      - |       0 B |
