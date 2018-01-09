using System;
using ProtoBuf;
using MessagePack;

namespace PerfBenchmark.Types
{
  [ProtoContract]
  [Serializable]
  [MessagePackObject]
  public class TypicalMessage
  {
    [ProtoMember(1)]
    [Key(0)]
    public virtual string StringProp { get; set; }

    [ProtoMember(2)]
    [Key(1)]
    public virtual int IntProp { get; set; }

    [ProtoMember(3)]
    [Key(2)]
    public virtual Guid GuidProp { get; set; }

    [ProtoMember(4)]
    [Key(3)]
    public virtual DateTime DateProp { get; set; }
  }
}