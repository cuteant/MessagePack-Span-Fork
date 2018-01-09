using System;
using System.IO;
using System.ComponentModel;
using System.Threading.Tasks;
using CuteAnt.AsyncEx;
using CuteAnt.Buffers;
//#if !NET40
//using CuteAnt.IO.Pipelines;
//#endif

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>MessageFormatterExtensions</summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class MessageFormatterExtensions
  {
    private const int c_initialBufferSize = 1024 * 64;
    private const int c_zeroSize = 0;

    #region -- SerializeToBytes --

    /// <summary>Serializes the specified item.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="initialBufferSize">The initial buffer size.</param>
    /// <returns></returns>
    public static byte[] SerializeToBytes(this IMessageFormatter formatter, object item, int initialBufferSize = c_initialBufferSize)
    {
//#if NET40
      using (var pooledStream = BufferManagerOutputStreamManager.Create())
      {
        var outputStream = pooledStream.Object;
        outputStream.Reinitialize(initialBufferSize);

        formatter.WriteToStream(item, outputStream);
        return outputStream.ToByteArray();
      }
//#else
//      using (var pooledPipe = PipelineManager.Create())
//      {
//        var pipe = pooledPipe.Object;
//        var outputStream = new PipelineStream(pipe, initialBufferSize);
//        formatter.WriteToStream(item, outputStream);
//        pipe.Flush();
//        var readBuffer = pipe.Reader.ReadAsync().GetResult().Buffer;
//        var length = (int)readBuffer.Length;
//        if (c_zeroSize == length) { return EmptyArray<byte>.Instance; }
//        return readBuffer.ToArray();
//      }
//#endif
    }

    #endregion

    #region -- SerializeToBytesAsync --

    /// <summary>Serializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="initialBufferSize">The initial buffer size.</param>
    /// <returns></returns>
    public static Task<byte[]> SerializeToBytesAsync(this IMessageFormatter formatter, object item, int initialBufferSize = c_initialBufferSize)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(SerializeToBytes(formatter, item, initialBufferSize));
      //#if NET40
      //      await TaskConstants.Completed;
      //      return SerializeToBytes(formatter, item, initialBufferSize);
      //#else
      //      using (var pooledPipe = PipelineManager.Create())
      //      {
      //        var pipe = pooledPipe.Object;
      //        var outputStream = new PipelineStream(pipe, initialBufferSize);
      //        formatter.WriteToStream(item, outputStream);
      //        await pipe.FlushAsync();
      //        var readBuffer = (await pipe.Reader.ReadAsync()).Buffer;
      //        var length = (int)readBuffer.Length;
      //        if (c_zeroSize == length) { return EmptyArray<byte>.Instance; }
      //        return readBuffer.ToArray();
      //      }
      //#endif
    }

    #endregion

    #region -- SerializeToByteArraySegment --

    /// <summary>Serializes the specified item.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="initialBufferSize">The initial buffer size.</param>
    /// <returns></returns>
    public static ArraySegment<byte> SerializeToByteArraySegment(this IMessageFormatter formatter, object item,
      int initialBufferSize = c_initialBufferSize)
    {
      //#if NET40
      using (var pooledStream = BufferManagerOutputStreamManager.Create())
      {
        var outputStream = pooledStream.Object;
        outputStream.Reinitialize(initialBufferSize, BufferManager.Shared);

        formatter.WriteToStream(item, outputStream);
        return outputStream.ToArraySegment();
      }
      //#else
      //      using (var pooledPipe = PipelineManager.Create())
      //      {
      //        var pipe = pooledPipe.Object;
      //        var outputStream = new PipelineStream(pipe, initialBufferSize);
      //        formatter.WriteToStream(item, outputStream);
      //        pipe.Flush();
      //        var readBuffer = pipe.Reader.ReadAsync().GetResult().Buffer;
      //        var length = (int)readBuffer.Length;
      //        if (c_zeroSize == length) { return s_emptySegment; }
      //        var buffer = BufferManager.Shared.Rent(length);
      //        readBuffer.CopyTo(buffer);
      //        return new ArraySegment<byte>(buffer, 0, length);
      //      }
      //#endif
    }

    #endregion

    #region -- SerializeToByteArraySegmentAsync --

    /// <summary>Serializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="initialBufferSize">The initial buffer size.</param>
    /// <returns></returns>
    public static Task<ArraySegment<byte>> SerializeToByteArraySegmentAsync(this IMessageFormatter formatter, object item,
      int initialBufferSize = c_initialBufferSize)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(SerializeToByteArraySegment(formatter, item, initialBufferSize));
      //#if NET40
      //      await TaskConstants.Completed;
      //      return SerializeToByteArraySegment(formatter, item, initialBufferSize);
      //#else
      //      using (var pooledPipe = PipelineManager.Create())
      //      {
      //        var pipe = pooledPipe.Object;
      //        var outputStream = new PipelineStream(pipe, initialBufferSize);
      //        formatter.WriteToStream(item, outputStream);
      //        await pipe.FlushAsync();
      //        var readBuffer = (await pipe.Reader.ReadAsync()).Buffer;
      //        var length = (int)readBuffer.Length;
      //        if (c_zeroSize == length) { return s_emptySegment; }
      //        var buffer = BufferManager.Shared.Rent(length);
      //        readBuffer.CopyTo(buffer);
      //        return new ArraySegment<byte>(buffer, 0, length);
      //      }
      //#endif
    }

    #endregion

    #region -- Deserialize Methods --

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public static object DeserializeFromBytes(this IMessageFormatter formatter, Type type, byte[] serializedObject)
    {
      if (serializedObject == null) { throw new ArgumentNullException(nameof(serializedObject)); }

      using (var ms = new MemoryStream(serializedObject))
      {
        return formatter.ReadFromStream(type, ms);
      }
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static object DeserializeFromBytes(this IMessageFormatter formatter, Type type, byte[] serializedObject, int offset, int count)
    {
      if (serializedObject == null) { throw new ArgumentNullException(nameof(serializedObject)); }

      using (var ms = new MemoryStream(serializedObject, offset, count))
      {
        return formatter.ReadFromStream(type, ms);
      }
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public static Task<object> DeserializeFromBytesAsync(this IMessageFormatter formatter, Type type, byte[] serializedObject)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(DeserializeFromBytes(formatter, type, serializedObject));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static Task<object> DeserializeFromBytesAsync(this IMessageFormatter formatter, Type type, byte[] serializedObject, int offset, int count)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(DeserializeFromBytes(formatter, type, serializedObject, offset, count));
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public static T DeserializeFromBytes<T>(this IMessageFormatter formatter, byte[] serializedObject)
    {
      return (T)DeserializeFromBytes(formatter, typeof(T), serializedObject);
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static T DeserializeFromBytes<T>(this IMessageFormatter formatter, byte[] serializedObject, int offset, int count)
    {
      return (T)DeserializeFromBytes(formatter, typeof(T), serializedObject, offset, count);
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public static Task<T> DeserializeFromBytesAsync<T>(this IMessageFormatter formatter, byte[] serializedObject)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(DeserializeFromBytes<T>(formatter, serializedObject));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static Task<T> DeserializeFromBytesAsync<T>(this IMessageFormatter formatter, byte[] serializedObject, int offset, int count)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(DeserializeFromBytes<T>(formatter, serializedObject, offset, count));
    }

    #endregion
  }
}
