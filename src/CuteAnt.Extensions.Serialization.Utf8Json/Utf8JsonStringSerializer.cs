using System;
using Utf8Json;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>Utf8JsonStringSerializer</summary>
  public class Utf8JsonStringSerializer : IStringSerializer
  {
    public Utf8JsonStringSerializer()
    {
    }

    /// <inheritdoc />
    public T DeserializeFromString<T>(string serializedText)
        => JsonSerializer.Deserialize<T>(serializedText);

    /// <inheritdoc />
    public object DeserializeFromString(string serializedText, Type expectedType)
    {
      if (null == expectedType) { throw new ArgumentNullException(nameof(expectedType)); }

      return JsonSerializer.NonGeneric.Deserialize(expectedType, serializedText);
    }

    /// <inheritdoc />
    public string SerializeToString<T>(T item)
        => JsonSerializer.ToJsonString(item);

    /// <inheritdoc />
    public string SerializeToString(object item, Type expectedType)
        => JsonSerializer.NonGeneric.ToJsonString(expectedType ?? item?.GetType(), item);
  }
}
