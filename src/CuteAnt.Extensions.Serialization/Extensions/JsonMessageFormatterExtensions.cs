using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using CuteAnt.AsyncEx;
using CuteAnt.Buffers;
using CuteAnt.Text;
using Newtonsoft.Json;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>MessageFormatterExtensions</summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static partial class JsonMessageFormatterExtensions
  {
    private const int c_defaultBufferSize = 1024 * 2;

    /// <summary>Serializes the specified item.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static byte[] SerializeToBytes(this IJsonMessageFormatter formatter, object item,
      Encoding effectiveEncoding, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      return SerializeToBytes(formatter, item, null, effectiveEncoding, bufferSize, bufferManager);
    }

    /// <summary>Serializes the specified item.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static byte[] SerializeToBytes(this IJsonMessageFormatter formatter, object item,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      if (item == null) { throw new ArgumentNullException(nameof(item)); }

      using (var pooledStream = BufferManagerOutputStreamManager.Create())
      {
        var outputStream = pooledStream.Object;
        outputStream.Reinitialize(bufferSize, bufferManager ?? BufferManager.GlobalManager);

        formatter.WriteToStream(item.GetType(), item, outputStream, effectiveEncoding, serializerSettings);
        return outputStream.ToByteArray();
      }
    }

    /// <summary>Serializes the specified item.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static ArraySegmentWrapper<byte> SerializeToByteArraySegment(this IJsonMessageFormatter formatter, object item,
      Encoding effectiveEncoding, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      return SerializeToByteArraySegment(formatter, item, null, effectiveEncoding, bufferSize, bufferManager);
    }

    /// <summary>Serializes the specified item.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static ArraySegmentWrapper<byte> SerializeToByteArraySegment(this IJsonMessageFormatter formatter, object item,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      if (item == null) { throw new ArgumentNullException(nameof(item)); }

      using (var pooledStream = BufferManagerOutputStreamManager.Create())
      {
        var outputStream = pooledStream.Object;
        outputStream.Reinitialize(bufferSize, bufferManager ?? BufferManager.GlobalManager);

        formatter.WriteToStream(item.GetType(), item, outputStream, effectiveEncoding, serializerSettings);
        return outputStream.ToArraySegment();
      }
    }

    /// <summary>Serializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<byte[]> SerializeToBytesAsync(this IJsonMessageFormatter formatter, object item,
      Encoding effectiveEncoding, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(SerializeToBytes(formatter, item, null, effectiveEncoding, bufferSize, bufferManager));
    }

    /// <summary>Serializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<byte[]> SerializeToBytesAsync(this IJsonMessageFormatter formatter, object item,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(SerializeToBytes(formatter, item, serializerSettings, effectiveEncoding, bufferSize, bufferManager));
    }

    /// <summary>Serializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<ArraySegmentWrapper<byte>> SerializeToByteArraySegmentAsync(this IJsonMessageFormatter formatter, object item,
      Encoding effectiveEncoding, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(SerializeToByteArraySegment(formatter, item, null, effectiveEncoding, bufferSize, bufferManager));
    }

    /// <summary>Serializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<ArraySegmentWrapper<byte>> SerializeToByteArraySegmentAsync(this IJsonMessageFormatter formatter, object item,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(SerializeToByteArraySegment(formatter, item, serializerSettings, effectiveEncoding, bufferSize, bufferManager));
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static object DeserializeFromBytes(this IJsonMessageFormatter formatter, Type type, byte[] serializedObject,
      Encoding effectiveEncoding, BufferManager bufferManager = null)
    {
      return DeserializeFromBytes(formatter, type, serializedObject, null, effectiveEncoding, bufferManager);
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static object DeserializeFromBytes(this IJsonMessageFormatter formatter, Type type, byte[] serializedObject,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      if (serializedObject == null) { throw new ArgumentNullException(nameof(serializedObject)); }

      using (var pooledReader = BufferManagerStreamReaderManager.Create())
      {
        var reader = pooledReader.Object;
        reader.Reinitialize(serializedObject, bufferManager ?? BufferManager.GlobalManager);
        return formatter.ReadFromStream(type, reader, effectiveEncoding, serializerSettings);
      }
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static object DeserializeFromBytes(this IJsonMessageFormatter formatter, Type type, byte[] serializedObject, int offset, int count,
      Encoding effectiveEncoding, BufferManager bufferManager = null)
    {
      return DeserializeFromBytes(formatter, type, serializedObject, offset, count, null, effectiveEncoding, bufferManager);
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static object DeserializeFromBytes(this IJsonMessageFormatter formatter, Type type, byte[] serializedObject, int offset, int count,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      if (serializedObject == null) { throw new ArgumentNullException(nameof(serializedObject)); }

      using (var pooledReader = BufferManagerStreamReaderManager.Create())
      {
        var reader = pooledReader.Object;
        reader.Reinitialize(serializedObject, offset, count, bufferManager ?? BufferManager.GlobalManager);
        return formatter.ReadFromStream(type, reader, effectiveEncoding, serializerSettings);
      }
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<object> DeserializeFromBytesAsync(this IJsonMessageFormatter formatter, Type type, byte[] serializedObject,
      Encoding effectiveEncoding, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(DeserializeFromBytes(formatter, type, serializedObject, null, effectiveEncoding, bufferManager));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<object> DeserializeFromBytesAsync(this IJsonMessageFormatter formatter, Type type, byte[] serializedObject,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(DeserializeFromBytes(formatter, type, serializedObject, serializerSettings, effectiveEncoding, bufferManager));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<object> DeserializeFromBytesAsync(this IJsonMessageFormatter formatter, Type type, byte[] serializedObject, int offset, int count,
      Encoding effectiveEncoding, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(DeserializeFromBytes(formatter, type, serializedObject, offset, count, null, effectiveEncoding, bufferManager));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<object> DeserializeFromBytesAsync(this IJsonMessageFormatter formatter, Type type, byte[] serializedObject, int offset, int count,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(DeserializeFromBytes(formatter, type, serializedObject, offset, count, serializerSettings, effectiveEncoding, bufferManager));
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static T DeserializeFromBytes<T>(this IJsonMessageFormatter formatter, byte[] serializedObject,
      Encoding effectiveEncoding, BufferManager bufferManager = null)
    {
      var obj = DeserializeFromBytes(formatter, typeof(T), serializedObject, null, effectiveEncoding, bufferManager);
      return (T)obj;
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static T DeserializeFromBytes<T>(this IJsonMessageFormatter formatter, byte[] serializedObject,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      var obj = DeserializeFromBytes(formatter, typeof(T), serializedObject, serializerSettings, effectiveEncoding, bufferManager);
      return (T)obj;
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static T DeserializeFromBytes<T>(this IJsonMessageFormatter formatter, byte[] serializedObject, int offset, int count,
      Encoding effectiveEncoding, BufferManager bufferManager = null)
    {
      var obj = DeserializeFromBytes(formatter, typeof(T), serializedObject, offset, count, null, effectiveEncoding, bufferManager);
      return (T)obj;
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static T DeserializeFromBytes<T>(this IJsonMessageFormatter formatter, byte[] serializedObject, int offset, int count,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      var obj = DeserializeFromBytes(formatter, typeof(T), serializedObject, offset, count, serializerSettings, effectiveEncoding, bufferManager);
      return (T)obj;
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<T> DeserializeFromBytesAsync<T>(this IJsonMessageFormatter formatter, byte[] serializedObject,
      Encoding effectiveEncoding, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(DeserializeFromBytes<T>(formatter, serializedObject, null, effectiveEncoding, bufferManager));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<T> DeserializeFromBytesAsync<T>(this IJsonMessageFormatter formatter, byte[] serializedObject,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(DeserializeFromBytes<T>(formatter, serializedObject, serializerSettings, effectiveEncoding, bufferManager));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<T> DeserializeFromBytesAsync<T>(this IJsonMessageFormatter formatter, byte[] serializedObject, int offset, int count,
      Encoding effectiveEncoding, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(DeserializeFromBytes<T>(formatter, serializedObject, offset, count, null, effectiveEncoding, bufferManager));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<T> DeserializeFromBytesAsync<T>(this IJsonMessageFormatter formatter, byte[] serializedObject, int offset, int count,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(DeserializeFromBytes<T>(formatter, serializedObject, offset, count, serializerSettings, effectiveEncoding, bufferManager));
    }


    /// <summary>Serializes the specified item.</summary>
    /// <param name="formatter">The json formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static string SerializeToString(this IJsonMessageFormatter formatter, object item,
      Encoding effectiveEncoding = null, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      return SerializeToString(formatter, item, null, effectiveEncoding, bufferSize, bufferManager);
    }

    /// <summary>Serializes the specified item.</summary>
    /// <param name="formatter">The json formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static string SerializeToString(this IJsonMessageFormatter formatter, object item,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      if (item == null) { throw new ArgumentNullException(nameof(item)); }

      var outputStream = BufferManagerOutputStreamManager.Take();
      outputStream.Reinitialize(bufferSize, Int32.MaxValue, Int32.MaxValue, effectiveEncoding ?? StringHelper.UTF8NoBOM, bufferManager ?? BufferManager.GlobalManager);

      BufferManagerStreamReader reader = null;
      try
      {
        formatter.WriteToStream(item.GetType(), item, outputStream, effectiveEncoding, serializerSettings);
        var inputStream = outputStream.ToReadOnlyStream();
        reader = BufferManagerStreamReaderManager.Take();
        reader.Reinitialize(inputStream, effectiveEncoding ?? Encoding.UTF8, false, bufferManager ?? BufferManager.GlobalManager);
        return reader.ReadString((int)inputStream.Length);
      }
      finally
      {
        BufferManagerStreamReaderManager.Return(reader);
        BufferManagerOutputStreamManager.Return(outputStream);
      }
    }

    /// <summary>Serializes the asynchronous.</summary>
    /// <param name="formatter">The json formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<string> SerializeToStringAsync(this IJsonMessageFormatter formatter, object item,
      Encoding effectiveEncoding = null, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(SerializeToString(formatter, item, null, effectiveEncoding, bufferSize, bufferManager));
    }

    /// <summary>Serializes the asynchronous.</summary>
    /// <param name="formatter">The json formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<string> SerializeToStringAsync(this IJsonMessageFormatter formatter, object item,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(SerializeToString(formatter, item, serializerSettings, effectiveEncoding, bufferSize, bufferManager));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="formatter">The json formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static object DeserializeFromString(this IJsonMessageFormatter formatter, Type type, string serializedObject,
      Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      return DeserializeFromString(formatter, type, serializedObject, null, effectiveEncoding, bufferManager);
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="formatter">The json formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static object DeserializeFromString(this IJsonMessageFormatter formatter, Type type, string serializedObject,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      if (serializedObject == null) { throw new ArgumentNullException(nameof(serializedObject)); }

      if (null == effectiveEncoding) { effectiveEncoding = Encoding.UTF8; }
      if (null == bufferManager) { bufferManager = BufferManager.GlobalManager; }

      var buffer = effectiveEncoding.GetBufferSegment(serializedObject, bufferManager);
      var reader = BufferManagerStreamReaderManager.Take();
      reader.Reinitialize(buffer, effectiveEncoding, bufferManager);
      try
      {
        return formatter.ReadFromStream(type, reader, effectiveEncoding, serializerSettings);
      }
      finally
      {
        bufferManager.ReturnBuffer(buffer);
        BufferManagerStreamReaderManager.Return(reader);
      }
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="formatter">The json formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<object> DeserializeFromStringAsync(this IJsonMessageFormatter formatter, Type type, string serializedObject,
      Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(DeserializeFromString(formatter, type, serializedObject, null, effectiveEncoding, bufferManager));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="formatter">The json formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<object> DeserializeFromStringAsync(this IJsonMessageFormatter formatter, Type type, string serializedObject,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(DeserializeFromString(formatter, type, serializedObject, serializerSettings, effectiveEncoding, bufferManager));
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The json formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static T DeserializeFromString<T>(this IJsonMessageFormatter formatter, string serializedObject,
      Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      var obj = DeserializeFromString(formatter, typeof(T), serializedObject, null, effectiveEncoding, bufferManager);
      return (T)obj;
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The json formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static T DeserializeFromString<T>(this IJsonMessageFormatter formatter, string serializedObject,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      var obj = DeserializeFromString(formatter, typeof(T), serializedObject, serializerSettings, effectiveEncoding, bufferManager);
      return (T)obj;
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The json formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<T> DeserializeFromStringAsync<T>(this IJsonMessageFormatter formatter, string serializedObject,
      Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(DeserializeFromString<T>(formatter, serializedObject, null, effectiveEncoding, bufferManager));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The json formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <param name="effectiveEncoding">The encoding.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<T> DeserializeFromStringAsync<T>(this IJsonMessageFormatter formatter, string serializedObject,
      JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, BufferManager bufferManager = null)
    {
      return TaskShim.FromResult(DeserializeFromString<T>(formatter, serializedObject, serializerSettings, effectiveEncoding, bufferManager));
    }
  }
}
