using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CuteAnt.Buffers;
#if !NET40
//using CuteAnt.IO.Pipelines;
#endif

namespace CuteAnt.Extensions.Serialization
{
  partial class MessageFormatter
  {
    public virtual ArraySegment<byte> WriteToMemoryPool<T>(T item)
    {
      return WriteToMemoryPool<T>(item, c_initialBufferSize);
    }

    /// <summary>Serializes the specified item.</summary>
    /// <param name="item">The item.</param>
    /// <param name="initialBufferSize">The initial buffer size.</param>
    /// <returns></returns>
    public virtual ArraySegment<byte> WriteToMemoryPool<T>(T item, int initialBufferSize)
    {
      using (var pooledStream = BufferManagerOutputStreamManager.Create())
      {
        var outputStream = pooledStream.Object;
        outputStream.Reinitialize(initialBufferSize, s_sharedBufferPool);

        WriteToStream<T>(item, outputStream);
        return outputStream.ToArraySegment();
      }
    }

    public virtual ArraySegment<byte> WriteToMemoryPool(object item)
    {
      return WriteToMemoryPool(item, c_initialBufferSize);
    }

    /// <summary>Serializes the specified item.</summary>
    /// <param name="item">The item.</param>
    /// <param name="initialBufferSize">The initial buffer size.</param>
    /// <returns></returns>
    public virtual ArraySegment<byte> WriteToMemoryPool(object item, int initialBufferSize)
    {
      //#if NET40
      using (var pooledStream = BufferManagerOutputStreamManager.Create())
      {
        var outputStream = pooledStream.Object;
        outputStream.Reinitialize(initialBufferSize, s_sharedBufferPool);

        WriteToStream(item, outputStream);
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

    public Task<ArraySegment<byte>> WriteToMemoryPoolAsync<T>(T item)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(WriteToMemoryPool<T>(item, c_initialBufferSize));
    }
    public Task<ArraySegment<byte>> WriteToMemoryPoolAsync<T>(T item, int initialBufferSize)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(WriteToMemoryPool<T>(item, initialBufferSize));
    }

    public Task<ArraySegment<byte>> WriteToMemoryPoolAsync(object item)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(WriteToMemoryPool(item, c_initialBufferSize));
    }

    /// <summary>Serializes the asynchronous.</summary>
    /// <param name="item">The item.</param>
    /// <param name="initialBufferSize">The initial buffer size.</param>
    /// <returns></returns>
    public Task<ArraySegment<byte>> WriteToMemoryPoolAsync(object item, int initialBufferSize)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(WriteToMemoryPool(item, initialBufferSize));
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
  }
}
