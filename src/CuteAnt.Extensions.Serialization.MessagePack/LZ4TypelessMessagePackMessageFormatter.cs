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
  public sealed class LZ4TypelessMessagePackMessageFormatter : TypelessMessagePackMessageFormatter
  {
    /// <summary>The default singlegton instance</summary>
    public new static readonly LZ4TypelessMessagePackMessageFormatter DefaultInstance = new LZ4TypelessMessagePackMessageFormatter();

    /// <summary>Constructor</summary>
    public LZ4TypelessMessagePackMessageFormatter() { }

    #region -- Deserialize --

    /// <inheritdoc />
    public sealed override T Deserialize<T>(byte[] serializedObject)
    {
      try
      {
        return (T)LZ4MessagePackSerializer.Deserialize<object>(new ArraySegment<byte>(serializedObject, 0, serializedObject.Length), s_typelessResolver);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return default;
      }
    }
    /// <inheritdoc />
    public sealed override T Deserialize<T>(in ArraySegment<byte> serializedObject)
    {
      try
      {
        return (T)LZ4MessagePackSerializer.Deserialize<object>(serializedObject, s_typelessResolver);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return default;
      }
    }
    /// <inheritdoc />
    public sealed override T Deserialize<T>(byte[] serializedObject, int offset, int count)
    {
      try
      {
        return (T)LZ4MessagePackSerializer.Deserialize<object>(new ArraySegment<byte>(serializedObject, offset, count), s_typelessResolver);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return default;
      }
    }
    /// <inheritdoc />
    public sealed override object Deserialize(Type type, byte[] serializedObject)
    {
      try
      {
        return LZ4MessagePackSerializer.Deserialize<object>(new ArraySegment<byte>(serializedObject, 0, serializedObject.Length), s_typelessResolver);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return GetDefaultValueForType(type);
      }
    }
    /// <inheritdoc />
    public sealed override object Deserialize(Type type, in ArraySegment<byte> serializedObject)
    {
      try
      {
        return LZ4MessagePackSerializer.Deserialize<object>(serializedObject, s_typelessResolver);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return GetDefaultValueForType(type);
      }
    }
    /// <inheritdoc />
    public sealed override object Deserialize(Type type, byte[] serializedObject, int offset, int count)
    {
      try
      {
        return LZ4MessagePackSerializer.Deserialize<object>(new ArraySegment<byte>(serializedObject, offset, count), s_typelessResolver);
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
    public sealed override T ReadFromStream<T>(Stream readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      try
      {
        return (T)LZ4MessagePackSerializer.Deserialize<object>(readStream, s_typelessResolver, false);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return default;
      }
    }

    /// <inheritdoc />
    public sealed override object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      // 不是 Stream 都会实现 Position、Length 这两个属性
      //if (readStream.Position == readStream.Length) { return GetDefaultValueForType(type); }

      try
      {
        return LZ4MessagePackSerializer.Deserialize<object>(readStream, s_typelessResolver, false);
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
    public sealed override byte[] Serialize<T>(T item)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return LZ4MessagePackSerializer.Serialize<object>(item, s_typelessResolver);
    }

    /// <inheritdoc />
    public sealed override byte[] Serialize<T>(T item, int initialBufferSize)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return LZ4MessagePackSerializer.Serialize<object>(item, s_typelessResolver);
    }

    #endregion

    #region -- SerializeObject --

    /// <inheritdoc />
    public sealed override byte[] SerializeObject(object item)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return LZ4MessagePackSerializer.Serialize(item, s_typelessResolver);
    }

    /// <inheritdoc />
    public sealed override byte[] SerializeObject(object item, int initialBufferSize)
    {
      if (null == item) { return EmptyArray<byte>.Instance; }
      return LZ4MessagePackSerializer.Serialize(item, s_typelessResolver);
    }

    #endregion

    #region -- WriteToMemoryPool --

    public sealed override ArraySegment<byte> WriteToMemoryPool<T>(T item)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = LZ4MessagePackSerializer.SerializeCore<object>(item, s_typelessResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public sealed override ArraySegment<byte> WriteToMemoryPool<T>(T item, int initialBufferSize)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = LZ4MessagePackSerializer.SerializeCore<object>(item, s_typelessResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public sealed override ArraySegment<byte> WriteToMemoryPool(object item)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = LZ4MessagePackSerializer.SerializeCore(item, s_typelessResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public sealed override ArraySegment<byte> WriteToMemoryPool(object item, int initialBufferSize)
    {
      if (null == item) { return BufferManager.Empty; }
      var serializedObject = LZ4MessagePackSerializer.SerializeCore(item, s_typelessResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public sealed override void WriteToStream<T>(T value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      LZ4MessagePackSerializer.Serialize<object>(writeStream, value, s_typelessResolver);
    }

    /// <inheritdoc />
    public sealed override void WriteToStream(object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      LZ4MessagePackSerializer.Serialize(writeStream, value, s_typelessResolver);
    }

    /// <inheritdoc />
    public sealed override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      LZ4MessagePackSerializer.Serialize(writeStream, value, s_typelessResolver);
    }

    #endregion
  }
}
