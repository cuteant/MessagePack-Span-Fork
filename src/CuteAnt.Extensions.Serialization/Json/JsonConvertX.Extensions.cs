using System;
using System.Buffers;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using CuteAnt;
using CuteAnt.Buffers;
using CuteAnt.Collections;
using CuteAnt.Extensions.Serialization;
using CuteAnt.Extensions.Serialization.Json;
using CuteAnt.Extensions.Serialization.Json.Utilities;
using CuteAnt.Pool;
using CuteAnt.Reflection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
//#if !NET40
//using CuteAnt.IO.Pipelines;
//#endif

namespace Newtonsoft.Json
{
    partial class JsonConvertX
    {
        #region @@ Constructors @@

        private static readonly FieldInfo s_checkAdditionalContentField;
        private static readonly MemberGetter<JsonSerializer> s_checkAdditionalContentGetter;
        private static readonly MemberSetter<JsonSerializer> s_checkAdditionalContentSetter;

        private static readonly FieldInfo s_formattingField;
        private static readonly MemberGetter<JsonSerializer> s_formattingGetter;
        private static readonly MemberSetter<JsonSerializer> s_formattingSetter;

        private static readonly DictionaryCache<JsonSerializerSettings, ObjectPool<JsonSerializer>> s_jsonSerializerPoolCache;
        private static readonly ObjectPool<JsonSerializer> s_defaultJsonSerializerPool;

        public static readonly IArrayPool<char> GlobalCharacterArrayPool;

        public static readonly IContractResolver DefaultContractResolver;
        public static readonly IContractResolver DefaultCamelCaseContractResolver;
        public static readonly IPAddressConverter DefaultIpAddressConverter;
        public static readonly IPEndPointConverter DefaultIpEndPointConverter;
        public static readonly CombGuidConverter DefaultCombGuidConverter;
        public static readonly StringEnumConverter DefaultStringEnumConverter;
        public static readonly StringEnumConverter DefaultStringEnumCamelCaseConverter;
        public static readonly StringEnumConverter DefaultStringEnumSnakeCaseConverter;

        public static readonly JsonSerializerSettings DefaultSettings;
        public static readonly JsonSerializerSettings CamelCaseSettings;

        public static readonly JsonSerializerSettings IndentedSettings;
        public static readonly JsonSerializerSettings IndentedCamelCaseSettings;

        public static readonly JsonSerializerSettings DefaultDeserializerSettings;

        public static readonly ISerializationBinder DefaultSerializationBinder;

        static JsonConvertX()
        {
            s_checkAdditionalContentField = typeof(JsonSerializer).LookupTypeField("_checkAdditionalContent");
            s_checkAdditionalContentGetter = s_checkAdditionalContentField.GetValueGetter<JsonSerializer>();
            s_checkAdditionalContentSetter = s_checkAdditionalContentField.GetValueSetter<JsonSerializer>();

            s_formattingField = typeof(JsonSerializer).LookupTypeField("_formatting");
            s_formattingGetter = s_formattingField.GetValueGetter<JsonSerializer>();
            s_formattingSetter = s_formattingField.GetValueSetter<JsonSerializer>();

            s_defaultJsonSerializerPool = _defaultObjectPoolProvider.Create(new JsonSerializerObjectPolicy(null));
            s_jsonSerializerPoolCache = new DictionaryCache<JsonSerializerSettings, ObjectPool<JsonSerializer>>(DictionaryCacheConstants.SIZE_SMALL);

            GlobalCharacterArrayPool = new JsonArrayPool<char>(ArrayPool<char>.Shared);

            DefaultContractResolver = new JsonContractResolver();
            DefaultCamelCaseContractResolver = new JsonCamelCasePropertyNamesContractResolver();
            DefaultIpAddressConverter = new IPAddressConverter();
            DefaultIpEndPointConverter = new IPEndPointConverter();
            DefaultCombGuidConverter = new CombGuidConverter();
            DefaultStringEnumConverter = new StringEnumConverter();
            DefaultStringEnumCamelCaseConverter = new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() };
            DefaultStringEnumSnakeCaseConverter = new StringEnumConverter { NamingStrategy = new SnakeCaseNamingStrategy() };

            DefaultSettings = CreateSerializerSettings(Formatting.None, TypeNameHandling.All, TypeNameAssemblyFormatHandling.Simple);
            CamelCaseSettings = CreateSerializerSettings(Formatting.None, TypeNameHandling.All, TypeNameAssemblyFormatHandling.Simple, true);

            IndentedSettings = CreateSerializerSettings(Formatting.Indented, TypeNameHandling.All, TypeNameAssemblyFormatHandling.Simple);
            IndentedCamelCaseSettings = CreateSerializerSettings(Formatting.Indented, TypeNameHandling.All, TypeNameAssemblyFormatHandling.Simple, true);

            DefaultDeserializerSettings = CreateSerializerSettings(Formatting.None, TypeNameHandling.All, TypeNameAssemblyFormatHandling.Simple);
            DefaultDeserializerSettings.DateParseHandling = DateParseHandling.None;

            DefaultSerializationBinder = JsonSerializationBinder.Instance;
        }

        #endregion

        #region -- JsonSerializerSettings --

        /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the default settings.</summary>
        public static JsonSerializerSettings CreateSerializerSettings(Formatting formatting, TypeNameHandling? typeNameHandling = null,
          TypeNameAssemblyFormatHandling? typeNameAssemblyFormatHandling = null, bool camelCase = false)
        {
            var setting = CreateJsonSerializerSettings();

            if (formatting == Formatting.Indented) { setting.Formatting = Formatting.Indented; }

            if (typeNameHandling.HasValue)
            {
                setting.TypeNameHandling = typeNameHandling.Value;
            }
            if (typeNameAssemblyFormatHandling.HasValue)
            {
                setting.TypeNameAssemblyFormatHandling = typeNameAssemblyFormatHandling.Value;
            }
            setting.ContractResolver = camelCase ? DefaultCamelCaseContractResolver : DefaultContractResolver;
            setting.Converters.Add(camelCase ? DefaultStringEnumCamelCaseConverter : DefaultStringEnumConverter);

            return setting;
        }

