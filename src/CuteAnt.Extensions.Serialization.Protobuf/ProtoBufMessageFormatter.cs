using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    protected static readonly ILogger s_logger = TraceLogger.GetLogger(typeof(ProtoBufMessageFormatter));

    #region @@ Properties @@

    internal static readonly RuntimeTypeModel s_model;
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
    public override object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      if (readStream.Position == readStream.Length) { return GetDefaultValueForType(type); }

      try
      {
        return s_model.Deserialize(readStream, null, type, null);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return GetDefaultValueForType(type);
      }
    }

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      if (null == value) { return; }

      s_model.Serialize(writeStream, value, null);
    }

    #endregion
  }
}
