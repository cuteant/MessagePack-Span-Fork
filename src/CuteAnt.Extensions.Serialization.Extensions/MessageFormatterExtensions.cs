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

    #region -- SerializeAsync --

    /// <summary>Serializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="initialBufferSize">The initial buffer size.</param>
    /// <returns></returns>
    public static Task<byte[]> SerializeAsync(this IMessageFormatter formatter, object item, int initialBufferSize = c_initialBufferSize)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(formatter.Serialize(item, initialBufferSize));
      //#if NET40
      //      await TaskConstants.Completed;
      //      return Serialize(formatter, item, initialBufferSize);
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

    #region -- WriteToMemoryPool --

    /// <summary>Serializes the specified item.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="initialBufferSize">The initial buffer size.</param>
    /// <returns></returns>
    public static ArraySegment<byte> WriteToMemoryPool(this IMessageFormatter formatter, object item,
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

    #region -- WriteToMemoryPoolAsync --

    /// <summary>Serializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="item">The item.</param>
    /// <param name="initialBufferSize">The initial buffer size.</param>
    /// <returns></returns>
    public static Task<ArraySegment<byte>> WriteToMemoryPoolAsync(this IMessageFormatter formatter, object item,
      int initialBufferSize = c_initialBufferSize)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(WriteToMemoryPool(formatter, item, initialBufferSize));
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
    public static object Deserialize(this IMessageFormatter formatter, Type type, byte[] serializedObject)
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
    public static object Deserialize(this IMessageFormatter formatter, Type type, byte[] serializedObject, int offset, int count)
    {
      if (serializedObject == null) { throw new ArgumentNullException(nameof(serializedObject)); }

      using (var ms = new MemoryStream(serializedObject, offset, count))
      {
        return formatter.ReadFromStream(type, ms);
      }
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public static T Deserialize<T>(this IMessageFormatter formatter, byte[] serializedObject)
    {
      return (T)Deserialize(formatter, typeof(T), serializedObject);
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static T Deserialize<T>(this IMessageFormatter formatter, byte[] serializedObject, int offset, int count)
    {
      return (T)Deserialize(formatter, typeof(T), serializedObject, offset, count);
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public static Task<object> DeserializeAsync(this IMessageFormatter formatter, Type type, byte[] serializedObject)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Deserialize(formatter, type, serializedObject));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static Task<object> DeserializeAsync(this IMessageFormatter formatter, Type type, byte[] serializedObject, int offset, int count)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Deserialize(formatter, type, serializedObject, offset, count));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public static Task<T> DeserializeAsync<T>(this IMessageFormatter formatter, byte[] serializedObject)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Deserialize<T>(formatter, serializedObject));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter">The formatter.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static Task<T> DeserializeAsync<T>(this IMessageFormatter formatter, byte[] serializedObject, int offset, int count)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Deserialize<T>(formatter, serializedObject, offset, count));
    }

    #endregion
  }
}
