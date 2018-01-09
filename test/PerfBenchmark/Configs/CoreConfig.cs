using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;
using BenchmarkDotNet.Validators;

namespace PerfBenchmark
{
  public class CoreConfig : ManualConfig
  {
    public CoreConfig()
    {
      Add(ConsoleLogger.Default);

      Add(JitOptimizationsValidator.FailOnError);

      Add(MemoryDiagnoser.Default);
      Add(StatisticColumn.OperationsPerSecond);

#if DESKTOPCLR
      Add(Job.Clr
          .With(BenchmarkDotNet.Environments.Runtime.Clr)
          .WithRemoveOutliers(false)
          .With(new GcMode { Server = true })
          .With(RunStrategy.Throughput)
          .WithLaunchCount(3)
          .WithWarmupCount(5)
          .WithTargetCount(10));
#else
      Add(Job.Core
#if NETCOREAPP2_0
          .With(CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp20))
#elif NETCOREAPP2_1
          .With(CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp21))
#endif
          .With(BenchmarkDotNet.Environments.Runtime.Core)
          .WithRemoveOutliers(false)
          .With(new GcMode { Server = true })
          .With(RunStrategy.Throughput)
          .WithLaunchCount(3)
          .WithWarmupCount(5)
          .WithTargetCount(10));
#endif

      Add(JsonExporter.Full, MarkdownExporter.Default);//, HtmlExporter.Default);
    }
  }
}
