using System;
using System.Collections.Generic;
using System.Text;
using PerfBenchmark.Types;
using BenchmarkDotNet.Attributes;

namespace PerfBenchmark
{
  [Config(typeof(CoreConfig))]
  public class LargeStructBenchmark : BaseBenchmark<LargeStruct>
  {
    protected override LargeStruct GetValue()
    {
      return LargeStruct.Create();
    }
  }

  [Config(typeof(CoreConfig))]
  public class TypicalMessageBenchmark : BaseBenchmark<TypicalMessage>
  {
    protected override TypicalMessage GetValue()
    {
      return new TypicalMessage()
      {
        StringProp = "hello",
        GuidProp = Guid.NewGuid(),
        IntProp = 123,
        DateProp = DateTime.UtcNow
      };
    }
  }
  [Config(typeof(CoreConfig))]
  public class TypicalMessageArrayBenchmark : BaseBenchmark<TypicalMessage[]>
  {
    protected override TypicalMessage[] GetValue()
    {
      var l = new List<TypicalMessage>();

      for (var i = 0; i < 100; i++)
      {
        var v = new TypicalMessage()
        {
          StringProp = "hello",
          GuidProp = Guid.NewGuid(),
          IntProp = 123,
          DateProp = DateTime.UtcNow
        };
        l.Add(v);
      }

      return l.ToArray();
    }
  }

  [Config(typeof(CoreConfig))]
  public class TypicalPersonBenchmark : BaseBenchmark<TypicalPersonData>
  {
    protected override TypicalPersonData GetValue()
    {
      return TypicalPersonData.MakeRandom();
    }
  }
  [Config(typeof(CoreConfig))]
  public class TypicalPersonArrayBenchmark : BaseBenchmark<TypicalPersonData[]>
  {
    protected override TypicalPersonData[] GetValue()
    {
      var l = new List<TypicalPersonData>();
      for (int i = 0; i < 100; i++)
      {
        l.Add(TypicalPersonData.MakeRandom());
      }
      return l.ToArray();
    }
  }
}
