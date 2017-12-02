using System;

namespace CuteAnt.Extensions.Serialization
{
  public interface IMessageFormatterFactory
  {
    IMessageFormatter Create();
  }
}
