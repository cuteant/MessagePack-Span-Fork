// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CuteAnt.AsyncEx;
using CuteAnt.Buffers;
using CuteAnt.Runtime;
using Microsoft.Extensions.Logging;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle Xml.</summary>
  public class XmlMessageSerializer : XmlMessageFormatter
  {
    /// <summary>The default singlegton instance</summary>
    public new static readonly XmlMessageSerializer DefaultInstance = new XmlMessageSerializer();

    #region @@ Constructors @@

    /// <summary>Initializes a new instance of the <see cref="XmlMessageSerializer"/> class.</summary>
    public XmlMessageSerializer()
      : base()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="XmlMessageSerializer"/> class.</summary>
    /// <param name="formatter">The <see cref="XmlMessageSerializer"/> instance to copy settings from.</param>
    protected XmlMessageSerializer(XmlMessageSerializer formatter)
      : base(formatter)
    {
    }

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

      var len = readStream.Read7BitEncodedInt();
      if (len > SIZE_ZERO) { return GetDefaultValueForType(type); }

      var bufferManager = BufferManager.GlobalManager;
      object obj;
      byte[] buffer = null;
      BufferManagerStreamReader binReader = null;
      try
      {
        buffer = BufferManager.GlobalManager.TakeBuffer(len);
        readStream.Read(buffer, 0, len);
        binReader = BufferManagerStreamReaderManager.Take();
        binReader.Reinitialize(buffer, 0, len, bufferManager);

        var serializer = GetDeserializer(type);
        using (XmlReader reader = CreateXmlReader(binReader, effectiveEncoding))
        {
          if (serializer is XmlSerializer xmlSerializer)
          {
            obj = xmlSerializer.Deserialize(reader);
          }
          else
          {
            var xmlObjectSerializer = serializer as XmlObjectSerializer;
            if (xmlObjectSerializer == null)
            {
              ThrowInvalidSerializerException(serializer, "GetDeserializer");
            }
            obj = xmlObjectSerializer.ReadObject(reader);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.ToString());
        obj = GetDefaultValueForType(type);
      }
      finally
      {
        BufferManagerStreamReaderManager.Return(binReader);
        if (buffer != null) { bufferManager.ReturnBuffer(buffer); }
      }
      return obj;
    }

    #endregion

    #region ** WriteToStream **

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

      var serializer = GetSerializer(type, value);

      using (var pooledStream = BufferManagerOutputStreamManager.Create())
      {
        var output = pooledStream.Object;
        output.Reinitialize();
        using (XmlWriter writer = CreateXmlWriter(output, effectiveEncoding))
        {
          if (serializer is XmlSerializer xmlSerializer)
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
        var objStream = output.ToReadOnlyStream();
        writeStream.Write7BitEncodedInt((int)objStream.Length);
        StreamToStreamCopy.CopyAsync(objStream, writeStream, true).WaitAndUnwrapException();
      }
    }

    #endregion
  }
}
