
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Processor=Intel Xeon CPU E3-1230 V2 3.30GHz, ProcessorCount=8
Frequency=3222681 Hz, Resolution=310.3006 ns, Timer=TSC
.NET Core SDK=2.2.0-preview1-007946
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Job-GHLLLB : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

RemoveOutliers=False  Platform=X64  Runtime=Core  
Server=True  Toolchain=.NET Core 2.0  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

                            Method |      Mean |     Error |    StdDev |    Median |            Op/s | Scaled | ScaledSD |  Gen 0 | Allocated |
---------------------------------- |----------:|----------:|----------:|----------:|----------------:|-------:|---------:|-------:|----------:|
              SerializeMessagePack | 0.3818 ns | 0.0202 ns | 0.0302 ns | 0.3745 ns | 2,619,088,130.0 |   1.00 |     0.00 | 0.0000 |       0 B |
            DeserializeMessagePack | 0.7874 ns | 0.0435 ns | 0.0651 ns | 0.7665 ns | 1,269,960,018.5 |   2.07 |     0.21 | 0.0000 |       0 B |
      SerializeMessagePackTypeless | 0.3802 ns | 0.0242 ns | 0.0362 ns | 0.3744 ns | 2,629,859,287.9 |   1.00 |     0.11 | 0.0000 |       0 B |
    DeserializeMessagePackTypeless | 0.7789 ns | 0.0379 ns | 0.0567 ns | 0.7614 ns | 1,283,829,070.6 |   2.05 |     0.19 | 0.0000 |       0 B |
   SerializeLz4MessagePackTypeless | 0.6356 ns | 0.0179 ns | 0.0268 ns | 0.6293 ns | 1,573,295,896.1 |   1.67 |     0.12 | 0.0000 |       0 B |
 DeserializeLz4MessagePackTypeless | 0.8125 ns | 0.0399 ns | 0.0597 ns | 0.7949 ns | 1,230,801,950.7 |   2.14 |     0.20 | 0.0000 |       0 B |
     SerializeMessagePackFormatter | 0.3743 ns | 0.0194 ns | 0.0290 ns | 0.3687 ns | 2,671,665,327.2 |   0.98 |     0.09 | 0.0000 |       0 B |
   DeserializeMessagePackFormatter | 0.8022 ns | 0.0525 ns | 0.0786 ns | 0.7700 ns | 1,246,553,885.5 |   2.11 |     0.24 | 0.0000 |       0 B |
                 SerializeUtf8Json | 0.8060 ns | 0.0724 ns | 0.1084 ns | 0.7627 ns | 1,240,719,808.3 |   2.12 |     0.31 | 0.0000 |       0 B |
               DeserializeUtf8Json | 1.8232 ns | 0.0076 ns | 0.0114 ns | 1.8213 ns |   548,483,507.8 |   4.80 |     0.28 | 0.0000 |       0 B |
        SerializeUtf8JsonFormatter | 0.8153 ns | 0.0922 ns | 0.1380 ns | 0.7591 ns | 1,226,472,829.5 |   2.15 |     0.38 | 0.0000 |       0 B |
      DeserializeUtf8JsonFormatter | 1.8968 ns | 0.0265 ns | 0.0397 ns | 1.9024 ns |   527,211,413.8 |   4.99 |     0.31 | 0.0000 |       0 B |
                  SerializeJsonNet | 3.7588 ns | 0.0111 ns | 0.0166 ns | 3.7579 ns |   266,040,420.4 |   9.89 |     0.57 | 0.0000 |       2 B |
                DeserializeJsonNet | 8.0541 ns | 0.0613 ns | 0.0918 ns | 8.1091 ns |   124,160,556.1 |  21.19 |     1.25 | 0.0000 |       1 B |
                 SerializeJsonNetX | 4.3459 ns | 0.2090 ns | 0.3128 ns | 4.2932 ns |   230,101,980.2 |  11.43 |     1.04 | 0.0000 |       1 B |
               DeserializeJsonNetX | 7.9872 ns | 0.0460 ns | 0.0689 ns | 7.9778 ns |   125,200,029.9 |  21.01 |     1.23 | 0.0000 |       1 B |
            SerializeJsonFormatter | 4.3123 ns | 0.0615 ns | 0.0920 ns | 4.2682 ns |   231,893,705.1 |  11.35 |     0.70 | 0.0000 |       1 B |
          DeserializeJsonFormatter | 7.8535 ns | 0.0553 ns | 0.0827 ns | 7.8873 ns |   127,332,354.0 |  20.66 |     1.21 |      - |       1 B |
              SerializeProtoBufNet | 1.6672 ns | 0.0064 ns | 0.0096 ns | 1.6661 ns |   599,793,185.4 |   4.39 |     0.25 | 0.0000 |       1 B |
            DeserializeProtoBufNet | 1.8857 ns | 0.0079 ns | 0.0118 ns | 1.8855 ns |   530,317,718.1 |   4.96 |     0.29 | 0.0000 |       0 B |
        SerializeProtoBufFormatter | 1.7674 ns | 0.0255 ns | 0.0381 ns | 1.7533 ns |   565,816,015.4 |   4.65 |     0.29 | 0.0000 |       0 B |
      DeserializeProtoBufFormatter | 1.9796 ns | 0.0080 ns | 0.0120 ns | 1.9774 ns |   505,145,600.3 |   5.21 |     0.30 | 0.0000 |       0 B |
                 SerializeHyperion | 1.2153 ns | 0.0567 ns | 0.0849 ns | 1.1918 ns |   822,814,072.2 |   3.20 |     0.29 | 0.0000 |       1 B |
               DeserializeHyperion | 0.7697 ns | 0.0677 ns | 0.1014 ns | 0.7538 ns | 1,299,250,773.7 |   2.02 |     0.29 | 0.0000 |       1 B |
        SerializeHyperionFormatter | 1.1466 ns | 0.0554 ns | 0.0830 ns | 1.1310 ns |   872,162,365.7 |   3.02 |     0.28 | 0.0000 |       1 B |
      DeserializeHyperionFormatter | 0.7211 ns | 0.0384 ns | 0.0574 ns | 0.6925 ns | 1,386,860,472.1 |   1.90 |     0.18 | 0.0000 |       1 B |
