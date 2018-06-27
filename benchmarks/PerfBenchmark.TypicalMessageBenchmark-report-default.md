
BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Xeon CPU E3-1230 V2 3.30GHz, 1 CPU, 8 logical and 4 physical cores
Frequency=3222679 Hz, Resolution=310.3008 ns, Timer=TSC
.NET Core SDK=2.1.300
  [Host]     : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Job-QDPHKJ : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT

RemoveOutliers=False  Platform=X64  Runtime=Core  
Server=True  Toolchain=.NET Core 2.1  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

                            Method |      Mean |     Error |    StdDev |    Median |              Op/s | Scaled | ScaledSD |  Gen 0 | Allocated |
---------------------------------- |----------:|----------:|----------:|----------:|------------------:|-------:|---------:|-------:|----------:|
              SerializeMessagePack | 0.0049 ns | 0.0003 ns | 0.0004 ns | 0.0047 ns | 204,645,411,561.6 |   1.00 |     0.00 | 0.0010 |      80 B |
    SerializeMessagePackNonGeneric | 0.0064 ns | 0.0007 ns | 0.0011 ns | 0.0061 ns | 155,049,277,364.7 |   1.33 |     0.24 | 0.0005 |      80 B |
            DeserializeMessagePack | 0.0061 ns | 0.0003 ns | 0.0004 ns | 0.0060 ns | 163,264,655,808.3 |   1.26 |     0.12 | 0.0010 |      96 B |
  DeserializeMessagePackNonGeneric | 0.0064 ns | 0.0001 ns | 0.0002 ns | 0.0063 ns | 156,650,332,895.8 |   1.31 |     0.10 | 0.0010 |      96 B |
      SerializeMessagePackTypeless | 0.0064 ns | 0.0001 ns | 0.0002 ns | 0.0063 ns | 156,098,009,957.8 |   1.32 |     0.10 | 0.0014 |     136 B |
    DeserializeMessagePackTypeless | 0.0085 ns | 0.0005 ns | 0.0007 ns | 0.0084 ns | 117,754,965,273.4 |   1.75 |     0.19 | 0.0010 |      96 B |
   SerializeLz4MessagePackTypeless | 0.0332 ns | 0.0008 ns | 0.0011 ns | 0.0330 ns |  30,127,064,208.3 |   6.83 |     0.54 | 0.2022 |   16552 B |
 DeserializeLz4MessagePackTypeless | 0.0094 ns | 0.0006 ns | 0.0009 ns | 0.0091 ns | 106,540,548,155.8 |   1.93 |     0.23 | 0.0010 |      96 B |
     SerializeMessagePackFormatter | 0.0051 ns | 0.0001 ns | 0.0001 ns | 0.0051 ns | 196,843,015,062.3 |   1.05 |     0.08 | 0.0005 |      64 B |
   DeserializeMessagePackFormatter | 0.0043 ns | 0.0001 ns | 0.0001 ns | 0.0043 ns | 232,149,886,605.0 |   0.89 |     0.07 | 0.0010 |      96 B |
                 SerializeUtf8Json | 0.0089 ns | 0.0004 ns | 0.0005 ns | 0.0087 ns | 112,489,461,431.2 |   1.83 |     0.17 | 0.0019 |     152 B |
               DeserializeUtf8Json | 0.0168 ns | 0.0001 ns | 0.0002 ns | 0.0168 ns |  59,443,466,847.0 |   3.46 |     0.25 | 0.0010 |      96 B |
        SerializeUtf8JsonFormatter | 0.0102 ns | 0.0001 ns | 0.0002 ns | 0.0102 ns |  97,880,759,465.3 |   2.10 |     0.15 | 0.0010 |     152 B |
      DeserializeUtf8JsonFormatter | 0.0170 ns | 0.0001 ns | 0.0002 ns | 0.0170 ns |  58,801,724,289.2 |   3.50 |     0.25 | 0.0010 |      96 B |
                  SerializeJsonNet | 0.0563 ns | 0.0006 ns | 0.0008 ns | 0.0561 ns |  17,747,446,372.8 |  11.60 |     0.85 | 0.0229 |    2048 B |
                DeserializeJsonNet | 0.1130 ns | 0.0020 ns | 0.0030 ns | 0.1129 ns |   8,852,633,795.9 |  23.25 |     1.78 | 0.0381 |    3304 B |
                 SerializeJsonNetX | 0.0803 ns | 0.0010 ns | 0.0015 ns | 0.0802 ns |  12,448,175,126.5 |  16.53 |     1.23 | 0.0687 |    6400 B |
               DeserializeJsonNetX | 0.1353 ns | 0.0018 ns | 0.0027 ns | 0.1340 ns |   7,390,945,339.7 |  27.85 |     2.08 | 0.0381 |    3904 B |
            SerializeJsonFormatter | 0.0779 ns | 0.0008 ns | 0.0012 ns | 0.0775 ns |  12,837,544,263.0 |  16.03 |     1.18 | 0.0610 |    6400 B |
          DeserializeJsonFormatter | 0.1518 ns | 0.0026 ns | 0.0039 ns | 0.1526 ns |   6,587,398,923.3 |  31.25 |     2.39 | 0.0458 |    4064 B |
              SerializeProtoBufNet | 0.0111 ns | 0.0002 ns | 0.0002 ns | 0.0110 ns |  90,196,783,989.7 |   2.28 |     0.17 | 0.0057 |     616 B |
            DeserializeProtoBufNet | 0.0239 ns | 0.0007 ns | 0.0011 ns | 0.0237 ns |  41,777,373,786.3 |   4.93 |     0.42 | 0.0038 |     384 B |
        SerializeProtoBufFormatter | 0.0143 ns | 0.0001 ns | 0.0002 ns | 0.0142 ns |  70,080,689,271.5 |   2.94 |     0.21 | 0.0029 |     304 B |
      DeserializeProtoBufFormatter | 0.0229 ns | 0.0004 ns | 0.0006 ns | 0.0232 ns |  43,621,516,560.4 |   4.72 |     0.37 | 0.0038 |     384 B |
                 SerializeHyperion | 0.0188 ns | 0.0011 ns | 0.0017 ns | 0.0178 ns |  53,135,227,062.5 |   3.87 |     0.44 | 0.0086 |     760 B |
               DeserializeHyperion | 0.0194 ns | 0.0003 ns | 0.0005 ns | 0.0194 ns |  51,482,516,834.8 |   4.00 |     0.30 | 0.0076 |     656 B |
        SerializeHyperionFormatter | 0.0137 ns | 0.0003 ns | 0.0004 ns | 0.0136 ns |  72,789,720,829.8 |   2.83 |     0.22 | 0.0019 |     248 B |
      DeserializeHyperionFormatter | 0.0128 ns | 0.0003 ns | 0.0005 ns | 0.0130 ns |  78,164,186,666.7 |   2.63 |     0.22 | 0.0038 |     360 B |
