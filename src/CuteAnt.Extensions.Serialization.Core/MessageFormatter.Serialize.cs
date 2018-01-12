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
    #region -- Serialize --

    public virtual byte[] Serialize<T>(T item)
    {
      return Serialize<T>(item, c_initialBufferSize);
    }

    public virtual byte[] Serialize<T>(T item, int initialBufferSize)
    {
      using (var pooledStream = BufferManagerOutputStreamManager.Create())
      {
        var outputStream = pooledStream.Object;
        outputStream.Reinitialize(initialBufferSize);

        WriteToStream<T>(item, outputStream);
        return outputStream.ToByteArray();
      }
    }

    public Task<byte[]> SerializeAsync<T>(T item)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Serialize(item, c_initialBufferSize));
    }

    public Task<byte[]> SerializeAsync<T>(T item, int initialBufferSize)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Serialize(item, initialBufferSize));
    }

    #endregion

    #region -- SerializeObject --

    public virtual byte[] SerializeObject(object item)
    {
      return SerializeObject(item, c_initialBufferSize);
    }

    public virtual byte[] SerializeObject(object item, int initialBufferSize)
    {
      //#if NET40
      using (var pooledStream = BufferManagerOutputStreamManager.Create())
      {
        var outputStream = pooledStream.Object;
        outputStream.Reinitialize(initialBufferSize);

        WriteToStream(item, outputStream);
        return outputStream.ToByteArray();
      }
      //#else
      //      using (var pooledPipe = PipelineManager.Create())
      //      {
      //        var pipe = pooledPipe.Object;
      //        var outputStream = new PipelineStream(pipe, initialBufferSize);
      //        WriteToStream(item, outputStream);
      //        pipe.Flush();
      //        var readBuffer = pipe.Reader.ReadAsync().GetResult().Buffer;
      //        var length = (int)readBuffer.Length;
      //        if (c_zeroSize == length) { return EmptyArray<byte>.Instance; }
      //        return readBuffer.ToArray();
      //      }
      //#endif
    }

    public Task<byte[]> SerializeObjectAsync(object item)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(SerializeObject(item, c_initialBufferSize));
    }

    public Task<byte[]> SerializeObjectAsync(object item, int initialBufferSize)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(SerializeObject(item, initialBufferSize));
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
  }
}
