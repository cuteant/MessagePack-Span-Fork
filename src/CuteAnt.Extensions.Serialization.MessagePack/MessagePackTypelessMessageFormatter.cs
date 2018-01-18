using System;
using System.IO;
using System.Text;
using CuteAnt.Buffers;
using CuteAnt.Extensions.Internal;
using CuteAnt.Extensions.Serialization.Internal;
using MessagePack;
using MessagePack.Formatters;
using Microsoft.Extensions.Logging;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle wire.</summary>
  public class MessagePackTypelessMessageFormatter : MessageFormatter
  {
    protected static readonly ILogger s_logger = TraceLogger.GetLogger(typeof(MessagePackTypelessMessageFormatter));

    /// <summary>The default singlegton instance</summary>
    public static readonly MessagePackTypelessMessageFormatter DefaultInstance = new MessagePackTypelessMessageFormatter();

    internal static IFormatterResolver s_typelessResolver = TypelessDefaultResolver.Instance;
    public static IFormatterResolver CurrentResolver => s_typelessResolver;

    /// <summary>Constructor</summary>
    public MessagePackTypelessMessageFormatter() { }

    #region -- Register --

    public static void Register(params IMessagePackFormatter[] formatters) => Register(formatters, null);
    public static void Register(params IFormatterResolver[] resolvers) => Register(null, resolvers);
    public static void Register(IMessagePackFormatter[] formatters, IFormatterResolver[] resolvers)
    {
      if ((null == formatters || formatters.Length == 0) && (null == resolvers || resolvers.Length == 0)) { return; }

      if (formatters != null && formatters.Length > 0) { TypelessDefaultResolver.Register(formatters); }
      if (resolvers != null && resolvers.Length > 0) { TypelessDefaultResolver.Register(resolvers); }
    }

    #endregion

    #region -- IsSupportedType --

    /// <inheritdoc />
    public override bool IsSupportedType(Type type) => true;

    #endregion

    #region -- DeepCopy --

    /// <inheritdoc />
    public sealed override object DeepCopyObject(object source)
    {
      if (source == null) { return null; }

      var serializedObject = MessagePackSerializer.SerializeUnsafe<object>(source, s_typelessResolver);
      return MessagePackSerializer.Deserialize<object>(serializedObject, s_typelessResolver);
    }

    /// <inheritdoc />
    public sealed override T DeepCopy<T>(T source)
    {
      if (source == null) { return default; }

      var serializedObject = MessagePackSerializer.SerializeUnsafe<T>(source, s_typelessResolver);
      return MessagePackSerializer.Deserialize<T>(serializedObject, s_typelessResolver);
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
    public override T Deserialize<T>(in ArraySegment<byte> serializedObject)
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
        return GetDefaultValueForType(type);
      }
    }
    /// <inheritdoc />
    public override object Deserialize(Type type, in ArraySegment<byte> serializedObject)
    {
      try
      {
        return MessagePackSerializer.Deserialize<object>(serializedObject, s_typelessResolver);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return GetDefaultValueForType(type);
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
        return GetDefaultValueForType(type);
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

    #region -- WriteToMemoryPool --

    public override ArraySegment<byte> WriteToMemoryPool<T>(T item)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = MessagePackSerializer.SerializeUnsafe<object>(item, s_typelessResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool<T>(T item, int initialBufferSize)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = MessagePackSerializer.SerializeUnsafe<object>(item, s_typelessResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool(object item)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = MessagePackSerializer.SerializeUnsafe<object>(item, s_typelessResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool(object item, int initialBufferSize)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = MessagePackSerializer.SerializeUnsafe<object>(item, s_typelessResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
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
