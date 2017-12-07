``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410117 Hz, Resolution=293.2451 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|              Method |         Mean |     Error |    StdDev |        Median |  Gen 0 | Allocated |
|-------------------- |-------------:|----------:|----------:|--------------:|-------:|----------:|
|    OperatorAddition |     1.785 ns | 0.0048 ns | 0.0045 ns |     1.7858 ns |      - |       0 B |
| OperatorSubtraction |     1.649 ns | 0.0058 ns | 0.0054 ns |     1.6476 ns |      - |       0 B |
|    OperatorEquality |     2.609 ns | 0.0873 ns | 0.1639 ns |     2.4993 ns |      - |       0 B |
|         MirrorAbout |    48.330 ns | 0.0578 ns | 0.0452 ns |    48.3189 ns |      - |       0 B |
|           ProjectOn |    20.296 ns | 0.0221 ns | 0.0206 ns |    20.2938 ns |      - |       0 B |
|              Rotate | 1,240.921 ns | 2.3886 ns | 2.2343 ns | 1,240.1585 ns | 0.4215 |    2216 B |
|            VectorTo |     1.768 ns | 0.0062 ns | 0.0058 ns |     1.7659 ns |      - |       0 B |
|          DistanceTo |     1.659 ns | 0.0055 ns | 0.0051 ns |     1.6577 ns |      - |       0 B |
|          ToVector3D |     1.167 ns | 0.3523 ns | 0.5381 ns |     0.8567 ns |      - |       0 B |
|            ToVector |    29.583 ns | 0.0072 ns | 0.0064 ns |    29.5818 ns | 0.0167 |      88 B |
|              Equals |     2.649 ns | 0.0918 ns | 0.1402 ns |     2.5716 ns |      - |       0 B |
| EqualsWithTolerance |    15.465 ns | 0.0380 ns | 0.0355 ns |    15.4664 ns |      - |       0 B |
