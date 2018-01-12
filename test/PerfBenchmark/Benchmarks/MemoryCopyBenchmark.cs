using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;

namespace PerfBenchmark
{
  [Config(typeof(CoreConfig))]
  public class MemoryCopyBenchmark
  {
    private const int OperationsPerInvoke = 50000;

    [Params(64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32786, 65535)]
    public int MaxCounter = 0;

    private byte[] bufferFrom;
    private byte[] bufferTo;

    [GlobalSetup]
    public void Setup()
    {
      bufferFrom = new byte[1024 * 64];
      bufferTo = new byte[1024 * 64];
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = OperationsPerInvoke)]
    public void ArrayCopy()
    {
      Array.Copy(bufferFrom, 0, bufferTo, 0, MaxCounter);
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public void BlockCopy()
    {
      Buffer.BlockCopy(bufferFrom, 0, bufferTo, 0, MaxCounter);
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public unsafe void MemoryCopy()
    {
      fixed (byte* pSrc = &bufferFrom[0])
      fixed (byte* pDst = &bufferTo[0])
      {
        Buffer.MemoryCopy(pSrc, pDst, bufferTo.Length, MaxCounter);
      }
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public unsafe void CopyBlock()
    {
      fixed (byte* pSrc = &bufferFrom[0])
      fixed (byte* pDst = &bufferTo[0])
      {
        Unsafe.CopyBlock(pDst, pSrc, (uint)MaxCounter);
      }
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public unsafe void CopyBlockUnaligned()
    {
      fixed (byte* pSrc = &bufferFrom[0])
      fixed (byte* pDst = &bufferTo[0])
      {
        Unsafe.CopyBlockUnaligned(pDst, pSrc, (uint)MaxCounter);
      }
    }
  }
}