        /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the limit properties settings.</summary>
        public static JsonSerializerSettings CreateLimitPropsSerializerSettings(ICollection<String> limitProps, Formatting formatting = Formatting.None,
          TypeNameHandling? typeNameHandling = null, TypeNameAssemblyFormatHandling? typeNameAssemblyFormatHandling = null, bool camelCase = false)
        {
            var setting = CreateSerializerSettings(formatting, typeNameHandling, typeNameAssemblyFormatHandling);
            if (camelCase)
            {
                setting.ContractResolver = new JsonCamelCasePropertyNamesLimitPropsContractResolver(limitProps);
                setting.Converters.Add(DefaultStringEnumCamelCaseConverter);
            }
            else
            {
                setting.ContractResolver = new JsonLimitPropsContractResolver(limitProps);
                setting.Converters.Add(DefaultStringEnumConverter);
            }

            return setting;
        }

        /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the property mappings settings.</summary>
        public static JsonSerializerSettings CreatePropertyMappingSerializerSettings(IDictionary<String, String> propertyMappings, bool limitProps = false,
          Formatting formatting = Formatting.None, TypeNameHandling? typeNameHandling = null, TypeNameAssemblyFormatHandling? typeNameAssemblyFormatHandling = null)
        {
            var setting = CreateSerializerSettings(formatting, typeNameHandling, typeNameAssemblyFormatHandling);
            setting.ContractResolver = new JsonPropertyMappingContractResolver(propertyMappings, limitProps);

            return setting;
        }

        private static JsonSerializerSettings CreateJsonSerializerSettings()
        {
            var setting = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,

                ObjectCreationHandling = ObjectCreationHandling.Replace,
                FloatParseHandling = FloatParseHandling.Decimal,
            };

            if (setting.TypeNameHandling == TypeNameHandling.All) { setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects; }
            setting.Converters.Add(DefaultIpAddressConverter);
            setting.Converters.Add(DefaultIpEndPointConverter);
            setting.Converters.Add(DefaultCombGuidConverter);

            setting.SerializationBinder = new JsonSerializationBinder();

