using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CuteAnt.AsyncEx;
using CuteAnt.IO;
using CuteAnt.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>Abstract <see cref="MessageFormatter"/> class to support Bson and Json.</summary>
  public abstract class BaseJsonMessageFormatter : MessageFormatter, IJsonMessageFormatter
  {
    #region @@ Fields @@

    protected static readonly ILogger s_logger = TraceLogger.GetLogger("CuteAnt.Extensions.Serialization.JsonMessageFormatter");

    // Though MaxDepth is not supported in portable library, we still override JsonReader's MaxDepth
    private int _maxDepth = FormattingUtilities.DefaultMaxDepth;

    private JsonSerializerSettings _jsonSerializerSettings;
    private JsonSerializerSettings _jsonDeserializerSettings;

    private readonly IArrayPool<char> _charPool;

    #endregion

    #region @@ Constructors @@

    /// <summary>Initializes a new instance of the <see cref="BaseJsonMessageFormatter"/> class.</summary>
    protected BaseJsonMessageFormatter()
    {
      // Initialize serializer settings
      _jsonSerializerSettings = CreateDefaultSerializerSettings();
      _jsonDeserializerSettings = CreateDefaultDeserializerSettings();

      _charPool = JsonConvertX.GlobalCharacterArrayPool;
    }

    /// <summary>Initializes a new instance of the <see cref="BaseJsonMessageFormatter"/> class.</summary>
    /// <param name="formatter">The <see cref="BaseJsonMessageFormatter"/> instance to copy settings from.</param>
#if !NETFX_CORE
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors",
        Justification = "MaxDepth is sealed in existing subclasses and its documentation carries warnings.")]
