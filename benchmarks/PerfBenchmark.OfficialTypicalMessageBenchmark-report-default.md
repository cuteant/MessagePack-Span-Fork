
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Processor=Intel Xeon CPU E3-1230 V2 3.30GHz, ProcessorCount=8
Frequency=3222681 Hz, Resolution=310.3006 ns, Timer=TSC
.NET Core SDK=2.2.0-preview1-007870
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Job-EPTCTF : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

RemoveOutliers=False  Platform=X64  Runtime=Core  
Server=True  Toolchain=.NET Core 2.0  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

                            Method |      Mean |     Error |    StdDev |    Median |              Op/s | Scaled | ScaledSD |  Gen 0 | Allocated |
---------------------------------- |----------:|----------:|----------:|----------:|------------------:|-------:|---------:|-------:|----------:|
              SerializeMessagePack | 0.0054 ns | 0.0003 ns | 0.0005 ns | 0.0054 ns | 185,002,421,263.1 |   1.00 |     0.00 | 0.0000 |       0 B |
            DeserializeMessagePack | 0.0067 ns | 0.0003 ns | 0.0004 ns | 0.0066 ns | 150,056,875,653.9 |   1.24 |     0.13 | 0.0000 |       0 B |
      SerializeMessagePackTypeless | 0.0069 ns | 0.0004 ns | 0.0006 ns | 0.0066 ns | 144,241,243,003.0 |   1.29 |     0.15 | 0.0000 |       0 B |
    DeserializeMessagePackTypeless | 0.0087 ns | 0.0003 ns | 0.0004 ns | 0.0085 ns | 115,176,764,801.8 |   1.62 |     0.15 | 0.0000 |       0 B |
   SerializeLz4MessagePackTypeless | 0.0320 ns | 0.0002 ns | 0.0003 ns | 0.0319 ns |  31,268,091,680.9 |   5.96 |     0.50 | 0.0000 |       0 B |
 DeserializeLz4MessagePackTypeless | 0.0102 ns | 0.0002 ns | 0.0003 ns | 0.0103 ns |  97,835,510,567.2 |   1.91 |     0.17 | 0.0000 |       0 B |
                 SerializeUtf8Json | 0.0113 ns | 0.0007 ns | 0.0010 ns | 0.0110 ns |  88,279,867,562.6 |   2.11 |     0.26 | 0.0000 |       0 B |
               DeserializeUtf8Json | 0.0186 ns | 0.0002 ns | 0.0003 ns | 0.0184 ns |  53,715,430,327.5 |   3.47 |     0.30 |      - |       0 B |
