using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using CuteAnt;
using CuteAnt.Buffers;
using CuteAnt.IO;
using CuteAnt.IO.Pipelines;
using BenchmarkDotNet.Attributes;
using CuteAnt.Extensions.Serialization;
using MessagePack;
using PerfBenchmark.Types;

namespace PerfBenchmark
{
  [Config(typeof(CoreConfig))]
  public class MemoryPoolBenchmark
  {
    private const int OperationsPerInvoke = 50000;

    private const int c_initialBufferSize = 1024 * 64;
    private const int c_zeroSize = 0;

    private static readonly MessagePackMessageFormatter _formatter = MessagePackMessageFormatter.DefaultInstance;


    private TypicalPersonData _persionData;

    [GlobalSetup]
    public void Setup()
    {
      _persionData = TypicalPersonData.MakeRandom();
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] MessagePack()
    {
      return MessagePackSerializer.Serialize(_persionData);
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] MessagePackTypeless()
    {
      return MessagePackSerializer.Typeless.Serialize(_persionData);
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] BufferManagerOutputStream()
    {
      using (var pooledStream = BufferManagerOutputStreamManager.Create())
      {
        var outputStream = pooledStream.Object;
        outputStream.Reinitialize(c_initialBufferSize, BufferManager.Shared);

        _formatter.WriteToStream(_persionData, outputStream);
        return outputStream.ToByteArray();
      }
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SystemBufferArrayPool()
    {
      using (var pooledStream = BufferManagerOutputStreamManager.Create())
      {
        var outputStream = pooledStream.Object;
        outputStream.Reinitialize(c_initialBufferSize, ArrayPool<byte>.Shared);

        _formatter.WriteToStream(_persionData, outputStream);
        return outputStream.ToByteArray();
      }
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] RecyclableMemoryStream()
    {
      using (var ms = MemoryStreamManager.GetStream())
      {
        _formatter.WriteToStream(_persionData, ms);
        return ms.ToArray();
      }
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] CorefxLabPipelines()
    {
      using (var pooledPipe = PipelineManager.Create())
      {
        var pipe = pooledPipe.Object;
        var outputStream = new PipelineStream(pipe, c_initialBufferSize);
        _formatter.WriteToStream(_persionData, outputStream);
        pipe.Flush();
        var readBuffer = pipe.Reader.ReadAsync().GetResult().Buffer;
        var length = (int)readBuffer.Length;
        if (c_zeroSize == length) { return EmptyArray<byte>.Instance; }
        return readBuffer.ToArray();
      }
    }
  }
}
