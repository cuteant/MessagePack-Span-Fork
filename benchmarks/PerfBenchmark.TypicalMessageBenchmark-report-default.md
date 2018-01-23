
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Processor=Intel Core i5-3450 CPU 3.10GHz (Ivy Bridge), ProcessorCount=4
Frequency=3027368 Hz, Resolution=330.3199 ns, Timer=TSC
.NET Core SDK=2.2.0-preview1-008000
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Job-ZKXDFU : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

RemoveOutliers=False  Platform=X64  Runtime=Core  
Server=True  Toolchain=.NET Core 2.0  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

                            Method |      Mean |     Error |    StdDev |    Median |              Op/s | Scaled | ScaledSD |  Gen 0 | Allocated |
---------------------------------- |----------:|----------:|----------:|----------:|------------------:|-------:|---------:|-------:|----------:|
              SerializeMessagePack | 0.0050 ns | 0.0000 ns | 0.0000 ns | 0.0050 ns | 200,887,588,501.5 |   1.00 |     0.00 | 0.0000 |       0 B |
    SerializeMessagePackNonGeneric | 0.0061 ns | 0.0001 ns | 0.0001 ns | 0.0060 ns | 164,706,930,807.6 |   1.22 |     0.02 | 0.0000 |       0 B |
            DeserializeMessagePack | 0.0063 ns | 0.0001 ns | 0.0001 ns | 0.0062 ns | 159,459,565,643.8 |   1.26 |     0.02 | 0.0000 |       0 B |
  DeserializeMessagePackNonGeneric | 0.0068 ns | 0.0001 ns | 0.0001 ns | 0.0067 ns | 147,985,370,171.2 |   1.36 |     0.02 | 0.0000 |       0 B |
      SerializeMessagePackTypeless | 0.0066 ns | 0.0000 ns | 0.0000 ns | 0.0066 ns | 151,000,753,854.6 |   1.33 |     0.01 | 0.0000 |       0 B |
    DeserializeMessagePackTypeless | 0.0086 ns | 0.0001 ns | 0.0001 ns | 0.0086 ns | 116,560,149,016.9 |   1.72 |     0.03 | 0.0000 |       0 B |
   SerializeLz4MessagePackTypeless | 0.0380 ns | 0.0003 ns | 0.0005 ns | 0.0378 ns |  26,328,999,897.2 |   7.63 |     0.11 | 0.0000 |       0 B |
 DeserializeLz4MessagePackTypeless | 0.0100 ns | 0.0001 ns | 0.0002 ns | 0.0100 ns |  99,836,811,967.7 |   2.01 |     0.05 | 0.0000 |       0 B |
     SerializeMessagePackFormatter | 0.0053 ns | 0.0000 ns | 0.0000 ns | 0.0053 ns | 188,562,424,213.5 |   1.07 |     0.01 | 0.0000 |       0 B |
   DeserializeMessagePackFormatter | 0.0042 ns | 0.0000 ns | 0.0001 ns | 0.0042 ns | 237,252,305,970.8 |   0.85 |     0.01 | 0.0000 |       0 B |
                 SerializeUtf8Json | 0.0092 ns | 0.0001 ns | 0.0001 ns | 0.0092 ns | 108,340,789,255.8 |   1.85 |     0.03 | 0.0000 |       0 B |
               DeserializeUtf8Json | 0.0175 ns | 0.0002 ns | 0.0003 ns | 0.0173 ns |  57,297,759,870.6 |   3.51 |     0.07 | 0.0000 |       0 B |
        SerializeUtf8JsonFormatter | 0.0109 ns | 0.0001 ns | 0.0002 ns | 0.0108 ns |  91,795,615,544.5 |   2.19 |     0.04 | 0.0000 |       0 B |
      DeserializeUtf8JsonFormatter | 0.0183 ns | 0.0001 ns | 0.0002 ns | 0.0183 ns |  54,636,869,888.5 |   3.68 |     0.05 | 0.0000 |       0 B |
                  SerializeJsonNet | 0.0606 ns | 0.0002 ns | 0.0003 ns | 0.0605 ns |  16,514,929,062.6 |  12.16 |     0.12 | 0.0000 |       0 B |
                DeserializeJsonNet | 0.1262 ns | 0.0005 ns | 0.0008 ns | 0.1263 ns |   7,926,620,923.2 |  25.35 |     0.26 | 0.0000 |       0 B |
                 SerializeJsonNetX | 0.0883 ns | 0.0009 ns | 0.0013 ns | 0.0884 ns |  11,323,214,238.0 |  17.74 |     0.30 | 0.0000 |       0 B |
               DeserializeJsonNetX | 0.1459 ns | 0.0008 ns | 0.0012 ns | 0.1455 ns |   6,854,724,477.3 |  29.31 |     0.34 | 0.0000 |       0 B |
            SerializeJsonFormatter | 0.0879 ns | 0.0007 ns | 0.0010 ns | 0.0876 ns |  11,377,222,332.4 |  17.66 |     0.25 | 0.0000 |       0 B |
          DeserializeJsonFormatter | 0.1571 ns | 0.0008 ns | 0.0011 ns | 0.1566 ns |   6,366,342,827.1 |  31.56 |     0.34 | 0.0000 |       0 B |
              SerializeProtoBufNet | 0.0115 ns | 0.0001 ns | 0.0001 ns | 0.0115 ns |  86,928,004,905.8 |   2.31 |     0.03 | 0.0000 |       0 B |
            DeserializeProtoBufNet | 0.0251 ns | 0.0007 ns | 0.0011 ns | 0.0256 ns |  39,887,295,017.8 |   5.04 |     0.22 | 0.0000 |       0 B |
        SerializeProtoBufFormatter | 0.0146 ns | 0.0000 ns | 0.0001 ns | 0.0146 ns |  68,719,195,333.8 |   2.92 |     0.03 | 0.0000 |       0 B |
      DeserializeProtoBufFormatter | 0.0242 ns | 0.0005 ns | 0.0008 ns | 0.0238 ns |  41,334,099,295.8 |   4.86 |     0.17 | 0.0000 |       0 B |
                 SerializeHyperion | 0.0155 ns | 0.0001 ns | 0.0001 ns | 0.0154 ns |  64,621,036,452.7 |   3.11 |     0.04 | 0.0000 |       0 B |
               DeserializeHyperion | 0.0164 ns | 0.0001 ns | 0.0001 ns | 0.0165 ns |  60,902,278,419.0 |   3.30 |     0.04 | 0.0000 |       0 B |
        SerializeHyperionFormatter | 0.0188 ns | 0.0001 ns | 0.0001 ns | 0.0188 ns |  53,280,458,210.4 |   3.77 |     0.04 | 0.0000 |       0 B |
      DeserializeHyperionFormatter | 0.0190 ns | 0.0001 ns | 0.0001 ns | 0.0190 ns |  52,752,982,981.6 |   3.81 |     0.04 | 0.0000 |       0 B |
