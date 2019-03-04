using System;
using System.Collections.Generic;
using CuteAnt.Pool;
using JsonExtensions.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CuteAnt.Extensions.Serialization
{
    /// <summary>Support deserialization of 'objects' from messages into actual types. Objects should have been
    /// serialized with JSON.NET (or some similar serializer).</summary>
    public sealed class JsonObjectTypeDeserializer : IObjectTypeDeserializer
    {
        /// <summary>Instance</summary>
        public static readonly IObjectTypeDeserializer Instance;

        private readonly ObjectPool<JsonSerializer> _jsonSerializerPool;

        #region @@ Constructors @@

        static JsonObjectTypeDeserializer()
        {
            Instance = new JsonObjectTypeDeserializer();
        }

        public JsonObjectTypeDeserializer()
          : this(JsonConvertX.DefaultDeserializerSettings)
        {
        }

        public JsonObjectTypeDeserializer(JsonSerializerSettings jsonSettings)
        {
            if (null == jsonSettings) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.jsonSettings); }

            _jsonSerializerPool = JsonConvertX.GetJsonSerializerPool(jsonSettings);
        }

        #endregion

        TValue IObjectTypeDeserializer.Deserialize<TValue>(IDictionary<string, object> dictionary)
        {
            if (null == dictionary) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary); }

            var objDictionary = JObject.FromObject(dictionary ?? new Dictionary<string, object>());

            using (var jsonReader = new JTokenReader(objDictionary))
            {
                var jsonDeserializer = _jsonSerializerPool.Take();

                try
                {
                    return jsonDeserializer.Deserialize<TValue>(jsonReader);
                }
                finally
                {
                    _jsonSerializerPool.Return(jsonDeserializer);
                }
            }
        }

        TValue IObjectTypeDeserializer.Deserialize<TValue>(IDictionary<string, object> dictionary, string key)
        {
            if (null == dictionary) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary); }

            if (dictionary.TryGetValue(key, out object value) || TryGetValueCamelCase(key, dictionary, out value))
            {
                return (TValue)Deserialize(value, typeof(TValue), false);
            }
            throw new KeyNotFoundException($"The key was not present: {key}");
        }

        TValue IObjectTypeDeserializer.Deserialize<TValue>(IDictionary<string, object> dictionary, string key, TValue defaultValue)
        {
            if (null == dictionary) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary); }

            if (dictionary.TryGetValue(key, out object value) || TryGetValueCamelCase(key, dictionary, out value))
            {
                return (TValue)Deserialize(value, typeof(TValue), false);
            }
            return defaultValue;
        }

        bool IObjectTypeDeserializer.TryDeserialize<TValue>(IDictionary<string, object> dictionary, string key, out TValue value)
        {
            if (null == dictionary) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary); }

            if (dictionary.TryGetValue(key, out object rawValue) || TryGetValueCamelCase(key, dictionary, out rawValue))
            {
                value = (TValue)Deserialize(rawValue, typeof(TValue), false);
                return true;
            }
            value = default; return false;
        }

        TValue IObjectTypeDeserializer.Deserialize<TValue>(object value) => (TValue)Deserialize(value, typeof(TValue), false);

        TValue IObjectTypeDeserializer.Deserialize<TValue>(object value, TValue defaultValue)
        {
            var result = Deserialize(value, typeof(TValue), true);
            if (null == result) { return defaultValue; }

            return (TValue)result;
        }

        public object Deserialize(object value, Type objectType, bool allowNull = false)
        {
            if (value?.GetType() == TypeConstants.CombGuidType)
            {
                if (objectType == TypeConstants.CombGuidType)
                {
                    return value;
                }
                else if (objectType == TypeConstants.StringType)
                {
                    return value.ToString();
                }
                else if (objectType == TypeConstants.GuidType)
                {
                    return (Guid)((CombGuid)value);
                }
                else if (objectType == TypeConstants.ByteArrayType)
                {
                    //return ((CombGuid)value).ToByteArray();
                    return ((CombGuid)value).GetByteArray();
                }
                else if (objectType == typeof(DateTime))
                {
                    return ((CombGuid)value).DateTime;
                }
                else if (objectType == typeof(DateTimeOffset))
                {
                    return new DateTimeOffset(((CombGuid)value).DateTime);
                }
                value = value.ToString();
            }
            var token = value as JToken ?? new JValue(value);
            if (token.Type == JTokenType.Null && allowNull) { return null; }

            using (var jsonReader = new JTokenReader(token))
            {
                var jsonDeserializer = _jsonSerializerPool.Take();

                try
                {
                    return jsonDeserializer.Deserialize(jsonReader, objectType);
                }
                finally
                {
                    _jsonSerializerPool.Return(jsonDeserializer);
                }
            }
        }

        #region -- 静态方法 --

        public static TValue Deserialize<TValue>(IDictionary<string, object> dictionary)
        {
            return Instance.Deserialize<TValue>(dictionary);
        }

        public static TValue Deserialize<TValue>(IDictionary<string, object> dictionary, string key)
        {
            return Instance.Deserialize<TValue>(dictionary, key);
        }

        public static TValue Deserialize<TValue>(IDictionary<string, object> dictionary, string key, TValue defaultValue)
        {
            return Instance.Deserialize(dictionary, key, defaultValue);
        }

        public static TValue Deserialize<TValue>(object value)
        {
            return Instance.Deserialize<TValue>(value);
        }

        public static TValue Deserialize<TValue>(object value, TValue defaultValue)
        {
            return Instance.Deserialize<TValue>(value, defaultValue);
        }

        #endregion

        #region **& TryGetValueCamelCase &**

        private static bool TryGetValueCamelCase(string key, IDictionary<string, object> dictionary, out object value)
        {
            if (char.IsUpper(key[0]))
            {
                var camelCaseKey = StringUtils.ToCamelCase(key);
                return dictionary.TryGetValue(camelCaseKey, out value);
            }

            value = null;
            return false;
        }

        #endregion
    }
}