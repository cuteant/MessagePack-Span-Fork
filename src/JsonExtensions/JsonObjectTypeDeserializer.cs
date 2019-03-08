using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using CuteAnt;
using CuteAnt.Pool;
using JsonExtensions.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonExtensions
{
    /// <summary>JsonObjectTypeDeserializer</summary>
    public static class JsonObjectTypeDeserializer
    {
        private static readonly JTokenType[] BigIntegerTypes;

        private static readonly JsonSerializerSettings FromSettings;
        private static readonly ObjectPool<JsonSerializer> JsonSerializerPool;

        static JsonObjectTypeDeserializer()
        {
            BigIntegerTypes = new[] { JTokenType.Integer, JTokenType.Float, JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Boolean, JTokenType.Bytes };

            FromSettings = new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                SerializationBinder = JsonSerializationBinder.Instance,
                Converters = new JsonConverter[] { JsonConvertX.DefaultStringEnumConverter, JsonConvertX.DefaultCombGuidConverter, JsonConvertX.DefaultIpAddressConverter, JsonConvertX.DefaultIpEndPointConverter }
            };

            JsonSerializerPool = JsonConvertX.GetJsonSerializerPool(FromSettings);
        }

        /// <summary>TBD</summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static TValue Deserialize<TValue>(this IDictionary<string, object> dictionary)
        {
            var objDictionary = JObject.FromObject(dictionary ?? new Dictionary<string, object>());

            return (TValue)ToObject(objDictionary, typeof(TValue));
        }

        /// <summary>TBD</summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue Deserialize<TValue>(this IDictionary<string, object> dictionary, string key)
        {
            if (null == dictionary) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary); }
            if (null == key) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key); }

            if (dictionary.TryGetValue(key, out object value) || TryGetValueCamelCase(key, dictionary, out value))
            {
                return (TValue)Deserialize(value, typeof(TValue), false);
            }
            throw new KeyNotFoundException($"The key was not present: {key}");
        }

        /// <summary>TBD</summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TValue Deserialize<TValue>(this IDictionary<string, object> dictionary, string key, TValue defaultValue)
        {
            if (null == dictionary) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary); }
            if (null == key) { return defaultValue; }

            if (dictionary.TryGetValue(key, out object value) || TryGetValueCamelCase(key, dictionary, out value))
            {
                return (TValue)Deserialize(value, typeof(TValue), false);
            }
            return defaultValue;
        }

        /// <summary>TBD</summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryDeserialize<TValue>(this IDictionary<string, object> dictionary, string key, out TValue value)
        {
            if (null == dictionary) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary); }
            if (null == key) { value = default; return false; }

            if (dictionary.TryGetValue(key, out object rawValue) || TryGetValueCamelCase(key, dictionary, out rawValue))
            {
                value = (TValue)Deserialize(rawValue, typeof(TValue), false);
                return true;
            }
            value = default; return false;
        }

        /// <summary>Creates an instance of the specified .NET type from the <see cref="Object"/>.</summary>
        /// <typeparam name="T">The object type that the token will be deserialized to.</typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns>The new object created from the JSON value.</returns>
        public static T Deserialize<T>(object value, T defaultValue)
        {
            var result = Deserialize(value, typeof(T), true);
            if (null == result) { return defaultValue; }

            return (T)result;
        }

        /// <summary>Creates an instance of the specified .NET type from the <see cref="Object"/>.</summary>
        /// <typeparam name="T">The object type that the token will be deserialized to.</typeparam>
        /// <param name="value"></param>
        /// <param name="allowNull"></param>
        /// <returns>The new object created from the JSON value.</returns>
        public static T Deserialize<T>(object value, bool allowNull = false)
        {
            return (T)Deserialize(value, typeof(T), allowNull);
        }

        /// <summary>Creates an instance of the specified .NET type from the <see cref="Object"/>.</summary>
        /// <param name="value"></param>
        /// <param name="objectType">The object type that the token will be deserialized to.</param>
        /// <param name="allowNull"></param>
        /// <returns>The new object created from the JSON value.</returns>
        public static object Deserialize(object value, Type objectType, bool allowNull = false)
        {
            if (null == objectType) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.objectType); }

            if (value is CombGuid comb)
            {
                if (TryConvert(comb, objectType, out object result))
                {
                    return result;
                }
                value = comb.ToString();
            }
            var token = value as JToken ?? new JValue(value);
            if (token.Type == JTokenType.Null && allowNull) { return null; }

            var typeCode = ConvertUtils.GetTypeCode(objectType, out bool isEnum);

            if (isEnum)
            {
                if (token.Type == JTokenType.String)
                {
                    try
                    {
                        // use serializer so JsonConverter(typeof(StringEnumConverter)) + EnumMemberAttributes are respected
                        return ToObject(token, objectType);
                    }
                    catch (Exception ex)
                    {
                        Type enumType = objectType.IsEnum() ? objectType : Nullable.GetUnderlyingType(objectType);
                        throw new ArgumentException("Could not convert '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, (string)token, enumType.Name), ex);
                    }
                }

                if (token.Type == JTokenType.Integer)
                {
                    Type enumType = objectType.IsEnum() ? objectType : Nullable.GetUnderlyingType(objectType);
                    return Enum.ToObject(enumType, ((JValue)token).Value);
                }
            }

            try
            {
                switch (typeCode)
                {
                    case PrimitiveTypeCode.BooleanNullable:
                        return (bool?)token;
                    case PrimitiveTypeCode.Boolean:
                        return (bool)token;
                    case PrimitiveTypeCode.CharNullable:
                        return (char?)token;
                    case PrimitiveTypeCode.Char:
                        return (char)token;
                    case PrimitiveTypeCode.SByte:
                        return (sbyte)token;
                    case PrimitiveTypeCode.SByteNullable:
                        return (sbyte?)token;
                    case PrimitiveTypeCode.ByteNullable:
                        return (byte?)token;
                    case PrimitiveTypeCode.Byte:
                        return (byte)token;
                    case PrimitiveTypeCode.Int16Nullable:
                        return (short?)token;
                    case PrimitiveTypeCode.Int16:
                        return (short)token;
                    case PrimitiveTypeCode.UInt16Nullable:
                        return (ushort?)token;
                    case PrimitiveTypeCode.UInt16:
                        return (ushort)token;
                    case PrimitiveTypeCode.Int32Nullable:
                        return (int?)token;
                    case PrimitiveTypeCode.Int32:
                        return (int)token;
                    case PrimitiveTypeCode.UInt32Nullable:
                        return (uint?)token;
                    case PrimitiveTypeCode.UInt32:
                        return (uint)token;
                    case PrimitiveTypeCode.Int64Nullable:
                        return (long?)token;
                    case PrimitiveTypeCode.Int64:
                        return (long)token;
                    case PrimitiveTypeCode.UInt64Nullable:
                        return (ulong?)token;
                    case PrimitiveTypeCode.UInt64:
                        return (ulong)token;
                    case PrimitiveTypeCode.SingleNullable:
                        return (float?)token;
                    case PrimitiveTypeCode.Single:
                        return (float)token;
                    case PrimitiveTypeCode.DoubleNullable:
                        return (double?)token;
                    case PrimitiveTypeCode.Double:
                        return (double)token;
                    case PrimitiveTypeCode.DecimalNullable:
                        return (decimal?)token;
                    case PrimitiveTypeCode.Decimal:
                        return (decimal)token;
                    //case PrimitiveTypeCode.DateTimeNullable:
                    //    return (DateTime?)token;
                    //case PrimitiveTypeCode.DateTime:
                    //    return (DateTime)token;
#if HAVE_DATE_TIME_OFFSET
                    //case PrimitiveTypeCode.DateTimeOffsetNullable:
                    //    return (DateTimeOffset?)token;
                    //case PrimitiveTypeCode.DateTimeOffset:
                    //    return (DateTimeOffset)token;
#endif
                    case PrimitiveTypeCode.String:
                        return (string)token;
                    case PrimitiveTypeCode.GuidNullable:
                        return (Guid?)token;
                    case PrimitiveTypeCode.Guid:
                        return (Guid)token;
                    case PrimitiveTypeCode.Uri:
                        return (Uri)token;
                    case PrimitiveTypeCode.TimeSpanNullable:
                        return (TimeSpan?)token;
                    case PrimitiveTypeCode.TimeSpan:
                        return (TimeSpan)token;
#if HAVE_BIG_INTEGER
                    case PrimitiveTypeCode.BigIntegerNullable:
                        return ToBigIntegerNullable(token);
                    case PrimitiveTypeCode.BigInteger:
                        return ToBigInteger(token);
#endif
                }
            }
            catch { }
            return ToObject(token, objectType);
        }

        private static object ToObject(JToken token, Type objectType)
        {
            try
            {
                using (var jsonReader = new JTokenReader(token))
                {
                    var jsonDeserializer = JsonSerializerPool.Take();

                    try
                    {
                        return jsonDeserializer.Deserialize(jsonReader, objectType);
                    }
                    finally
                    {
                        JsonSerializerPool.Return(jsonDeserializer);
                    }
                }
            }
            catch
            {
                return JsonConvertX.DeserializeObject(token.ToString(), objectType, FromSettings);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool TryConvert(CombGuid comb, Type objectType, out object value)
        {
            if (objectType == TypeConstants.CombGuidType)
            {
                value = comb;
                return true;
            }
            else if (objectType == TypeConstants.StringType)
            {
                value = comb.ToString();
                return true;
            }
            else if (objectType == TypeConstants.GuidType)
            {
                value = (Guid)comb;
                return true;
            }
            else if (objectType == TypeConstants.ByteArrayType)
            {
                //return ((CombGuid)value).ToByteArray();
                value = comb.GetByteArray();
                return true;
            }
            else if (objectType == typeof(DateTime))
            {
                value = comb.DateTime;
                return true;
            }
            else if (objectType == typeof(DateTimeOffset))
            {
                value = new DateTimeOffset(comb.DateTime);
                return true;
            }
            value = null; return false;
        }

        private static bool TryGetValueCamelCase(string key, IDictionary<string, object> dictionary, out object value)
        {
            if (char.IsUpper(key[0]))
            {
                var camelCaseKey = StringUtils.ToCamelCase(key);
                return dictionary.TryGetValue(camelCaseKey, out value);
            }
            value = null; return false;
        }

        private static BigInteger ToBigInteger(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, BigIntegerTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to BigInteger.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            return ConvertUtils.ToBigInteger(v.Value);
        }

        private static BigInteger? ToBigIntegerNullable(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, BigIntegerTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to BigInteger.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value == null)
            {
                return null;
            }

            return ConvertUtils.ToBigInteger(v.Value);
        }

        private static JValue EnsureValue(JToken value)
        {
            if (value == null) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value); }

            if (value is JProperty property)
            {
                value = property.Value;
            }

            JValue v = value as JValue;

            return v;
        }

        private static string GetType(JToken token)
        {
            if (token == null) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.token); }

            if (token is JProperty p) { token = p.Value; }

            return token.Type.ToString();
        }

        private static bool ValidateToken(JToken o, JTokenType[] validTypes, bool nullable)
        {
            return (Array.IndexOf(validTypes, o.Type) != -1) || (nullable && (o.Type == JTokenType.Null || o.Type == JTokenType.Undefined));
        }
    }
}
