using System;
using System.Data;
using System.Text;
using CuteAnt.Buffers;
using CuteAnt.Extensions.Serialization.Protobuf;
using CuteAnt.Runtime;
using Microsoft.Extensions.Logging;
using ProtoBuf;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle protobuf-net.</summary>
  public class ProtoBufMessageSerializer : ProtoBufMessageFormatter
  {
    /// <summary>The default singlegton instance</summary>
    public new static readonly ProtoBufMessageSerializer DefaultInstance = new ProtoBufMessageSerializer();

    #region -- Constructors --

    static ProtoBufMessageSerializer()
    {
      s_typeInfoDictionary[typeof(DataSet)] = ProtoBufSerializationTokenType.DataSet;
      s_typeInfoDictionary[typeof(DataTable)] = ProtoBufSerializationTokenType.DataTable;
    }

    /// <summary>Constructor</summary>
    public ProtoBufMessageSerializer()
    {
    }

    #endregion

    #region -- ReadFromStream --

    /// <inheritdoc />
    public override object ReadFromStream(Type type, BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      if (readStream.Position == readStream.Length) { return GetDefaultValueForType(type); }

      object obj;
      try
      {
        var serializationTokenType = GetSerializationTokenType(type);
        switch (serializationTokenType)
        {
          case ProtoBufSerializationTokenType.DataSet:
            obj = DeserializeDataSet(readStream);
            break;
          case ProtoBufSerializationTokenType.DataTable:
            obj = DeserializeDataTable(readStream);
            break;
          case ProtoBufSerializationTokenType.Default:
          default:
            //var len = readStream.Read7BitEncodedInt();
            //if (len <= SIZE_ZERO) { return GetDefaultValueForType(type); }
            //byte[] buffer = null;
            //BufferManagerStreamReader binReader = null;
            //var bufferManager = BufferManager.GlobalManager;
            //try
            //{
            //  buffer = BufferManager.GlobalManager.TakeBuffer(len);
            //  readStream.Read(buffer, 0, len);
            //  binReader = BufferManagerStreamReaderManager.Take();
            //  binReader.Reinitialize(buffer, 0, len, bufferManager);
            //  obj = s_model.Deserialize(binReader, null, type);
            //}
            //finally
            //{
            //  if (binReader != null) { BufferManagerStreamReaderManager.Return(binReader); }
            //  if (buffer != null) { bufferManager.ReturnBuffer(buffer); }
            //}
            obj = s_model.DeserializeWithLengthPrefix(readStream, null, type, PrefixStyle.Base128, 0);
            break;
        }
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.ToString());
        obj = GetDefaultValueForType(type);
      }
      return obj;
    }

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public override void WriteToStream(Type type, object value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      var serializationTokenType = GetSerializationTokenType(type);
      switch (serializationTokenType)
      {
        case ProtoBufSerializationTokenType.DataSet:
          if (null == value)
          {
            writeStream.Write7BitEncodedInt(SIZE_ZERO);
            return;
          }
          SerializeDataSet(value, writeStream);
          break;
        case ProtoBufSerializationTokenType.DataTable:
          if (null == value)
          {
            writeStream.Write7BitEncodedInt(SIZE_ZERO);
            return;
          }
          SerializeDataTable(value, writeStream);
          break;
        case ProtoBufSerializationTokenType.Default:
        default:
          //var output = BufferManagerOutputStreamManager.Take();
          //output.Reinitialize();
          //try
          //{
          //  s_model.Serialize(output, value);
          //  var objStream = output.ToReadOnlyStream();
          //  writeStream.Write7BitEncodedInt((int)objStream.Length);
          //  StreamToStreamCopy.CopyAsync(objStream, writeStream, true).WaitAndUnwrapException();
          //}
          //finally
          //{
          //  BufferManagerOutputStreamManager.Return(output);
          //}
          s_model.SerializeWithLengthPrefix(writeStream, value, type, PrefixStyle.Base128, 0);
          break;
      }
    }

    #endregion

    #region **& DataTable &**

    private static void SerializeDataTable(object obj, BufferManagerOutputStream stream)
    {
      var dt = (DataTable)obj;

      var output = BufferManagerOutputStreamManager.Take();
      output.Reinitialize();
      try
      {
        DataSerializer.Serialize(output, dt);
        var objStream = output.ToReadOnlyStream();
        stream.Write7BitEncodedInt((int)objStream.Length);
        StreamToStreamCopy.CopyAsync(objStream, stream, true).GetAwaiter().GetResult();
      }
      finally
      {
        BufferManagerOutputStreamManager.Return(output);
      }
    }

    private static object DeserializeDataTable(BufferManagerStreamReader stream)
    {
      var len = stream.Read7BitEncodedInt();
      if (0 == len) { return default(DataTable); }

      var bufferManager = BufferManager.GlobalManager;
      object obj;
      byte[] buffer = null;
      BufferManagerStreamReader binReader = null;
      try
      {
        buffer = BufferManager.GlobalManager.TakeBuffer(len);
        stream.Read(buffer, 0, len);
        binReader = BufferManagerStreamReaderManager.Take();
        binReader.Reinitialize(buffer, 0, len, bufferManager);
        obj = DataSerializer.DeserializeDataTable(binReader);
      }
      finally
      {
        if (binReader != null) { BufferManagerStreamReaderManager.Return(binReader); }
        if (buffer != null) { bufferManager.ReturnBuffer(buffer); }
      }
      return obj;
    }

    #endregion
  }
}
