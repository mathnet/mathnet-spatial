``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410097 Hz, Resolution=293.2468 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                           Method |          Mean |      Error |     StdDev |        Median |  Gen 0 | Allocated |
|--------------------------------- |--------------:|-----------:|-----------:|--------------:|-------:|----------:|
|                           Length |     0.1500 ns |  0.1242 ns |  0.3663 ns |     0.0000 ns |      - |       0 B |
|                 OperatorEquality |     1.9741 ns |  0.0807 ns |  0.2209 ns |     1.9726 ns |      - |       0 B |
|                 OperatorAddition |     0.4587 ns |  0.2583 ns |  0.7535 ns |     0.0000 ns |      - |       0 B |
|              OperatorSubtraction |     0.9729 ns |  0.3531 ns |  1.0412 ns |     0.6519 ns |      - |       0 B |
|            OperatorUnaryNegation |     2.7540 ns |  0.4691 ns |  1.3683 ns |     2.4950 ns |      - |       0 B |
|                 OperatorMultiply |     1.2764 ns |  0.4746 ns |  1.3994 ns |     0.8728 ns |      - |       0 B |
|                 OperatorDivision |     1.3428 ns |  0.4526 ns |  1.3345 ns |     1.0624 ns |      - |       0 B |
|                        FromPolar |    64.3484 ns |  1.9428 ns |  5.6673 ns |    63.0724 ns |      - |       0 B |
|                            Parse | 1,098.8610 ns | 27.9918 ns | 79.8621 ns | 1,089.1299 ns | 0.0725 |     384 B |
|                         OfVector |     4.0640 ns |  0.5986 ns |  1.7648 ns |     3.8767 ns |      - |       0 B |
|      IsParallelToDoubleTolerance |    20.0952 ns |  0.5811 ns |  1.7043 ns |    19.5544 ns |      - |       0 B |
|       IsParallelToAngleTolerance |    42.4472 ns |  1.1230 ns |  3.2934 ns |    42.1252 ns |      - |       0 B |
| IsPerpendicularToDoubleTolerance |    19.2001 ns |  0.4859 ns |  1.4098 ns |    19.2126 ns |      - |       0 B |
|  IsPerpendicularToAngleTolerance |    43.7986 ns |  1.0736 ns |  3.1656 ns |    43.4620 ns |      - |       0 B |
|                    SignedAngleTo |    94.1201 ns |  2.6820 ns |  7.9078 ns |    93.0415 ns |      - |       0 B |
|                          AngleTo |    35.5259 ns |  1.1548 ns |  3.3685 ns |    35.5115 ns |      - |       0 B |
|                           Rotate |    65.0169 ns |  2.2445 ns |  6.5472 ns |    64.0402 ns |      - |       0 B |
|                       DotProduct |     0.1323 ns |  0.1246 ns |  0.3673 ns |     0.0000 ns |      - |       0 B |
|                     CrossProduct |     0.9111 ns |  0.3776 ns |  1.1015 ns |     0.4941 ns |      - |       0 B |
|                        ProjectOn |     3.1975 ns |  0.4843 ns |  1.4279 ns |     2.7713 ns |      - |       0 B |
|                        Normalize |     4.1881 ns |  0.5221 ns |  1.5394 ns |     4.1268 ns |      - |       0 B |
|                          ScaleBy |     1.2528 ns |  0.4372 ns |  1.2822 ns |     0.8512 ns |      - |       0 B |
|                           Negate |     3.6869 ns |  0.4740 ns |  1.3901 ns |     3.5136 ns |      - |       0 B |
|                         Subtract |     0.3242 ns |  0.2191 ns |  0.6390 ns |     0.0000 ns |      - |       0 B |
|                              Add |     0.6192 ns |  0.3388 ns |  0.9884 ns |     0.0000 ns |      - |       0 B |
|                         ToVector |    26.6259 ns |  0.8637 ns |  2.5193 ns |    26.2296 ns | 0.0152 |      80 B |
|                           Equals |     1.9990 ns |  0.0765 ns |  0.1975 ns |     1.9700 ns |      - |       0 B |
|              EqualsWithTolerance |     2.7043 ns |  0.1444 ns |  0.4258 ns |     2.6520 ns |      - |       0 B |
