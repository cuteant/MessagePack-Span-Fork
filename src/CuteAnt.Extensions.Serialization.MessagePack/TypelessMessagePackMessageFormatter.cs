using System;
using System.IO;
using System.Text;
using CuteAnt.Buffers;
using CuteAnt.Extensions.Internal;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle wire.</summary>
  public class TypelessMessagePackMessageFormatter : MessageFormatter
  {
    protected static readonly ILogger s_logger = TraceLogger.GetLogger(typeof(TypelessMessagePackMessageFormatter));

    /// <summary>The default singlegton instance</summary>
    public static readonly TypelessMessagePackMessageFormatter DefaultInstance = new TypelessMessagePackMessageFormatter();

    protected readonly IFormatterResolver _typelessResolver;

    /// <summary>Constructor</summary>
    public TypelessMessagePackMessageFormatter() => _typelessResolver = MessagePackStandardResolver.Typeless;

    /// <summary>Constructor</summary>
    public TypelessMessagePackMessageFormatter(IFormatterResolver resolver)
    {
      if (null == resolver) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resolver); }
      _typelessResolver = resolver;
    }

    #region -- IsSupportedType --

    /// <inheritdoc />
    public override bool IsSupportedType(Type type) => true;

    #endregion

    #region -- DeepCopy --

    /// <inheritdoc />
    public sealed override object DeepCopyObject(object source)
    {
      if (source == null) { return null; }

      var serializedObject = MessagePackSerializer.SerializeUnsafe(source, _typelessResolver);
      return MessagePackSerializer.Deserialize<object>(serializedObject, _typelessResolver);
    }

    /// <inheritdoc />
    public sealed override T DeepCopy<T>(T source)
    {
      if (source == null) { return default; }

      var serializedObject = MessagePackSerializer.SerializeUnsafe<object>(source, _typelessResolver);
      return (T)MessagePackSerializer.Deserialize<object>(serializedObject, _typelessResolver);
    }

    #endregion

    #region -- Deserialize --

    /// <inheritdoc />
    public override T Deserialize<T>(byte[] serializedObject)
    {
      try
      {
        return (T)MessagePackSerializer.Deserialize<object>(serializedObject, _typelessResolver);
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
        return (T)MessagePackSerializer.Deserialize<object>(serializedObject, _typelessResolver);
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
        return (T)MessagePackSerializer.Deserialize<object>(serializedObject, offset, count, _typelessResolver);
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
        return MessagePackSerializer.Deserialize<object>(serializedObject, _typelessResolver);
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
        return MessagePackSerializer.Deserialize<object>(serializedObject, _typelessResolver);
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
        return MessagePackSerializer.Deserialize<object>(serializedObject, offset, count, _typelessResolver);
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
      if (null == readStream) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.readStream); }

      try
      {
        return (T)MessagePackSerializer.Deserialize<object>(readStream, _typelessResolver, false);
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
      if (null == readStream) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.readStream); }

      // 不是 Stream 都会实现 Position、Length 这两个属性
      //if (readStream.Position == readStream.Length) { return GetDefaultValueForType(type); }

      try
      {
        return MessagePackSerializer.Deserialize<object>(readStream, _typelessResolver, false);
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
      return MessagePackSerializer.Serialize<object>(item, _typelessResolver);
    }

    /// <inheritdoc />
    public override byte[] Serialize<T>(T item, int initialBufferSize)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return MessagePackSerializer.Serialize<object>(item, _typelessResolver);
    }

    #endregion

    #region -- SerializeObject --

    /// <inheritdoc />
    public override byte[] SerializeObject(object item)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return MessagePackSerializer.Serialize(item, _typelessResolver);
    }

    /// <inheritdoc />
    public override byte[] SerializeObject(object item, int initialBufferSize)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return MessagePackSerializer.Serialize(item, _typelessResolver);
    }

    #endregion

    #region -- WriteToMemoryPool --

    public override ArraySegment<byte> WriteToMemoryPool<T>(T item)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = MessagePackSerializer.SerializeUnsafe<object>(item, _typelessResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool<T>(T item, int initialBufferSize)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = MessagePackSerializer.SerializeUnsafe<object>(item, _typelessResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool(object item)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = MessagePackSerializer.SerializeUnsafe(item, _typelessResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool(object item, int initialBufferSize)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = MessagePackSerializer.SerializeUnsafe(item, _typelessResolver);
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

      if (null == writeStream) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.writeStream); }

      MessagePackSerializer.Serialize<object>(writeStream, value, _typelessResolver);
    }

    /// <inheritdoc />
    public override void WriteToStream(object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (null == writeStream) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.writeStream); }

      MessagePackSerializer.Serialize(writeStream, value, _typelessResolver);
    }

    /// <inheritdoc />
    public override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (null == writeStream) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.writeStream); }

      MessagePackSerializer.Serialize(writeStream, value, _typelessResolver);
    }

    #endregion
  }
}
