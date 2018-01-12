
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Processor=Intel Xeon CPU E3-1230 V2 3.30GHz, ProcessorCount=8
Frequency=3222681 Hz, Resolution=310.3006 ns, Timer=TSC
.NET Core SDK=2.2.0-preview1-007870
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Job-HHQCRL : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

RemoveOutliers=False  Platform=X64  Runtime=Core  
Server=True  Toolchain=.NET Core 2.0  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

                            Method |       Mean |     Error |    StdDev |     Median |                Op/s |   Scaled | ScaledSD |  Gen 0 | Allocated |
---------------------------------- |-----------:|----------:|----------:|-----------:|--------------------:|---------:|---------:|-------:|----------:|
              SerializeMessagePack |  0.0062 ns | 0.0004 ns | 0.0007 ns |  0.0060 ns |   162,570,979,237.3 |     1.00 |     0.00 | 0.0000 |       0 B |
            DeserializeMessagePack |  0.0006 ns | 0.0000 ns | 0.0000 ns |  0.0005 ns | 1,808,890,741,726.7 |     0.09 |     0.01 |      - |       0 B |
      SerializeMessagePackTypeless |  0.0080 ns | 0.0003 ns | 0.0004 ns |  0.0079 ns |   124,430,941,496.2 |     1.32 |     0.12 | 0.0000 |       0 B |
    DeserializeMessagePackTypeless |  0.0007 ns | 0.0000 ns | 0.0000 ns |  0.0007 ns | 1,440,234,408,053.8 |     0.11 |     0.01 |      - |       0 B |
   SerializeLz4MessagePackTypeless |  0.0479 ns | 0.0026 ns | 0.0038 ns |  0.0476 ns |    20,874,434,805.8 |     7.85 |     0.88 | 0.0000 |       0 B |
 DeserializeLz4MessagePackTypeless |  0.0008 ns | 0.0000 ns | 0.0000 ns |  0.0008 ns | 1,222,092,968,534.2 |     0.13 |     0.01 |      - |       0 B |
     SerializeMessagePackFormatter |  0.0083 ns | 0.0002 ns | 0.0003 ns |  0.0082 ns |   120,603,580,700.4 |     1.36 |     0.12 | 0.0000 |       0 B |
   DeserializeMessagePackFormatter |  0.0005 ns | 0.0000 ns | 0.0001 ns |  0.0005 ns | 2,107,078,687,309.4 |     0.08 |     0.01 |      - |       0 B |
                 SerializeUtf8Json |  0.0136 ns | 0.0001 ns | 0.0001 ns |  0.0136 ns |    73,334,561,410.7 |     2.24 |     0.18 | 0.0000 |       0 B |
               DeserializeUtf8Json |  0.0011 ns | 0.0000 ns | 0.0000 ns |  0.0011 ns |   896,354,764,354.5 |     0.18 |     0.01 |      - |       0 B |
        SerializeUtf8JsonFormatter |  0.0143 ns | 0.0001 ns | 0.0001 ns |  0.0143 ns |    70,039,654,219.9 |     2.34 |     0.19 | 0.0000 |       0 B |
      DeserializeUtf8JsonFormatter |  0.0016 ns | 0.0000 ns | 0.0000 ns |  0.0016 ns |   636,591,374,802.4 |     0.26 |     0.02 |      - |       0 B |
                  SerializeJsonNet |  0.0730 ns | 0.0019 ns | 0.0028 ns |  0.0725 ns |    13,699,254,401.7 |    11.97 |     1.05 | 0.0000 |       0 B |
                DeserializeJsonNet |  0.1239 ns | 0.0029 ns | 0.0043 ns |  0.1236 ns |     8,070,617,848.5 |    20.32 |     1.75 | 0.0000 |       0 B |
                 SerializeJsonNetX |  0.0971 ns | 0.0038 ns | 0.0057 ns |  0.0960 ns |    10,303,057,172.9 |    15.91 |     1.56 | 0.0000 |       0 B |
               DeserializeJsonNetX |  0.1490 ns | 0.0064 ns | 0.0096 ns |  0.1466 ns |     6,711,272,541.5 |    24.43 |     2.49 | 0.0000 |       0 B |
            SerializeJsonFormatter |  0.0844 ns | 0.0014 ns | 0.0021 ns |  0.0847 ns |    11,853,754,492.6 |    13.83 |     1.15 | 0.0000 |       0 B |
          DeserializeJsonFormatter |  0.0333 ns | 0.0012 ns | 0.0017 ns |  0.0326 ns |    30,034,249,781.4 |     5.46 |     0.52 | 0.0000 |       0 B |
              SerializeProtoBufNet |  0.0110 ns | 0.0001 ns | 0.0002 ns |  0.0110 ns |    90,527,890,787.7 |     1.81 |     0.15 | 0.0000 |       0 B |
            DeserializeProtoBufNet |  0.0091 ns | 0.0005 ns | 0.0008 ns |  0.0092 ns |   109,453,248,786.9 |     1.50 |     0.17 | 0.0000 |       0 B |
        SerializeProtoBufFormatter |  0.0146 ns | 0.0000 ns | 0.0000 ns |  0.0146 ns |    68,439,072,728.7 |     2.40 |     0.19 | 0.0000 |       0 B |
      DeserializeProtoBufFormatter |  0.0014 ns | 0.0000 ns | 0.0000 ns |  0.0013 ns |   734,244,847,349.7 |     0.22 |     0.02 | 0.0000 |       0 B |
                 SerializeHyperion |  0.0246 ns | 0.0006 ns | 0.0009 ns |  0.0248 ns |    40,631,681,883.5 |     4.04 |     0.35 | 0.0000 |       0 B |
               DeserializeHyperion |  0.0185 ns | 0.0008 ns | 0.0013 ns |  0.0181 ns |    53,975,873,523.3 |     3.04 |     0.31 | 0.0000 |       0 B |
        SerializeHyperionFormatter |  0.0213 ns | 0.0004 ns | 0.0006 ns |  0.0212 ns |    46,887,912,944.1 |     3.50 |     0.30 | 0.0000 |       0 B |
      DeserializeHyperionFormatter | 10.1263 ns | 0.1873 ns | 0.2803 ns | 10.0127 ns |        98,752,860.0 | 1,660.25 |   139.30 |      - |       1 B |
