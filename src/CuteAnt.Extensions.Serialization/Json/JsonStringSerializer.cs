using System;
using Newtonsoft.Json;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>JsonStringSerializer</summary>
  public class JsonStringSerializer : IStringSerializer
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
      _serializerSettings = serializerSettings ?? throw new ArgumentNullException(nameof(serializerSettings));
      _deserializerSettings = deserializerSettings ?? throw new ArgumentNullException(nameof(deserializerSettings));
    }

    /// <inheritdoc />
    public T DeserializeFromString<T>(string serializedText)
        => JsonConvertX.DeserializeObject<T>(serializedText, _deserializerSettings);

    /// <inheritdoc />
    public object DeserializeFromString(string serializedText, Type expectedType)
        => JsonConvertX.DeserializeObject(serializedText, expectedType, _deserializerSettings);

    /// <inheritdoc />
    public string SerializeToString<T>(T item)
        => JsonConvertX.SerializeObject(item, typeof(T), _serializerSettings);

    /// <inheritdoc />
    public string SerializeToString(object item, Type expectedType)
        => JsonConvertX.SerializeObject(item, expectedType, _serializerSettings);
  }
}
