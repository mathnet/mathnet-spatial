``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410117 Hz, Resolution=293.2451 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2117.0


```
|              Method |        Mean |     Error |    StdDev |      Median |  Gen 0 | Allocated |
|-------------------- |------------:|----------:|----------:|------------:|-------:|----------:|
|    OperatorAddition |   0.1625 ns | 0.1318 ns | 0.3472 ns |   0.0000 ns |      - |       0 B |
| OperatorSubtraction |   0.2795 ns | 0.2971 ns | 0.2779 ns |   0.1248 ns |      - |       0 B |
|    OperatorEquality |   1.4927 ns | 0.0075 ns | 0.0070 ns |   1.4917 ns |      - |       0 B |
|            VectorTo |   0.1640 ns | 0.2282 ns | 0.2716 ns |   0.0000 ns |      - |       0 B |
|          DistanceTo |   1.3638 ns | 0.0184 ns | 0.0173 ns |   1.3692 ns |      - |       0 B |
|          ToVector2D |   1.0911 ns | 0.3104 ns | 0.3187 ns |   0.9217 ns |      - |       0 B |
|           ToPoint3D |   0.8651 ns | 0.0046 ns | 0.0041 ns |   0.8647 ns |      - |       0 B |
|            ToVector |  21.5281 ns | 0.4891 ns | 0.8566 ns |  21.2334 ns | 0.0152 |      80 B |
|            ToString | 473.6381 ns | 0.3264 ns | 0.2725 ns | 473.5192 ns | 0.0110 |      60 B |
|              Equals |   1.5096 ns | 0.0022 ns | 0.0017 ns |   1.5090 ns |      - |       0 B |
| EqualsWithTolerance |   2.9489 ns | 0.0546 ns | 0.0511 ns |   2.9569 ns |      - |       0 B |
