using System;
using Utf8Json;

namespace CuteAnt.Extensions.Serialization
{
    /// <summary>Utf8JsonStringSerializer</summary>
    public sealed class Utf8JsonStringSerializer : IStringSerializer
    {
        private readonly IJsonFormatterResolver _defaultResolver = Utf8JsonStandardResolver.Default;

        public Utf8JsonStringSerializer() { }
        public Utf8JsonStringSerializer(IJsonFormatterResolver resolver)
        {
            if (null == resolver) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resolver); }
            _defaultResolver = resolver;
        }

        /// <inheritdoc />
        public T DeserializeFromString<T>(string serializedText)
            => JsonSerializer.Deserialize<T>(serializedText, _defaultResolver);

        /// <inheritdoc />
        public object DeserializeFromString(string serializedText, Type expectedType)
        {
            if (null == expectedType) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.expectedType); }

            return JsonSerializer.NonGeneric.Deserialize(expectedType, serializedText, _defaultResolver);
        }

        /// <inheritdoc />
        public T DeserializeFromString<T>(string serializedText, Type expectedType)
        {
            return (T)JsonSerializer.NonGeneric.Deserialize(expectedType ?? typeof(T), serializedText, _defaultResolver);
        }

        /// <inheritdoc />
        public string SerializeToString<T>(T item)
            => JsonSerializer.ToJsonString<object>(item, _defaultResolver);

        /// <inheritdoc />
        public string SerializeToString(object item, Type expectedType)
            => JsonSerializer.ToJsonString(item, _defaultResolver);
    }
}
