``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                                      Method |          Mean |      Error |     StdDev |        Median |  Gen 0 | Allocated |
|-------------------------------------------- |--------------:|-----------:|-----------:|--------------:|-------:|----------:|
|                                  Orthogonal |    90.9480 ns |  3.1700 ns |  2.9652 ns |    89.3699 ns |      - |       0 B |
|                                      Length |     0.4878 ns |  0.2883 ns |  0.6738 ns |     0.0000 ns |      - |       0 B |
|    OperatorEqualityUnitVector3DUnitVector3D |     1.5221 ns |  0.0019 ns |  0.0016 ns |     1.5216 ns |      - |       0 B |
|        OperatorEqualityUnitVector3DVector3D |     2.0801 ns |  0.0027 ns |  0.0023 ns |     2.0798 ns |      - |       0 B |
|    OperatorAdditionUnitVector3DUnitVector3D |     1.3789 ns |  0.3336 ns |  0.6266 ns |     1.0126 ns |      - |       0 B |
|        OperatorAdditionUnitVector3DVector3D |     1.8788 ns |  0.0058 ns |  0.0051 ns |     1.8770 ns |      - |       0 B |
|                         OperatorSubtraction |     1.4821 ns |  0.0098 ns |  0.0077 ns |     1.4804 ns |      - |       0 B |
| OperatorSubtractionUnitVector3DUnitVector3D |     1.6467 ns |  0.5133 ns |  0.6304 ns |     1.3304 ns |      - |       0 B |
|     OperatorSubtractionUnitVector3DVector3D |     1.9656 ns |  0.3094 ns |  0.2894 ns |     1.8504 ns |      - |       0 B |
|                       OperatorUnaryNegation |     1.2703 ns |  0.0243 ns |  0.0203 ns |     1.2658 ns |      - |       0 B |
|                            OperatorMultiply |     6.4691 ns |  0.4271 ns |  0.8530 ns |     5.8939 ns |      - |       0 B |
|                            OperatorDivision |     1.8296 ns |  0.0051 ns |  0.0042 ns |     1.8305 ns |      - |       0 B |
|    OperatorMultiplyUnitVector3DUnitVector3D |     6.3255 ns |  0.0158 ns |  0.0132 ns |     6.3204 ns |      - |       0 B |
|                                      Create |   143.2534 ns |  3.1810 ns |  5.8166 ns |   144.1365 ns |      - |       0 B |
|                                    OfVector |   142.8297 ns |  3.2054 ns |  4.6985 ns |   139.4553 ns |      - |       0 B |
|                                       Parse | 1,620.6525 ns | 32.3476 ns | 67.5215 ns | 1,604.4705 ns | 0.0916 |     488 B |
|                              EqualsVector3D |     1.9363 ns |  0.0031 ns |  0.0024 ns |     1.9355 ns |      - |       0 B |
|                          EqualsUnitVector3D |     1.6748 ns |  0.0704 ns |  0.1137 ns |     1.6873 ns |      - |       0 B |
|           EqualsUnitVector3DDoubleTolerance |    17.5688 ns |  0.3913 ns |  0.5486 ns |    17.5952 ns |      - |       0 B |
|               EqualsVector3DDoubleTolerance |    15.8372 ns |  0.3517 ns |  0.9078 ns |    15.7629 ns |      - |       0 B |
|                                     ScaleBy |     2.1798 ns |  0.3497 ns |  0.7965 ns |     2.2368 ns |      - |       0 B |
|                              ProjectOnPlane |   198.1866 ns |  3.9044 ns |  4.3397 ns |   194.9352 ns |      - |       0 B |
|                       ProjectOnUnitVector3D |     7.6031 ns |  0.0103 ns |  0.0081 ns |     7.6038 ns |      - |       0 B |
|         IsParallelToVector3DDoubleTolerance |   177.9741 ns |  3.5707 ns |  3.3400 ns |   178.1212 ns |      - |       0 B |
|     IsParallelToUnitVector3DDoubleTolerance |     7.4942 ns |  0.1855 ns |  0.4372 ns |     7.2344 ns |      - |       0 B |
|      IsParallelToUnitVector3DAngleTolerance |    20.9017 ns |  0.3063 ns |  0.2558 ns |    20.9394 ns |      - |       0 B |
|                        IsParallelToVector3D |   185.4114 ns |  3.6645 ns |  6.3211 ns |   180.7373 ns |      - |       0 B |
|                   IsPerpendicularToVector3D |   169.4077 ns |  3.4051 ns |  6.3116 ns |   169.8891 ns |      - |       0 B |
|                           IsPerpendicularTo |     6.1232 ns |  0.1660 ns |  0.3945 ns |     6.1050 ns |      - |       0 B |
|                                      Negate |    81.0375 ns |  0.0720 ns |  0.0601 ns |    81.0194 ns |      - |       0 B |
|                          DotProductVector3D |     1.1544 ns |  0.0091 ns |  0.0071 ns |     1.1519 ns |      - |       0 B |
|                      DotProductUnitVector3D |     5.4443 ns |  0.4185 ns |  0.6638 ns |     5.1687 ns |      - |       0 B |
|                                    Subtract |     1.2715 ns |  0.0073 ns |  0.0057 ns |     1.2704 ns |      - |       0 B |
|                                         Add |     2.2937 ns |  0.3460 ns |  0.9296 ns |     2.1832 ns |      - |       0 B |
|                                CrossProduct |   153.5568 ns |  2.2911 ns |  2.2502 ns |   153.0945 ns |      - |       0 B |
|                               SignedAngleTo |   830.5692 ns | 16.9199 ns | 36.4220 ns |   825.9335 ns |      - |       0 B |
|                             AngleToVector3D |   181.7327 ns |  3.4682 ns |  3.0745 ns |   182.3579 ns |      - |       0 B |
|                         AngleToUnitVector3D |    12.5936 ns |  0.5519 ns |  0.7554 ns |    12.5622 ns |      - |       0 B |
|                                      Rotate | 1,517.6872 ns | 28.9631 ns | 24.1855 ns | 1,506.0153 ns | 0.4425 |    2320 B |
|                                   ToPoint3D |     1.0232 ns |  0.3171 ns |  0.3393 ns |     0.9875 ns |      - |       0 B |
|                                  ToVector3D |     0.3735 ns |  0.0062 ns |  0.0048 ns |     0.3731 ns |      - |       0 B |
|                                    ToVector |    32.4290 ns |  0.7329 ns |  2.1610 ns |    32.1808 ns | 0.0167 |      88 B |
