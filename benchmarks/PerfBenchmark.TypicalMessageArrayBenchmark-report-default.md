
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Processor=Intel Xeon CPU E3-1230 V2 3.30GHz, ProcessorCount=8
Frequency=3222681 Hz, Resolution=310.3006 ns, Timer=TSC
.NET Core SDK=2.2.0-preview1-007870
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Job-VLPZAY : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

RemoveOutliers=False  Platform=X64  Runtime=Core  
Server=True  Toolchain=.NET Core 2.0  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

                            Method |      Mean |     Error |    StdDev |            Op/s | Scaled | ScaledSD |  Gen 0 | Allocated |
---------------------------------- |----------:|----------:|----------:|----------------:|-------:|---------:|-------:|----------:|
              SerializeMessagePack | 0.4059 ns | 0.0286 ns | 0.0428 ns | 2,463,789,983.6 |   1.00 |     0.00 | 0.0000 |       0 B |
            DeserializeMessagePack | 0.7920 ns | 0.0121 ns | 0.0181 ns | 1,262,561,676.9 |   1.97 |     0.16 | 0.0000 |       0 B |
      SerializeMessagePackTypeless | 0.3952 ns | 0.0192 ns | 0.0287 ns | 2,530,624,801.7 |   0.98 |     0.10 | 0.0000 |       0 B |
    DeserializeMessagePackTypeless | 0.8139 ns | 0.0251 ns | 0.0376 ns | 1,228,619,568.0 |   2.02 |     0.18 | 0.0000 |       0 B |
   SerializeLz4MessagePackTypeless | 0.6460 ns | 0.0022 ns | 0.0033 ns | 1,547,917,303.2 |   1.60 |     0.12 | 0.0000 |       0 B |
 DeserializeLz4MessagePackTypeless | 0.9314 ns | 0.0049 ns | 0.0073 ns | 1,073,709,055.7 |   2.31 |     0.18 | 0.0000 |       0 B |
     SerializeMessagePackFormatter | 0.3954 ns | 0.0184 ns | 0.0276 ns | 2,528,784,583.6 |   0.98 |     0.10 | 0.0000 |       0 B |
   DeserializeMessagePackFormatter | 0.8068 ns | 0.0052 ns | 0.0077 ns | 1,239,493,190.2 |   2.00 |     0.16 | 0.0000 |       0 B |
                 SerializeUtf8Json | 0.8431 ns | 0.0720 ns | 0.1078 ns | 1,186,079,556.2 |   2.09 |     0.31 | 0.0000 |       0 B |
               DeserializeUtf8Json | 1.9125 ns | 0.0061 ns | 0.0091 ns |   522,886,885.7 |   4.75 |     0.37 | 0.0000 |       0 B |
        SerializeUtf8JsonFormatter | 0.8471 ns | 0.1012 ns | 0.1515 ns | 1,180,493,333.0 |   2.10 |     0.40 | 0.0000 |       0 B |
      DeserializeUtf8JsonFormatter | 1.9074 ns | 0.0070 ns | 0.0105 ns |   524,262,255.5 |   4.74 |     0.36 | 0.0000 |       0 B |
