using System;
using ServiceStack.Text;

namespace CuteAnt.Extensions.Serialization
{
  public sealed class JsvStringSerializer : IStringSerializer
  {
    public static readonly JsvStringSerializer Instance = new JsvStringSerializer();

    /// <inheritdoc />
    public T DeserializeFromString<T>(string serializedText)
        => TypeSerializer.DeserializeFromString<T>(serializedText);

    /// <inheritdoc />
    public object DeserializeFromString(string serializedText, Type expectedType)
    {
      if (null == expectedType) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.expectedType); }

      return TypeSerializer.DeserializeFromString(serializedText, expectedType);
    }

    /// <inheritdoc />
    public T DeserializeFromString<T>(string serializedText, Type expectedType)
    {
      if (null == expectedType) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.expectedType); }

      return (T)TypeSerializer.DeserializeFromString(serializedText, expectedType);
    }

    /// <inheritdoc />
    public string SerializeToString<T>(T item)
        => TypeSerializer.SerializeToString(item);

    /// <inheritdoc />
    public string SerializeToString(object item, Type expectedType)
        => TypeSerializer.SerializeToString(item, expectedType ?? item?.GetType());
  }
}
