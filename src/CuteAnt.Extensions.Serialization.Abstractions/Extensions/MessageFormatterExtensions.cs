using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CuteAnt.Buffers;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>MessageFormatterExtensions</summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class MessageFormatterExtensions
  {
    private const int c_defaultBufferSize = 1024 * 2;

    /// <summary>Serializes the specified item.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static byte[] SerializeToBytes(this IMessageFormatter formatter, object item, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      if (item == null) { throw new ArgumentNullException(nameof(item)); }

      var outputStream = BufferManagerOutputStreamManager.Take();
      outputStream.Reinitialize(bufferSize, bufferManager ?? BufferManager.GlobalManager);

      try
      {
        formatter.WriteToStream(item.GetType(), item, outputStream);
        return outputStream.ToByteArray();
      }
      finally
      {
        BufferManagerOutputStreamManager.Return(outputStream);
      }
    }

    /// <summary>Serializes the specified item.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static ArraySegmentWrapper<byte> SerializeToByteArraySegment(this IMessageFormatter formatter, object item, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      if (item == null) { throw new ArgumentNullException(nameof(item)); }

      var outputStream = BufferManagerOutputStreamManager.Take();
      outputStream.Reinitialize(bufferSize, bufferManager ?? BufferManager.GlobalManager);

      try
      {
        formatter.WriteToStream(item.GetType(), item, outputStream);
        return outputStream.ToArraySegment();
      }
      finally
      {
        BufferManagerOutputStreamManager.Return(outputStream);
      }
    }

    /// <summary>Serializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<byte[]> SerializeToBytesAsync(this IMessageFormatter formatter, object item, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(SerializeToBytes(formatter, item, bufferSize, bufferManager));
    }

    /// <summary>Serializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<ArraySegmentWrapper<byte>> SerializeToByteArraySegmentAsync(this IMessageFormatter formatter, object item, int bufferSize = c_defaultBufferSize, BufferManager bufferManager = null)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(SerializeToByteArraySegment(formatter, item, bufferSize, bufferManager));
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static object DeserializeFromBytes(this IMessageFormatter formatter, Type type, byte[] serializedObject, BufferManager bufferManager = null)
    {
      if (serializedObject == null) { throw new ArgumentNullException(nameof(serializedObject)); }

      using (var pooldReader = BufferManagerStreamReaderManager.Create())
      {
        var reader = pooldReader.Object;
        reader.Reinitialize(serializedObject, bufferManager ?? BufferManager.GlobalManager);

        return formatter.ReadFromStream(type, reader);
      }
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static object DeserializeFromBytes(this IMessageFormatter formatter, Type type, byte[] serializedObject, int offset, int count, BufferManager bufferManager = null)
    {
      if (serializedObject == null) { throw new ArgumentNullException(nameof(serializedObject)); }

      using (var pooldReader = BufferManagerStreamReaderManager.Create())
      {
        var reader = pooldReader.Object;
        reader.Reinitialize(serializedObject, offset, count, bufferManager ?? BufferManager.GlobalManager);

        return formatter.ReadFromStream(type, reader);
      }
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<object> DeserializeFromBytesAsync(this IMessageFormatter formatter, Type type, byte[] serializedObject, BufferManager bufferManager = null)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(DeserializeFromBytes(formatter, type, serializedObject, bufferManager));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<object> DeserializeFromBytesAsync(this IMessageFormatter formatter, Type type, byte[] serializedObject, int offset, int count, BufferManager bufferManager = null)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(DeserializeFromBytes(formatter, type, serializedObject, offset, count, bufferManager));
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static T DeserializeFromBytes<T>(this IMessageFormatter formatter, byte[] serializedObject, BufferManager bufferManager = null)
    {
      return (T)DeserializeFromBytes(formatter, typeof(T), serializedObject, bufferManager);
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static T DeserializeFromBytes<T>(this IMessageFormatter formatter, byte[] serializedObject, int offset, int count, BufferManager bufferManager = null)
    {
      return (T)DeserializeFromBytes(formatter, typeof(T), serializedObject, offset, count, bufferManager);
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<T> DeserializeFromBytesAsync<T>(this IMessageFormatter formatter, byte[] serializedObject, BufferManager bufferManager = null)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(DeserializeFromBytes<T>(formatter, serializedObject, bufferManager));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <param name="bufferManager">The buffer manager.</param>
    /// <returns></returns>
    public static Task<T> DeserializeFromBytesAsync<T>(this IMessageFormatter formatter, byte[] serializedObject, int offset, int count, BufferManager bufferManager = null)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(DeserializeFromBytes<T>(formatter, serializedObject, offset, count, bufferManager));
    }
  }
}
