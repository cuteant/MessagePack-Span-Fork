using System;
using System.Collections.Generic;
using JsonExtensions;

namespace CuteAnt.Extensions.Serialization
{
    /// <summary>Support deserialization of 'objects' from messages into actual types. Objects should have been
    /// serialized with JSON.NET (or some similar serializer).</summary>
    public sealed class DefaultObjectTypeDeserializer : IObjectTypeDeserializer
    {
        public static readonly DefaultObjectTypeDeserializer Instance = new DefaultObjectTypeDeserializer();

        private DefaultObjectTypeDeserializer() { }

        public TValue Deserialize<TValue>(IDictionary<string, object> dictionary)
        {
            return JsonObjectTypeDeserializer.Deserialize<TValue>(dictionary);
        }

        public TValue Deserialize<TValue>(IDictionary<string, object> dictionary, string key)
        {
            return JsonObjectTypeDeserializer.Deserialize<TValue>(dictionary, key);
        }

        public TValue Deserialize<TValue>(IDictionary<string, object> dictionary, string key, TValue defaultValue)
        {
            return JsonObjectTypeDeserializer.Deserialize<TValue>(dictionary, key, defaultValue);
        }

        public bool TryDeserialize<TValue>(IDictionary<string, object> dictionary, string key, out TValue value)
        {
            return JsonObjectTypeDeserializer.TryDeserialize<TValue>(dictionary, key, out value);
        }

        public TValue Deserialize<TValue>(object value)
        {
            return JsonObjectTypeDeserializer.Deserialize<TValue>(value, false);
        }

        public TValue Deserialize<TValue>(object value, TValue defaultValue)
        {
            return JsonObjectTypeDeserializer.Deserialize<TValue>(value, defaultValue);
        }

        public object Deserialize(object value, Type objectType, bool allowNull = false)
        {
            return JsonObjectTypeDeserializer.Deserialize(value, objectType, allowNull);
        }
    }
}