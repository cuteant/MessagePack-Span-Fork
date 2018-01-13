
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Processor=Intel Xeon CPU E3-1230 V2 3.30GHz, ProcessorCount=8
Frequency=3222681 Hz, Resolution=310.3006 ns, Timer=TSC
.NET Core SDK=2.2.0-preview1-007870
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Job-IOQCRV : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

RemoveOutliers=False  Platform=X64  Runtime=Core  
Server=True  Toolchain=.NET Core 2.0  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

                            Method |      Mean |     Error |    StdDev |              Op/s | Scaled | ScaledSD |  Gen 0 | Allocated |
---------------------------------- |----------:|----------:|----------:|------------------:|-------:|---------:|-------:|----------:|
              SerializeMessagePack | 0.0050 ns | 0.0004 ns | 0.0006 ns | 198,932,764,281.6 |   1.00 |     0.00 | 0.0000 |       0 B |
            DeserializeMessagePack | 0.0062 ns | 0.0002 ns | 0.0002 ns | 161,229,547,216.1 |   1.24 |     0.11 | 0.0000 |       0 B |
      SerializeMessagePackTypeless | 0.0066 ns | 0.0001 ns | 0.0001 ns | 151,348,973,159.6 |   1.33 |     0.11 | 0.0000 |       0 B |
    DeserializeMessagePackTypeless | 0.0088 ns | 0.0007 ns | 0.0011 ns | 113,633,726,715.6 |   1.77 |     0.26 | 0.0000 |       0 B |
   SerializeLz4MessagePackTypeless | 0.0486 ns | 0.0031 ns | 0.0047 ns |  20,568,159,757.7 |   9.76 |     1.21 | 0.0000 |       0 B |
 DeserializeLz4MessagePackTypeless | 0.0098 ns | 0.0003 ns | 0.0004 ns | 102,231,427,852.1 |   1.96 |     0.18 | 0.0000 |       0 B |
     SerializeMessagePackFormatter | 0.0074 ns | 0.0002 ns | 0.0003 ns | 135,370,928,488.1 |   1.48 |     0.13 | 0.0000 |       0 B |
   DeserializeMessagePackFormatter | 0.0097 ns | 0.0004 ns | 0.0006 ns | 103,270,232,076.6 |   1.94 |     0.20 | 0.0000 |       0 B |
                 SerializeUtf8Json | 0.0100 ns | 0.0006 ns | 0.0008 ns |  99,728,013,712.3 |   2.01 |     0.23 | 0.0000 |       0 B |
               DeserializeUtf8Json | 0.0178 ns | 0.0017 ns | 0.0025 ns |  56,060,513,779.1 |   3.58 |     0.57 | 0.0000 |       0 B |
        SerializeUtf8JsonFormatter | 0.0097 ns | 0.0007 ns | 0.0011 ns | 102,679,433,176.1 |   1.95 |     0.27 | 0.0000 |       0 B |
      DeserializeUtf8JsonFormatter | 0.0170 ns | 0.0003 ns | 0.0005 ns |  58,898,380,627.5 |   3.41 |     0.29 | 0.0000 |       0 B |
