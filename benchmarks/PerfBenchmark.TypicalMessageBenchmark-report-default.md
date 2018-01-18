
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Processor=Intel Xeon CPU E3-1230 V2 3.30GHz, ProcessorCount=8
Frequency=3222681 Hz, Resolution=310.3006 ns, Timer=TSC
.NET Core SDK=2.2.0-preview1-007946
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Job-DDSTTN : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

RemoveOutliers=False  Platform=X64  Runtime=Core  
Server=True  Toolchain=.NET Core 2.0  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

                            Method |      Mean |     Error |    StdDev |    Median |              Op/s | Scaled | ScaledSD |  Gen 0 | Allocated |
---------------------------------- |----------:|----------:|----------:|----------:|------------------:|-------:|---------:|-------:|----------:|
              SerializeMessagePack | 0.0051 ns | 0.0006 ns | 0.0009 ns | 0.0049 ns | 194,325,714,382.2 |   1.00 |     0.00 | 0.0000 |       0 B |
            DeserializeMessagePack | 0.0066 ns | 0.0003 ns | 0.0005 ns | 0.0069 ns | 151,427,737,156.7 |   1.31 |     0.17 | 0.0000 |       0 B |
      SerializeMessagePackTypeless | 0.0066 ns | 0.0001 ns | 0.0001 ns | 0.0065 ns | 152,016,553,717.7 |   1.30 |     0.14 | 0.0000 |       0 B |
    DeserializeMessagePackTypeless | 0.0097 ns | 0.0011 ns | 0.0016 ns | 0.0086 ns | 103,488,907,835.1 |   1.91 |     0.37 | 0.0000 |       0 B |
   SerializeLz4MessagePackTypeless | 0.0482 ns | 0.0028 ns | 0.0042 ns | 0.0488 ns |  20,743,280,327.4 |   9.54 |     1.29 | 0.0000 |       0 B |
 DeserializeLz4MessagePackTypeless | 0.0097 ns | 0.0002 ns | 0.0003 ns | 0.0096 ns | 102,933,434,725.1 |   1.92 |     0.21 | 0.0000 |       0 B |
     SerializeMessagePackFormatter | 0.0052 ns | 0.0002 ns | 0.0003 ns | 0.0051 ns | 192,640,597,443.3 |   1.03 |     0.12 | 0.0000 |       0 B |
   DeserializeMessagePackFormatter | 0.0043 ns | 0.0003 ns | 0.0004 ns | 0.0041 ns | 234,500,583,859.3 |   0.84 |     0.12 | 0.0000 |       0 B |
                 SerializeUtf8Json | 0.0090 ns | 0.0001 ns | 0.0002 ns | 0.0089 ns | 111,396,363,060.6 |   1.78 |     0.19 | 0.0000 |       0 B |
               DeserializeUtf8Json | 0.0166 ns | 0.0002 ns | 0.0003 ns | 0.0165 ns |  60,145,477,517.2 |   3.29 |     0.34 | 0.0000 |       0 B |
        SerializeUtf8JsonFormatter | 0.0095 ns | 0.0000 ns | 0.0000 ns | 0.0095 ns | 105,649,571,219.9 |   1.87 |     0.19 | 0.0000 |       0 B |
      DeserializeUtf8JsonFormatter | 0.0171 ns | 0.0003 ns | 0.0004 ns | 0.0170 ns |  58,363,402,142.9 |   3.39 |     0.36 | 0.0000 |       0 B |
                  SerializeJsonNet | 0.0576 ns | 0.0006 ns | 0.0009 ns | 0.0571 ns |  17,361,560,847.6 |  11.39 |     1.20 | 0.0000 |       0 B |
                DeserializeJsonNet | 0.1183 ns | 0.0004 ns | 0.0006 ns | 0.1183 ns |   8,452,185,828.4 |  23.40 |     2.43 | 0.0000 |       0 B |
                 SerializeJsonNetX | 0.0865 ns | 0.0033 ns | 0.0050 ns | 0.0852 ns |  11,565,266,786.5 |  17.10 |     2.02 | 0.0000 |       0 B |
               DeserializeJsonNetX | 0.1446 ns | 0.0065 ns | 0.0098 ns | 0.1427 ns |   6,913,722,362.6 |  28.61 |     3.53 | 0.0000 |       0 B |
            SerializeJsonFormatter | 0.0845 ns | 0.0033 ns | 0.0049 ns | 0.0833 ns |  11,830,466,628.1 |  16.72 |     1.98 | 0.0000 |       0 B |
          DeserializeJsonFormatter | 0.1551 ns | 0.0016 ns | 0.0024 ns | 0.1541 ns |   6,449,446,767.7 |  30.67 |     3.22 | 0.0000 |       0 B |
              SerializeProtoBufNet | 0.0110 ns | 0.0003 ns | 0.0004 ns | 0.0110 ns |  90,528,743,388.5 |   2.18 |     0.24 | 0.0000 |       0 B |
            DeserializeProtoBufNet | 0.0230 ns | 0.0018 ns | 0.0027 ns | 0.0222 ns |  43,530,573,254.7 |   4.54 |     0.71 | 0.0000 |       0 B |
        SerializeProtoBufFormatter | 0.0144 ns | 0.0002 ns | 0.0003 ns | 0.0143 ns |  69,552,724,834.6 |   2.84 |     0.30 | 0.0000 |       0 B |
      DeserializeProtoBufFormatter | 0.0239 ns | 0.0032 ns | 0.0048 ns | 0.0223 ns |  41,803,202,251.7 |   4.73 |     1.06 | 0.0000 |       0 B |
                 SerializeHyperion | 0.0159 ns | 0.0008 ns | 0.0011 ns | 0.0162 ns |  62,989,545,893.2 |   3.14 |     0.39 | 0.0000 |       0 B |
               DeserializeHyperion | 0.0170 ns | 0.0008 ns | 0.0012 ns | 0.0167 ns |  58,853,195,044.6 |   3.36 |     0.42 | 0.0000 |       0 B |
        SerializeHyperionFormatter | 0.0205 ns | 0.0008 ns | 0.0012 ns | 0.0205 ns |  48,871,831,139.9 |   4.05 |     0.49 | 0.0000 |       0 B |
      DeserializeHyperionFormatter | 0.0172 ns | 0.0002 ns | 0.0004 ns | 0.0171 ns |  58,020,177,806.3 |   3.41 |     0.36 | 0.0000 |       0 B |
