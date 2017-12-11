``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                       Method |       Mean |      Error |     StdDev |     Median |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|----------------------------- |-----------:|-----------:|-----------:|-----------:|---------:|---------:|---------:|----------:|
|    GetConvexHullFromPoints10 |   3.617 us |  0.0724 us |  0.2054 us |   3.464 us |   0.5646 |        - |        - |   2.89 KB |
|   GetConvexHullFromPoints100 |   9.892 us |  0.1969 us |  0.4403 us |   9.907 us |   1.7395 |   0.0153 |        - |   8.95 KB |
|  GetConvexHullFromPoints1000 |  64.748 us |  1.2914 us |  3.6213 us |  62.101 us |  10.2539 |   0.4883 |        - |  52.83 KB |
| GetConvexHullFromPoints10000 | 758.106 us | 14.7189 us | 16.9504 us | 749.307 us | 166.0156 | 166.0156 | 166.0156 | 675.06 KB |
