using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using CuteAnt.Buffers;

namespace PerfBenchmark.Benchmarks
{
  [Config(typeof(CoreConfig))]
  public class Utf8GetStringBenchmark
  {
    private const int OperationsPerInvoke = 50000;

    private static byte[] ReadBytes(string fileName)
    {
      var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Infos", fileName);
      using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
      {
        using (var sr = new StreamReader(fs))
        {
          return Encoding.UTF8.GetBytes(sr.ReadToEnd());
        }
      }
    }

    #region -- Encoding.UTF8.GetString --

    private byte[] _msg4kBytes;
    [GlobalSetup(Target = nameof(Utf8GetString4K))]
    public void SetupUtf8GetString4K()
    {
      _msg4kBytes = ReadBytes("msg4k.txt");
    }
    [Benchmark(Baseline = true, OperationsPerInvoke = OperationsPerInvoke)]
    public string Utf8GetString4K()
    {
      return Encoding.UTF8.GetString(_msg4kBytes);
    }

    private byte[] _msg16kBytes;
    [GlobalSetup(Target = nameof(Utf8GetString16K))]
    public void SetupUtf8GetString16K()
    {
      _msg16kBytes = ReadBytes("msg16k.txt");
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public string Utf8GetString16K()
    {
      return Encoding.UTF8.GetString(_msg16kBytes);
    }

    private byte[] _msg32kBytes;
    [GlobalSetup(Target = nameof(Utf8GetString32K))]
    public void SetupUtf8GetString32K()
    {
      _msg32kBytes = ReadBytes("msg32k.txt");
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public string Utf8GetString32K()
    {
      return Encoding.UTF8.GetString(_msg32kBytes);
    }

    private byte[] _msg64kBytes;
    [GlobalSetup(Target = nameof(Utf8GetString64K))]
    public void SetupUtf8GetString64K()
    {
      _msg64kBytes = ReadBytes("msg64k.txt");
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public string Utf8GetString64K()
    {
      return Encoding.UTF8.GetString(_msg64kBytes);
    }

    #endregion

    #region -- Encoding.UTF8.GetStringWithBuffer --

    private byte[] _msg4kBytes1;
    [GlobalSetup(Target = nameof(Utf8BufferGetString4K))]
    public void SetupUtf8BufferGetString4K()
    {
      _msg4kBytes1 = ReadBytes("msg4k.txt");
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public string Utf8BufferGetString4K()
    {
      return Encoding.UTF8.GetStringWithBuffer(_msg4kBytes1, BufferManager.Shared);
    }

    private byte[] _msg16kBytes1;
    [GlobalSetup(Target = nameof(Utf8BufferGetString16K))]
    public void SetupUtf8BufferGetString16K()
    {
      _msg16kBytes1 = ReadBytes("msg16k.txt");
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public string Utf8BufferGetString16K()
    {
      return Encoding.UTF8.GetStringWithBuffer(_msg16kBytes1, BufferManager.Shared);
    }

    private byte[] _msg32kBytes1;
    [GlobalSetup(Target = nameof(Utf8BufferGetString32K))]
    public void SetupUtf8BufferGetString32K()
    {
      _msg32kBytes1 = ReadBytes("msg32k.txt");
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public string Utf8BufferGetString32K()
    {
      return Encoding.UTF8.GetStringWithBuffer(_msg32kBytes1, BufferManager.Shared);
    }

    private byte[] _msg64kBytes1;
    [GlobalSetup(Target = nameof(Utf8BufferGetString64K))]
    public void SetupUtf8BufferGetString64K()
    {
      _msg64kBytes1 = ReadBytes("msg64k.txt");
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public string Utf8BufferGetString64K()
    {
      return Encoding.UTF8.GetStringWithBuffer(_msg64kBytes1, BufferManager.Shared);
    }

    #endregion

  }
}
