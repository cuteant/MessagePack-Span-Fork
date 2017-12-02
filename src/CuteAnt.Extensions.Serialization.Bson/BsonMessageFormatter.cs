// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CuteAnt.AsyncEx;
using CuteAnt.Buffers;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
#if NET40
using System.Reflection;
#endif

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle Bson.</summary>
  public class BsonMessageFormatter : BaseJsonMessageFormatter
  {
    #region @@ Fields @@

    /// <summary>The default singlegton instance</summary>
    public static readonly BsonMessageFormatter DefaultInstance = new BsonMessageFormatter();

    private static readonly Type OpenDictionaryType = typeof(Dictionary<,>);

    private const String c_value = "Value";

    private static readonly ConcurrentHashSet<RuntimeTypeHandle> s_supportedTypeInfoSet = new ConcurrentHashSet<RuntimeTypeHandle>();
    private static readonly ConcurrentHashSet<RuntimeTypeHandle> s_unsupportedTypeInfoSet = new ConcurrentHashSet<RuntimeTypeHandle>();

    #endregion

    #region @@ Constructors @@

    static BsonMessageFormatter()
    {
      s_unsupportedTypeInfoSet.TryAdd(typeof(DataSet).TypeHandle);
      s_unsupportedTypeInfoSet.TryAdd(typeof(DataTable).TypeHandle);
    }

    /// <summary>Initializes a new instance of the <see cref="BsonMessageFormatter"/> class.</summary>
    public BsonMessageFormatter()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="BsonMessageFormatter"/> class.</summary>
    /// <param name="formatter">The <see cref="BsonMessageFormatter"/> instance to copy settings from.</param>
    protected BsonMessageFormatter(BsonMessageFormatter formatter)
      : base(formatter)
    {
    }

    #endregion

    #region @@ Properties @@

#if !NETFX_CORE // MaxDepth and DBNull not supported in portable library; no need to override there
    /// <inheritdoc />
    public sealed override Int32 MaxDepth
    {
      get { return base.MaxDepth; }
      set { base.MaxDepth = value; }
    }
#endif

    #endregion

    #region -- ReadFromStreamAsync --

#if !NETFX_CORE
    /// <inheritdoc />
    public override Task<object> ReadFromStreamAsync(Type type, BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      if (type == typeof(DBNull) && readStream.Position == readStream.Length)
      {
        // Lower-level Json.Net deserialization can convert null to DBNull.Value. However this formatter treats
        // DBNull.Value like null and serializes no content. Json.Net code won't be invoked at all (for read or
        // write). Override BaseJsonMediaTypeFormatter.ReadFromStream()'s call to GetDefaultValueForType()
        // (which would return null in this case) and instead return expected DBNull.Value. Special case exists
        // primarily for parity with JsonMediaTypeFormatter.
        return TaskShim.FromResult((Object)DBNull.Value);
      }
      else
      {
        return base.ReadFromStreamAsync(type, readStream, effectiveEncoding);
      }
    }
#endif

    #endregion

    #region -- ReadFromStream --

    /// <inheritdoc />
    public override Object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding, JsonSerializerSettings serializerSettings)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }
      if (effectiveEncoding == null) { throw new ArgumentNullException(nameof(effectiveEncoding)); }

      // Special-case for simple types: Deserialize a Dictionary with a single element named Value.
      // Serialization created this Dictionary<string, object> to work around BSON restrictions: BSON cannot
      // handle a top-level simple type.  NewtonSoft.Json throws a JsonWriterException with message "Error
      // writing Binary value. BSON must start with an Object or Array. Path ''." when WriteToStream() is given
      // such a value.
      //
      // Added clause for typeof(byte[]) needed because NewtonSoft.Json sometimes throws above Exception when
      // WriteToStream() is given a byte[] value.  (Not clear where the bug lies and, worse, it doesn't reproduce
      // reliably.)
      //
      // Request for typeof(object) may cause a simple value to round trip as a JObject.
      if (IsSimpleType(type) || type == typeof(Byte[]))
      {
        // Read as exact expected Dictionary<string, T> to ensure NewtonSoft.Json does correct top-level conversion.
        Type dictionaryType = OpenDictionaryType.MakeGenericType(new Type[] { typeof(String), type });
        var dictionary = base.ReadFromStream(dictionaryType, readStream, effectiveEncoding, serializerSettings) as IDictionary;
        if (dictionary == null)
        {
          // Not valid since BaseJsonMediaTypeFormatter.ReadFromStream(Type, Stream, HttpContent, IFormatterLogger)
          // handles empty content and does not call ReadFromStream(Type, Stream, Encoding, IFormatterLogger)
          // in that case.
          throw Error.InvalidOperation(FormattingSR.MediaTypeFormatter_BsonParseError_MissingData,
              dictionaryType.Name);
        }

        // Unfortunately IDictionary doesn't have TryGetValue()...
        var firstKey = String.Empty;
        foreach (DictionaryEntry item in dictionary)
        {
          if (dictionary.Count == 1 && (item.Key as String) == c_value)
          {
            // Success
            return item.Value;
          }
          else
          {
            if (item.Key != null)
            {
              firstKey = item.Key.ToString();
            }

            break;
          }
        }

        throw Error.InvalidOperation(FormattingSR.MediaTypeFormatter_BsonParseError_UnexpectedData,
            dictionary.Count, firstKey);
      }
      else
      {
        return base.ReadFromStream(type, readStream, effectiveEncoding, serializerSettings);
      }
    }

    #endregion

    #region -- CreateJsonReader --

    /// <inheritdoc />
    public override JsonReader CreateJsonReader(Type type, Stream readStream, Encoding effectiveEncoding, IArrayPool<char> charPool)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }
      if (effectiveEncoding == null) { throw new ArgumentNullException(nameof(effectiveEncoding)); }

