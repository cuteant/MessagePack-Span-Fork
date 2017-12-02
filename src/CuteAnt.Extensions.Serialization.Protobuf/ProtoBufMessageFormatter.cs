using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using CuteAnt.Buffers;
using CuteAnt.Extensions.Serialization.Protobuf;
using CuteAnt.Runtime;
using CuteAnt.Text;
using Microsoft.Extensions.Logging;
using ProtoBuf.Meta;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle protobuf-net.</summary>
  public partial class ProtoBufMessageFormatter : MessageFormatter
  {
    #region @@ Properties @@

    internal static readonly RuntimeTypeModel s_model;
    internal static readonly Dictionary<Type, ProtoBufSerializationTokenType> s_typeInfoDictionary =
      new Dictionary<Type, ProtoBufSerializationTokenType>();
    private static readonly ConcurrentHashSet<Type> s_unsupportedTypeInfoSet = new ConcurrentHashSet<Type>();

    /// <summary>The default singlegton instance</summary>
    public static readonly ProtoBufMessageFormatter DefaultInstance = new ProtoBufMessageFormatter();

    /// <summary>Type Model</summary>
    public static RuntimeTypeModel Model => s_model;

    #endregion

    #region -- Constructors --

    static ProtoBufMessageFormatter()
    {
      s_model = RuntimeTypeModel.Default;
      // 考虑到类的继承问题，禁用 [DataContract] / [XmlType]
      s_model.AutoAddProtoContractTypesOnly = true;

      s_typeInfoDictionary[typeof(DataSet)] = ProtoBufSerializationTokenType.DataSet;
      s_typeInfoDictionary[typeof(DataTable)] = ProtoBufSerializationTokenType.DataTable;
    }

    /// <summary>Constructor</summary>
    public ProtoBufMessageFormatter()
    {
    }

    #endregion

    #region -- IsSupportedType --

    /// <inheritdoc />
    public override bool IsSupportedType(Type type)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }

      if (s_typeInfoDictionary.ContainsKey(type)) { return true; }
      if (s_unsupportedTypeInfoSet.Contains(type)) { return false; }

#if NET40
      if (type.IsGenericType && type.IsConstructedGenericType() == false) { s_unsupportedTypeInfoSet.TryAdd(type); return false; }
#else
      if (type.IsGenericType && type.IsConstructedGenericType == false) { s_unsupportedTypeInfoSet.TryAdd(type); return false; }
#endif
      if (!s_model.CanSerialize(type))
      {
        //if (bforced)
        //{
        //  Build(type);
        //}
        //else
        //{
        s_unsupportedTypeInfoSet.TryAdd(type); return false;
        //}
      }

      return true;
    }

    #endregion

    #region -- DeepCopy --

    public override object DeepCopy(object source) => s_model.DeepClone(source);

    #endregion

    #region -- ReadFromStream --

    /// <inheritdoc />
    public override object ReadFromStream(Type type, BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      if (readStream.Position == readStream.Length) { return GetDefaultValueForType(type); }

      try
      {
        var serializationTokenType = GetSerializationTokenType(type);
        switch (serializationTokenType)
        {
          case ProtoBufSerializationTokenType.DataSet:
            return DeserializeDataSet(readStream);
          case ProtoBufSerializationTokenType.DataTable:
            return DataSerializer.DeserializeDataTable(readStream);
          case ProtoBufSerializationTokenType.Default:
          default:
            return s_model.Deserialize(readStream, null, type, null);
        }
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.ToString());
        return GetDefaultValueForType(type);
      }
    }

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public override void WriteToStream(Type type, object value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      if (null == value) { return; }

      var serializationTokenType = GetSerializationTokenType(type);
      switch (serializationTokenType)
      {
        case ProtoBufSerializationTokenType.DataSet:
          SerializeDataSet(value, writeStream);
          break;
        case ProtoBufSerializationTokenType.DataTable:
          DataSerializer.Serialize(writeStream, (DataTable)value);
          break;
        case ProtoBufSerializationTokenType.Default:
        default:
          s_model.Serialize(writeStream, value, null);
          break;
      }
    }

    #endregion

    #region ==& GetSerializationTokenType &==

#if !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    internal static ProtoBufSerializationTokenType GetSerializationTokenType(Type type)
    {
      if (s_typeInfoDictionary.TryGetValue(type, out ProtoBufSerializationTokenType serializationTokenType))
      {
        return serializationTokenType;
      }
      return ProtoBufSerializationTokenType.Default;
    }

    #endregion

    #region ==& DataSet &==

    internal static void SerializeDataSet(object obj, BufferManagerOutputStream stream)
    {
      var ds = (DataSet)obj;

      if (0 == ds.Tables.Count)
      {
        stream.Write7BitEncodedInt(0);
        return;
      }

      var output = BufferManagerOutputStreamManager.Take();
      output.Reinitialize();
      try
      {
        DataSerializer.Serialize(output, ds);
        var objStream = output.ToReadOnlyStream();
        stream.Write7BitEncodedInt((int)objStream.Length);
        var sb = StringBuilderCache.Acquire();
        for (int idx = 0; idx < ds.Tables.Count; idx++)
        {
          if (sb.Length > 0) { sb.Append(","); }
          sb.Append(ds.Tables[idx].TableName);
        }
        stream.WriteValue(StringBuilderCache.GetStringAndRelease(sb));
        StreamToStreamCopy.CopyAsync(objStream, stream, true).GetAwaiter().GetResult();
      }
      finally
      {
        BufferManagerOutputStreamManager.Return(output);
      }
    }

    internal static object DeserializeDataSet(BufferManagerStreamReader stream)
    {
      var len = stream.Read7BitEncodedInt();
      if (0 == len) { return default(DataSet); }

      var tbNameStr = stream.ReadString();
      var tbNames = tbNameStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
        obj = DataSerializer.DeserializeDataSet(binReader, tbNames);
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
