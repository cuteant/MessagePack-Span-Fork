using System;
using CuteAnt.Pool;
using Newtonsoft.Json;

namespace CuteAnt.Extensions.Serialization
{
    /// <summary>JsonStringSerializer</summary>
    public sealed class JsonStringSerializer : IStringSerializer
    {
        private readonly ObjectPool<JsonSerializer> _serializerPool;
        private readonly ObjectPool<JsonSerializer> _deserializerPool;

        public static readonly JsonStringSerializer Instance = new JsonStringSerializer();

        public JsonStringSerializer()
          : this(JsonConvertX.DefaultSettings, JsonConvertX.DefaultDeserializerSettings)
        {
        }
        public JsonStringSerializer(JsonSerializerSettings serializerSettings, JsonSerializerSettings deserializerSettings)
        {
            if (null == serializerSettings) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.serializerSettings); }
            if (null == deserializerSettings) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.deserializerSettings); }
            _serializerPool = JsonConvertX.GetJsonSerializerPool(serializerSettings);
            _deserializerPool = JsonConvertX.GetJsonSerializerPool(deserializerSettings);
        }

        /// <inheritdoc />
        public T DeserializeFromString<T>(string serializedText)
            => (T)_deserializerPool.DeserializeObject(serializedText, typeof(T));

        /// <inheritdoc />
        public object DeserializeFromString(string serializedText, Type expectedType)
            => _deserializerPool.DeserializeObject(serializedText, expectedType);

        /// <inheritdoc />
        public T DeserializeFromString<T>(string serializedText, Type expectedType)
            => (T)_deserializerPool.DeserializeObject(serializedText, expectedType ?? typeof(T));

        /// <inheritdoc />
        public string SerializeToString<T>(T item)
            => _serializerPool.SerializeObject(item);

        /// <inheritdoc />
        public string SerializeToString(object item, Type expectedType)
            => _serializerPool.SerializeObject(item/*, expectedType*/);
    }
}
