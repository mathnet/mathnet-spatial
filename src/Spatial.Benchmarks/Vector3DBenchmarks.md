``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410117 Hz, Resolution=293.2451 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|                 Method |          Mean |     Error |    StdDev |        Median |  Gen 0 | Allocated |
|----------------------- |--------------:|----------:|----------:|--------------:|-------:|----------:|
|                 Length |     1.0989 ns | 0.0050 ns | 0.0039 ns |     1.0984 ns |      - |       0 B |
|                  Parse |            NA |        NA |        NA |            NA |    N/A |       N/A |
|              Normalize |   136.0978 ns | 3.0187 ns | 5.0436 ns |   131.9405 ns |      - |       0 B |
|             DotProduct |     1.4328 ns | 0.2884 ns | 0.2962 ns |     1.3258 ns |      - |       0 B |
| OperatorMultiplyVector |     0.7207 ns | 0.0061 ns | 0.0047 ns |     0.7201 ns |      - |       0 B |
|            OperatorAdd |     1.5960 ns | 0.0084 ns | 0.0078 ns |     1.5928 ns |      - |       0 B |
|                ScaleBy |     1.7746 ns | 0.0046 ns | 0.0038 ns |     1.7752 ns |      - |       0 B |
| OperatorMultiplyDouble |     1.7620 ns | 0.0058 ns | 0.0046 ns |     1.7621 ns |      - |       0 B |
|      IsParallelToAngle |   335.4198 ns | 0.0934 ns | 0.0828 ns |   335.4110 ns |      - |       0 B |
|     IsParallelToDouble |   299.7251 ns | 0.1483 ns | 0.1238 ns |   299.6907 ns |      - |       0 B |
|                 Rotate | 1,201.8385 ns | 0.9987 ns | 0.7797 ns | 1,201.7425 ns | 0.4425 |    2320 B |
|                AngleTo |   327.9350 ns | 0.0837 ns | 0.0782 ns |   327.9405 ns |      - |       0 B |
|          SignedAngleTo |   921.8131 ns | 0.6306 ns | 0.5266 ns |   921.6602 ns |      - |       0 B |
|         OperatorEquals |     4.6718 ns | 0.0263 ns | 0.0233 ns |     4.6670 ns |      - |       0 B |
|                 Equals |    13.2884 ns | 0.2961 ns | 0.4340 ns |    12.9257 ns |      - |       0 B |

Benchmarks with issues:
  Vector3DBenchmarks.Parse: DefaultJob
