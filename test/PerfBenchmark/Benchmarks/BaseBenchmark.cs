using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using CuteAnt.Extensions.Serialization;
using MessagePack;
using Newtonsoft.Json;

namespace PerfBenchmark
{
  public abstract class BaseBenchmark<T>
  {
    private const int OperationsPerInvoke = 50000;

    private static readonly JsonSerializerSettings _jsonSerializerSettings;
    private static readonly JsonMessageFormatter _jsonFormatter;

    static BaseBenchmark()
    {
      _jsonFormatter = JsonMessageFormatter.DefaultInstance;
      _jsonSerializerSettings = _jsonFormatter.DefaultSerializerSettings;
    }

    protected abstract T GetValue();
    private T _value;

    [GlobalSetup]
    public void Setup()
    {
      _value = GetValue();
    }

    #region -- MessagePack --

    [Benchmark(Baseline = true, OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SerializeMessagePack()
    {
      return MessagePackSerializer.Serialize(_value);
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SerializeMessagePackNonGeneric()
    {
      return MessagePackSerializer.Serialize((object)_value);
    }
    private byte[] _messagePackData;
    [GlobalSetup(Target = nameof(DeserializeMessagePack))]
    public void SetupDeserializeMessagePack()
    {
      _messagePackData = MessagePackSerializer.Serialize(GetValue());
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeMessagePack()
    {
      return MessagePackSerializer.Deserialize<T>(_messagePackData);
    }
    private byte[] _messagePackData_a;
    [GlobalSetup(Target = nameof(DeserializeMessagePackNonGeneric))]
    public void SetupDeserializeMessagePackNonGeneric()
    {
      _messagePackData_a = MessagePackSerializer.Serialize(GetValue());
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeMessagePackNonGeneric()
    {
      return (T)MessagePackSerializer.NonGeneric.Deserialize(typeof(T), _messagePackData_a);
    }

    #endregion

    #region -- MessagePackTypeless --

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SerializeMessagePackTypeless()
    {
      return MessagePackSerializer.Typeless.Serialize(_value);
    }
    private byte[] _messagePackTypelessData;
    [GlobalSetup(Target = nameof(DeserializeMessagePackTypeless))]
    public void SetupDeserializeMessagePackTypeless()
    {
      _messagePackTypelessData = MessagePackSerializer.Typeless.Serialize(GetValue());
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeMessagePackTypeless()
    {
      return (T)MessagePackSerializer.Typeless.Deserialize(_messagePackTypelessData);
    }

    #endregion

    #region -- LZ4MessagePackTypeless --

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SerializeLz4MessagePackTypeless()
    {
      return LZ4MessagePackSerializer.Typeless.Serialize(_value);
    }
    private byte[] _messagePackTypelessData2;
    [GlobalSetup(Target = nameof(DeserializeLz4MessagePackTypeless))]
    public void SetupDeserializeLz4MessagePackTypeless()
    {
      _messagePackTypelessData2 = LZ4MessagePackSerializer.Typeless.Serialize(GetValue());
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeLz4MessagePackTypeless()
    {
      return (T)LZ4MessagePackSerializer.Typeless.Deserialize(_messagePackTypelessData2);
    }

    #endregion
  }
}