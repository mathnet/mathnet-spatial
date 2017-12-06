``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410117 Hz, Resolution=293.2451 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                 Method |         Mean |      Error |     StdDev |        Median |  Gen 0 | Allocated |
|----------------------- |-------------:|-----------:|-----------:|--------------:|-------:|----------:|
|                 Length |     1.385 ns |  0.3264 ns |  0.6209 ns |     1.5461 ns |      - |       0 B |
|              Normalize |   142.580 ns |  3.1340 ns |  6.6107 ns |   140.3477 ns |      - |       0 B |
|             DotProduct |     1.086 ns |  0.3295 ns |  0.7636 ns |     0.9842 ns |      - |       0 B |
| OperatorMultiplyVector |     1.011 ns |  0.1969 ns |  0.1644 ns |     1.0081 ns |      - |       0 B |
|            OperatorAdd |     2.615 ns |  0.4627 ns |  0.4752 ns |     2.4897 ns |      - |       0 B |
|                ScaleBy |     1.557 ns |  0.0487 ns |  0.0380 ns |     1.5513 ns |      - |       0 B |
| OperatorMultiplyDouble |     1.420 ns |  0.0053 ns |  0.0045 ns |     1.4193 ns |      - |       0 B |
|      IsParallelToAngle |   362.248 ns |  7.1844 ns | 16.2165 ns |   363.1630 ns |      - |       0 B |
|     IsParallelToDouble |   321.965 ns |  6.4002 ns | 11.5408 ns |   323.9992 ns |      - |       0 B |
|                 Rotate | 1,318.857 ns | 26.5503 ns | 61.0037 ns | 1,316.5528 ns | 0.4425 |    2320 B |
|                AngleTo |   342.210 ns |  7.1548 ns | 10.4874 ns |   335.2963 ns |      - |       0 B |
|          SignedAngleTo |   928.628 ns | 18.8798 ns | 47.0172 ns |   900.6708 ns |      - |       0 B |
|         OperatorEquals |     4.704 ns |  0.1312 ns |  0.2298 ns |     4.7116 ns |      - |       0 B |
|                 Equals |    14.277 ns |  0.3182 ns |  0.6712 ns |    14.3222 ns |      - |       0 B |
