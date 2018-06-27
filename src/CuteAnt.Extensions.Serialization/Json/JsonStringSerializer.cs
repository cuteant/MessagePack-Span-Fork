using System;
using Newtonsoft.Json;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>JsonStringSerializer</summary>
  public sealed class JsonStringSerializer : IStringSerializer
  {
    private JsonSerializerSettings _serializerSettings;
    private JsonSerializerSettings _deserializerSettings;

    public static readonly JsonStringSerializer Instance = new JsonStringSerializer();

    public JsonStringSerializer()
      : this(JsonConvertX.DefaultSettings, JsonConvertX.DefaultDeserializerSettings)
    {
    }
    public JsonStringSerializer(JsonSerializerSettings serializerSettings, JsonSerializerSettings deserializerSettings)
    {
      if (null == serializerSettings) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.serializerSettings); }
      if (null == deserializerSettings) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.deserializerSettings); }
      _serializerSettings = serializerSettings;
      _deserializerSettings = deserializerSettings;
    }

    /// <inheritdoc />
    public T DeserializeFromString<T>(string serializedText)
        => JsonConvertX.DeserializeObject<T>(serializedText, _deserializerSettings);

    /// <inheritdoc />
    public object DeserializeFromString(string serializedText, Type expectedType)
        => JsonConvertX.DeserializeObject(serializedText, expectedType, _deserializerSettings);

    /// <inheritdoc />
    public T DeserializeFromString<T>(string serializedText, Type expectedType)
        => (T)JsonConvertX.DeserializeObject(serializedText, expectedType, _deserializerSettings);

    /// <inheritdoc />
    public string SerializeToString<T>(T item)
        => JsonConvertX.SerializeObject(item, typeof(T), _serializerSettings);

    /// <inheritdoc />
    public string SerializeToString(object item, Type expectedType)
        => JsonConvertX.SerializeObject(item, expectedType, _serializerSettings);
  }
}
