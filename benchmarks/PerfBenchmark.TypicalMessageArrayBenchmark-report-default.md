
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Processor=Intel Core i5-3450 CPU 3.10GHz (Ivy Bridge), ProcessorCount=4
Frequency=3027365 Hz, Resolution=330.3203 ns, Timer=TSC
.NET Core SDK=2.2.0-preview1-007870
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Job-EIBVRF : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

RemoveOutliers=False  Platform=X64  Runtime=Core  
Server=True  Toolchain=.NET Core 2.0  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

                            Method |      Mean |     Error |    StdDev |    Median |            Op/s | Scaled | ScaledSD |  Gen 0 | Allocated |
---------------------------------- |----------:|----------:|----------:|----------:|----------------:|-------:|---------:|-------:|----------:|
              SerializeMessagePack | 0.5140 ns | 0.0046 ns | 0.0069 ns | 0.5134 ns | 1,945,651,396.7 |   1.00 |     0.00 | 0.0000 |       0 B |
            DeserializeMessagePack | 0.8133 ns | 0.0069 ns | 0.0104 ns | 0.8165 ns | 1,229,563,128.9 |   1.58 |     0.03 | 0.0000 |       0 B |
      SerializeMessagePackTypeless | 0.5244 ns | 0.0088 ns | 0.0131 ns | 0.5253 ns | 1,906,915,159.9 |   1.02 |     0.03 | 0.0000 |       0 B |
    DeserializeMessagePackTypeless | 0.8409 ns | 0.0135 ns | 0.0202 ns | 0.8349 ns | 1,189,228,391.5 |   1.64 |     0.04 | 0.0000 |       0 B |
   SerializeLz4MessagePackTypeless | 0.8168 ns | 0.0126 ns | 0.0188 ns | 0.8183 ns | 1,224,332,147.7 |   1.59 |     0.04 | 0.0000 |       0 B |
 DeserializeLz4MessagePackTypeless | 0.8711 ns | 0.0116 ns | 0.0173 ns | 0.8683 ns | 1,147,914,734.9 |   1.70 |     0.04 | 0.0000 |       0 B |
     SerializeMessagePackFormatter | 0.5203 ns | 0.0057 ns | 0.0086 ns | 0.5196 ns | 1,921,896,071.5 |   1.01 |     0.02 | 0.0000 |       0 B |
   DeserializeMessagePackFormatter | 0.8186 ns | 0.0105 ns | 0.0158 ns | 0.8125 ns | 1,221,633,733.7 |   1.59 |     0.04 | 0.0000 |       0 B |
                 SerializeUtf8Json | 1.2993 ns | 0.0146 ns | 0.0218 ns | 1.2921 ns |   769,650,257.4 |   2.53 |     0.05 | 0.0000 |       0 B |
               DeserializeUtf8Json | 2.1272 ns | 0.0178 ns | 0.0267 ns | 2.1228 ns |   470,095,196.1 |   4.14 |     0.07 | 0.0000 |       0 B |
        SerializeUtf8JsonFormatter | 1.2899 ns | 0.0046 ns | 0.0068 ns | 1.2883 ns |   775,235,120.7 |   2.51 |     0.04 | 0.0000 |       0 B |
      DeserializeUtf8JsonFormatter | 2.1107 ns | 0.0057 ns | 0.0086 ns | 2.1086 ns |   473,768,373.9 |   4.11 |     0.06 | 0.0000 |       0 B |
                  SerializeJsonNet | 4.2483 ns | 0.0401 ns | 0.0600 ns | 4.2236 ns |   235,388,240.2 |   8.27 |     0.16 | 0.0000 |       2 B |
                DeserializeJsonNet | 8.1695 ns | 0.1078 ns | 0.1614 ns | 8.0719 ns |   122,407,228.4 |  15.90 |     0.37 | 0.0000 |       1 B |
                 SerializeJsonNetX | 4.4221 ns | 0.0249 ns | 0.0372 ns | 4.4343 ns |   226,134,901.4 |   8.61 |     0.13 | 0.0000 |       1 B |
               DeserializeJsonNetX | 8.5366 ns | 0.0204 ns | 0.0305 ns | 8.5273 ns |   117,143,114.4 |  16.61 |     0.23 | 0.0000 |       1 B |
            SerializeJsonFormatter | 4.6506 ns | 0.0490 ns | 0.0733 ns | 4.6204 ns |   215,024,560.7 |   9.05 |     0.18 | 0.0000 |       1 B |
          DeserializeJsonFormatter | 8.3698 ns | 0.0273 ns | 0.0409 ns | 8.3772 ns |   119,476,756.6 |  16.29 |     0.23 | 0.0000 |       1 B |
              SerializeProtoBufNet | 1.8683 ns | 0.0266 ns | 0.0398 ns | 1.8725 ns |   535,235,520.9 |   3.64 |     0.09 | 0.0000 |       1 B |
            DeserializeProtoBufNet | 2.1352 ns | 0.0392 ns | 0.0586 ns | 2.1586 ns |   468,334,846.8 |   4.16 |     0.12 | 0.0000 |       0 B |
        SerializeProtoBufFormatter | 1.8501 ns | 0.0219 ns | 0.0328 ns | 1.8308 ns |   540,518,663.7 |   3.60 |     0.08 | 0.0000 |       0 B |
      DeserializeProtoBufFormatter | 2.0300 ns | 0.0089 ns | 0.0133 ns | 2.0250 ns |   492,610,081.2 |   3.95 |     0.06 | 0.0000 |       0 B |
                 SerializeHyperion | 1.2132 ns | 0.0066 ns | 0.0098 ns | 1.2116 ns |   824,235,053.6 |   2.36 |     0.04 | 0.0000 |       1 B |
               DeserializeHyperion | 0.7945 ns | 0.0463 ns | 0.0693 ns | 0.8391 ns | 1,258,654,712.1 |   1.55 |     0.13 | 0.0000 |       1 B |
        SerializeHyperionFormatter | 1.1547 ns | 0.0074 ns | 0.0110 ns | 1.1519 ns |   866,006,594.7 |   2.25 |     0.04 | 0.0000 |       1 B |
      DeserializeHyperionFormatter | 0.7389 ns | 0.0346 ns | 0.0518 ns | 0.7052 ns | 1,353,342,044.1 |   1.44 |     0.10 | 0.0000 |       1 B |
