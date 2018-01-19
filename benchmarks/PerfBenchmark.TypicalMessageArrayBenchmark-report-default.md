
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Processor=Intel Core i5-3450 CPU 3.10GHz (Ivy Bridge), ProcessorCount=4
Frequency=3027365 Hz, Resolution=330.3203 ns, Timer=TSC
.NET Core SDK=2.2.0-preview1-007946
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Job-JGPINV : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

RemoveOutliers=False  Platform=X64  Runtime=Core  
Server=True  Toolchain=.NET Core 2.0  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

                            Method |      Mean |     Error |    StdDev |    Median |            Op/s | Scaled | ScaledSD |  Gen 0 | Allocated |
---------------------------------- |----------:|----------:|----------:|----------:|----------------:|-------:|---------:|-------:|----------:|
              SerializeMessagePack | 0.3986 ns | 0.0059 ns | 0.0088 ns | 0.3968 ns | 2,508,966,659.8 |   1.00 |     0.00 | 0.0000 |       0 B |
    SerializeMessagePackNonGeneric | 0.3921 ns | 0.0024 ns | 0.0036 ns | 0.3907 ns | 2,550,081,433.7 |   0.98 |     0.02 | 0.0000 |       0 B |
            DeserializeMessagePack | 0.8107 ns | 0.0061 ns | 0.0091 ns | 0.8090 ns | 1,233,535,842.3 |   2.03 |     0.05 | 0.0000 |       0 B |
  DeserializeMessagePackNonGeneric | 0.7981 ns | 0.0043 ns | 0.0065 ns | 0.7960 ns | 1,252,993,124.8 |   2.00 |     0.04 | 0.0000 |       0 B |
      SerializeMessagePackTypeless | 0.3936 ns | 0.0019 ns | 0.0029 ns | 0.3930 ns | 2,540,447,541.8 |   0.99 |     0.02 | 0.0000 |       0 B |
    DeserializeMessagePackTypeless | 0.8043 ns | 0.0038 ns | 0.0057 ns | 0.8027 ns | 1,243,378,141.3 |   2.02 |     0.04 | 0.0000 |       0 B |
   SerializeLz4MessagePackTypeless | 0.6651 ns | 0.0041 ns | 0.0062 ns | 0.6638 ns | 1,503,493,252.9 |   1.67 |     0.04 | 0.0000 |       0 B |
 DeserializeLz4MessagePackTypeless | 0.8391 ns | 0.0062 ns | 0.0092 ns | 0.8372 ns | 1,191,757,754.5 |   2.11 |     0.05 | 0.0000 |       0 B |
     SerializeMessagePackFormatter | 0.3081 ns | 0.0010 ns | 0.0014 ns | 0.3081 ns | 3,245,175,449.3 |   0.77 |     0.02 | 0.0000 |       0 B |
   DeserializeMessagePackFormatter | 0.3548 ns | 0.0020 ns | 0.0029 ns | 0.3541 ns | 2,818,634,449.0 |   0.89 |     0.02 | 0.0000 |       0 B |
                 SerializeUtf8Json | 0.8048 ns | 0.0057 ns | 0.0086 ns | 0.8009 ns | 1,242,564,659.5 |   2.02 |     0.05 | 0.0000 |       0 B |
               DeserializeUtf8Json | 1.9396 ns | 0.0039 ns | 0.0059 ns | 1.9400 ns |   515,557,747.2 |   4.87 |     0.10 | 0.0000 |       0 B |
        SerializeUtf8JsonFormatter | 0.8683 ns | 0.0015 ns | 0.0023 ns | 0.8681 ns | 1,151,661,360.4 |   2.18 |     0.05 | 0.0000 |       0 B |
      DeserializeUtf8JsonFormatter | 1.9597 ns | 0.0044 ns | 0.0065 ns | 1.9610 ns |   510,279,406.0 |   4.92 |     0.10 | 0.0000 |       0 B |
                  SerializeJsonNet | 4.1301 ns | 0.0158 ns | 0.0237 ns | 4.1257 ns |   242,122,783.8 |  10.37 |     0.22 | 0.0000 |       2 B |
                DeserializeJsonNet | 8.5002 ns | 0.0417 ns | 0.0624 ns | 8.4830 ns |   117,644,444.3 |  21.34 |     0.47 | 0.0000 |       1 B |
                 SerializeJsonNetX | 4.1589 ns | 0.0498 ns | 0.0746 ns | 4.1983 ns |   240,448,007.3 |  10.44 |     0.29 | 0.0000 |       1 B |
               DeserializeJsonNetX | 8.1840 ns | 0.0265 ns | 0.0396 ns | 8.1751 ns |   122,189,627.2 |  20.54 |     0.44 | 0.0000 |       1 B |
            SerializeJsonFormatter | 4.2898 ns | 0.0175 ns | 0.0263 ns | 4.2842 ns |   233,111,555.8 |  10.77 |     0.23 | 0.0000 |       1 B |
          DeserializeJsonFormatter | 8.4033 ns | 0.0461 ns | 0.0690 ns | 8.4232 ns |   119,001,258.1 |  21.09 |     0.47 | 0.0000 |       1 B |
              SerializeProtoBufNet | 1.7259 ns | 0.0040 ns | 0.0060 ns | 1.7276 ns |   579,391,463.2 |   4.33 |     0.09 | 0.0000 |       1 B |
            DeserializeProtoBufNet | 2.0732 ns | 0.0107 ns | 0.0160 ns | 2.0681 ns |   482,345,248.5 |   5.20 |     0.12 | 0.0000 |       0 B |
        SerializeProtoBufFormatter | 1.8317 ns | 0.0449 ns | 0.0672 ns | 1.8142 ns |   545,928,386.2 |   4.60 |     0.19 | 0.0000 |       0 B |
      DeserializeProtoBufFormatter | 2.0443 ns | 0.0057 ns | 0.0085 ns | 2.0434 ns |   489,166,689.0 |   5.13 |     0.11 | 0.0000 |       0 B |
                 SerializeHyperion | 1.2073 ns | 0.0051 ns | 0.0076 ns | 1.2052 ns |   828,262,294.3 |   3.03 |     0.07 | 0.0000 |       1 B |
               DeserializeHyperion | 0.6927 ns | 0.0047 ns | 0.0071 ns | 0.6907 ns | 1,443,665,188.7 |   1.74 |     0.04 | 0.0000 |       1 B |
        SerializeHyperionFormatter | 1.1608 ns | 0.0062 ns | 0.0093 ns | 1.1591 ns |   861,507,635.9 |   2.91 |     0.07 | 0.0000 |       1 B |
      DeserializeHyperionFormatter | 0.6996 ns | 0.0043 ns | 0.0065 ns | 0.6988 ns | 1,429,295,802.2 |   1.76 |     0.04 | 0.0000 |       1 B |
