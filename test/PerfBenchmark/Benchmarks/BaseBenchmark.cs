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
    private byte[] _messagePackData;
    [GlobalSetup(Target = nameof(DeserializeMessagePack))]
    public void SetupDeserializeMessagePack()
    {
      _messagePackData = MessagePackSerializer.Serialize(_value);
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeMessagePack()
    {
      return MessagePackSerializer.Deserialize<T>(_messagePackData);
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
      _messagePackTypelessData = MessagePackSerializer.Typeless.Serialize(_value);
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
      _messagePackTypelessData2 = LZ4MessagePackSerializer.Typeless.Serialize(_value);
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeLz4MessagePackTypeless()
    {
      return (T)LZ4MessagePackSerializer.Typeless.Deserialize(_messagePackTypelessData2);
    }

    #endregion

    #region -- MessagePackFormatter --

    private static readonly MessagePackMessageFormatter _messagePackFormatter = MessagePackMessageFormatter.DefaultInstance;
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SerializeMessagePackFormatter()
    {
      return _messagePackFormatter.SerializeObject(_value);
    }
    private byte[] _messagePackData1;
    [GlobalSetup(Target = nameof(DeserializeMessagePackFormatter))]
    public void SetupDeserializeMessagePackFormatter()
    {
      _messagePackData1 = SerializeMessagePackFormatter();
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeMessagePackFormatter()
    {
      return _messagePackFormatter.Deserialize<T>(_messagePackData1);
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
      _utf8JsonData = Utf8Json.JsonSerializer.Serialize(_value);
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeUtf8Json()
    {
      return Utf8Json.JsonSerializer.Deserialize<T>(_utf8JsonData);
    }

    #endregion

    #region -- Utf5JsonFormatter --

    private static readonly Utf8JsonMessageFormatter _utf8JsonFormatter = Utf8JsonMessageFormatter.DefaultInstance;
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SerializeUtf8JsonFormatter()
    {
      return _utf8JsonFormatter.SerializeObject(_value);
    }
    private byte[] _utf8JsonData1;
    [GlobalSetup(Target = nameof(DeserializeUtf8JsonFormatter))]
    public void SetupDeserializeUtf8JsonFormatter()
    {
      _utf8JsonData1 = SerializeUtf8JsonFormatter();
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeUtf8JsonFormatter()
    {
      return _utf8JsonFormatter.Deserialize<T>(_utf8JsonData1);
    }

    #endregion

    #region -- JsonConvert --

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SerializeJsonNet()
    {
      return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_value, _jsonSerializerSettings));
    }
    private byte[] _jsonData;
    [GlobalSetup(Target = nameof(DeserializeJsonNet))]
    public void SetupDeserializeJsonNet()
    {
      _jsonData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_value, _jsonSerializerSettings));
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeJsonNet()
    {
      return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(_jsonData), _jsonSerializerSettings);
    }

    #endregion

    #region -- JsonConvertX --

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SerializeJsonNetX()
    {
      return JsonConvertX.SerializeToByteArray(_value, _jsonSerializerSettings);
    }
    private byte[] _jsonData1;
    [GlobalSetup(Target = nameof(DeserializeJsonNetX))]
    public void SetupDeserializeJsonNetX()
    {
      _jsonData1 = JsonConvertX.SerializeToByteArray(_value, _jsonSerializerSettings);
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeJsonNetX()
    {
      return JsonConvertX.DeserializeFromByteArray<T>(_jsonData1, _jsonSerializerSettings);
    }

    #endregion

    #region -- JsonFormatter --

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SerializeJsonFormatter()
    {
      return _jsonFormatter.SerializeObject(_value);
    }
    private byte[] _jsonData2;
    [GlobalSetup(Target = nameof(DeserializeJsonFormatter))]
    public void SetupDeserializeJsonFormatter()
    {
      _jsonData2 = _jsonFormatter.SerializeObject(_value);
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeJsonFormatter()
    {
      return _jsonFormatter.Deserialize<T>(_jsonData2);
    }

    #endregion

    #region -- ProtoBuf --

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SerializeProtoBufNet()
    {
      var s = new MemoryStream();
      ProtoBuf.Serializer.Serialize(s, _value);
      return s.ToArray();
    }
    private byte[] _protoBufData;
    [GlobalSetup(Target = nameof(DeserializeProtoBufNet))]
    public void SetupDeserializeProtoBufNet()
    {
      _protoBufData = SerializeProtoBufNet();
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeProtoBufNet()
    {
      var ms = new MemoryStream(_protoBufData);
      return ProtoBuf.Serializer.Deserialize<T>(ms);
    }

    #endregion

    #region -- ProtoBufFormatter --

    private static readonly ProtoBufMessageFormatter _protobufFormatter = ProtoBufMessageFormatter.DefaultInstance;
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SerializeProtoBufFormatter()
    {
      return _protobufFormatter.SerializeObject(_value);
    }
    private byte[] _protoBufData1;
    [GlobalSetup(Target = nameof(DeserializeProtoBufFormatter))]
    public void SetupDeserializeProtoBufFormatter()
    {
      _protoBufData1 = SerializeProtoBufFormatter();
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeProtoBufFormatter()
    {
      return _protobufFormatter.Deserialize<T>(_protoBufData1);
    }

    #endregion

    #region -- Hyperion --

    private Hyperion.Serializer _serializer;
    [GlobalSetup(Target = nameof(SerializeHyperion))]
    public void SetupSerializeHyperion()
    {
      var options = new Hyperion.SerializerOptions(
          versionTolerance: true,
          preserveObjectReferences: true
      );
      _serializer = new Hyperion.Serializer(options);
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SerializeHyperion()
    {
      var s = new MemoryStream();
      _serializer.Serialize(_value, s);
      return s.ToArray();
    }


    private Hyperion.Serializer _serializer1;
    private byte[] _hyperionData;
    [GlobalSetup(Target = nameof(DeserializeHyperion))]
    public void SetupDeserializeHyperion()
    {
      var options = new Hyperion.SerializerOptions(
          versionTolerance: true,
          preserveObjectReferences: true
      );
      _serializer1 = new Hyperion.Serializer(options);
      var s = new MemoryStream();
      _serializer1.Serialize(_value, s);
      _hyperionData = s.ToArray();
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeHyperion()
    {
      var ms = new MemoryStream(_hyperionData);
      return _serializer1.Deserialize<T>(ms);
    }

    #endregion

    #region -- HyperionFormatter --

    private static readonly HyperionMessageFormatter _hyperionFormatter = HyperionMessageFormatter.DefaultInstance;
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public byte[] SerializeHyperionFormatter()
    {
      return _hyperionFormatter.SerializeObject(_value);
    }
    private byte[] _hyperionData1;
    [GlobalSetup(Target = nameof(DeserializeHyperionFormatter))]
    public void SetupDeserializeHyperionFormatter()
    {
      _hyperionData1 = SerializeProtoBufFormatter();
    }
    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public T DeserializeHyperionFormatter()
    {
      return _hyperionFormatter.Deserialize<T>(_hyperionData1);
    }

    #endregion
  }
}