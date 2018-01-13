using System;
using MessagePack;

namespace PerfBenchmark.Types
{
  [Serializable]
  [MessagePackObject]
  public class TypicalMessage
  {
    [Key(0)]
    public virtual string StringProp { get; set; }

    [Key(1)]
    public virtual int IntProp { get; set; }

    [Key(2)]
    public virtual Guid GuidProp { get; set; }

    [Key(3)]
    public virtual DateTime DateProp { get; set; }
  }
}