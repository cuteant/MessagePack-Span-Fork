
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Processor=Intel Xeon CPU E3-1230 V2 3.30GHz, ProcessorCount=8
Frequency=3222681 Hz, Resolution=310.3006 ns, Timer=TSC
.NET Core SDK=2.2.0-preview1-007870
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Job-KFQOSF : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

RemoveOutliers=False  Platform=X64  Runtime=Core  
Server=True  Toolchain=.NET Core 2.0  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

                            Method |      Mean |     Error |    StdDev |            Op/s | Scaled | ScaledSD |  Gen 0 | Allocated |
---------------------------------- |----------:|----------:|----------:|----------------:|-------:|---------:|-------:|----------:|
              SerializeMessagePack | 0.3975 ns | 0.0306 ns | 0.0458 ns | 2,515,654,804.2 |   1.00 |     0.00 | 0.0000 |       0 B |
            DeserializeMessagePack | 0.8160 ns | 0.0283 ns | 0.0423 ns | 1,225,492,092.4 |   2.07 |     0.20 | 0.0000 |       0 B |
      SerializeMessagePackTypeless | 0.3912 ns | 0.0206 ns | 0.0309 ns | 2,556,371,744.1 |   0.99 |     0.11 | 0.0000 |       0 B |
    DeserializeMessagePackTypeless | 0.8573 ns | 0.0126 ns | 0.0189 ns | 1,166,481,910.2 |   2.18 |     0.18 | 0.0000 |       0 B |
   SerializeLz4MessagePackTypeless | 0.6639 ns | 0.0091 ns | 0.0136 ns | 1,506,339,443.7 |   1.69 |     0.14 |      - |       0 B |
 DeserializeLz4MessagePackTypeless | 0.8866 ns | 0.0116 ns | 0.0174 ns | 1,127,957,407.5 |   2.25 |     0.19 | 0.0000 |       0 B |
                 SerializeUtf8Json | 1.0556 ns | 0.0367 ns | 0.0550 ns |   947,322,693.2 |   2.68 |     0.26 | 0.0000 |       0 B |
               DeserializeUtf8Json | 1.9133 ns | 0.0109 ns | 0.0164 ns |   522,652,025.1 |   4.86 |     0.40 | 0.0000 |       0 B |
