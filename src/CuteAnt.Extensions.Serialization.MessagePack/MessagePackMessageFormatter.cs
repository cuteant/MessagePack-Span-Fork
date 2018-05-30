using System;
using System.IO;
using System.Text;
using CuteAnt.Buffers;
using CuteAnt.Extensions.Internal;
using MessagePack;
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

    protected readonly IFormatterResolver _defaultResolver;

    /// <summary>Constructor</summary>
    public MessagePackMessageFormatter() => _defaultResolver = MessagePackStandardResolver.Default;

    /// <summary>Constructor</summary>
    public MessagePackMessageFormatter(IFormatterResolver resolver) => _defaultResolver = resolver ?? throw new ArgumentNullException(nameof(resolver));

    #region -- IsSupportedType --

    /// <inheritdoc />
    public override bool IsSupportedType(Type type) => true;

    #endregion

    #region -- DeepCopy --

    /// <inheritdoc />
    public sealed override object DeepCopyObject(object source)
    {
      if (source == null) { return null; }

      var type = source.GetType();
      var serializedObject = MessagePackSerializer.SerializeUnsafe(source, _defaultResolver);
      return MessagePackSerializer.NonGeneric.Deserialize(type, serializedObject, _defaultResolver);
    }

    /// <inheritdoc />
    public sealed override T DeepCopy<T>(T source)
    {
      if (source == null) { return default; }

      var type = source.GetType(); // 要获取对象本身的类型，忽略基类、接口
      var serializedObject = MessagePackSerializer.SerializeUnsafe<object>(source, _defaultResolver);
      return (T)MessagePackSerializer.NonGeneric.Deserialize(type, serializedObject, _defaultResolver);
    }

    #endregion

    #region -- Deserialize --

    /// <inheritdoc />
    public override T Deserialize<T>(byte[] serializedObject)
    {
      try
      {
        return MessagePackSerializer.Deserialize<T>(serializedObject, _defaultResolver);
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
        return MessagePackSerializer.Deserialize<T>(serializedObject, _defaultResolver);
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
        return MessagePackSerializer.Deserialize<T>(serializedObject, offset, count, _defaultResolver);
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
        return MessagePackSerializer.NonGeneric.Deserialize(type, serializedObject, _defaultResolver);
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
        return MessagePackSerializer.NonGeneric.Deserialize(type, serializedObject, _defaultResolver);
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
        return MessagePackSerializer.NonGeneric.Deserialize(type, new ArraySegment<byte>(serializedObject, offset, count), _defaultResolver);
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
        return MessagePackSerializer.Deserialize<T>(readStream, _defaultResolver, false);
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
        return MessagePackSerializer.NonGeneric.Deserialize(type, readStream, _defaultResolver, false);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return GetDefaultValueForType(type);
      }
    }

    #endregion

    /*
     * 这里序列化时需要忽略接口、基类，直接序列化类型本身
     * 要不然会造成反序列化的紊乱
     * 参考 CuteAnt.Extensions.Serialization.ExtensionsTests.MessagePackTests
     */

    #region -- Serialize --

    /// <inheritdoc />
    public override byte[] Serialize<T>(T item)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return MessagePackSerializer.Serialize<object>(item, _defaultResolver);
    }

    /// <inheritdoc />
    public override byte[] Serialize<T>(T item, int initialBufferSize)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return MessagePackSerializer.Serialize<object>(item, _defaultResolver);
    }

    #endregion

    #region -- SerializeObject --

    /// <inheritdoc />
    public override byte[] SerializeObject(object item)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return MessagePackSerializer.Serialize(item, _defaultResolver);
    }

    /// <inheritdoc />
    public override byte[] SerializeObject(object item, int initialBufferSize)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return MessagePackSerializer.Serialize(item, _defaultResolver);
    }

    #endregion

    #region -- WriteToMemoryPool --

    public override ArraySegment<byte> WriteToMemoryPool<T>(T item)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = MessagePackSerializer.SerializeUnsafe<object>(item, _defaultResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool<T>(T item, int initialBufferSize)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = MessagePackSerializer.SerializeUnsafe<object>(item, _defaultResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool(object item)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = MessagePackSerializer.SerializeUnsafe(item, _defaultResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool(object item, int initialBufferSize)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = MessagePackSerializer.SerializeUnsafe(item, _defaultResolver);
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

      MessagePackSerializer.Serialize<object>(writeStream, value, _defaultResolver);
    }

    /// <inheritdoc />
    public override void WriteToStream(object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      MessagePackSerializer.Serialize(writeStream, value, _defaultResolver);
    }

    /// <inheritdoc />
    public override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      MessagePackSerializer.Serialize(writeStream, value, _defaultResolver);
    }

    #endregion
  }
}
