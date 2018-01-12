
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Processor=Intel Core i5-3450 CPU 3.10GHz (Ivy Bridge), ProcessorCount=4
Frequency=3027365 Hz, Resolution=330.3203 ns, Timer=TSC
.NET Core SDK=2.2.0-preview1-007870
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Job-ZTAHSG : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

RemoveOutliers=False  Platform=X64  Runtime=Core  
Server=True  Toolchain=.NET Core 2.0  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

                            Method |      Mean |     Error |    StdDev |    Median |              Op/s | Scaled | ScaledSD |  Gen 0 | Allocated |
---------------------------------- |----------:|----------:|----------:|----------:|------------------:|-------:|---------:|-------:|----------:|
              SerializeMessagePack | 0.0060 ns | 0.0001 ns | 0.0001 ns | 0.0060 ns | 165,492,773,147.2 |   1.00 |     0.00 | 0.0000 |       0 B |
            DeserializeMessagePack | 0.0068 ns | 0.0001 ns | 0.0002 ns | 0.0069 ns | 147,351,739,584.4 |   1.12 |     0.03 | 0.0000 |       0 B |
      SerializeMessagePackTypeless | 0.0087 ns | 0.0001 ns | 0.0002 ns | 0.0087 ns | 114,983,562,025.8 |   1.44 |     0.04 | 0.0000 |       0 B |
    DeserializeMessagePackTypeless | 0.0085 ns | 0.0002 ns | 0.0003 ns | 0.0085 ns | 118,112,905,655.3 |   1.40 |     0.05 | 0.0000 |       0 B |
   SerializeLz4MessagePackTypeless | 0.0383 ns | 0.0005 ns | 0.0007 ns | 0.0380 ns |  26,120,657,577.8 |   6.34 |     0.17 | 0.0000 |       0 B |
 DeserializeLz4MessagePackTypeless | 0.0099 ns | 0.0001 ns | 0.0001 ns | 0.0099 ns | 100,798,851,612.3 |   1.64 |     0.03 | 0.0000 |       0 B |
     SerializeMessagePackFormatter | 0.0088 ns | 0.0001 ns | 0.0001 ns | 0.0088 ns | 113,858,026,292.0 |   1.45 |     0.03 | 0.0000 |       0 B |
   DeserializeMessagePackFormatter | 0.0102 ns | 0.0011 ns | 0.0017 ns | 0.0093 ns |  98,451,119,577.8 |   1.68 |     0.28 | 0.0000 |       0 B |
                 SerializeUtf8Json | 0.0137 ns | 0.0000 ns | 0.0001 ns | 0.0137 ns |  72,882,774,150.5 |   2.27 |     0.04 | 0.0000 |       0 B |
               DeserializeUtf8Json | 0.0185 ns | 0.0001 ns | 0.0002 ns | 0.0184 ns |  54,194,643,555.5 |   3.05 |     0.06 | 0.0000 |       0 B |
        SerializeUtf8JsonFormatter | 0.0144 ns | 0.0000 ns | 0.0001 ns | 0.0144 ns |  69,348,270,982.8 |   2.39 |     0.05 | 0.0000 |       0 B |
      DeserializeUtf8JsonFormatter | 0.0192 ns | 0.0001 ns | 0.0002 ns | 0.0192 ns |  52,033,361,538.0 |   3.18 |     0.06 | 0.0000 |       0 B |
                  SerializeJsonNet | 0.0574 ns | 0.0005 ns | 0.0007 ns | 0.0575 ns |  17,428,112,395.2 |   9.50 |     0.21 | 0.0000 |       0 B |
                DeserializeJsonNet | 0.1216 ns | 0.0004 ns | 0.0007 ns | 0.1214 ns |   8,226,099,586.4 |  20.13 |     0.39 | 0.0000 |       0 B |
                 SerializeJsonNetX | 0.0849 ns | 0.0004 ns | 0.0006 ns | 0.0848 ns |  11,772,505,792.9 |  14.06 |     0.28 | 0.0000 |       0 B |
               DeserializeJsonNetX | 0.1427 ns | 0.0014 ns | 0.0021 ns | 0.1424 ns |   7,005,320,619.6 |  23.63 |     0.56 | 0.0000 |       0 B |
            SerializeJsonFormatter | 0.0885 ns | 0.0011 ns | 0.0016 ns | 0.0888 ns |  11,296,611,441.2 |  14.66 |     0.38 | 0.0000 |       0 B |
          DeserializeJsonFormatter | 0.1557 ns | 0.0004 ns | 0.0007 ns | 0.1556 ns |   6,423,753,825.0 |  25.77 |     0.50 | 0.0000 |       0 B |
              SerializeProtoBufNet | 0.0116 ns | 0.0001 ns | 0.0001 ns | 0.0116 ns |  86,176,928,001.0 |   1.92 |     0.04 | 0.0000 |       0 B |
            DeserializeProtoBufNet | 0.0227 ns | 0.0002 ns | 0.0003 ns | 0.0226 ns |  44,136,078,508.3 |   3.75 |     0.08 | 0.0000 |       0 B |
        SerializeProtoBufFormatter | 0.0150 ns | 0.0001 ns | 0.0001 ns | 0.0150 ns |  66,765,055,895.6 |   2.48 |     0.05 | 0.0000 |       0 B |
      DeserializeProtoBufFormatter | 0.0232 ns | 0.0001 ns | 0.0002 ns | 0.0231 ns |  43,185,141,942.7 |   3.83 |     0.08 | 0.0000 |       0 B |
                 SerializeHyperion | 0.0170 ns | 0.0001 ns | 0.0002 ns | 0.0170 ns |  58,880,321,233.2 |   2.81 |     0.06 | 0.0000 |       0 B |
               DeserializeHyperion | 0.0162 ns | 0.0001 ns | 0.0001 ns | 0.0162 ns |  61,839,826,113.4 |   2.68 |     0.05 | 0.0000 |       0 B |
        SerializeHyperionFormatter | 0.0207 ns | 0.0002 ns | 0.0002 ns | 0.0207 ns |  48,377,608,145.6 |   3.42 |     0.08 | 0.0000 |       0 B |
      DeserializeHyperionFormatter | 0.0180 ns | 0.0003 ns | 0.0004 ns | 0.0178 ns |  55,472,028,043.6 |   2.98 |     0.09 | 0.0000 |       0 B |