            return setting;
        }

        #endregion

        #region -- JsonSerializer.IsCheckAdditionalContentSetX --

        [MethodImpl(InlineMethod.Value)]
        public static bool IsCheckAdditionalContentSetX(this JsonSerializer jsonSerializer)
        {
            if (null == jsonSerializer) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.jsonSerializer); }
            return s_checkAdditionalContentGetter(jsonSerializer) != null;
        }

        [MethodImpl(InlineMethod.Value)]
        public static bool? GetCheckAdditionalContent(this JsonSerializer jsonSerializer)
        {
            if (null == jsonSerializer) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.jsonSerializer); }
            return (bool?)s_checkAdditionalContentGetter(jsonSerializer);
        }

        [MethodImpl(InlineMethod.Value)]
        public static void SetCheckAdditionalContent(this JsonSerializer jsonSerializer, bool? checkAdditionalContent = null)
        {
            if (null == jsonSerializer) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.jsonSerializer); }
            s_checkAdditionalContentSetter(jsonSerializer, checkAdditionalContent);
        }

        #endregion

        #region -- JsonSerializer.Formatting --

        [MethodImpl(InlineMethod.Value)]
        public static Formatting? GetFormatting(this JsonSerializer jsonSerializer)
        {
            if (null == jsonSerializer) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.jsonSerializer); }
            return (Formatting?)s_formattingGetter(jsonSerializer);
        }

        [MethodImpl(InlineMethod.Value)]
        public static void SetFormatting(this JsonSerializer jsonSerializer, Formatting? formatting = null)
        {
            if (null == jsonSerializer) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.jsonSerializer); }
            s_formattingSetter(jsonSerializer, formatting);
        }

        #endregion

        #region == JsonDynamicContract.TryGetMemberX ==

        private static readonly ThreadSafeStore<string, CallSite<Func<CallSite, object, object>>> _callSiteGetters =
            new ThreadSafeStore<string, CallSite<Func<CallSite, object, object>>>(CreateCallSiteGetter);

        private static CallSite<Func<CallSite, object, object>> CreateCallSiteGetter(string name)
        {
            GetMemberBinder getMemberBinder = (GetMemberBinder)DynamicUtils.BinderWrapper.GetMember(name, typeof(DynamicUtils));

            return CallSite<Func<CallSite, object, object>>.Create(new NoThrowGetBinderMember(getMemberBinder));
        }
        internal static bool TryGetMemberX(this JsonDynamicContract jsonDynamicContract, IDynamicMetaObjectProvider dynamicProvider, string name, out object value)
        {
            ValidationUtils.ArgumentNotNull(dynamicProvider, nameof(dynamicProvider));

            CallSite<Func<CallSite, object, object>> callSite = _callSiteGetters.Get(name);

            object result = callSite.Target(callSite, dynamicProvider);

            if (!ReferenceEquals(result, NoThrowExpressionVisitor.ErrorResult))
            {
                value = result;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        #endregion

        #region == CreateJsonSerializationException ==

        internal static JsonSerializationException CreateJsonSerializationException(JsonReader reader, string message)
        {
            return CreateJsonSerializationException(reader, message, null);
        }

        internal static JsonSerializationException CreateJsonSerializationException(JsonReader reader, string message, Exception ex)
        {
            return CreateJsonSerializationException(reader as IJsonLineInfo, reader.Path, message, ex);
        }

        internal static JsonSerializationException CreateJsonSerializationException(IJsonLineInfo lineInfo, string path, string message, Exception ex)
        {
            message = JsonPosition.FormatMessage(lineInfo, path, message);

            return new JsonSerializationException(message, ex);
        }

        #endregion

        #region -- Allocate & Free JsonSerializer --

        public static ObjectPool<JsonSerializer> GetJsonSerializerPool(JsonSerializerSettings jsonSettings)
        {
            return s_jsonSerializerPoolCache.GetItem(jsonSettings, s_getJsonSerializerPoolFunc);
        }

        public static JsonSerializer AllocateSerializer(JsonSerializerSettings jsonSettings)
        {
            if (null == jsonSettings) { return s_defaultJsonSerializerPool.Take(); }

            var pool = s_jsonSerializerPoolCache.GetItem(jsonSettings, s_getJsonSerializerPoolFunc);
            return pool.Take();
            //return JsonSerializer.CreateDefault(jsonSettings);
        }

        [MethodImpl(InlineMethod.Value)]
        private static JsonSerializer AllocateSerializerInternal(JsonSerializerSettings jsonSettings)
        {
            if (null == jsonSettings) { return s_defaultJsonSerializerPool.Take(); }

            var pool = s_jsonSerializerPoolCache.GetItem(jsonSettings, s_getJsonSerializerPoolFunc);
            return pool.Take();
            //return JsonSerializer.CreateDefault(jsonSettings);
        }

        public static void FreeSerializer(JsonSerializerSettings jsonSettings, JsonSerializer jsonSerializer)
        {
            if (null == jsonSettings) { s_defaultJsonSerializerPool.Return(jsonSerializer); return; }

            if (s_jsonSerializerPoolCache.TryGetValue(jsonSettings, out ObjectPool<JsonSerializer> pool))
            {
                pool.Return(jsonSerializer);
            }
        }

        [MethodImpl(InlineMethod.Value)]
        private static void FreeSerializerInternal(JsonSerializerSettings jsonSettings, JsonSerializer jsonSerializer)
        {
            if (null == jsonSettings) { s_defaultJsonSerializerPool.Return(jsonSerializer); return; }

            if (s_jsonSerializerPoolCache.TryGetValue(jsonSettings, out ObjectPool<JsonSerializer> pool))
            {
                pool.Return(jsonSerializer);
            }
        }

        private static readonly Func<JsonSerializerSettings, ObjectPool<JsonSerializer>> s_getJsonSerializerPoolFunc = GetJsonSerializerPoolInternal;
        private static readonly SynchronizedObjectPoolProvider _defaultObjectPoolProvider = SynchronizedObjectPoolProvider.Default;
        private static ObjectPool<JsonSerializer> GetJsonSerializerPoolInternal(JsonSerializerSettings jsonSettings)
        {
            return _defaultObjectPoolProvider.Create(new JsonSerializerObjectPolicy(jsonSettings));
        }

        #endregion

        #region -- Serialize to Byte-Array --

        private const int c_initialBufferSize = JsonMessageFormatterExtensions.c_initialBufferSize;
        private const int c_zeroSize = 0;

        /// <summary>Serializes the specified object to a JSON byte array.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static byte[] SerializeToByteArray(object value, int initialBufferSize = c_initialBufferSize)
        {
            return SerializeToByteArray(value, null, (JsonSerializerSettings)null, initialBufferSize);
        }

        /// <summary>Serializes the specified object to a JSON byte array using formatting.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static byte[] SerializeToByteArray(object value, Formatting formatting, int initialBufferSize = c_initialBufferSize)
        {
            return SerializeToByteArray(value, formatting, (JsonSerializerSettings)null, initialBufferSize);
        }

        /// <summary>Serializes the specified object to a JSON byte array using a collection of <see cref="JsonConverter"/>.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="converters">A collection of converters used while serializing.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static byte[] SerializeToByteArray(object value, params JsonConverter[] converters)
        {
            if (converters != null && converters.Length > 0)
            {
                var settings = new JsonSerializerSettings { Converters = converters };
                var jsonSerializer = JsonSerializer.CreateDefault(settings);
                return SerializeToByteArrayInternal(value, null, jsonSerializer);
            }
            else
            {
                return SerializeToByteArray(value, null, (JsonSerializerSettings)null);
            }
        }

        /// <summary>Serializes the specified object to a JSON byte array using formatting and a collection of <see cref="JsonConverter"/>.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="converters">A collection of converters used while serializing.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static byte[] SerializeToByteArray(object value, Formatting formatting, params JsonConverter[] converters)
        {
            if (converters != null && converters.Length > 0)
            {
                var settings = new JsonSerializerSettings { Converters = converters };
                var jsonSerializer = JsonSerializer.CreateDefault(settings);
                jsonSerializer.Formatting = formatting;
                return SerializeToByteArrayInternal(value, null, jsonSerializer);
            }
            else
            {
                return SerializeToByteArray(value, null, formatting, (JsonSerializerSettings)null);
            }
        }

        /// <summary>Serializes the specified object to a JSON byte array using <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static byte[] SerializeToByteArray(object value, JsonSerializerSettings settings, int initialBufferSize = c_initialBufferSize)
        {
            return SerializeToByteArray(value, null, settings, initialBufferSize);
        }

        /// <summary>Serializes the specified object to a JSON byte array using a type, formatting and <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="type">The type of the value being serialized.
        /// This parameter is used when <see cref="JsonSerializer.TypeNameHandling"/> is <see cref="TypeNameHandling.Auto"/> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static byte[] SerializeToByteArray(object value, Type type, JsonSerializerSettings settings, int initialBufferSize = c_initialBufferSize)
        {
            var jsonSerializer = JsonSerializer.Create(settings);

            return SerializeToByteArrayInternal(value, type, jsonSerializer, initialBufferSize);
        }

        /// <summary>Serializes the specified object to a JSON byte array using formatting and <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static byte[] SerializeToByteArray(object value, Formatting formatting, JsonSerializerSettings settings, int initialBufferSize = c_initialBufferSize)
        {
            return SerializeToByteArray(value, null, formatting, settings, initialBufferSize);
        }

        /// <summary>Serializes the specified object to a JSON byte array using a type, formatting and <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="type">The type of the value being serialized.
        /// This parameter is used when <see cref="JsonSerializer.TypeNameHandling"/> is <see cref="TypeNameHandling.Auto"/> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static byte[] SerializeToByteArray(object value, Type type, Formatting formatting, JsonSerializerSettings settings, int initialBufferSize = c_initialBufferSize)
        {
            var jsonSerializer = JsonSerializer.Create(settings);
            jsonSerializer.Formatting = formatting;

            return SerializeToByteArrayInternal(value, type, jsonSerializer, initialBufferSize);
        }

        private static byte[] SerializeToByteArrayInternal(object value, Type type, JsonSerializer jsonSerializer, int initialBufferSize = c_initialBufferSize)
        {
            //#if NET40
            using (var pooledOutputStream = BufferManagerOutputStreamManager.Create())
            {
                var outputStream = pooledOutputStream.Object;
                outputStream.Reinitialize(initialBufferSize);
                using (JsonTextWriter jsonWriter = new JsonTextWriter(new StreamWriterX(outputStream)))
                {
                    jsonWriter.ArrayPool = GlobalCharacterArrayPool;
                    //jsonWriter.CloseOutput = false;
                    jsonWriter.Formatting = jsonSerializer.Formatting;

                    jsonSerializer.Serialize(jsonWriter, value, type);
                    jsonWriter.Flush();
                }
                return outputStream.ToByteArray();
            }
            //#else
            //      using (var pooledPipe = PipelineManager.Create())
            //      {
            //        var pipe = pooledPipe.Object;
            //        var outputStream = new PipelineStream(pipe, initialBufferSize);
            //        using (JsonTextWriter jsonWriter = new JsonTextWriter(new StreamWriterX(outputStream)))
            //        {
            //          jsonWriter.ArrayPool = GlobalCharacterArrayPool;
            //          //jsonWriter.CloseOutput = false;
            //          jsonWriter.Formatting = jsonSerializer.Formatting;

            //          jsonSerializer.Serialize(jsonWriter, value, type);
            //          jsonWriter.Flush();
            //        }
            //        var readBuffer = pipe.Reader.ReadAsync().GetResult().Buffer;
            //        var length = (int)readBuffer.Length;
            //        if (c_zeroSize == length) { return EmptyArray<byte>.Instance; }
            //        return readBuffer.ToArray();
            //      }
            //#endif
        }

        #endregion

        #region -- Serialize to Array-Segment --

        /// <summary>Serializes the specified object to a JSON byte array.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static ArraySegment<Byte> SerializeToArraySegment(object value, int initialBufferSize = c_initialBufferSize)
        {
            return SerializeToArraySegment(value, null, (JsonSerializerSettings)null, initialBufferSize);
        }

        /// <summary>Serializes the specified object to a JSON byte array using formatting.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static ArraySegment<Byte> SerializeToArraySegment(object value, Formatting formatting, int initialBufferSize = c_initialBufferSize)
        {
            return SerializeToArraySegment(value, formatting, (JsonSerializerSettings)null, initialBufferSize);
        }

        /// <summary>Serializes the specified object to a JSON byte array using a collection of <see cref="JsonConverter"/>.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="converters">A collection of converters used while serializing.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static ArraySegment<Byte> SerializeToArraySegment(object value, params JsonConverter[] converters)
        {
            if (converters != null && converters.Length > 0)
            {
                var settings = new JsonSerializerSettings { Converters = converters };
                var jsonSerializer = JsonSerializer.CreateDefault(settings);
                return SerializeToArraySegmentInternal(value, null, jsonSerializer);
            }
            else
            {
                return SerializeToArraySegment(value, null, (JsonSerializerSettings)null);
            }
        }

        /// <summary>Serializes the specified object to a JSON byte array using formatting and a collection of <see cref="JsonConverter"/>.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="converters">A collection of converters used while serializing.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static ArraySegment<Byte> SerializeToArraySegment(object value, Formatting formatting, params JsonConverter[] converters)
        {
            if (converters != null && converters.Length > 0)
            {
                var settings = new JsonSerializerSettings { Converters = converters };
                var jsonSerializer = JsonSerializer.CreateDefault(settings);
                jsonSerializer.Formatting = formatting;
                return SerializeToArraySegmentInternal(value, null, jsonSerializer);
            }
            else
            {
                return SerializeToArraySegment(value, null, formatting, (JsonSerializerSettings)null);
            }
        }

        /// <summary>Serializes the specified object to a JSON byte array using <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static ArraySegment<Byte> SerializeToArraySegment(object value, JsonSerializerSettings settings, int initialBufferSize = c_initialBufferSize)
        {
            return SerializeToArraySegment(value, null, settings, initialBufferSize);
        }

        /// <summary>Serializes the specified object to a JSON byte array using a type, formatting and <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="type">The type of the value being serialized.
        /// This parameter is used when <see cref="JsonSerializer.TypeNameHandling"/> is <see cref="TypeNameHandling.Auto"/> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static ArraySegment<Byte> SerializeToArraySegment(object value, Type type, JsonSerializerSettings settings, int initialBufferSize = c_initialBufferSize)
        {
            var jsonSerializer = JsonSerializer.Create(settings);

            return SerializeToArraySegmentInternal(value, type, jsonSerializer, initialBufferSize);
        }

        /// <summary>Serializes the specified object to a JSON byte array using formatting and <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static ArraySegment<Byte> SerializeToArraySegment(object value, Formatting formatting, JsonSerializerSettings settings, int initialBufferSize = c_initialBufferSize)
        {
            return SerializeToArraySegment(value, null, formatting, settings, initialBufferSize);
        }

        /// <summary>Serializes the specified object to a JSON byte array using a type, formatting and <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="type">The type of the value being serialized.
        /// This parameter is used when <see cref="JsonSerializer.TypeNameHandling"/> is <see cref="TypeNameHandling.Auto"/> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static ArraySegment<Byte> SerializeToArraySegment(object value, Type type, Formatting formatting, JsonSerializerSettings settings, int initialBufferSize = c_initialBufferSize)
        {
            var jsonSerializer = JsonSerializer.Create(settings);
            jsonSerializer.Formatting = formatting;

            return SerializeToArraySegmentInternal(value, type, jsonSerializer, initialBufferSize);
        }

        private static ArraySegment<Byte> SerializeToArraySegmentInternal(object value, Type type, JsonSerializer jsonSerializer, int initialBufferSize = c_initialBufferSize)
        {
            //#if NET40
            using (var pooledOutputStream = BufferManagerOutputStreamManager.Create())
            {
                var outputStream = pooledOutputStream.Object;
                outputStream.Reinitialize(initialBufferSize);
                using (JsonTextWriter jsonWriter = new JsonTextWriter(new StreamWriterX(outputStream)))
                {
                    jsonWriter.ArrayPool = GlobalCharacterArrayPool;
                    //jsonWriter.CloseOutput = false;
                    jsonWriter.Formatting = jsonSerializer.Formatting;

                    jsonSerializer.Serialize(jsonWriter, value, type);
                    jsonWriter.Flush();
                }
                return outputStream.ToArraySegment();
            }
            //#else
            //      using (var pooledPipe = PipelineManager.Create())
            //      {
            //        var pipe = pooledPipe.Object;
            //        var outputStream = new PipelineStream(pipe, initialBufferSize);
            //        using (JsonTextWriter jsonWriter = new JsonTextWriter(new StreamWriterX(outputStream)))
            //        {
            //          jsonWriter.ArrayPool = GlobalCharacterArrayPool;
            //          //jsonWriter.CloseOutput = false;
            //          jsonWriter.Formatting = jsonSerializer.Formatting;

            //          jsonSerializer.Serialize(jsonWriter, value, type);
            //          jsonWriter.Flush();
            //        }
            //        var readBuffer = pipe.Reader.ReadAsync().GetResult().Buffer;
            //        var length = (int)readBuffer.Length;
            //        if (c_zeroSize == length) { return default; }
            //        var buffer = BufferManager.Shared.Rent(length);
            //        readBuffer.CopyTo(buffer);
            //        return new ArraySegment<byte>(buffer, 0, length);
            //      }
            //#endif
        }

        #endregion

        #region -- DeserializeObject from Byte-Array --

        /// <summary>Deserializes the JSON to a .NET object.</summary>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromByteArray(byte[] bytes)
        {
            return DeserializeFromByteArray(bytes, null, (JsonSerializerSettings)null);
        }

        /// <summary>Deserializes the JSON to a .NET object using <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromByteArray(byte[] bytes, JsonSerializerSettings settings)
        {
            return DeserializeFromByteArray(bytes, null, settings);
        }

        /// <summary>Deserializes the JSON to the specified .NET type.</summary>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <param name="type">The <see cref="Type"/> of object being deserialized.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromByteArray(byte[] bytes, Type type)
        {
            return DeserializeFromByteArray(bytes, type, (JsonSerializerSettings)null);
        }

        /// <summary>Deserializes the JSON to the specified .NET type.</summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeFromByteArray<T>(byte[] bytes)
        {
            return DeserializeFromByteArray<T>(bytes, (JsonSerializerSettings)null);
        }

        /// <summary>Deserializes the JSON to the specified .NET type using a collection of <see cref="JsonConverter"/>.</summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeFromByteArray<T>(byte[] bytes, params JsonConverter[] converters)
        {
            return (T)DeserializeFromByteArray(bytes, typeof(T), converters);
        }

        /// <summary>Deserializes the JSON to the specified .NET type using <see cref="JsonSerializerSettings"/>.</summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeFromByteArray<T>(byte[] bytes, JsonSerializerSettings settings)
        {
            return (T)DeserializeFromByteArray(bytes, typeof(T), settings);
        }

        /// <summary>Deserializes the JSON to the specified .NET type using a collection of <see cref="JsonConverter"/>.</summary>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromByteArray(byte[] bytes, Type type, params JsonConverter[] converters)
        {
            if (converters != null && converters.Length > 0)
            {
                ValidationUtils.ArgumentNotNull(bytes, nameof(bytes));

                var settings = new JsonSerializerSettings { Converters = converters };
                var jsonSerializer = JsonSerializer.CreateDefault(settings);

                // by default DeserializeObject should check for additional content
                if (!jsonSerializer.IsCheckAdditionalContentSetX())
                {
                    jsonSerializer.CheckAdditionalContent = true;
                }

                using (var reader = new JsonTextReader(new StreamReader(new MemoryStream(bytes))))
                {
                    reader.ArrayPool = GlobalCharacterArrayPool;
                    reader.CloseInput = false;

                    return jsonSerializer.Deserialize(reader, type);
                }
            }
            else
            {
                return DeserializeFromByteArray(bytes, type, (JsonSerializerSettings)null);
            }
        }

        /// <summary>Deserializes the JSON to the specified .NET type using <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <param name="type">The type of the object to deserialize to.</param>
        /// <param name="settings">
        /// The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromByteArray(byte[] bytes, Type type, JsonSerializerSettings settings)
        {
            ValidationUtils.ArgumentNotNull(bytes, nameof(bytes));

            var jsonSerializer = AllocateSerializerInternal(settings);
            var isCheckAdditionalContentNoSet = !jsonSerializer.IsCheckAdditionalContentSetX();
            try
            {
                // by default DeserializeObject should check for additional content
                if (isCheckAdditionalContentNoSet)
                {
                    jsonSerializer.CheckAdditionalContent = true;
                }

                using (var reader = new JsonTextReader(new StreamReader(new MemoryStream(bytes))))
                {
                    reader.ArrayPool = GlobalCharacterArrayPool;
                    reader.CloseInput = false;

                    return jsonSerializer.Deserialize(reader, type);
                }
            }
            finally
            {
                if (isCheckAdditionalContentNoSet) { jsonSerializer.SetCheckAdditionalContent(); }
                FreeSerializerInternal(settings, jsonSerializer);
            }
        }


        /// <summary>Deserializes the JSON to a .NET object.</summary>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <param name="index">The index of the first byte to deserialize.</param>
        /// <param name="count">The number of bytes to deserialize.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromByteArray(byte[] bytes, int index, int count)
        {
            return DeserializeFromByteArray(bytes, index, count, null, (JsonSerializerSettings)null);
        }

        /// <summary>Deserializes the JSON to a .NET object using <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <param name="index">The index of the first byte to deserialize.</param>
        /// <param name="count">The number of bytes to deserialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromByteArray(byte[] bytes, int index, int count, JsonSerializerSettings settings)
        {
            return DeserializeFromByteArray(bytes, index, count, null, settings);
        }

        /// <summary>Deserializes the JSON to the specified .NET type.</summary>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <param name="index">The index of the first byte to deserialize.</param>
        /// <param name="count">The number of bytes to deserialize.</param>
        /// <param name="type">The <see cref="Type"/> of object being deserialized.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromByteArray(byte[] bytes, int index, int count, Type type)
        {
            return DeserializeFromByteArray(bytes, index, count, type, (JsonSerializerSettings)null);
        }

        /// <summary>Deserializes the JSON to the specified .NET type.</summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <param name="index">The index of the first byte to deserialize.</param>
        /// <param name="count">The number of bytes to deserialize.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeFromByteArray<T>(byte[] bytes, int index, int count)
        {
            return DeserializeFromByteArray<T>(bytes, index, count, (JsonSerializerSettings)null);
        }

        /// <summary>Deserializes the JSON to the specified .NET type using a collection of <see cref="JsonConverter"/>.</summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <param name="index">The index of the first byte to deserialize.</param>
        /// <param name="count">The number of bytes to deserialize.</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeFromByteArray<T>(byte[] bytes, int index, int count, params JsonConverter[] converters)
        {
            return (T)DeserializeFromByteArray(bytes, index, count, typeof(T), converters);
        }

        /// <summary>Deserializes the JSON to the specified .NET type using <see cref="JsonSerializerSettings"/>.</summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <param name="index">The index of the first byte to deserialize.</param>
        /// <param name="count">The number of bytes to deserialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeFromByteArray<T>(byte[] bytes, int index, int count, JsonSerializerSettings settings)
        {
            return (T)DeserializeFromByteArray(bytes, index, count, typeof(T), settings);
        }

        /// <summary>Deserializes the JSON to the specified .NET type using a collection of <see cref="JsonConverter"/>.</summary>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <param name="index">The index of the first byte to deserialize.</param>
        /// <param name="count">The number of bytes to deserialize.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromByteArray(byte[] bytes, int index, int count, Type type, params JsonConverter[] converters)
        {
            if (converters != null && converters.Length > 0)
            {
                ValidationUtils.ArgumentNotNull(bytes, nameof(bytes));

                var settings = new JsonSerializerSettings { Converters = converters };
                var jsonSerializer = JsonSerializer.CreateDefault(settings);

                // by default DeserializeObject should check for additional content
                if (!jsonSerializer.IsCheckAdditionalContentSetX())
                {
                    jsonSerializer.CheckAdditionalContent = true;
                }

                using (var reader = new JsonTextReader(new StreamReaderX(new MemoryStream(bytes, index, count))))
                {
                    reader.ArrayPool = GlobalCharacterArrayPool;
                    reader.CloseInput = false;

                    return jsonSerializer.Deserialize(reader, type);
                }
            }
            else
            {
                return DeserializeFromByteArray(bytes, index, count, type, (JsonSerializerSettings)null);
            }
        }

        /// <summary>Deserializes the JSON to the specified .NET type using <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="bytes">The byte array containing the JSON data to read.</param>
        /// <param name="index">The index of the first byte to deserialize.</param>
        /// <param name="count">The number of bytes to deserialize.</param>
        /// <param name="type">The type of the object to deserialize to.</param>
        /// <param name="settings">
        /// The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromByteArray(byte[] bytes, int index, int count, Type type, JsonSerializerSettings settings)
        {
            ValidationUtils.ArgumentNotNull(bytes, nameof(bytes));

            var jsonSerializer = AllocateSerializerInternal(settings);
            var isCheckAdditionalContentNoSet = !jsonSerializer.IsCheckAdditionalContentSetX();
            try
            {
                // by default DeserializeObject should check for additional content
                if (isCheckAdditionalContentNoSet)
                {
                    jsonSerializer.CheckAdditionalContent = true;
                }

                using (var reader = new JsonTextReader(new StreamReaderX(new MemoryStream(bytes, index, count))))
                {
                    reader.ArrayPool = GlobalCharacterArrayPool;
                    reader.CloseInput = false;

                    return jsonSerializer.Deserialize(reader, type);
                }
            }
            finally
            {
                if (isCheckAdditionalContentNoSet) { jsonSerializer.SetCheckAdditionalContent(); }
                FreeSerializerInternal(settings, jsonSerializer);
            }
        }

        #endregion

        #region -- Serialize to Stream --

        /// <summary>Serializes the specified object to a <see cref="Stream"/>.</summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        public static void SerializeToStream(Stream stream, object value)
        {
            SerializeToStream(stream, value, null, (JsonSerializerSettings)null);
        }

        /// <summary>Serializes the specified object to a <see cref="Stream"/> using formatting.</summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        public static void SerializeToStream(Stream stream, object value, Formatting formatting)
        {
            SerializeToStream(stream, value, formatting, (JsonSerializerSettings)null);
        }

        /// <summary>Serializes the specified object to a <see cref="Stream"/> using a collection of <see cref="JsonConverter"/>.</summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="converters">A collection of converters used while serializing.</param>
        public static void SerializeToStream(Stream stream, object value, params JsonConverter[] converters)
        {
            if (converters != null && converters.Length > 0)
            {
                var settings = new JsonSerializerSettings { Converters = converters };
                var jsonSerializer = JsonSerializer.CreateDefault(settings);
                SerializeToStreamInternal(stream, value, null, jsonSerializer);
            }
            else
            {
                SerializeToStream(stream, value, null, (JsonSerializerSettings)null);
            }
        }

        /// <summary>Serializes the specified object to a <see cref="Stream"/> using formatting and a collection of <see cref="JsonConverter"/>.</summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="converters">A collection of converters used while serializing.</param>
        public static void SerializeToStream(Stream stream, object value, Formatting formatting, params JsonConverter[] converters)
        {
            if (converters != null && converters.Length > 0)
            {
                var settings = new JsonSerializerSettings { Converters = converters };
                var jsonSerializer = JsonSerializer.CreateDefault(settings);
                jsonSerializer.Formatting = formatting;
                SerializeToStreamInternal(stream, value, null, jsonSerializer);
            }
            else
            {
                SerializeToStream(stream, value, null, formatting, (JsonSerializerSettings)null);
            }
        }

        /// <summary>Serializes the specified object to a <see cref="Stream"/> using <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        public static void SerializeToStream(Stream stream, object value, JsonSerializerSettings settings)
        {
            SerializeToStream(stream, value, null, settings);
        }

        /// <summary>Serializes the specified object to a <see cref="Stream"/> using a type, formatting and <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="type">The type of the value being serialized.
        /// This parameter is used when <see cref="JsonSerializer.TypeNameHandling"/> is <see cref="TypeNameHandling.Auto"/> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.</param>
        public static void SerializeToStream(Stream stream, object value, Type type, JsonSerializerSettings settings)
        {
            var jsonSerializer = JsonSerializer.Create(settings);

            SerializeToStreamInternal(stream, value, type, jsonSerializer);
        }

        /// <summary>Serializes the specified object to a <see cref="Stream"/> using formatting and <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        public static void SerializeToStream(Stream stream, object value, Formatting formatting, JsonSerializerSettings settings)
        {
            SerializeToStream(stream, value, null, formatting, settings);
        }

        /// <summary>Serializes the specified object to a <see cref="Stream"/> using a type, formatting and <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="type">The type of the value being serialized.
        /// This parameter is used when <see cref="JsonSerializer.TypeNameHandling"/> is <see cref="TypeNameHandling.Auto"/> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.</param>
        public static void SerializeToStream(Stream stream, object value, Type type, Formatting formatting, JsonSerializerSettings settings)
        {
            var jsonSerializer = JsonSerializer.Create(settings);
            jsonSerializer.Formatting = formatting;

            SerializeToStreamInternal(stream, value, type, jsonSerializer);
        }

        private static void SerializeToStreamInternal(Stream stream, object value, Type type, JsonSerializer jsonSerializer)
        {
            using (JsonTextWriter jsonWriter = new JsonTextWriter(new StreamWriterX(stream)))
            {
                jsonWriter.ArrayPool = GlobalCharacterArrayPool;
                //jsonWriter.CloseOutput = false;
                jsonWriter.Formatting = jsonSerializer.Formatting;

                jsonSerializer.Serialize(jsonWriter, value, type);
                jsonWriter.Flush();
            }
        }

        #endregion

        #region -- Deserialize from Stream --

        /// <summary>Deserializes the JSON to a .NET object.</summary>
        /// <param name="stream">The <see cref="Stream"/> containing the JSON data to read.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromStream(Stream stream)
        {
            return DeserializeFromStream(stream, null, (JsonSerializerSettings)null);
        }

        /// <summary>Deserializes the JSON to a .NET object using <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="stream">The <see cref="Stream"/> containing the JSON data to read.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromStream(Stream stream, JsonSerializerSettings settings)
        {
            return DeserializeFromStream(stream, null, settings);
        }

        /// <summary>Deserializes the JSON to the specified .NET type.</summary>
        /// <param name="stream">The <see cref="Stream"/> containing the JSON data to read.</param>
        /// <param name="type">The <see cref="Type"/> of object being deserialized.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromStream(Stream stream, Type type)
        {
            return DeserializeFromStream(stream, type, (JsonSerializerSettings)null);
        }

        /// <summary>Deserializes the JSON to the specified .NET type.</summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> containing the JSON data to read.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeFromStream<T>(Stream stream)
        {
            return DeserializeFromStream<T>(stream, (JsonSerializerSettings)null);
        }

        /// <summary>Deserializes the JSON to the specified .NET type using a collection of <see cref="JsonConverter"/>.</summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> containing the JSON data to read.</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeFromStream<T>(Stream stream, params JsonConverter[] converters)
        {
            return (T)DeserializeFromStream(stream, typeof(T), converters);
        }

        /// <summary>Deserializes the JSON to the specified .NET type using <see cref="JsonSerializerSettings"/>.</summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> containing the JSON data to read.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeFromStream<T>(Stream stream, JsonSerializerSettings settings)
        {
            return (T)DeserializeFromStream(stream, typeof(T), settings);
        }

        /// <summary>Deserializes the JSON to the specified .NET type using a collection of <see cref="JsonConverter"/>.</summary>
        /// <param name="stream">The <see cref="Stream"/> containing the JSON data to read.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromStream(Stream stream, Type type, params JsonConverter[] converters)
        {
            if (converters != null && converters.Length > 0)
            {
                ValidationUtils.ArgumentNotNull(stream, nameof(stream));

                var settings = new JsonSerializerSettings { Converters = converters };
                var jsonSerializer = JsonSerializer.CreateDefault(settings);

                // by default DeserializeObject should check for additional content
                if (!jsonSerializer.IsCheckAdditionalContentSetX())
                {
                    jsonSerializer.CheckAdditionalContent = true;
                }

                using (var reader = new JsonTextReader(new StreamReaderX(stream)))
                {
                    reader.ArrayPool = GlobalCharacterArrayPool;

                    return jsonSerializer.Deserialize(reader, type);
                }
            }
            else
            {
                return DeserializeFromStream(stream, type, (JsonSerializerSettings)null);
            }
        }

        /// <summary>Deserializes the JSON to the specified .NET type using <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="stream">The <see cref="Stream"/> containing the JSON data to read.</param>
        /// <param name="type">The type of the object to deserialize to.</param>
        /// <param name="settings">
        /// The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromStream(Stream stream, Type type, JsonSerializerSettings settings)
        {
            ValidationUtils.ArgumentNotNull(stream, nameof(stream));

            var jsonSerializer = AllocateSerializerInternal(settings);
            var isCheckAdditionalContentNoSet = !jsonSerializer.IsCheckAdditionalContentSetX();
            try
            {
                // by default DeserializeObject should check for additional content
                if (isCheckAdditionalContentNoSet)
                {
                    jsonSerializer.CheckAdditionalContent = true;
                }

                using (var reader = new JsonTextReader(new StreamReaderX(stream)))
                {
                    reader.ArrayPool = GlobalCharacterArrayPool;

                    return jsonSerializer.Deserialize(reader, type);
                }
            }
            finally
            {
                if (isCheckAdditionalContentNoSet) { jsonSerializer.SetCheckAdditionalContent(); }
                FreeSerializerInternal(settings, jsonSerializer);
            }
        }

        #endregion

        #region -- Serialize to TextWriter --

        /// <summary>Serializes the specified object to a <see cref="TextWriter"/>.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        public static void SerializeToWriter(TextWriter textWriter, object value)
        {
            SerializeToWriter(textWriter, value, null, (JsonSerializerSettings)null);
        }

        /// <summary>Serializes the specified object to a <see cref="TextWriter"/> using formatting.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        public static void SerializeToWriter(TextWriter textWriter, object value, Formatting formatting)
        {
            SerializeToWriter(textWriter, value, formatting, (JsonSerializerSettings)null);
        }

        /// <summary>Serializes the specified object to a <see cref="TextWriter"/> using a collection of <see cref="JsonConverter"/>.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="converters">A collection of converters used while serializing.</param>
        public static void SerializeToWriter(TextWriter textWriter, object value, params JsonConverter[] converters)
        {
            if (converters != null && converters.Length > 0)
            {
                var settings = new JsonSerializerSettings { Converters = converters };
                var jsonSerializer = JsonSerializer.CreateDefault(settings);
                SerializeToWriterInternal(textWriter, value, null, jsonSerializer);
            }
            else
            {
                SerializeToWriter(textWriter, value, null, (JsonSerializerSettings)null);
            }
        }

        /// <summary>Serializes the specified object to a <see cref="TextWriter"/> using formatting and a collection of <see cref="JsonConverter"/>.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="converters">A collection of converters used while serializing.</param>
        public static void SerializeToWriter(TextWriter textWriter, object value, Formatting formatting, params JsonConverter[] converters)
        {
            if (converters != null && converters.Length > 0)
            {
                var settings = new JsonSerializerSettings { Converters = converters };
                var jsonSerializer = JsonSerializer.CreateDefault(settings);
                jsonSerializer.Formatting = formatting;
                SerializeToWriterInternal(textWriter, value, null, jsonSerializer);
            }
            else
            {
                SerializeToWriter(textWriter, value, null, formatting, (JsonSerializerSettings)null);
            }
        }

        /// <summary>Serializes the specified object to a <see cref="TextWriter"/> using <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        public static void SerializeToWriter(TextWriter textWriter, object value, JsonSerializerSettings settings)
        {
            SerializeToWriter(textWriter, value, null, settings);
        }

        /// <summary>Serializes the specified object to a JSON byte array using a type, formatting and <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="type">The type of the value being serialized.
        /// This parameter is used when <see cref="JsonSerializer.TypeNameHandling"/> is <see cref="TypeNameHandling.Auto"/> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.</param>
        public static void SerializeToWriter(TextWriter textWriter, object value, Type type, JsonSerializerSettings settings)
        {
            var jsonSerializer = JsonSerializer.Create(settings);

            SerializeToWriterInternal(textWriter, value, type, jsonSerializer);
        }

        /// <summary>Serializes the specified object to a <see cref="TextWriter"/> using formatting and <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        public static void SerializeToWriter(TextWriter textWriter, object value, Formatting formatting, JsonSerializerSettings settings)
        {
            SerializeToWriter(textWriter, value, null, formatting, settings);
        }

        /// <summary>Serializes the specified object to a <see cref="TextWriter"/> using a type, formatting and <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write to.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="type">The type of the value being serialized.
        /// This parameter is used when <see cref="JsonSerializer.TypeNameHandling"/> is <see cref="TypeNameHandling.Auto"/> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.</param>
        public static void SerializeToWriter(TextWriter textWriter, object value, Type type, Formatting formatting, JsonSerializerSettings settings)
        {
            var jsonSerializer = JsonSerializer.Create(settings);
            jsonSerializer.Formatting = formatting;

            SerializeToWriterInternal(textWriter, value, type, jsonSerializer);
        }

        private static void SerializeToWriterInternal(TextWriter textWriter, object value, Type type, JsonSerializer jsonSerializer)
        {
            if (null == textWriter) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.textWriter); }

            using (JsonTextWriter jsonWriter = new JsonTextWriter(textWriter))
            {
                jsonWriter.ArrayPool = GlobalCharacterArrayPool;
                jsonWriter.CloseOutput = false;
                jsonWriter.Formatting = jsonSerializer.Formatting;

                jsonSerializer.Serialize(jsonWriter, value, type);
                jsonWriter.Flush();
            }
        }

        #endregion

        #region -- Deserialize from TextReader --

        /// <summary>Deserializes the JSON to a .NET object.</summary>
        /// <param name="reader">The <see cref="TextReader"/> containing the JSON data to read.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromReader(TextReader reader)
        {
            return DeserializeFromReader(reader, null, (JsonSerializerSettings)null);
        }

        /// <summary>Deserializes the JSON to a .NET object using <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="reader">The <see cref="TextReader"/> containing the JSON data to read.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromReader(TextReader reader, JsonSerializerSettings settings)
        {
            return DeserializeFromReader(reader, null, settings);
        }

        /// <summary>Deserializes the JSON to the specified .NET type.</summary>
        /// <param name="reader">The <see cref="TextReader"/> containing the JSON data to read.</param>
        /// <param name="type">The <see cref="Type"/> of object being deserialized.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromReader(TextReader reader, Type type)
        {
            return DeserializeFromReader(reader, type, (JsonSerializerSettings)null);
        }

        /// <summary>Deserializes the JSON to the specified .NET type.</summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="reader">The <see cref="TextReader"/> containing the JSON data to read.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeFromReader<T>(TextReader reader)
        {
            return DeserializeFromReader<T>(reader, (JsonSerializerSettings)null);
        }

        /// <summary>Deserializes the JSON to the specified .NET type using a collection of <see cref="JsonConverter"/>.</summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="reader">The <see cref="TextReader"/> containing the JSON data to read.</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeFromReader<T>(TextReader reader, params JsonConverter[] converters)
        {
            return (T)DeserializeFromReader(reader, typeof(T), converters);
        }

        /// <summary>Deserializes the JSON to the specified .NET type using <see cref="JsonSerializerSettings"/>.</summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="reader">The <see cref="TextReader"/> containing the JSON data to read.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeFromReader<T>(TextReader reader, JsonSerializerSettings settings)
        {
            return (T)DeserializeFromReader(reader, typeof(T), settings);
        }

        /// <summary>Deserializes the JSON to the specified .NET type using a collection of <see cref="JsonConverter"/>.</summary>
        /// <param name="reader">The <see cref="TextReader"/> containing the JSON data to read.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromReader(TextReader reader, Type type, params JsonConverter[] converters)
        {
            if (converters != null && converters.Length > 0)
            {
                ValidationUtils.ArgumentNotNull(reader, nameof(reader));

                var settings = new JsonSerializerSettings { Converters = converters };
                var jsonSerializer = JsonSerializer.CreateDefault(settings);

                // by default DeserializeObject should check for additional content
                if (!jsonSerializer.IsCheckAdditionalContentSetX())
                {
                    jsonSerializer.CheckAdditionalContent = true;
                }

                using (var jsonReader = new JsonTextReader(reader))
                {
                    jsonReader.ArrayPool = GlobalCharacterArrayPool;
                    jsonReader.CloseInput = false;

                    return jsonSerializer.Deserialize(jsonReader, type);
                }
            }
            else
            {
                return DeserializeFromReader(reader, type, (JsonSerializerSettings)null);
            }
        }

        /// <summary>Deserializes the JSON to the specified .NET type using <see cref="JsonSerializerSettings"/>.</summary>
        /// <param name="reader">The <see cref="TextReader"/> containing the JSON data to read.</param>
        /// <param name="type">The type of the object to deserialize to.</param>
        /// <param name="settings">
        /// The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeFromReader(TextReader reader, Type type, JsonSerializerSettings settings)
        {
            ValidationUtils.ArgumentNotNull(reader, nameof(reader));

            var jsonSerializer = AllocateSerializerInternal(settings);
            var isCheckAdditionalContentNoSet = !jsonSerializer.IsCheckAdditionalContentSetX();
            try
            {
                // by default DeserializeObject should check for additional content
                if (isCheckAdditionalContentNoSet)
                {
                    jsonSerializer.CheckAdditionalContent = true;
                }

                using (var jsonReader = new JsonTextReader(reader))
                {
                    jsonReader.ArrayPool = GlobalCharacterArrayPool;
                    jsonReader.CloseInput = false;

                    return jsonSerializer.Deserialize(jsonReader, type);
                }
            }
            finally
            {
                if (isCheckAdditionalContentNoSet) { jsonSerializer.SetCheckAdditionalContent(); }
                FreeSerializerInternal(settings, jsonSerializer);
            }
        }

        #endregion
    }
}
