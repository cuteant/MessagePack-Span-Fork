
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.125)
Processor=Intel Core i5-3450 CPU 3.10GHz (Ivy Bridge), ProcessorCount=4
Frequency=3027365 Hz, Resolution=330.3203 ns, Timer=TSC
.NET Core SDK=2.2.0-preview1-007946
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Job-XVWYPT : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

RemoveOutliers=False  Platform=X64  Runtime=Core  
Server=True  Toolchain=.NET Core 2.0  LaunchCount=3  
RunStrategy=Throughput  TargetCount=10  WarmupCount=5  

                            Method |      Mean |     Error |    StdDev |    Median |              Op/s | Scaled | ScaledSD |  Gen 0 | Allocated |
---------------------------------- |----------:|----------:|----------:|----------:|------------------:|-------:|---------:|-------:|----------:|
              SerializeMessagePack | 0.0050 ns | 0.0001 ns | 0.0001 ns | 0.0050 ns | 198,056,926,257.0 |   1.00 |     0.00 | 0.0000 |       0 B |
    SerializeMessagePackNonGeneric | 0.0060 ns | 0.0000 ns | 0.0000 ns | 0.0060 ns | 166,192,935,197.6 |   1.19 |     0.02 | 0.0000 |       0 B |
            DeserializeMessagePack | 0.0062 ns | 0.0002 ns | 0.0003 ns | 0.0061 ns | 160,167,317,319.4 |   1.24 |     0.06 | 0.0000 |       0 B |
  DeserializeMessagePackNonGeneric | 0.0070 ns | 0.0002 ns | 0.0002 ns | 0.0071 ns | 141,994,673,995.1 |   1.40 |     0.05 | 0.0000 |       0 B |
      SerializeMessagePackTypeless | 0.0067 ns | 0.0000 ns | 0.0001 ns | 0.0067 ns | 148,637,564,061.7 |   1.33 |     0.03 | 0.0000 |       0 B |
    DeserializeMessagePackTypeless | 0.0096 ns | 0.0003 ns | 0.0004 ns | 0.0096 ns | 103,629,186,328.5 |   1.91 |     0.08 | 0.0000 |       0 B |
   SerializeLz4MessagePackTypeless | 0.0373 ns | 0.0010 ns | 0.0015 ns | 0.0368 ns |  26,809,278,441.5 |   7.39 |     0.33 | 0.0000 |       0 B |
 DeserializeLz4MessagePackTypeless | 0.0100 ns | 0.0001 ns | 0.0002 ns | 0.0100 ns | 100,207,196,187.3 |   1.98 |     0.06 | 0.0000 |       0 B |
     SerializeMessagePackFormatter | 0.0056 ns | 0.0001 ns | 0.0001 ns | 0.0056 ns | 177,850,160,636.8 |   1.11 |     0.03 | 0.0000 |       0 B |
   DeserializeMessagePackFormatter | 0.0042 ns | 0.0000 ns | 0.0001 ns | 0.0042 ns | 238,156,774,899.2 |   0.83 |     0.02 | 0.0000 |       0 B |
                 SerializeUtf8Json | 0.0091 ns | 0.0001 ns | 0.0001 ns | 0.0091 ns | 110,027,992,942.5 |   1.80 |     0.04 | 0.0000 |       0 B |
               DeserializeUtf8Json | 0.0170 ns | 0.0001 ns | 0.0001 ns | 0.0170 ns |  58,660,749,879.8 |   3.38 |     0.06 | 0.0000 |       0 B |
        SerializeUtf8JsonFormatter | 0.0106 ns | 0.0001 ns | 0.0001 ns | 0.0105 ns |  94,682,426,594.9 |   2.09 |     0.04 | 0.0000 |       0 B |
      DeserializeUtf8JsonFormatter | 0.0178 ns | 0.0002 ns | 0.0003 ns | 0.0176 ns |  56,295,089,396.1 |   3.52 |     0.09 | 0.0000 |       0 B |
                  SerializeJsonNet | 0.0598 ns | 0.0007 ns | 0.0010 ns | 0.0600 ns |  16,729,341,649.2 |  11.84 |     0.29 | 0.0000 |       0 B |
                DeserializeJsonNet | 0.1235 ns | 0.0015 ns | 0.0022 ns | 0.1225 ns |   8,099,036,459.3 |  24.46 |     0.62 | 0.0000 |       0 B |
                 SerializeJsonNetX | 0.0850 ns | 0.0007 ns | 0.0010 ns | 0.0850 ns |  11,765,797,628.8 |  16.84 |     0.36 | 0.0000 |       0 B |
               DeserializeJsonNetX | 0.1459 ns | 0.0017 ns | 0.0025 ns | 0.1463 ns |   6,852,911,788.6 |  28.91 |     0.70 | 0.0000 |       0 B |
            SerializeJsonFormatter | 0.0855 ns | 0.0007 ns | 0.0010 ns | 0.0853 ns |  11,699,415,542.8 |  16.93 |     0.36 | 0.0000 |       0 B |
          DeserializeJsonFormatter | 0.1526 ns | 0.0006 ns | 0.0009 ns | 0.1524 ns |   6,553,459,700.8 |  30.23 |     0.57 | 0.0000 |       0 B |
              SerializeProtoBufNet | 0.0113 ns | 0.0001 ns | 0.0001 ns | 0.0113 ns |  88,512,290,769.1 |   2.24 |     0.04 | 0.0000 |       0 B |
            DeserializeProtoBufNet | 0.0228 ns | 0.0001 ns | 0.0002 ns | 0.0227 ns |  43,868,108,513.3 |   4.52 |     0.09 | 0.0000 |       0 B |
        SerializeProtoBufFormatter | 0.0148 ns | 0.0001 ns | 0.0001 ns | 0.0148 ns |  67,621,489,090.5 |   2.93 |     0.06 | 0.0000 |       0 B |
      DeserializeProtoBufFormatter | 0.0246 ns | 0.0001 ns | 0.0002 ns | 0.0247 ns |  40,579,653,735.2 |   4.88 |     0.10 | 0.0000 |       0 B |
                 SerializeHyperion | 0.0155 ns | 0.0001 ns | 0.0002 ns | 0.0155 ns |  64,677,351,914.0 |   3.06 |     0.06 | 0.0000 |       0 B |
               DeserializeHyperion | 0.0168 ns | 0.0002 ns | 0.0004 ns | 0.0169 ns |  59,657,688,642.1 |   3.32 |     0.09 | 0.0000 |       0 B |
        SerializeHyperionFormatter | 0.0190 ns | 0.0001 ns | 0.0002 ns | 0.0190 ns |  52,499,706,619.1 |   3.77 |     0.08 | 0.0000 |       0 B |
      DeserializeHyperionFormatter | 0.0197 ns | 0.0006 ns | 0.0008 ns | 0.0202 ns |  50,874,817,814.0 |   3.89 |     0.17 | 0.0000 |       0 B |