#endif
    protected BaseJsonMessageFormatter(BaseJsonMessageFormatter formatter)
      : base(formatter)
    {
      Contract.Assert(formatter != null);
      DefaultSerializerSettings = formatter.DefaultSerializerSettings;
      DefaultDeserializerSettings = formatter.DefaultDeserializerSettings;

#if !NETFX_CORE // MaxDepth is not supported in portable library and so _maxDepth never changes there
      MaxDepth = formatter._maxDepth;
#endif
    }

    #endregion

    #region @@ Properties @@

    /// <summary>Gets or sets the default <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</summary>
    public JsonSerializerSettings DefaultSerializerSettings
    {
      get => _jsonSerializerSettings;
      set => _jsonSerializerSettings = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Gets or sets the default <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</summary>
    public JsonSerializerSettings DefaultDeserializerSettings
    {
      get => _jsonDeserializerSettings;
      set => _jsonDeserializerSettings = value ?? throw new ArgumentNullException(nameof(value));
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

    private bool _IsStrictMode = false;
    /// <summary>Can only be used with JsonMessageFormatter, default value: true.</summary>
    public bool IsStrictMode { get => _IsStrictMode; set => _IsStrictMode = value; }

    #endregion

    #region -- CreateDefaultSerializerSettings --

    /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the default serializer settings used by the <see cref="BaseJsonMessageFormatter"/>.</summary>
    public JsonSerializerSettings CreateDefaultSerializerSettings()
    {
      return JsonConvertX.CreateDefaultSerializerSettings();
    }

    /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the default deserializer settings used by the <see cref="BaseJsonMessageFormatter"/>.</summary>
    public JsonSerializerSettings CreateDefaultDeserializerSettings()
    {
      var settings = JsonConvertX.CreateDefaultSerializerSettings();
      settings.DateParseHandling = DateParseHandling.None;
      return settings;
    }

    #endregion

    #region -- CreateLimitPropsSerializerSettings --

    /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the limit properties settings used by the <see cref="BaseJsonMessageFormatter"/>.</summary>
    /// <param name="limitProps"></param>
    /// <returns></returns>
    [Obsolete("=> JsonConvertX.CreateLimitPropsSerializerSettings")]
    public JsonSerializerSettings CreateLimitPropsSerializerSettings(ICollection<String> limitProps)
    {
      return JsonConvertX.CreateLimitPropsSerializerSettings(limitProps, Formatting.None, TypeNameHandling.Auto, TypeNameAssemblyFormatHandling.Simple);
    }

    #endregion

    #region -- CreateCamelCasePropertyNamesSerializerSettings --

    /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the default settings used by the <see cref="BaseJsonMessageFormatter"/>.</summary>
    [Obsolete("=> JsonConvertX.CreateCamelCaseSerializerSettings")]
    public JsonSerializerSettings CreateCamelCasePropertyNamesSerializerSettings()
    {
      return JsonConvertX.CreateCamelCaseSerializerSettings();
    }

    #endregion

    #region -- CreateCamelCasePropertyNamesLimitPropsSerializerSettings --

    /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the limit properties settings used by the <see cref="BaseJsonMessageFormatter"/>.</summary>
    /// <param name="limitProps"></param>
    /// <returns></returns>
    [Obsolete("=> JsonConvertX.CreateLimitPropsSerializerSettings")]
    public JsonSerializerSettings CreateCamelCasePropertyNamesLimitPropsSerializerSettings(ICollection<String> limitProps)
    {
      return JsonConvertX.CreateLimitPropsSerializerSettings(limitProps, Formatting.None, TypeNameHandling.Auto, TypeNameAssemblyFormatHandling.Simple, true);
    }

    #endregion

    #region -- CreatePropertyMappingSerializerSettings --

    /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the property mappings settings used by the <see cref="BaseJsonMessageFormatter"/>.</summary>
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

    ///// <summary>Determines whether this <see cref="BaseJsonMessageFormatter"/> can read objects
    ///// of the specified <paramref name="type"/>.</summary>
    ///// <param name="type">The <see cref="Type"/> of object that will be read.</param>
    ///// <returns><c>true</c> if objects of this <paramref name="type"/> can be read, otherwise <c>false</c>.</returns>
    //public override Boolean CanReadType(Type type)
    //{
    //  if (type == null) { throw new ArgumentNullException(nameof(type)); }

    //  return true;
    //}

    ///// <summary>Determines whether this <see cref="BaseJsonMessageFormatter"/> can write objects
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

    public override object DeepCopy(object source)
    {
      if (source == null) { return null; }

      var type = source.GetType();
      using (var ms = MemoryStreamManager.GetStream())
      {
        WriteToStream(type, source, ms, Encoding.UTF8);
        ms.Seek(0, System.IO.SeekOrigin.Begin);
        return ReadFromStream(type, ms, Encoding.UTF8);
      }
    }

    #endregion

    #region -- ReadFromStreamAsync --

#if !NET40
    /// <summary>Called during deserialization to read an object of the specified <paramref name="type"/>
    /// from the specified <paramref name="readStream"/>.</summary>
    /// <param name="type">The <see cref="Type"/> of object to read.</param>
    /// <param name="readStream">The <see cref="Stream"/> from which to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <returns>A <see cref="Task"/> whose result will be the object instance that has been read.</returns>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
    public override async Task<Object> ReadFromStreamAsync(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      if (IsStrictMode && type == null) { throw new ArgumentNullException(nameof(type)); }
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      await TaskConstants.Completed;
      return ReadFromStream(type, readStream, effectiveEncoding);
    }
#endif

    #endregion

    #region -- ReadFromStream --

    /// <summary>Called during deserialization to read an object of the specified <paramref name="type"/>
    /// from the specified <paramref name="readStream"/>.</summary>
    /// <param name="type">The <see cref="Type"/> of object to read.</param>
    /// <param name="readStream">The <see cref="Stream"/> from which to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <returns>The <see cref="object"/> instance that has been read.</returns>
    public override Object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding)
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
      if (IsStrictMode && type == null) { throw new ArgumentNullException(nameof(type)); }
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      using (JsonReader jsonReader = CreateJsonReaderInternal(type, readStream, effectiveEncoding))
      {
        //jsonReader.CloseInput = false;
        jsonReader.MaxDepth = _maxDepth;

        if (null == serializerSettings) { serializerSettings = _jsonDeserializerSettings; }

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

    #region ** CreateJsonReaderInternal **

    internal JsonReader CreateJsonReaderInternal(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      Contract.Assert(readStream != null);

      if (null == effectiveEncoding) { effectiveEncoding = Encoding.UTF8; }

      JsonReader reader = CreateJsonReader(type, readStream, effectiveEncoding, _charPool);
      if (reader == null)
      {
        throw Error.InvalidOperation(FormattingSR.MediaTypeFormatter_JsonReaderFactoryReturnedNull, "CreateJsonReader");
      }

      return reader;
    }

    #endregion

    #region -- CreateJsonReader --

    /// <summary>Called during deserialization to get the <see cref="JsonReader"/>.</summary>
    /// <remarks>Public for delegating wrappers of this class.</remarks>
    /// <param name="type">The <see cref="Type"/> of object to read.</param>
    /// <param name="readStream">The <see cref="Stream"/> from which to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <param name="charPool">The reader's character buffer pool.</param>
    /// <returns>The <see cref="JsonWriter"/> used during deserialization.</returns>
    public abstract JsonReader CreateJsonReader(Type type, Stream readStream, Encoding effectiveEncoding, IArrayPool<char> charPool);

    #endregion

    #region == CreateJsonSerializerInternal ==

    //[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This is a public extensibility point, we can't predict what exceptions will come through")]
    //internal JsonSerializer CreateJsonSerializerInternal(JsonSerializerSettings serializerSettings)
    //{
    //  if (serializerSettings == null) { throw new ArgumentNullException(nameof(serializerSettings)); }

    //  JsonSerializer serializer = null;
    //  try
    //  {
    //    serializer = CreateJsonSerializer(serializerSettings);
    //  }
    //  catch (Exception exception)
    //  {
    //    throw Error.InvalidOperation(exception, FormattingSR.JsonSerializerFactoryThrew, "CreateJsonSerializer");
    //  }

    //  if (serializer == null)
    //  {
    //    throw Error.InvalidOperation(FormattingSR.JsonSerializerFactoryReturnedNull, "CreateJsonSerializer");
    //  }

    //  return serializer;
    //}

    #endregion

    #region ++ CreateJsonSerializer ++

    ///// <summary>Called during serialization and deserialization to get the <see cref="JsonSerializer"/>.</summary>
    ///// <remarks>Public for delegating wrappers of this class.</remarks>
    ///// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    ///// <returns>The <see cref="JsonSerializer"/> used during serialization and deserialization.</returns>
    //protected virtual JsonSerializer CreateJsonSerializer(JsonSerializerSettings serializerSettings)
    //{
    //  return JsonConvertX.AllocateSerializer(serializerSettings);
    //}

    #endregion

    #region ++ ReleaseJsonSerializer ++

    ///// <summary>Releases the <paramref name="serializer"/> instance.</summary>
    ///// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    ///// <param name="serializer">The <see cref="JsonSerializer"/> to release.</param>
    ///// <remarks>This method works in tandem with <see cref="ReleaseJsonSerializer(JsonSerializerSettings, JsonSerializer)"/> to
    ///// manage the lifetimes of <see cref="JsonSerializer"/> instances.</remarks>
    //protected virtual void ReleaseJsonSerializer(JsonSerializerSettings serializerSettings, JsonSerializer serializer)
    //    => JsonConvertX.FreeSerializer(serializerSettings, serializer);

    #endregion

    #region -- WriteToStreamAsync --

#if !NET40
    /// <summary>Returns a <see cref="Task"/> that serializes the given <paramref name="value"/> of the given <paramref name="type"/>
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <param name="type">The type of the object to write.</param>
    /// <param name="value">The object value to write.  It may be <c>null</c>.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
    public override async Task WriteToStreamAsync(Type type, Object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (IsStrictMode && type == null) { throw new ArgumentNullException(nameof(type)); }
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      WriteToStream(type, value, writeStream, effectiveEncoding);
      await TaskConstants.Completed;
    }
#endif

    #endregion

    #region -- WriteToStream --

    /// <summary>Called during serialization to write an object of the specified <paramref name="type"/>
    /// to the specified <paramref name="writeStream"/>.</summary>
    /// <param name="type">The <see cref="Type"/> of object to write.</param>
    /// <param name="value">The object to write.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    public override void WriteToStream(Type type, Object value, Stream writeStream, Encoding effectiveEncoding)
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
      if (IsStrictMode && type == null) { throw new ArgumentNullException(nameof(type)); }
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      using (JsonWriter jsonWriter = CreateJsonWriterInternal(type, writeStream, effectiveEncoding))
      {
        //jsonWriter.CloseOutput = false;

        if (null == serializerSettings) { serializerSettings = _jsonSerializerSettings; }
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

    #region == CreateJsonWriterInternal ==

    internal JsonWriter CreateJsonWriterInternal(Type type, Stream writeStream, Encoding effectiveEncoding)
    {
      Contract.Assert(writeStream != null);

      if (null == effectiveEncoding) { effectiveEncoding = StringHelper.SecureUTF8NoBOM; }

      JsonWriter writer = CreateJsonWriter(type, writeStream, effectiveEncoding, _charPool);
      if (writer == null)
      {
        throw Error.InvalidOperation(FormattingSR.MediaTypeFormatter_JsonWriterFactoryReturnedNull, "CreateJsonWriter");
      }

      return writer;
    }

    #endregion

    #region -- CreateJsonWriter --

    /// <summary>Called during serialization to get the <see cref="JsonWriter"/>.</summary>
    /// <remarks>Public for delegating wrappers of this class.  Expected to be called only from</remarks>
    /// <param name="type">The <see cref="Type"/> of object to write.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <param name="charPool">The writer's character array pool.</param>
    /// <returns>The <see cref="JsonWriter"/> used during serialization.</returns>
    public abstract JsonWriter CreateJsonWriter(Type type, Stream writeStream, Encoding effectiveEncoding, IArrayPool<char> charPool);

    #endregion
  }
}
