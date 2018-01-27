using System;

namespace CuteAnt.Extensions.Serialization
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
  public class SerializingTokenAttribute : Attribute
  {
    public readonly SerializingToken Token;

    public SerializingTokenAttribute(SerializingToken token) => Token = token;
  }
}