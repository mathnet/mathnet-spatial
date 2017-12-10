``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                       Method |          Mean |       Error |      StdDev |        Median |    Gen 0 |    Gen 1 |    Gen 2 |  Allocated |
|----------------------------- |--------------:|------------:|------------:|--------------:|---------:|---------:|---------:|-----------:|
|    GetConvexHullFromPoints10 |      4.505 us |   0.0896 us |   0.1706 us |      4.551 us |   0.6180 |        - |        - |     3.2 KB |
|   GetConvexHullFromPoints100 |     43.881 us |   0.8662 us |   2.3270 us |     44.133 us |   2.9297 |        - |        - |   15.19 KB |
|  GetConvexHullFromPoints1000 |    576.279 us |  11.5100 us |  29.7110 us |    556.423 us |  25.3906 |   0.9766 |        - |  134.86 KB |
| GetConvexHullFromPoints10000 | 33,014.561 us | 617.7802 us | 515.8744 us | 33,155.430 us | 250.0000 | 125.0000 | 125.0000 | 1233.61 KB |
