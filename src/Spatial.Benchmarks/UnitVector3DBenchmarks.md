``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                                      Method |          Mean |      Error |     StdDev |        Median |  Gen 0 | Allocated |
|-------------------------------------------- |--------------:|-----------:|-----------:|--------------:|-------:|----------:|
|                                  Orthogonal |    94.5469 ns |  2.1810 ns |  4.5042 ns |    95.1725 ns |      - |       0 B |
|                                      Length |     0.0607 ns |  0.0827 ns |  0.0953 ns |     0.0000 ns |      - |       0 B |
|    OperatorEqualityUnitVector3DUnitVector3D |     1.5009 ns |  0.0123 ns |  0.0096 ns |     1.4976 ns |      - |       0 B |
|        OperatorEqualityUnitVector3DVector3D |     2.2283 ns |  0.0244 ns |  0.0177 ns |     2.2293 ns |      - |       0 B |
|    OperatorAdditionUnitVector3DUnitVector3D |     1.9051 ns |  0.3726 ns |  0.3826 ns |     1.7012 ns |      - |       0 B |
|        OperatorAdditionUnitVector3DVector3D |     1.7358 ns |  0.0058 ns |  0.0045 ns |     1.7359 ns |      - |       0 B |
|                         OperatorSubtraction |     1.8568 ns |  0.3424 ns |  0.7729 ns |     1.9339 ns |      - |       0 B |
| OperatorSubtractionUnitVector3DUnitVector3D |     1.8466 ns |  0.3418 ns |  0.9753 ns |     1.7806 ns |      - |       0 B |
|     OperatorSubtractionUnitVector3DVector3D |     1.7147 ns |  0.2797 ns |  0.2335 ns |     1.6538 ns |      - |       0 B |
|                       OperatorUnaryNegation |     2.4302 ns |  0.3625 ns |  0.8034 ns |     2.2275 ns |      - |       0 B |
|                            OperatorMultiply |     6.2685 ns |  0.4291 ns |  0.6680 ns |     6.4226 ns |      - |       0 B |
|                            OperatorDivision |     2.0585 ns |  0.3508 ns |  0.7244 ns |     2.1612 ns |      - |       0 B |
|    OperatorMultiplyUnitVector3DUnitVector3D |     7.0023 ns |  0.4378 ns |  0.8005 ns |     6.9383 ns |      - |       0 B |
|                                      Create |   144.9530 ns |  3.2253 ns |  6.9428 ns |   144.9267 ns |      - |       0 B |
|                                    OfVector |   150.7669 ns |  3.3074 ns |  2.9319 ns |   150.4880 ns |      - |       0 B |
|                                       Parse | 1,616.2906 ns | 32.7998 ns | 52.0239 ns | 1,609.8286 ns | 0.0916 |     488 B |
|                              EqualsVector3D |     2.1921 ns |  0.0815 ns |  0.1755 ns |     2.2258 ns |      - |       0 B |
|                          EqualsUnitVector3D |     1.6381 ns |  0.0745 ns |  0.1487 ns |     1.6026 ns |      - |       0 B |
|           EqualsUnitVector3DDoubleTolerance |    17.3994 ns |  0.3851 ns |  1.0670 ns |    17.0661 ns |      - |       0 B |
|               EqualsVector3DDoubleTolerance |    14.9096 ns |  0.0069 ns |  0.0058 ns |    14.9098 ns |      - |       0 B |
|                                     ScaleBy |     1.2469 ns |  0.3591 ns |  0.3842 ns |     1.0959 ns |      - |       0 B |
|                              ProjectOnPlane |   203.9481 ns |  4.0638 ns |  9.2554 ns |   202.1629 ns |      - |       0 B |
|                       ProjectOnUnitVector3D |     8.2296 ns |  0.4572 ns |  0.7118 ns |     7.7364 ns |      - |       0 B |
|         IsParallelToVector3DDoubleTolerance |   165.0534 ns |  3.2257 ns |  2.8595 ns |   163.6927 ns |      - |       0 B |
|     IsParallelToUnitVector3DDoubleTolerance |     7.4068 ns |  0.1805 ns |  0.3208 ns |     7.1935 ns |      - |       0 B |
|      IsParallelToUnitVector3DAngleTolerance |    20.0535 ns |  0.4291 ns |  0.6016 ns |    19.6178 ns |      - |       0 B |
|                        IsParallelToVector3D |   187.0954 ns |  3.7747 ns |  6.8066 ns |   186.8320 ns |      - |       0 B |
|                   IsPerpendicularToVector3D |   168.1991 ns |  3.3970 ns |  6.5449 ns |   168.4613 ns |      - |       0 B |
|                           IsPerpendicularTo |     6.5370 ns |  0.1654 ns |  0.3417 ns |     6.5596 ns |      - |       0 B |
|                                      Negate |    85.2889 ns |  1.4766 ns |  1.1528 ns |    84.8213 ns |      - |       0 B |
|                          DotProductVector3D |     0.7573 ns |  0.3206 ns |  0.6693 ns |     0.3723 ns |      - |       0 B |
|                      DotProductUnitVector3D |     7.2567 ns |  0.4380 ns |  0.8333 ns |     7.1050 ns |      - |       0 B |
|                                    Subtract |     2.1125 ns |  0.3837 ns |  0.9267 ns |     2.0569 ns |      - |       0 B |
|                                         Add |     2.1943 ns |  0.3458 ns |  0.7443 ns |     2.1496 ns |      - |       0 B |
|                                CrossProduct |   150.7092 ns |  3.2975 ns |  7.0273 ns |   149.2142 ns |      - |       0 B |
|                               SignedAngleTo |   805.4243 ns | 16.2003 ns | 14.3611 ns |   796.8258 ns |      - |       0 B |
|                             AngleToVector3D |   181.9530 ns |  3.9711 ns |  3.5203 ns |   181.6821 ns |      - |       0 B |
|                         AngleToUnitVector3D |    13.0088 ns |  0.5489 ns |  1.0575 ns |    12.8208 ns |      - |       0 B |
|                                      Rotate | 1,433.0760 ns | 60.6187 ns | 56.7028 ns | 1,403.1423 ns | 0.4425 |    2320 B |
|                                   ToPoint3D |     0.7323 ns |  0.3105 ns |  0.8072 ns |     0.6569 ns |      - |       0 B |
|                                  ToVector3D |     0.8859 ns |  0.3246 ns |  0.7714 ns |     0.8698 ns |      - |       0 B |
|                                    ToVector |    32.9099 ns |  0.7354 ns |  2.1684 ns |    32.6193 ns | 0.0167 |      88 B |
