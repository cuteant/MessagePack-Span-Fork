using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using CuteAnt.IO;
using CuteAnt.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
#if !NET40
using System.Threading.Tasks;
using CuteAnt.AsyncEx;
#endif

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>Abstract <see cref="MessageFormatter"/> class to support Bson and Json.</summary>
  public class JsonMessageFormatter : MessageFormatter, IJsonMessageFormatter
  {
    #region @@ Fields @@

    /// <summary>The default singlegton instance</summary>
    public static readonly JsonMessageFormatter DefaultInstance = new JsonMessageFormatter();

    protected static readonly ILogger s_logger = TraceLogger.GetLogger("CuteAnt.Extensions.Serialization.JsonMessageFormatter");

    // Though MaxDepth is not supported in portable library, we still override JsonReader's MaxDepth
    private int _maxDepth = FormattingUtilities.DefaultMaxDepth;

    private JsonSerializerSettings _defaultSerializerSettings;
    private JsonSerializerSettings _defaultDeserializerSettings;

    private readonly IArrayPool<char> _charPool;

    #endregion

    #region @@ Constructors @@

    /// <summary>Initializes a new instance of the <see cref="JsonMessageFormatter"/> class.</summary>
    public JsonMessageFormatter()
    {
      // Initialize serializer settings
      _defaultSerializerSettings = CreateDefaultSerializerSettings();
      _defaultDeserializerSettings = _defaultSerializerSettings;

      _charPool = JsonConvertX.GlobalCharacterArrayPool;
    }

    /// <summary>Initializes a new instance of the <see cref="JsonMessageFormatter"/> class.</summary>
    /// <param name="formatter">The <see cref="JsonMessageFormatter"/> instance to copy settings from.</param>
#if !NETFX_CORE
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors",
        Justification = "MaxDepth is sealed in existing subclasses and its documentation carries warnings.")]
