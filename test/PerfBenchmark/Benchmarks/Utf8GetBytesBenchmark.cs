using System;
using System.Buffers;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using CuteAnt.Buffers;

namespace PerfBenchmark
{
  [Config(typeof(CoreConfig))]
  public class Utf8GetBytesBenchmark
  {
    private const int OperationsPerInvoke = 50000;

    private static string ReadText(string fileName)
    {
      var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Infos", fileName);
      using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
      {
        using (var sr = new StreamReader(fs))
        {
          return sr.ReadToEnd();
        }
      }
    }

    #region -- Encoding.UTF8 --

    private string _msg4kBytes;
    [GlobalSetup(Target = nameof(Utf8GetBytes4K))]
    public void SetupUtf8GetBytes4K()
    {
      _msg4kBytes = ReadText("msg4k.txt");
    }
    [Benchmark(Baseline = true, OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] Utf8GetBytes4K()
    {
      return Encoding.UTF8.GetBytes(_msg4kBytes);
    }

    private string _msg16kBytes;
    [GlobalSetup(Target = nameof(Utf8GetBytes16K))]
    public void SetupUtf8GetBytes16K()
    {
      _msg16kBytes = ReadText("msg16k.txt");
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] Utf8GetBytes16K()
    {
      return Encoding.UTF8.GetBytes(_msg16kBytes);
    }

    private string _msg32kBytes;
    [GlobalSetup(Target = nameof(Utf8GetBytes32K))]
    public void SetupUtf8GetBytes32K()
    {
      _msg32kBytes = ReadText("msg32k.txt");
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] Utf8GetBytes32K()
    {
      return Encoding.UTF8.GetBytes(_msg32kBytes);
    }

    private string _msg64kBytes;
    [GlobalSetup(Target = nameof(Utf8GetBytes64K))]
    public void SetupUtf8GetBytes64K()
    {
      _msg64kBytes = ReadText("msg64k.txt");
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] Utf8GetBytes64K()
    {
      return Encoding.UTF8.GetBytes(_msg64kBytes);
    }

    #endregion

    #region -- Encoding.UTF8.GetBytesWithBuffer --

    private static readonly ArrayPool<byte> _shared = BufferManager.Shared;

    private string _msg4kBytes1;
    [GlobalSetup(Target = nameof(Utf8BufferGetBytes4K))]
    public void SetupUtf8BufferGetBytes4K()
    {
      _msg4kBytes1 = ReadText("msg4k.txt");
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public void Utf8BufferGetBytes4K()
    {
      var buffer = Encoding.UTF8.GetBufferSegment(_msg4kBytes1, _shared);
      _shared.Return(buffer.Array);
    }

    private string _msg16kBytes1;
    [GlobalSetup(Target = nameof(Utf8BufferGetBytes16K))]
    public void SetupUtf8BufferGetBytes16K()
    {
      _msg16kBytes1 = ReadText("msg16k.txt");
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public void Utf8BufferGetBytes16K()
    {
      var buffer = Encoding.UTF8.GetBufferSegment(_msg16kBytes1, _shared);
      _shared.Return(buffer.Array);
    }

    private string _msg32kBytes1;
    [GlobalSetup(Target = nameof(Utf8BufferGetBytes32K))]
    public void SetupUtf8BufferGetBytes32K()
    {
      _msg32kBytes1 = ReadText("msg32k.txt");
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public void Utf8BufferGetBytes32K()
    {
      var buffer = Encoding.UTF8.GetBufferSegment(_msg32kBytes1, _shared);
      _shared.Return(buffer.Array);
    }

    private string _msg64kBytes1;
    [GlobalSetup(Target = nameof(Utf8BufferGetBytes64K))]
    public void SetupUtf8BufferGetBytes64K()
    {
      _msg64kBytes1 = ReadText("msg64k.txt");
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public void Utf8BufferGetBytes64K()
    {
      var buffer = Encoding.UTF8.GetBufferSegment(_msg64kBytes1, _shared);
      _shared.Return(buffer.Array);
    }

    #endregion

  }
}
