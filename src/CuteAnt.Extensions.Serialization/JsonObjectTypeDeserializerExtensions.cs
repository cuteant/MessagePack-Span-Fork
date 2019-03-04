using System;
using System.Collections.Generic;

namespace CuteAnt.Extensions.Serialization
{
    /// <summary>Support deserialization of 'objects' from messages into actual types. Objects should have been
    /// serialized with JSON.NET (or some similar serializer).</summary>
    public static class JsonObjectTypeDeserializerExtensions
    {
        private static readonly IObjectTypeDeserializer _objectTypeDeserializer;

        static JsonObjectTypeDeserializerExtensions()
        {
            _objectTypeDeserializer = JsonObjectTypeDeserializer.Instance;
        }

        public static TValue Deserialize<TValue>(this IDictionary<string, object> dictionary)
        {
            return _objectTypeDeserializer.Deserialize<TValue>(dictionary);
        }

        public static TValue Deserialize<TValue>(this IDictionary<string, object> dictionary, string key)
        {
            return _objectTypeDeserializer.Deserialize<TValue>(dictionary, key);
        }

        public static TValue Deserialize<TValue>(this IDictionary<string, object> dictionary, string key, TValue defaultValue)
        {
            return _objectTypeDeserializer.Deserialize(dictionary, key, defaultValue);
        }

        public static TValue Deserialize<TValue>(this object value)
        {
            return _objectTypeDeserializer.Deserialize<TValue>(value);
        }

        public static TValue Deserialize<TValue>(this object value, TValue defaultValue)
        {
            return _objectTypeDeserializer.Deserialize<TValue>(value, defaultValue);
        }

        public static object Deserialize(this object value, Type expectedType, bool allowNull = false)
        {
            return _objectTypeDeserializer.Deserialize(value, expectedType, allowNull);
        }
    }
}