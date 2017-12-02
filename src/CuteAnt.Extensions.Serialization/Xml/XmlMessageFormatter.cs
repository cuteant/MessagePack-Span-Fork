// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
#if !NETFX_CORE // In portable library we have our own implementation of Concurrent Dictionary which is in the internal namespace
using System.Collections.Concurrent;
#endif
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CuteAnt.Buffers;
using Microsoft.Extensions.Logging;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle Xml.</summary>
  public class XmlMessageFormatter : MessageFormatter
  {
    #region @@ Fields @@

    private ConcurrentDictionary<Type, Object> _serializerCache = new ConcurrentDictionary<Type, Object>();
    private XmlDictionaryReaderQuotas _readerQuotas = FormattingUtilities.CreateDefaultReaderQuotas();

    /// <summary>The default singlegton instance</summary>
    public static readonly XmlMessageFormatter DefaultInstance = new XmlMessageFormatter();

    #endregion

    #region @@ Constructors @@

    /// <summary>Initializes a new instance of the <see cref="XmlMessageFormatter"/> class.</summary>
    public XmlMessageFormatter()
    {
      WriterSettings = new XmlWriterSettings
      {
        OmitXmlDeclaration = true,
        CloseOutput = false,
        CheckCharacters = false
      };
    }

    /// <summary>Initializes a new instance of the <see cref="XmlMessageFormatter"/> class.</summary>
    /// <param name="formatter">The <see cref="XmlMessageFormatter"/> instance to copy settings from.</param>
    protected XmlMessageFormatter(XmlMessageFormatter formatter)
      : base(formatter)
    {
      UseXmlSerializer = formatter.UseXmlSerializer;
      WriterSettings = formatter.WriterSettings;
#if !NETFX_CORE // MaxDepth is not supported in portable libraries
      MaxDepth = formatter.MaxDepth;
#endif
    }

    #endregion

    #region @@ Properties @@

    /// <summary>Gets or sets a value indicating whether to use <see cref="XmlSerializer"/> instead of <see cref="DataContractSerializer"/> by default.</summary>
    /// <value><c>true</c> if use <see cref="XmlSerializer"/> by default; otherwise, <c>false</c>. The default is <c>false</c>.</value>
    [DefaultValue(false)]
    public Boolean UseXmlSerializer { get; set; }

    /// <summary>Gets or sets a value indicating whether to indent elements when writing data.</summary>
    public Boolean Indent
    {
      get { return WriterSettings.Indent; }
      set { WriterSettings.Indent = value; }
    }

    /// <summary>Gets the <see cref="XmlWriterSettings"/> to be used while writing.</summary>
    public XmlWriterSettings WriterSettings { get; private set; }

#if !NETFX_CORE // MaxDepth is not supported in portable libraries
    /// <summary>Gets or sets the maximum depth allowed by this formatter.</summary>
    public Int32 MaxDepth
    {
      get { return _readerQuotas.MaxDepth; }
      set
      {
        if (value < FormattingUtilities.DefaultMinDepth)
        {
          throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof(value), value, FormattingUtilities.DefaultMinDepth);
        }

        _readerQuotas.MaxDepth = value;
      }
    }
