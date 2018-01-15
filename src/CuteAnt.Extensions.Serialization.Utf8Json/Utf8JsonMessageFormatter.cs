using System;
using System.IO;
using System.Text;
using CuteAnt.Buffers;
using CuteAnt.Extensions.Internal;
using Utf8Json;
using Utf8Json.Resolvers;
using Microsoft.Extensions.Logging;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle wire.</summary>
  public class Utf8JsonMessageFormatter : MessageFormatter
  {
    protected static readonly ILogger s_logger = TraceLogger.GetLogger(typeof(Utf8JsonMessageFormatter));

    /// <summary>The default singlegton instance</summary>
    public static readonly Utf8JsonMessageFormatter DefaultInstance = new Utf8JsonMessageFormatter();

    private static IJsonFormatterResolver s_defaultResolver = StandardResolver.Default;
    protected static IJsonFormatterResolver DefaultResolver => s_defaultResolver;

    /// <summary>Constructor</summary>
    public Utf8JsonMessageFormatter() { }

    #region -- Register --

    public static void Register(IJsonFormatter[] formatters, IJsonFormatterResolver[] resolvers)
    {
      if ((null == formatters || formatters.Length == 0) && (null == resolvers || resolvers.Length == 0)) { return; }

      if (formatters != null && formatters.Length > 0) { CompositeResolver.Register(formatters); }
      if (resolvers != null && resolvers.Length > 0) { CompositeResolver.Register(resolvers); }

      s_defaultResolver = CompositeResolver.Instance;
      JsonSerializer.SetDefaultResolver(s_defaultResolver);
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

      var type = source.GetType();
      var serializedObject = JsonSerializer.NonGeneric.SerializeUnsafe(type, source, s_defaultResolver);
      return JsonSerializer.NonGeneric.Deserialize(type, serializedObject.Array, serializedObject.Offset, s_defaultResolver);
    }

    /// <inheritdoc />
    public override T DeepCopy<T>(T source)
    {
      if (source == null) { return default; }

      var serializedObject = JsonSerializer.SerializeUnsafe<T>(source, s_defaultResolver);
      return JsonSerializer.Deserialize<T>(serializedObject.Array, serializedObject.Offset, s_defaultResolver);
    }

    #endregion

    #region -- Deserialize --

    /// <inheritdoc />
    public override T Deserialize<T>(byte[] serializedObject)
    {
      try
      {
        return JsonSerializer.Deserialize<T>(serializedObject, 0, s_defaultResolver);
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
        return JsonSerializer.Deserialize<T>(serializedObject.Array, serializedObject.Offset, s_defaultResolver);
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
        return JsonSerializer.Deserialize<T>(serializedObject, offset, s_defaultResolver);
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
        return JsonSerializer.NonGeneric.Deserialize(type, serializedObject, 0, s_defaultResolver);
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
        return JsonSerializer.NonGeneric.Deserialize(type, serializedObject.Array, serializedObject.Offset, s_defaultResolver);
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
        return JsonSerializer.NonGeneric.Deserialize(type, serializedObject, offset, s_defaultResolver);
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
        return JsonSerializer.Deserialize<T>(readStream, s_defaultResolver);
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
        return JsonSerializer.NonGeneric.Deserialize(type, readStream, s_defaultResolver);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return GetDefaultValueForType(type);
      }
    }

    #endregion

    #region -- Serialize --

    public override byte[] Serialize<T>(T item)
    {
      return JsonSerializer.Serialize(item, s_defaultResolver);
    }

    public override byte[] Serialize<T>(T item, int initialBufferSize)
    {
      return JsonSerializer.Serialize(item, s_defaultResolver);
    }

    #endregion

    #region -- SerializeObject --

    /// <inheritdoc />
    public override byte[] SerializeObject(object item)
    {
      return JsonSerializer.NonGeneric.Serialize(item, s_defaultResolver);
    }

    /// <inheritdoc />
    public override byte[] SerializeObject(object item, int initialBufferSize)
    {
      return JsonSerializer.NonGeneric.Serialize(item, s_defaultResolver);
    }

    #endregion

    #region -- WriteToMemoryPool --

    public override ArraySegment<byte> WriteToMemoryPool<T>(T item)
    {
      var serializedObject = JsonSerializer.SerializeUnsafe(item, s_defaultResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool<T>(T item, int initialBufferSize)
    {
      var serializedObject = JsonSerializer.SerializeUnsafe(item, s_defaultResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool(object item)
    {
      var serializedObject = JsonSerializer.NonGeneric.SerializeUnsafe(item, s_defaultResolver);
      var length = serializedObject.Count;
      var buffer = BufferManager.Shared.Rent(length);
      PlatformDependent.CopyMemory(serializedObject.Array, serializedObject.Offset, buffer, 0, length);
      return new ArraySegment<byte>(buffer, 0, length);
    }

    public override ArraySegment<byte> WriteToMemoryPool(object item, int initialBufferSize)
    {
      var serializedObject = JsonSerializer.NonGeneric.SerializeUnsafe(item, s_defaultResolver);
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

      JsonSerializer.Serialize(writeStream, value, s_defaultResolver);
    }

    /// <inheritdoc />
    public override void WriteToStream(object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      JsonSerializer.NonGeneric.Serialize(value.GetType(), writeStream, value, s_defaultResolver);
    }

    /// <inheritdoc />
    public override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      JsonSerializer.NonGeneric.Serialize(type ?? value.GetType(), writeStream, value, s_defaultResolver);
    }

    #endregion
  }
}
