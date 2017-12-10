``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                 Method |         Mean |      Error |     StdDev |  Gen 0 | Allocated |
|----------------------- |-------------:|-----------:|-----------:|-------:|----------:|
|                 Length |     1.090 ns |  0.3259 ns |  0.6200 ns |      - |       0 B |
|                  Parse | 1,475.740 ns | 29.3944 ns | 51.4819 ns | 0.0916 |     488 B |
|              Normalize |   143.067 ns |  3.1537 ns |  5.5234 ns |      - |       0 B |
|             DotProduct |     1.169 ns |  0.3321 ns |  0.8269 ns |      - |       0 B |
| OperatorMultiplyVector |     1.315 ns |  0.3356 ns |  0.7295 ns |      - |       0 B |
|            OperatorAdd |     2.883 ns |  0.3521 ns |  0.9336 ns |      - |       0 B |
|                ScaleBy |     2.503 ns |  0.3474 ns |  0.8390 ns |      - |       0 B |
| OperatorMultiplyDouble |     1.783 ns |  0.3445 ns |  0.6881 ns |      - |       0 B |
|      IsParallelToAngle |   373.480 ns |  7.4813 ns | 16.2638 ns |      - |       0 B |
|     IsParallelToDouble |   331.186 ns |  6.6274 ns | 12.9262 ns |      - |       0 B |
|                 Rotate | 1,362.064 ns | 27.4617 ns | 59.1143 ns | 0.4425 |    2320 B |
|                AngleTo |   364.268 ns |  7.6067 ns | 18.3709 ns |      - |       0 B |
|          SignedAngleTo | 1,041.950 ns | 20.9388 ns | 59.3999 ns |      - |       0 B |
|         OperatorEquals |     5.333 ns |  0.1481 ns |  0.4319 ns |      - |       0 B |
|                 Equals |    14.338 ns |  0.3266 ns |  0.8371 ns |      - |       0 B |