#endif

    #endregion

    #region -- SetSerializer --

    /// <summary>Registers the <see cref="XmlObjectSerializer"/> to use to read or write the specified <paramref name="type"/>.</summary>
    /// <param name="type">The type of object that will be serialized or deserialized with <paramref name="serializer"/>.</param>
    /// <param name="serializer">The <see cref="XmlObjectSerializer"/> instance to use.</param>
    public void SetSerializer(Type type, XmlObjectSerializer serializer)
    {
      VerifyAndSetSerializer(type, serializer);
    }

    /// <summary>Registers the <see cref="XmlObjectSerializer"/> to use to read or write the specified <typeparamref name="T"/> type.</summary>
    /// <typeparam name="T">The type of object that will be serialized or deserialized with <paramref name="serializer"/>.</typeparam>
    /// <param name="serializer">The <see cref="XmlObjectSerializer"/> instance to use.</param>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The T represents a Type parameter.")]
    public void SetSerializer<T>(XmlObjectSerializer serializer)
    {
      SetSerializer(typeof(T), serializer);
    }

    /// <summary>Registers the <see cref="XmlSerializer"/> to use to read or write the specified <paramref name="type"/>.</summary>
    /// <param name="type">The type of objects for which <paramref name="serializer"/> will be used.</param>
    /// <param name="serializer">The <see cref="XmlSerializer"/> instance to use.</param>
    public void SetSerializer(Type type, XmlSerializer serializer)
    {
      VerifyAndSetSerializer(type, serializer);
    }

    /// <summary>Registers the <see cref="XmlSerializer"/> to use to read or write the specified <typeparamref name="T"/> type.</summary>
    /// <typeparam name="T">The type of object that will be serialized or deserialized with <paramref name="serializer"/>.</typeparam>
    /// <param name="serializer">The <see cref="XmlSerializer"/> instance to use.</param>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The T represents a Type parameter.")]
    public void SetSerializer<T>(XmlSerializer serializer)
    {
      SetSerializer(typeof(T), serializer);
    }

    #endregion

    #region -- RemoveSerializer --

    /// <summary>Unregisters the serializer currently associated with the given <paramref name="type"/>.</summary>
    /// <remarks>Unless another serializer is registered for the <paramref name="type"/>, a default one will be created.</remarks>
    /// <param name="type">The type of object whose serializer should be removed.</param>
    /// <returns><c>true</c> if a serializer was registered for the <paramref name="type"/>; otherwise <c>false</c>.</returns>
    public Boolean RemoveSerializer(Type type)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }

      Object value;
      return _serializerCache.TryRemove(type, out value);
    }

    #endregion

    #region -- IsSupportedType --

    private static readonly ConcurrentHashSet<RuntimeTypeHandle> s_unsupportedTypeInfoSet = new ConcurrentHashSet<RuntimeTypeHandle>();

    /// <inheritdoc />
    public override bool IsSupportedType(Type type)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }

      var typeHandle = type.TypeHandle;
      if (s_unsupportedTypeInfoSet.Contains(typeHandle)) { return false; }

      if (!CanReadType(type) || !CanWriteType(type)) { s_unsupportedTypeInfoSet.TryAdd(typeHandle); return false; }

      return true;
    }

    #endregion

    #region -- CanReadType --

    /// <summary>Determines whether this <see cref="XmlMessageFormatter"/> can read objects of the specified <paramref name="type"/>.</summary>
    /// <param name="type">The type of object that will be read.</param>
    /// <returns><c>true</c> if objects of this <paramref name="type"/> can be read, otherwise <c>false</c>.</returns>
    public override Boolean CanReadType(Type type)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }

      // If there is a registered non-null serializer, we can support this type.
      // Otherwise attempt to create the default serializer.
      var serializer = GetCachedSerializer(type, throwOnError: false);

      // Null means we tested it before and know it is not supported
      return serializer != null;
    }

    #endregion

    #region -- CanWriteType --

    /// <summary>Determines whether this <see cref="XmlMessageFormatter"/> can write objects of the specified <paramref name="type"/>.</summary>
    /// <param name="type">The type of object that will be written.</param>
    /// <returns><c>true</c> if objects of this <paramref name="type"/> can be written, otherwise <c>false</c>.</returns>
    public override Boolean CanWriteType(Type type)
    {
      // Performance-sensitive
      if (type == null) { throw new ArgumentNullException(nameof(type)); }

      if (UseXmlSerializer)
      {
        TryGetDelegatingTypeForIEnumerableGenericOrSame(ref type);
      }
      else
      {
        TryGetDelegatingTypeForIQueryableGenericOrSame(ref type);
      }

      // If there is a registered non-null serializer, we can support this type.
      var serializer = GetCachedSerializer(type, throwOnError: false);

      // Null means we tested it before and know it is not supported
      return serializer != null;
    }

    #endregion

    #region -- ReadFromStreamAsync --

    ///// <summary>Called during deserialization to read an object of the specified <paramref name="type"/>
    ///// from the specified <paramref name="readStream"/>.</summary>
    ///// <param name="type">The type of object to read.</param>
    ///// <param name="readStream">The <see cref="BufferManagerStreamReader"/> from which to read.</param>
    ///// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    ///// <returns>A <see cref="Task"/> whose result will be the object instance that has been read.</returns>
    //[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
    //public override Task<Object> ReadFromStreamAsync(Type type, BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    //{
    //  if (type == null) { throw new ArgumentNullException(nameof(type)); }
    //  if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

    //  try
    //  {
    //    return TaskShim.FromResult(ReadFromStream(type, readStream, effectiveEncoding));
    //  }
    //  catch (Exception e)
    //  {
    //    return TaskConstants.FromError<Object>(e);
    //  }
    //}

    #endregion

    #region -- ReadFromStream --

    /// <inheritdoc />
    public override Object ReadFromStream(Type type, BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      // If content length is 0 then return default value for this type
      if (readStream.Position == readStream.Length)
      {
        return GetDefaultValueForType(type);
      }
      if (effectiveEncoding == null) { effectiveEncoding = Encoding.UTF8; }

      object serializer = GetDeserializer(type);

      try
      {
        using (XmlReader reader = CreateXmlReader(readStream, effectiveEncoding))
        {
          var xmlSerializer = serializer as XmlSerializer;
          if (xmlSerializer != null)
          {
            return xmlSerializer.Deserialize(reader);
          }
          else
          {
            var xmlObjectSerializer = serializer as XmlObjectSerializer;
            if (xmlObjectSerializer == null)
            {
              ThrowInvalidSerializerException(serializer, "GetDeserializer");
            }
            return xmlObjectSerializer.ReadObject(reader);
          }
        }
      }
      catch (Exception e)
      {
        Logger.LogError(e.ToString());
        return GetDefaultValueForType(type);
      }
    }

    #endregion

    #region += GetDeserializer =+

    /// <summary>Called during deserialization to get the XML serializer to use for deserializing objects.</summary>
    /// <param name="type">The type of object to deserialize.</param>
    /// <returns>An instance of <see cref="XmlObjectSerializer"/> or <see cref="XmlSerializer"/> to use for deserializing the object.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "The term deserializer is spelled correctly.")]
    protected internal virtual Object GetDeserializer(Type type)
    {
      return GetSerializerForType(type);
    }

    #endregion

    #region += CreateXmlReader =+

    /// <summary>Called during deserialization to get the XML reader to use for reading objects from the stream.</summary>
    /// <param name="readStream">The <see cref="Stream"/> to read from.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <returns>The <see cref="XmlReader"/> to use for reading objects.</returns>
    protected internal virtual XmlReader CreateXmlReader(Stream readStream, Encoding effectiveEncoding)
    {
      // Get the character encoding for the content
      if (null == effectiveEncoding) { effectiveEncoding = Encoding.UTF8; }
#if NETFX_CORE
      // Force a preamble into the stream, since CreateTextReader in WinRT only supports auto-detecting encoding.
      return XmlDictionaryReader.CreateTextReader(new ReadOnlyStreamWithEncodingPreamble(readStream, effectiveEncoding), _readerQuotas);
#else
      return XmlDictionaryReader.CreateTextReader(new NonClosingDelegatingStream(readStream), effectiveEncoding, _readerQuotas, null);
#endif
    }

    #endregion

    #region -- WriteToStreamAsync --

    ///// <inheritdoc/>
    //[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
    //public override Task WriteToStreamAsync(Type type, Object value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
    //{
    //  if (type == null) { throw new ArgumentNullException(nameof(type)); }
    //  if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

    //  try
    //  {
    //    WriteToStream(type, value, writeStream, effectiveEncoding);
    //    return TaskConstants.Completed;
    //  }
    //  catch (Exception e)
    //  {
    //    return TaskConstants.FromError(e);
    //  }
    //}

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public override void WriteToStream(Type type, Object value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      var isRemapped = false;
      if (UseXmlSerializer)
      {
        isRemapped = TryGetDelegatingTypeForIEnumerableGenericOrSame(ref type);
      }
      else
      {
        isRemapped = TryGetDelegatingTypeForIQueryableGenericOrSame(ref type);
      }

      if (null == value)
      {
        writeStream.Write7BitEncodedInt(SIZE_ZERO);
        return;
      }

      if (isRemapped && value != null)
      {
        value = GetTypeRemappingConstructor(type).Invoke(new Object[] { value });
      }

      object serializer = GetSerializer(type, value);

      using (XmlWriter writer = CreateXmlWriter(writeStream, effectiveEncoding))
      {
        var xmlSerializer = serializer as XmlSerializer;
        if (xmlSerializer != null)
        {
          xmlSerializer.Serialize(writer, value);
        }
        else
        {
          var xmlObjectSerializer = serializer as XmlObjectSerializer;
          if (xmlObjectSerializer == null)
          {
            ThrowInvalidSerializerException(serializer, "GetSerializer");
          }
          xmlObjectSerializer.WriteObject(writer, value);
        }
      }
    }

    #endregion

    #region += GetSerializer =+

    /// <summary>Called during serialization to get the XML serializer to use for serializing objects.</summary>
    /// <param name="type">The type of object to serialize.</param>
    /// <param name="value">The object to serialize.</param>
    /// <returns>An instance of <see cref="XmlObjectSerializer"/> or <see cref="XmlSerializer"/> to use for serializing the object.</returns>
    protected internal virtual Object GetSerializer(Type type, Object value)
    {
      return GetSerializerForType(type);
    }

    #endregion

    #region += CreateXmlWriter =+

    /// <summary>Called during serialization to get the XML writer to use for writing objects to the stream.</summary>
    /// <param name="writeStream">The <see cref="Stream"/> to write to.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <returns>The <see cref="XmlWriter"/> to use for writing objects.</returns>
    protected internal virtual XmlWriter CreateXmlWriter(Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == effectiveEncoding) { effectiveEncoding = Encoding.UTF8; }
      XmlWriterSettings writerSettings = WriterSettings.Clone();
      writerSettings.Encoding = effectiveEncoding;
      return XmlWriter.Create(writeStream, writerSettings);
    }

    #endregion

    #region -- CreateXmlSerializer --

    /// <summary>Called during deserialization to get the XML serializer.</summary>
    /// <param name="type">The type of object that will be serialized or deserialized.</param>
    /// <returns>The <see cref="XmlSerializer"/> used to serialize the object.</returns>
    public virtual XmlSerializer CreateXmlSerializer(Type type)
    {
      return new XmlSerializer(type);
    }

    #endregion

    #region -- CreateDataContractSerializer --

    /// <summary>Called during deserialization to get the DataContractSerializer serializer.</summary>
    /// <param name="type">The type of object that will be serialized or deserialized.</param>
    /// <returns>The <see cref="DataContractSerializer"/> used to serialize the object.</returns>
    public virtual DataContractSerializer CreateDataContractSerializer(Type type)
    {
      return new DataContractSerializer(type);
    }

    #endregion

    #region -- InvokeCreateXmlReader --

    /// <summary>This method is to support infrastructure and is not intended to be used directly from your code.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public XmlReader InvokeCreateXmlReader(Stream readStream, Encoding effectiveEncoding)
    {
      return CreateXmlReader(readStream, effectiveEncoding);
    }

    #endregion

    #region -- InvokeCreateXmlWriter --

    /// <summary>This method is to support infrastructure and is not intended to be used directly from your code.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public XmlWriter InvokeCreateXmlWriter(Stream writeStream, Encoding effectiveEncoding)
    {
      return CreateXmlWriter(writeStream, effectiveEncoding);
    }

    #endregion

    #region -- InvokeGetDeserializer --

    /// <summary>This method is to support infrastructure and is not intended to be used directly from your code.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Object InvokeGetDeserializer(Type type)
    {
      return GetDeserializer(type);
    }

    #endregion

    #region -- InvokeGetSerializer --

    /// <summary>This method is to support infrastructure and is not intended to be used directly from your code.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Object InvokeGetSerializer(Type type, Object value)
    {
      return GetSerializer(type, value);
    }

    #endregion

    #region ** CreateDefaultSerializer **

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Since we use an extensible factory method we cannot control the exceptions being thrown")]
    private Object CreateDefaultSerializer(Type type, Boolean throwOnError)
    {
      Contract.Assert(type != null, "type cannot be null.");

      Exception exception = null;
      Object serializer = null;

      try
      {
        if (UseXmlSerializer)
        {
          serializer = CreateXmlSerializer(type);
        }
        else
        {
#if !NETFX_CORE
          // REVIEW: Is there something comparable in WinRT?
          // Verify that type is a valid data contract by forcing the serializer to try to create a data contract
          FormattingUtilities.XsdDataContractExporter.GetRootElementName(type);
#endif
          serializer = CreateDataContractSerializer(type);
        }
      }
      catch (Exception caught)
      {
        exception = caught;
      }

      if (serializer == null && throwOnError)
      {
        if (exception != null)
        {
          throw Error.InvalidOperation(exception, FormattingSR.SerializerCannotSerializeType,
                        UseXmlSerializer ? typeof(XmlSerializer).Name : typeof(DataContractSerializer).Name,
                        type.Name);
        }
        else
        {
          throw Error.InvalidOperation(FormattingSR.SerializerCannotSerializeType,
                    UseXmlSerializer ? typeof(XmlSerializer).Name : typeof(DataContractSerializer).Name,
                    type.Name);
        }
      }

      return serializer;
    }

    #endregion

    #region ** GetCachedSerializer **

    private Object GetCachedSerializer(Type type, Boolean throwOnError)
    {
      // Performance-sensitive
      Object serializer;
      if (!_serializerCache.TryGetValue(type, out serializer))
      {
        // Race condition on creation has no side effects
        serializer = CreateDefaultSerializer(type, throwOnError);
        _serializerCache.TryAdd(type, serializer);
      }
      return serializer;
    }

    #endregion

    #region ** VerifyAndSetSerializer **

    private void VerifyAndSetSerializer(Type type, Object serializer)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (serializer == null) { throw new ArgumentNullException(nameof(serializer)); }

      SetSerializerInternal(type, serializer);
    }

    #endregion

    #region ** SetSerializerInternal **

    private void SetSerializerInternal(Type type, Object serializer)
    {
      Contract.Assert(type != null, "type cannot be null.");
      Contract.Assert(serializer != null, "serializer cannot be null.");

      _serializerCache.AddOrUpdate(type, serializer, (key, value) => serializer);
    }

    #endregion

    #region ** GetSerializerForType **

    private Object GetSerializerForType(Type type)
    {
      // Performance-sensitive
      Contract.Assert(type != null, "Type cannot be null");

      var serializer = GetCachedSerializer(type, throwOnError: true);

      if (serializer == null)
      {
        // A null serializer indicates the type has already been tested
        // and found unsupportable.
        throw Error.InvalidOperation(FormattingSR.SerializerCannotSerializeType,
                      UseXmlSerializer ? typeof(XmlSerializer).Name : typeof(DataContractSerializer).Name,
                      type.Name);
      }

      Contract.Assert(serializer is XmlSerializer || serializer is XmlObjectSerializer, "Only XmlSerializer or XmlObjectSerializer are supported.");
      return serializer;
    }

    #endregion

    #region ==& ThrowInvalidSerializerException &==

    internal static void ThrowInvalidSerializerException(Object serializer, String getSerializerMethodName)
    {
      if (serializer == null)
      {
        throw Error.InvalidOperation(FormattingSR.XmlMediaTypeFormatter_NullReturnedSerializer, getSerializerMethodName);
      }
      else
      {
        throw Error.InvalidOperation(FormattingSR.XmlMediaTypeFormatter_InvalidSerializerType, serializer.GetType().Name, getSerializerMethodName);
      }
    }

    #endregion
  }
}
