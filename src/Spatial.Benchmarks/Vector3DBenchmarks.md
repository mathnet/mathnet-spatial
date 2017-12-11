``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                 Method |          Mean |      Error |     StdDev |        Median |  Gen 0 | Allocated |
|----------------------- |--------------:|-----------:|-----------:|--------------:|-------:|----------:|
|                 Length |     0.8711 ns |  0.3132 ns |  0.5147 ns |     0.5431 ns |      - |       0 B |
|                  Parse | 1,408.3713 ns |  0.9194 ns |  0.7178 ns | 1,408.3072 ns | 0.0916 |     488 B |
|              Normalize |   138.8195 ns |  3.0986 ns |  3.0432 ns |   137.2774 ns |      - |       0 B |
|             DotProduct |     1.8355 ns |  0.3327 ns |  0.8824 ns |     1.6489 ns |      - |       0 B |
| OperatorMultiplyVector |     0.8974 ns |  0.0062 ns |  0.0052 ns |     0.8978 ns |      - |       0 B |
|            OperatorAdd |     1.8159 ns |  0.3366 ns |  0.6239 ns |     1.9812 ns |      - |       0 B |
|                ScaleBy |     2.4423 ns |  0.2961 ns |  0.3168 ns |     2.3989 ns |      - |       0 B |
| OperatorMultiplyDouble |     2.0005 ns |  0.6226 ns |  0.5824 ns |     1.7851 ns |      - |       0 B |
|      IsParallelToAngle |   385.0084 ns |  9.2148 ns | 10.2422 ns |   384.3022 ns |      - |       0 B |
|     IsParallelToDouble |   330.7290 ns |  6.6276 ns | 13.2360 ns |   326.4269 ns |      - |       0 B |
|                 Rotate | 1,255.2868 ns |  0.2620 ns |  0.2046 ns | 1,255.2662 ns | 0.4425 |    2320 B |
|                AngleTo |   354.9510 ns |  7.2053 ns | 10.1008 ns |   353.8203 ns |      - |       0 B |
|          SignedAngleTo |   999.7607 ns | 20.0946 ns | 35.7181 ns | 1,007.3817 ns |      - |       0 B |
|         OperatorEquals |     5.0183 ns |  0.0921 ns |  0.0905 ns |     5.0080 ns |      - |       0 B |
|                 Equals |    13.4264 ns |  0.3009 ns |  0.2814 ns |    13.3187 ns |      - |       0 B |
