using System;
using System.IO;
using System.Text;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Microsoft.Extensions.Logging;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle wire.</summary>
  public class MessagePackMessageFormatter : MessageFormatter
  {
    protected static readonly ILogger s_logger = TraceLogger.GetLogger(typeof(MessagePackMessageFormatter));

    /// <summary>The default singlegton instance</summary>
    public static readonly MessagePackMessageFormatter DefaultInstance = new MessagePackMessageFormatter();

    private static IFormatterResolver s_defaultResolver = StandardResolver.Instance;
    protected static IFormatterResolver DefaultResolver => s_defaultResolver;
    private static IFormatterResolver s_typelessResolver = TypelessContractlessStandardResolver.Instance;
    protected static IFormatterResolver TypelessResolver => s_typelessResolver;

    /// <summary>Constructor</summary>
    public MessagePackMessageFormatter() { }

    #region -- Register --

    public static void Register(IMessagePackFormatter[] formatters, IFormatterResolver[] resolvers)
    {
      if ((null == formatters || formatters.Length == 0) && (null == resolvers || resolvers.Length == 0)) { return; }

      if (formatters != null && formatters.Length > 0) { CompositeResolver.Register(formatters); }
      if (resolvers != null && resolvers.Length > 0) { CompositeResolver.Register(resolvers); }

      s_defaultResolver = CompositeResolver.Instance;
      MessagePackSerializer.SetDefaultResolver(s_defaultResolver);
    }

    public static void RegisterTypeless(IMessagePackFormatter[] formatters, IFormatterResolver[] resolvers)
    {
      if ((null == formatters || formatters.Length == 0) && (null == resolvers || resolvers.Length == 0)) { return; }

      if (formatters != null && formatters.Length > 0) { TypelessCompositeResolver.Register(formatters); }
      if (resolvers != null && resolvers.Length > 0) { TypelessCompositeResolver.Register(resolvers); }

      s_typelessResolver = TypelessCompositeResolver.Instance;
      MessagePackSerializer.Typeless.RegisterDefaultResolver(s_typelessResolver);
    }

    #endregion

    #region -- IsSupportedType --

    /// <inheritdoc />
    public override bool IsSupportedType(Type type) => true;

    #endregion

    #region -- DeepCopy --

    /// <inheritdoc />
    public override object DeepCopyObject(object source)
    {
      if (source == null) { return null; }

      var serializedObject = MessagePackSerializer.SerializeUnsafe<object>(source, s_typelessResolver);
      return MessagePackSerializer.Deserialize<object>(serializedObject, s_typelessResolver);
    }

    /// <inheritdoc />
    public override T DeepCopy<T>(T source)
    {
      if (source == null) { return default; }

      var serializedObject = MessagePackSerializer.SerializeUnsafe<T>(source, s_defaultResolver);
      return MessagePackSerializer.Deserialize<T>(serializedObject, s_defaultResolver);
    }

    #endregion

    #region -- Deserialize --

    /// <inheritdoc />
    public override T Deserialize<T>(byte[] serializedObject)
    {
      try
      {
        return (T)MessagePackSerializer.Deserialize<object>(serializedObject, s_typelessResolver);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return default;
      }
    }
    /// <inheritdoc />
    public override T Deserialize<T>(byte[] serializedObject, int offset, int count)
    {
      try
      {
        return (T)MessagePackSerializer.Deserialize<object>(serializedObject, offset, count, s_typelessResolver);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return default;
      }
    }
    /// <inheritdoc />
    public override object Deserialize(Type type, byte[] serializedObject)
    {
      try
      {
        return MessagePackSerializer.Deserialize<object>(serializedObject, s_typelessResolver);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return default;
      }
    }
    /// <inheritdoc />
    public override object Deserialize(Type type, byte[] serializedObject, int offset, int count)
    {
      try
      {
        return MessagePackSerializer.Deserialize<object>(serializedObject, offset, count, s_typelessResolver);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return default;
      }
    }

    #endregion

    #region -- ReadFromStream --

    /// <inheritdoc />
    public override T ReadFromStream<T>(Stream readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      try
      {
        return (T)MessagePackSerializer.Deserialize<object>(readStream, s_typelessResolver, false);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return default;
      }
    }

    /// <inheritdoc />
    public override object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      // 不是 Stream 都会实现 Position、Length 这两个属性
      //if (readStream.Position == readStream.Length) { return GetDefaultValueForType(type); }

      try
      {
        return MessagePackSerializer.Deserialize<object>(readStream, s_typelessResolver, false);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return GetDefaultValueForType(type);
      }
    }

    #endregion

    #region -- Serialize --

    /// <inheritdoc />
    public override byte[] Serialize<T>(T item)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return MessagePackSerializer.Serialize<object>(item, s_typelessResolver);
    }

    /// <inheritdoc />
    public override byte[] Serialize<T>(T item, int initialBufferSize)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return MessagePackSerializer.Serialize<object>(item, s_typelessResolver);
    }

    #endregion

    #region -- SerializeObject --

    /// <inheritdoc />
    public override byte[] SerializeObject(object item)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return MessagePackSerializer.Serialize<object>(item, s_typelessResolver);
    }

    /// <inheritdoc />
    public override byte[] SerializeObject(object item, int initialBufferSize)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return MessagePackSerializer.Serialize<object>(item, s_typelessResolver);
    }

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public override void WriteToStream<T>(T value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      MessagePackSerializer.Serialize<object>(writeStream, value, s_typelessResolver);
    }

    /// <inheritdoc />
    public override void WriteToStream(object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      MessagePackSerializer.Serialize(writeStream, value, s_typelessResolver);
    }

    /// <inheritdoc />
    public override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      MessagePackSerializer.Serialize(writeStream, value, s_typelessResolver);
    }

    #endregion
  }
}
