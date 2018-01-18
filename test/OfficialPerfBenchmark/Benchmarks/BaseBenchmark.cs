using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using MessagePack;

namespace PerfBenchmark
{
  public abstract class BaseBenchmark<T>
  {
    private const int OperationsPerInvoke = 50000;

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
    private byte[] _messagePackData1;
    [GlobalSetup(Target = nameof(DeserializeMessagePackNonGeneric))]
    public void SetupDeserializeMessagePackNonGeneric()
    {
      _messagePackData1 = MessagePackSerializer.Serialize(GetValue());
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeMessagePackNonGeneric()
    {
      return (T)MessagePackSerializer.NonGeneric.Deserialize(typeof(T), _messagePackData1);
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

    #region -- Utf5Json --

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SerializeUtf8Json()
    {
      return Utf8Json.JsonSerializer.Serialize(_value);
    }
    private byte[] _utf8JsonData;
    [GlobalSetup(Target = nameof(DeserializeUtf8Json))]
    public void SetupDeserializeUtf8Json()
    {
      _utf8JsonData = Utf8Json.JsonSerializer.Serialize(GetValue());
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeUtf8Json()
    {
      return Utf8Json.JsonSerializer.Deserialize<T>(_utf8JsonData);
    }

    #endregion
  }
}