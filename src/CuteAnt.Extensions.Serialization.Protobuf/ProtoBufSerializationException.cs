using System;
using System.Runtime.Serialization;

namespace CuteAnt.Extensions.Serialization.Protobuf
{
  [Serializable]
  public class ProtoBufSerializationException : SerializationException
  {
    /// <summary>使用默认属性初始化 <see cref="ProtoBufSerializationException"/> 类的新实例。</summary>
    public ProtoBufSerializationException()
    {
    }

    /// <summary>用指定的消息初始化 <see cref="ProtoBufSerializationException"/> 类的新实例。</summary>
    /// <param name="message"></param>
    public ProtoBufSerializationException(string message)
      : base(message)
    {
    }

    /// <summary>使用指定的错误消息以及对作为此异常原因的内部异常的引用来初始化 <see cref="ProtoBufSerializationException"/> 类的新实例。</summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public ProtoBufSerializationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>用序列化数据初始化 <see cref="ProtoBufSerializationException"/> 类的新实例。</summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected ProtoBufSerializationException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
