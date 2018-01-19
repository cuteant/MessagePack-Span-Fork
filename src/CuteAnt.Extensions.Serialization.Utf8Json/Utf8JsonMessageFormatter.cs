using System;
using System.IO;
using System.Text;
using CuteAnt.Buffers;
using CuteAnt.Extensions.Internal;
using Utf8Json;
using Microsoft.Extensions.Logging;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle wire.</summary>
  public class Utf8JsonMessageFormatter : MessageFormatter
  {
    protected static readonly ILogger s_logger = TraceLogger.GetLogger(typeof(Utf8JsonMessageFormatter));

    /// <summary>The default singlegton instance</summary>
    public static readonly Utf8JsonMessageFormatter DefaultInstance = new Utf8JsonMessageFormatter();

    private readonly IJsonFormatterResolver _defaultResolver = Utf8JsonStandardResolver.Default;
    public IJsonFormatterResolver DefaultResolver => _defaultResolver;

    /// <summary>Constructor</summary>
    public Utf8JsonMessageFormatter() { }

    /// <summary>Constructor</summary>
    public Utf8JsonMessageFormatter(IJsonFormatterResolver resolver)
    {
      _defaultResolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
    }

    #region -- IsSupportedType --

    /// <inheritdoc />
    public override bool IsSupportedType(Type type) => true;

    #endregion

    #region -- DeepCopy --

    /// <inheritdoc />
    public override object DeepCopyObject(object source)
    {
      if (source == null) { return null; }

      var type = source.GetType();
      var serializedObject = JsonSerializer.NonGeneric.SerializeUnsafe(type, source, _defaultResolver);
      return JsonSerializer.NonGeneric.Deserialize(type, serializedObject.Array, serializedObject.Offset, _defaultResolver);
    }

    /// <inheritdoc />
    public override T DeepCopy<T>(T source)
    {
      if (source == null) { return default; }

      var serializedObject = JsonSerializer.SerializeUnsafe<T>(source, _defaultResolver);
      return JsonSerializer.Deserialize<T>(serializedObject.Array, serializedObject.Offset, _defaultResolver);
    }

    #endregion

    #region -- Deserialize --

    /// <inheritdoc />
    public override T Deserialize<T>(byte[] serializedObject)
    {
      try
      {
        return JsonSerializer.Deserialize<T>(serializedObject, 0, _defaultResolver);
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
        return JsonSerializer.Deserialize<T>(serializedObject.Array, serializedObject.Offset, _defaultResolver);
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
        return JsonSerializer.Deserialize<T>(serializedObject, offset, _defaultResolver);
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
        return JsonSerializer.NonGeneric.Deserialize(type, serializedObject, 0, _defaultResolver);
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
        return JsonSerializer.NonGeneric.Deserialize(type, serializedObject.Array, serializedObject.Offset, _defaultResolver);
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
        return JsonSerializer.NonGeneric.Deserialize(type, serializedObject, offset, _defaultResolver);
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
        return JsonSerializer.Deserialize<T>(readStream, _defaultResolver);
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
        return JsonSerializer.NonGeneric.Deserialize(type, readStream, _defaultResolver);
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
     * 参考 CuteAnt.Extensions.Serialization.ExtensionsTests.Utf8JsonTests
     */

    #region -- Serialize --

    public override byte[] Serialize<T>(T item)
    {
      return JsonSerializer.Serialize<object>(item, _defaultResolver);
    }

    public override byte[] Serialize<T>(T item, int initialBufferSize)
    {
      return JsonSerializer.Serialize<object>(item, _defaultResolver);
    }

    #endregion

    #region -- SerializeObject --

    /// <inheritdoc />
    public override byte[] SerializeObject(object item)
    {
      return JsonSerializer.Serialize(item, _defaultResolver);
    }

    /// <inheritdoc />
    public override byte[] SerializeObject(object item, int initialBufferSize)
    {
      return JsonSerializer.Serialize(item, _defaultResolver);
    }

    #endregion

    #region -- WriteToMemoryPool --

    public override ArraySegment<byte> WriteToMemoryPool<T>(T item)
    {
      var serializedObject = JsonSerializer.SerializeUnsafe<object>(item, _defaultResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool<T>(T item, int initialBufferSize)
    {
      var serializedObject = JsonSerializer.SerializeUnsafe<object>(item, _defaultResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool(object item)
    {
      var serializedObject = JsonSerializer.SerializeUnsafe(item, _defaultResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool(object item, int initialBufferSize)
    {
      var serializedObject = JsonSerializer.SerializeUnsafe(item, _defaultResolver);
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

      JsonSerializer.Serialize<object>(writeStream, value, _defaultResolver);
    }

    /// <inheritdoc />
    public override void WriteToStream(object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      JsonSerializer.Serialize(writeStream, value, _defaultResolver);
    }

    /// <inheritdoc />
    public override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      JsonSerializer.Serialize(writeStream, value, _defaultResolver);
    }

    #endregion
  }
}
