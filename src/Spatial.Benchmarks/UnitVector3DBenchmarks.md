``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410097 Hz, Resolution=293.2468 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                                      Method |          Mean |      Error |      StdDev |        Median |  Gen 0 | Allocated |
|-------------------------------------------- |--------------:|-----------:|------------:|--------------:|-------:|----------:|
|                                  Orthogonal |    99.7557 ns |  2.3605 ns |   6.5409 ns |   100.3948 ns |      - |       0 B |
|                                      Length |     0.2003 ns |  0.1373 ns |   0.4049 ns |     0.0000 ns |      - |       0 B |
|    OperatorEqualityUnitVector3DUnitVector3D |     1.5854 ns |  0.0725 ns |   0.2092 ns |     1.5775 ns |      - |       0 B |
|        OperatorEqualityUnitVector3DVector3D |     2.3153 ns |  0.1049 ns |   0.3076 ns |     2.3022 ns |      - |       0 B |
|    OperatorAdditionUnitVector3DUnitVector3D |     2.0102 ns |  0.4604 ns |   1.3576 ns |     1.9898 ns |      - |       0 B |
|        OperatorAdditionUnitVector3DVector3D |     2.1014 ns |  0.4339 ns |   1.2726 ns |     2.0422 ns |      - |       0 B |
|                         OperatorSubtraction |     2.0948 ns |  0.5309 ns |   1.5569 ns |     2.1434 ns |      - |       0 B |
| OperatorSubtractionUnitVector3DUnitVector3D |     1.7709 ns |  0.4594 ns |   1.3401 ns |     1.6603 ns |      - |       0 B |
|     OperatorSubtractionUnitVector3DVector3D |     1.7428 ns |  0.4215 ns |   1.2161 ns |     1.4460 ns |      - |       0 B |
|                       OperatorUnaryNegation |     1.9568 ns |  0.4653 ns |   1.3572 ns |     1.8195 ns |      - |       0 B |
|                            OperatorMultiply |     6.6568 ns |  0.4648 ns |   1.3261 ns |     6.5903 ns |      - |       0 B |
|                            OperatorDivision |     2.0195 ns |  0.3692 ns |   1.0827 ns |     1.8799 ns |      - |       0 B |
|    OperatorMultiplyUnitVector3DUnitVector3D |     6.8557 ns |  0.4910 ns |   1.4324 ns |     6.6616 ns |      - |       0 B |
|                                      Create |   154.6131 ns |  3.7348 ns |  10.8946 ns |   153.2068 ns |      - |       0 B |
|                                    OfVector |   158.9117 ns |  3.6715 ns |   9.7999 ns |   158.1376 ns |      - |       0 B |
|                                       Parse | 1,806.0698 ns | 43.7129 ns | 128.2023 ns | 1,793.5780 ns | 0.0916 |     488 B |
|                              EqualsVector3D |     2.2403 ns |  0.0967 ns |   0.2851 ns |     2.1985 ns |      - |       0 B |
|                          EqualsUnitVector3D |     1.6557 ns |  0.0811 ns |   0.2352 ns |     1.6388 ns |      - |       0 B |
|           EqualsUnitVector3DDoubleTolerance |    17.3613 ns |  0.5801 ns |   1.7103 ns |    17.1636 ns |      - |       0 B |
|               EqualsVector3DDoubleTolerance |    15.7152 ns |  0.4204 ns |   1.2329 ns |    15.0999 ns |      - |       0 B |
|                                     ScaleBy |     1.6604 ns |  0.4654 ns |   1.3575 ns |     1.6127 ns |      - |       0 B |
|                              ProjectOnPlane |   217.0214 ns |  5.9112 ns |  17.4293 ns |   213.5779 ns |      - |       0 B |
|                       ProjectOnUnitVector3D |     8.5268 ns |  0.6016 ns |   1.7739 ns |     8.6425 ns |      - |       0 B |
|         IsParallelToVector3DDoubleTolerance |   190.8106 ns |  3.9718 ns |  11.7110 ns |   188.6794 ns |      - |       0 B |
|     IsParallelToUnitVector3DDoubleTolerance |     8.0916 ns |  0.2667 ns |   0.7864 ns |     7.9980 ns |      - |       0 B |
|      IsParallelToUnitVector3DAngleTolerance |    22.6879 ns |  0.6619 ns |   1.9515 ns |    22.4749 ns |      - |       0 B |
|                        IsParallelToVector3D |   202.6106 ns |  4.2224 ns |  12.4498 ns |   202.1065 ns |      - |       0 B |
|                   IsPerpendicularToVector3D |   181.9944 ns |  3.8277 ns |  11.1657 ns |   181.0228 ns |      - |       0 B |
|                           IsPerpendicularTo |     7.0305 ns |  0.2076 ns |   0.5957 ns |     6.9974 ns |      - |       0 B |
|                                      Negate |    92.7270 ns |  2.7194 ns |   8.0182 ns |    91.3132 ns |      - |       0 B |
|                          DotProductVector3D |     1.9075 ns |  0.3646 ns |   1.0692 ns |     1.8989 ns |      - |       0 B |
|                      DotProductUnitVector3D |     7.3437 ns |  0.5137 ns |   1.5146 ns |     7.0544 ns |      - |       0 B |
|                                    Subtract |     2.3062 ns |  0.4707 ns |   1.3806 ns |     2.1487 ns |      - |       0 B |
|                                         Add |     2.4180 ns |  0.3682 ns |   0.9699 ns |     2.3014 ns |      - |       0 B |
|                                CrossProduct |   158.5271 ns |  4.2095 ns |  12.2794 ns |   157.0455 ns |      - |       0 B |
|                               SignedAngleTo |   888.6230 ns | 18.2214 ns |  53.4403 ns |   881.6278 ns |      - |       0 B |
|                             AngleToVector3D |   191.2188 ns |  5.0709 ns |  14.8721 ns |   188.3360 ns |      - |       0 B |
|                         AngleToUnitVector3D |    14.6797 ns |  0.7867 ns |   2.3072 ns |    14.3016 ns |      - |       0 B |
|                                      Rotate | 1,584.2827 ns | 43.0062 ns | 126.8049 ns | 1,584.3343 ns | 0.4425 |    2320 B |
|                                   ToPoint3D |     1.4061 ns |  0.4259 ns |   1.2559 ns |     1.2490 ns |      - |       0 B |
|                                  ToVector3D |     1.2000 ns |  0.3464 ns |   0.9995 ns |     1.1598 ns |      - |       0 B |
|                                    ToVector |    34.7686 ns |  0.9669 ns |   2.8204 ns |    34.4566 ns | 0.0167 |      88 B |