#if NET40
      var reader = new BsonReader(new BinaryReader(readStream, effectiveEncoding));
#else
      var reader = new BsonDataReader(new BinaryReader(readStream, effectiveEncoding));
#endif

      try
      {
        // Special case discussed at http://stackoverflow.com/questions/16910369/bson-array-deserialization-with-json-net
        // Dispensed with string (aka IEnumerable<char>) case above in ReadFromStream()
        reader.ReadRootValueAsArray =
            typeof(IEnumerable).IsAssignableFrom(type) && !typeof(IDictionary).IsAssignableFrom(type);
      }
      catch
      {
        // Ensure instance is cleaned up in case of an issue
        ((IDisposable)reader).Dispose();
        throw;
      }

      return reader;
    }

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public override void WriteToStream(Type type, Object value, Stream writeStream, Encoding effectiveEncoding, JsonSerializerSettings serializerSettings)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }
      if (effectiveEncoding == null) { throw new ArgumentNullException(nameof(effectiveEncoding)); }

      if (value == null)
      {
        // Cannot serialize null at the top level.  Json.Net throws Newtonsoft.Json.JsonWriterException : Error
        // writing Null value. BSON must start with an Object or Array. Path ''.  Fortunately
        // BaseJsonMediaTypeFormatter.ReadFromStream(Type, Stream, HttpContent, IFormatterLogger) treats zero-
        // length content as null or the default value of a struct.
        return;
      }

#if !NETFX_CORE // DBNull not supported in portable library
      if (value == DBNull.Value)
      {
        // ReadFromStreamAsync() override above converts null to DBNull.Value if given Type is DBNull; normally
        // however DBNull.Value round-trips as null. There's a known edge case where a full .NET application
        // uses the portable version of this formatter. The full .NET application may pass DBNull.Value,
        // leading to a JsonWriterException. If a DBNull is at a lower level in the value, the portable
        // Json.Net assembly will also fail to special-case that DBNull, serialize it as an empty JSON object
        // rather than null, and not meet the receiver's expectations.
        return;
      }
#endif

      // See comments in ReadFromStream() above about this special case and the need to include byte[] in it.
      // Using runtime type here because Json.Net will throw during serialization whenever it cannot handle the
      // runtime type at the top level. For e.g. passed type may be typeof(object) and value may be a string.
      Type runtimeType = value.GetType();
      if (IsSimpleType(runtimeType) || runtimeType == typeof(Byte[]))
      {
        // Wrap value in a Dictionary with a single property named "Value" to provide BSON with an Object.  Is
        // written out as binary equivalent of { "Value": value } JSON.
        var temporaryDictionary = new Dictionary<String, Object>
                {
                    { c_value, value },
                };
        base.WriteToStream(typeof(Dictionary<String, Object>), temporaryDictionary, writeStream, effectiveEncoding, serializerSettings);
      }
      else
      {
        base.WriteToStream(type, value, writeStream, effectiveEncoding, serializerSettings);
      }
    }

    #endregion

    #region -- CreateJsonWriter --

    /// <inheritdoc />
    public override JsonWriter CreateJsonWriter(Type type, Stream writeStream, Encoding effectiveEncoding, IArrayPool<char> charPool)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }
      if (effectiveEncoding == null) { throw new ArgumentNullException(nameof(effectiveEncoding)); }

#if NET40
      return new BsonWriter(new BinaryWriter(writeStream, effectiveEncoding));
#else
      return new BsonDataWriter(new BinaryWriter(writeStream, effectiveEncoding));
#endif
    }

    #endregion

    #region **& IsSimpleType &**

    // Return true if Json.Net will likely convert value of given type to a Json primitive, not JsonArray nor
    // JsonObject.
    // To do: https://aspnetwebstack.codeplex.com/workitem/1467
    private static Boolean IsSimpleType(Type type)
    {
      Contract.Assert(type != null);

#if NETFX_CORE // TypeDescriptor is not supported in portable library
      return type.IsValueType() || type == typeof(string);
#else
      // CanConvertFrom() check is similar to MVC / Web API ModelMetadata.IsComplexType getters. This is
      // sufficient for many cases but Json.Net uses JsonConverterAttribute and built-in converters, not type
      // descriptors.
      return TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
#endif
    }

    #endregion
  }
}
