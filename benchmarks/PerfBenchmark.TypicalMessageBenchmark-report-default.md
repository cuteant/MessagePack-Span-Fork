
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Processor=Intel Xeon CPU E3-1230 V2 3.30GHz, ProcessorCount=8
Frequency=3222681 Hz, Resolution=310.3006 ns, Timer=TSC
.NET Core SDK=2.2.0-preview1-007946
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Job-JKQNFX : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

RemoveOutliers=False  Platform=X64  Runtime=Core  
Server=True  Toolchain=.NET Core 2.0  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

                            Method |      Mean |     Error |    StdDev |    Median |              Op/s | Scaled | ScaledSD |  Gen 0 | Allocated |
---------------------------------- |----------:|----------:|----------:|----------:|------------------:|-------:|---------:|-------:|----------:|
              SerializeMessagePack | 0.0052 ns | 0.0005 ns | 0.0007 ns | 0.0049 ns | 192,539,477,752.8 |   1.00 |     0.00 | 0.0000 |       0 B |
            DeserializeMessagePack | 0.0062 ns | 0.0003 ns | 0.0005 ns | 0.0061 ns | 160,469,842,444.5 |   1.22 |     0.15 | 0.0000 |       0 B |
      SerializeMessagePackTypeless | 0.0065 ns | 0.0000 ns | 0.0001 ns | 0.0065 ns | 154,429,670,652.8 |   1.26 |     0.13 | 0.0000 |       0 B |
    DeserializeMessagePackTypeless | 0.0088 ns | 0.0003 ns | 0.0004 ns | 0.0087 ns | 113,238,976,151.7 |   1.72 |     0.19 | 0.0000 |       0 B |
   SerializeLz4MessagePackTypeless | 0.0386 ns | 0.0033 ns | 0.0049 ns | 0.0378 ns |  25,937,509,818.6 |   7.52 |     1.20 | 0.0000 |       0 B |
 DeserializeLz4MessagePackTypeless | 0.0107 ns | 0.0008 ns | 0.0012 ns | 0.0104 ns |  93,129,128,243.7 |   2.09 |     0.31 | 0.0000 |       0 B |
     SerializeMessagePackFormatter | 0.0068 ns | 0.0003 ns | 0.0005 ns | 0.0066 ns | 146,224,485,747.3 |   1.33 |     0.16 | 0.0000 |       0 B |
   DeserializeMessagePackFormatter | 0.0088 ns | 0.0005 ns | 0.0008 ns | 0.0085 ns | 113,203,729,952.9 |   1.72 |     0.23 | 0.0000 |       0 B |
                 SerializeUtf8Json | 0.0090 ns | 0.0006 ns | 0.0008 ns | 0.0087 ns | 110,648,270,852.4 |   1.76 |     0.24 | 0.0000 |       0 B |
               DeserializeUtf8Json | 0.0166 ns | 0.0002 ns | 0.0004 ns | 0.0165 ns |  60,410,839,517.9 |   3.23 |     0.33 | 0.0000 |       0 B |
        SerializeUtf8JsonFormatter | 0.0094 ns | 0.0000 ns | 0.0001 ns | 0.0094 ns | 106,581,284,066.5 |   1.83 |     0.18 | 0.0000 |       0 B |
      DeserializeUtf8JsonFormatter | 0.0176 ns | 0.0006 ns | 0.0010 ns | 0.0174 ns |  56,958,391,201.0 |   3.42 |     0.39 | 0.0000 |       0 B |
                  SerializeJsonNet | 0.0569 ns | 0.0034 ns | 0.0051 ns | 0.0559 ns |  17,573,311,988.7 |  11.10 |     1.47 | 0.0000 |       0 B |
                DeserializeJsonNet | 0.1327 ns | 0.0099 ns | 0.0148 ns | 0.1341 ns |   7,533,165,735.6 |  25.89 |     3.85 | 0.0000 |       0 B |
                 SerializeJsonNetX | 0.0952 ns | 0.0075 ns | 0.0112 ns | 0.0911 ns |  10,501,733,005.5 |  18.57 |     2.84 | 0.0000 |       0 B |
               DeserializeJsonNetX | 0.1687 ns | 0.0250 ns | 0.0375 ns | 0.1560 ns |   5,927,454,233.8 |  32.90 |     7.93 | 0.0000 |       0 B |
            SerializeJsonFormatter | 0.0961 ns | 0.0117 ns | 0.0175 ns | 0.0889 ns |  10,408,323,066.7 |  18.74 |     3.86 | 0.0000 |       0 B |
          DeserializeJsonFormatter | 0.1531 ns | 0.0023 ns | 0.0035 ns | 0.1528 ns |   6,530,183,820.5 |  29.86 |     3.05 | 0.0000 |       0 B |
              SerializeProtoBufNet | 0.0119 ns | 0.0010 ns | 0.0015 ns | 0.0111 ns |  84,223,431,149.7 |   2.32 |     0.37 | 0.0000 |       0 B |
            DeserializeProtoBufNet | 0.0243 ns | 0.0018 ns | 0.0027 ns | 0.0233 ns |  41,152,410,589.1 |   4.74 |     0.71 | 0.0000 |       0 B |
        SerializeProtoBufFormatter | 0.0141 ns | 0.0004 ns | 0.0006 ns | 0.0140 ns |  70,949,767,585.8 |   2.75 |     0.30 | 0.0000 |       0 B |
      DeserializeProtoBufFormatter | 0.0269 ns | 0.0053 ns | 0.0080 ns | 0.0222 ns |  37,193,454,292.8 |   5.24 |     1.62 | 0.0000 |       0 B |
                 SerializeHyperion | 0.0212 ns | 0.0036 ns | 0.0053 ns | 0.0181 ns |  47,128,535,219.0 |   4.14 |     1.11 | 0.0000 |       0 B |
               DeserializeHyperion | 0.0170 ns | 0.0003 ns | 0.0005 ns | 0.0169 ns |  58,902,241,362.1 |   3.31 |     0.34 | 0.0000 |       0 B |
        SerializeHyperionFormatter | 0.0210 ns | 0.0015 ns | 0.0023 ns | 0.0199 ns |  47,720,072,862.0 |   4.09 |     0.60 | 0.0000 |       0 B |
      DeserializeHyperionFormatter | 0.0181 ns | 0.0014 ns | 0.0020 ns | 0.0176 ns |  55,228,574,662.3 |   3.53 |     0.53 | 0.0000 |       0 B |