#endif
    public JsonMessageFormatter(JsonMessageFormatter formatter)
      : base(formatter)
    {
      Contract.Assert(formatter != null);

      DefaultSerializerSettings = formatter.DefaultSerializerSettings;
      DefaultDeserializerSettings = formatter.DefaultDeserializerSettings;

      MaxDepth = formatter._maxDepth;
      JsonFormatting = formatter.JsonFormatting;
    }

    #endregion

    #region @@ Properties @@

    /// <summary>Gets or sets the default <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</summary>
    public JsonSerializerSettings DefaultSerializerSettings
    {
      get => _defaultSerializerSettings;
      set => _defaultSerializerSettings = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Gets or sets the default <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</summary>
    public JsonSerializerSettings DefaultDeserializerSettings
    {
      get => _defaultDeserializerSettings;
      set => _defaultDeserializerSettings = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Gets or sets a value indicating whether to indent elements when writing data.</summary>
    [Obsolete("Indent is obsolete. Use Formatting instead.")]
    public bool Indent
    {
      get => Formatting.Indented == JsonFormatting.GetValueOrDefault();
      set => JsonFormatting = value ? Formatting.Indented : Formatting.None;
    }

    /// <summary>Indicates how JSON text output is formatted.</summary>
    public Formatting? JsonFormatting { get; set; }

#if !NETFX_CORE // MaxDepth is not supported in portable library
    /// <summary>Gets or sets the maximum depth allowed by this formatter.</summary>
    /// <remarks>
    /// Any override must call the base getter and setter. The setter may be called before a derived class
    /// constructor runs, so any override should be very careful about using derived class state.
    /// </remarks>
    public virtual Int32 MaxDepth
    {
      get { return _maxDepth; }
      set
      {
        if (value < FormattingUtilities.DefaultMinDepth)
        {
          throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof(value), value, FormattingUtilities.DefaultMinDepth);
        }

        _maxDepth = value;
      }
    }

#endif

    #endregion

    #region -- CreateDefaultSerializerSettings --

    /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the default serializer settings used by the <see cref="JsonMessageFormatter"/>.</summary>
    public JsonSerializerSettings CreateDefaultSerializerSettings()
    {
      return JsonConvertX.CreateDefaultSerializerSettings();
    }

    #endregion

    #region -- CreateLimitPropsSerializerSettings --

    /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the limit properties settings used by the <see cref="JsonMessageFormatter"/>.</summary>
    /// <param name="limitProps"></param>
    /// <returns></returns>
    [Obsolete("=> JsonConvertX.CreateLimitPropsSerializerSettings")]
    public JsonSerializerSettings CreateLimitPropsSerializerSettings(ICollection<String> limitProps)
    {
      return JsonConvertX.CreateLimitPropsSerializerSettings(limitProps, Formatting.None, TypeNameHandling.Auto, TypeNameAssemblyFormatHandling.Simple);
    }

    #endregion

    #region -- CreateCamelCasePropertyNamesSerializerSettings --

    /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the default settings used by the <see cref="JsonMessageFormatter"/>.</summary>
    [Obsolete("=> JsonConvertX.CreateCamelCaseSerializerSettings")]
    public JsonSerializerSettings CreateCamelCasePropertyNamesSerializerSettings()
    {
      return JsonConvertX.CreateCamelCaseSerializerSettings();
    }

    #endregion

    #region -- CreateCamelCasePropertyNamesLimitPropsSerializerSettings --

    /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the limit properties settings used by the <see cref="JsonMessageFormatter"/>.</summary>
    /// <param name="limitProps"></param>
    /// <returns></returns>
    [Obsolete("=> JsonConvertX.CreateLimitPropsSerializerSettings")]
    public JsonSerializerSettings CreateCamelCasePropertyNamesLimitPropsSerializerSettings(ICollection<String> limitProps)
    {
      return JsonConvertX.CreateLimitPropsSerializerSettings(limitProps, Formatting.None, TypeNameHandling.Auto, TypeNameAssemblyFormatHandling.Simple, true);
    }

    #endregion

    #region -- CreatePropertyMappingSerializerSettings --

    /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the property mappings settings used by the <see cref="JsonMessageFormatter"/>.</summary>
    /// <param name="propertyMappings"></param>
    /// <param name="limitProps"></param>
    /// <returns></returns>
    [Obsolete("=> JsonConvertX.CreatePropertyMappingSerializerSettings")]
    public JsonSerializerSettings CreatePropertyMappingSerializerSettings(IDictionary<String, String> propertyMappings, Boolean limitProps = false)
    {
      return JsonConvertX.CreatePropertyMappingSerializerSettings(propertyMappings, limitProps, Formatting.None,
          TypeNameHandling.All, TypeNameAssemblyFormatHandling.Simple);
    }

    #endregion

    #region -- IsSupportedType --

    /// <inheritdoc />
    public override bool IsSupportedType(Type type) => true;

    ///// <summary>Determines whether this <see cref="JsonMessageFormatter"/> can read objects
    ///// of the specified <paramref name="type"/>.</summary>
    ///// <param name="type">The <see cref="Type"/> of object that will be read.</param>
    ///// <returns><c>true</c> if objects of this <paramref name="type"/> can be read, otherwise <c>false</c>.</returns>
    //public override Boolean CanReadType(Type type)
    //{
    //  if (type == null) { throw new ArgumentNullException(nameof(type)); }

    //  return true;
    //}

    ///// <summary>Determines whether this <see cref="JsonMessageFormatter"/> can write objects
    ///// of the specified <paramref name="type"/>.</summary>
    ///// <param name="type">The <see cref="Type"/> of object that will be written.</param>
    ///// <returns><c>true</c> if objects of this <paramref name="type"/> can be written, otherwise <c>false</c>.</returns>
    //public override Boolean CanWriteType(Type type)
    //{
    //  if (type == null) { throw new ArgumentNullException(nameof(type)); }

    //  return true;
    //}

    #endregion

    #region -- DeepCopy --

    public sealed override object DeepCopyObject(object source)
    {
      if (source == null) { return null; }

      var type = source.GetType();
      var settings = JsonConvertX.DefaultSettings;
      var json = JsonConvertX.SerializeObject(source, type, settings);
      return JsonConvertX.DeserializeObject(json, type, settings);
    }

    #endregion

    #region -- ReadFromStreamAsync --

#if !NET40
    /// <inheritdoc />
    public sealed override async Task<T> ReadFromStreamAsync<T>(Stream readStream, Encoding effectiveEncoding)
    {
      await TaskConstants.Completed;
      return (T)ReadFromStream(typeof(T), readStream, effectiveEncoding, null);
    }

    /// <inheritdoc />
    public sealed override async Task<Object> ReadFromStreamAsync(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      await TaskConstants.Completed;
      return ReadFromStream(type, readStream, effectiveEncoding, null);
    }
#endif

    #endregion

    #region -- ReadFromStream --

    public sealed override T ReadFromStream<T>(Stream readStream, Encoding effectiveEncoding)
    {
      return (T)ReadFromStream(typeof(T), readStream, effectiveEncoding, null);
    }

    /// <inheritdoc />
    public sealed override Object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      return ReadFromStream(type, readStream, effectiveEncoding, null);
    }

    /// <summary>Called during deserialization to read an object of the specified <paramref name="type"/>
    /// from the specified <paramref name="readStream"/>.</summary>
    /// <param name="type">The <see cref="Type"/> of object to read.</param>
    /// <param name="readStream">The <see cref="Stream"/> from which to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <returns>The <see cref="object"/> instance that has been read.</returns>
    public virtual Object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding, JsonSerializerSettings serializerSettings)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      if (null == effectiveEncoding) { effectiveEncoding = Encoding.UTF8; }

      using (JsonReader jsonReader = new JsonTextReader(new StreamReaderX(readStream, effectiveEncoding)) { ArrayPool = _charPool })
      {
        //jsonReader.CloseInput = false;
        jsonReader.MaxDepth = _maxDepth;

        if (null == serializerSettings) { serializerSettings = _defaultDeserializerSettings; }

        //var jsonSerializer = CreateJsonSerializerInternal(serializerSettings);
        var jsonSerializer = JsonConvertX.AllocateSerializer(serializerSettings);
        var isCheckAdditionalContentNoSet = !jsonSerializer.IsCheckAdditionalContentSetX();
        // by default DeserializeObject should check for additional content
        if (isCheckAdditionalContentNoSet)
        {
          jsonSerializer.CheckAdditionalContent = true;
        }

        // Error must always be marked as handled
        // Failure to do so can cause the exception to be rethrown at every recursive level and overflow the stack for x64 CLR processes
        EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> errorHandler = JsonErrorHandler;
        jsonSerializer.Error += errorHandler;

        try
        {
          return jsonSerializer.Deserialize(jsonReader, type);
        }
        finally
        {
          // Clean up the error handler in case CreateJsonSerializer() reuses a serializer
          jsonSerializer.Error -= errorHandler;
          if (isCheckAdditionalContentNoSet) { jsonSerializer.SetCheckAdditionalContent(); }
          //ReleaseJsonSerializer(serializerSettings, jsonSerializer);
          JsonConvertX.FreeSerializer(serializerSettings, jsonSerializer);
        }
      }
    }

    private void JsonErrorHandler(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
    {
      var errorContext = e.ErrorContext;
      s_logger.LogError(errorContext.Error.ToString());
      errorContext.Handled = true;
    }

    #endregion

    #region -- WriteToStreamAsync --

#if !NET40
    /// <inheritdoc />
    public sealed override Task WriteToStreamAsync<T>(T value, Stream writeStream, Encoding effectiveEncoding)
    {
      WriteToStream(typeof(T), value, writeStream, effectiveEncoding, null);
      return TaskConstants.Completed;
    }

    /// <inheritdoc />
    public sealed override Task WriteToStreamAsync(Object value, Stream writeStream, Encoding effectiveEncoding)
    {
      WriteToStream(null, value, writeStream, effectiveEncoding, null);
      return TaskConstants.Completed;
    }

    /// <inheritdoc />
    public sealed override Task WriteToStreamAsync(Type type, Object value, Stream writeStream, Encoding effectiveEncoding)
    {
      WriteToStream(type, value, writeStream, effectiveEncoding, null);
      return TaskConstants.Completed;
    }
#endif

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public sealed override void WriteToStream<T>(T value, Stream writeStream, Encoding effectiveEncoding)
    {
      WriteToStream(typeof(T), value, writeStream, effectiveEncoding, null);
    }

    /// <inheritdoc />
    public sealed override void WriteToStream(object value, Stream writeStream, Encoding effectiveEncoding)
    {
      WriteToStream(null, value, writeStream, effectiveEncoding, null);
    }

    /// <inheritdoc />
    public sealed override void WriteToStream(Type type, Object value, Stream writeStream, Encoding effectiveEncoding)
    {
      WriteToStream(type, value, writeStream, effectiveEncoding, null);
    }

    /// <summary>Called during serialization to write an object of the specified <paramref name="type"/>
    /// to the specified <paramref name="writeStream"/>.</summary>
    /// <param name="type">The <see cref="Type"/> of object to write.</param>
    /// <param name="value">The object to write.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    public virtual void WriteToStream(Type type, Object value, Stream writeStream, Encoding effectiveEncoding, JsonSerializerSettings serializerSettings)
    {
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      using (JsonWriter jsonWriter = CreateJsonWriter(writeStream, effectiveEncoding, _charPool, JsonFormatting))
      {
        //jsonWriter.CloseOutput = false;

        if (null == serializerSettings) { serializerSettings = _defaultSerializerSettings; }
        //var jsonSerializer = CreateJsonSerializerInternal(serializerSettings);
        var jsonSerializer = JsonConvertX.AllocateSerializer(serializerSettings);
        if (null == JsonFormatting)
        {
          jsonSerializer.Serialize(jsonWriter, value, type);
          jsonWriter.Flush();
        }
        else
        {
          var previousFormatting = jsonSerializer.GetFormatting();
          jsonSerializer.Formatting = JsonFormatting.Value;
          jsonSerializer.Serialize(jsonWriter, value, type);
          jsonWriter.Flush();
          jsonSerializer.SetFormatting(previousFormatting);
        }
        //ReleaseJsonSerializer(serializerSettings, jsonSerializer);
        JsonConvertX.FreeSerializer(serializerSettings, jsonSerializer);
      }
    }

    #endregion

    #region **& CreateJsonWriter &**

    [MethodImpl(InlineMethod.Value)]
    private static JsonWriter CreateJsonWriter(Stream writeStream, Encoding effectiveEncoding, IArrayPool<char> charPool, Formatting? jsonFormatting)
    {
      if (null == effectiveEncoding) { effectiveEncoding = StringHelper.SecureUTF8NoBOM; }

      var jsonWriter = new JsonTextWriter(new StreamWriterX(writeStream, effectiveEncoding)) { ArrayPool = charPool };
      if (Formatting.Indented == jsonFormatting.GetValueOrDefault())
      {
        jsonWriter.Formatting = Formatting.Indented;
      }

      return jsonWriter;
    }

    #endregion
  }
}
