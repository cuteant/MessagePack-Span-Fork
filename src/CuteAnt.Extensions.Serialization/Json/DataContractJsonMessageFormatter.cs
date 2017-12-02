// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
#if !NETFX_CORE
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
#endif
using System.Diagnostics.Contracts;
using System.IO;
#if !NETFX_CORE
using System.Runtime.Serialization.Json;
#endif
using System.Text;
#if !NETFX_CORE
using System.Xml;
#endif
using Newtonsoft.Json;
#if NET40
using System.Reflection;
#endif

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle Json.</summary>
  public class DataContractJsonMessageFormatter : BaseJsonMessageFormatter
  {
    #region @@ Fields @@

    /// <summary>The default singlegton instance</summary>
    public static readonly DataContractJsonMessageFormatter DefaultInstance = new DataContractJsonMessageFormatter();

#if !NETFX_CORE // DataContractJsonSerializer and MediaTypeMappings are not supported in portable library
    private ConcurrentDictionary<Type, DataContractJsonSerializer> _dataContractSerializerCache = new ConcurrentDictionary<Type, DataContractJsonSerializer>();
    private XmlDictionaryReaderQuotas _readerQuotas = FormattingUtilities.CreateDefaultReaderQuotas();
#endif

    #endregion

    #region @@ Constructors @@

    /// <summary>Initializes a new instance of the <see cref="DataContractJsonMessageFormatter"/> class.</summary>
    public DataContractJsonMessageFormatter()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DataContractJsonMessageFormatter"/> class.</summary>
    /// <param name="formatter">The <see cref="DataContractJsonMessageFormatter"/> instance to copy settings from.</param>
    protected DataContractJsonMessageFormatter(DataContractJsonMessageFormatter formatter)
      : base(formatter)
    {
      Contract.Assert(formatter != null);

      JsonFormatting = formatter.JsonFormatting;
    }

    #endregion

    #region @@ Properties @@

#if !NETFX_CORE // MaxDepth not supported in portable library; no need to override there
    /// <inheritdoc/>
    public sealed override Int32 MaxDepth
    {
      get { return base.MaxDepth; }
      set
      {
        base.MaxDepth = value;
        _readerQuotas.MaxDepth = value;
      }
    }
#endif

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

    #region -- CreateJsonReader --

    /// <inheritdoc />
    public override JsonReader CreateJsonReader(Type type, Stream readStream, Encoding effectiveEncoding, IArrayPool<char> charPool)
    {
      return null;
    }

    #endregion

    #region -- CreateJsonWriter --

    /// <inheritdoc />
    public override JsonWriter CreateJsonWriter(Type type, Stream writeStream, Encoding effectiveEncoding, IArrayPool<char> charPool)
    {
      return null;
    }

    #endregion

    #region -- CanReadType --

    /// <inheritdoc />
    public override Boolean CanReadType(Type type)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }

      // If there is a registered non-null serializer, we can support this type.
      DataContractJsonSerializer serializer =
          _dataContractSerializerCache.GetOrAdd(type, (t) => CreateDataContractSerializer(t, throwOnError: false));

      // Null means we tested it before and know it is not supported
      return serializer != null;
    }

    #endregion

    #region -- CanWriteType --

    /// <inheritdoc />
    public override Boolean CanWriteType(Type type)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }

      TryGetDelegatingTypeForIQueryableGenericOrSame(ref type);

      // If there is a registered non-null serializer, we can support this type.
      Object serializer =
          _dataContractSerializerCache.GetOrAdd(type, (t) => CreateDataContractSerializer(t, throwOnError: false));

      // Null means we tested it before and know it is not supported
      return serializer != null;
    }

    #endregion

    #region -- ReadFromStream --

    /// <inheritdoc />
    public override Object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding, JsonSerializerSettings serializerSettings)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }
      if (effectiveEncoding == null) { throw new ArgumentNullException(nameof(effectiveEncoding)); }

      var dataContractSerializer = GetDataContractSerializer(type);
      using (XmlReader reader = JsonReaderWriterFactory.CreateJsonReader(new NonClosingDelegatingStream(readStream), effectiveEncoding, _readerQuotas, null))
      {
        return dataContractSerializer.ReadObject(reader);
      }
    }

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public override void WriteToStream(Type type, Object value, Stream writeStream, Encoding effectiveEncoding, JsonSerializerSettings serializerSettings)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }
      if (effectiveEncoding == null) { throw new ArgumentNullException(nameof(effectiveEncoding)); }

      if (TryGetDelegatingTypeForIQueryableGenericOrSame(ref type))
      {
        if (value != null)
        {
          value = GetTypeRemappingConstructor(type).Invoke(new Object[] { value });
        }
      }

      var dataContractSerializer = GetDataContractSerializer(type);
      using (XmlWriter writer = JsonReaderWriterFactory.CreateJsonWriter(writeStream, effectiveEncoding, ownsStream: false))
      {
        dataContractSerializer.WriteObject(writer, value);
      }
    }

    #endregion

    #region ** CreateDataContractSerializer **

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catch all is around an extensibile method")]
    private DataContractJsonSerializer CreateDataContractSerializer(Type type, Boolean throwOnError)
    {
      Contract.Assert(type != null);

      DataContractJsonSerializer serializer = null;
      Exception exception = null;

      try
      {
        // Verify that type is a valid data contract by forcing the serializer to try to create a data contract
        FormattingUtilities.XsdDataContractExporter.GetRootElementName(type);
        serializer = CreateDataContractSerializer(type);
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
                        typeof(DataContractJsonSerializer).Name,
                        type.Name);
        }
        else
        {
          throw Error.InvalidOperation(FormattingSR.SerializerCannotSerializeType,
                        typeof(DataContractJsonSerializer).Name,
                        type.Name);
        }
      }

      return serializer;
    }

    #endregion

    #region -- CreateDataContractSerializer --

    /// <summary>Called during deserialization to get the <see cref="DataContractJsonSerializer"/>.</summary>
    /// <remarks>Public for delegating wrappers of this class.</remarks>
    /// <param name="type">The type of object that will be serialized or deserialized.</param>
    /// <returns>The <see cref="DataContractJsonSerializer"/> used to serialize the object.</returns>
    public virtual DataContractJsonSerializer CreateDataContractSerializer(Type type)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }

      return new DataContractJsonSerializer(type);
    }

    #endregion

    #region ** GetDataContractSerializer **

    private DataContractJsonSerializer GetDataContractSerializer(Type type)
    {
      Contract.Assert(type != null, "Type cannot be null");

      DataContractJsonSerializer serializer =
          _dataContractSerializerCache.GetOrAdd(type, (t) => CreateDataContractSerializer(type, throwOnError: true));

      if (serializer == null)
      {
        // A null serializer means the type cannot be serialized
        throw Error.InvalidOperation(FormattingSR.SerializerCannotSerializeType, typeof(DataContractJsonSerializer).Name, type.Name);
      }

      return serializer;
    }

    #endregion
  }
}
