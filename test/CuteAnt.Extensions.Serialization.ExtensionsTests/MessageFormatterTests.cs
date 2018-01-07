using System.IO;
using System.Threading.Tasks;
using CuteAnt.Buffers;
using Xunit;

namespace CuteAnt.Extensions.Serialization.Tests
{
  public abstract class SerializeTestBase
  {
    protected IMessageFormatter _formatter;
    private bool _igoreError;
    public SerializeTestBase(IMessageFormatter formatter, bool igoreError = false)
    {
      _formatter = formatter;
      _igoreError = igoreError;
    }

    [Fact]
    public void DeepCopyTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var newPoco = (SerializerPocoSerializable)_formatter.DeepCopy(poco);
      Helper.ComparePoco(poco, newPoco);
    }

    [Fact]
    public void EmptyStreamTest()
    {
      var ms = new MemoryStream();
      _formatter.WriteToStream(typeof(SerializerPocoSerializable), default(SerializerPocoSerializable), ms);
      ms.Position = 0;
      var obj = _formatter.ReadFromStream(typeof(SerializerPocoSerializable), ms);
      Assert.Null(obj);
    }

    [Fact]
    public void SerializeToBytesTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = _formatter.SerializeToBytes(poco);
      var newPoco = _formatter.DeserializeFromBytes<SerializerPocoSerializable>(serializedObject);
      Helper.ComparePoco(poco, newPoco);
    }

    [Fact]
    public async Task SerializeToBytesAsyncTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = await _formatter.SerializeToBytesAsync(poco);
      var newPoco = await _formatter.DeserializeFromBytesAsync<SerializerPocoSerializable>(serializedObject);
      Helper.ComparePoco(poco, newPoco);
    }

    [Fact]
    public void SerializeToByteArraySegmentTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = _formatter.SerializeToByteArraySegment(poco);
      var newPoco = _formatter.DeserializeFromBytes<SerializerPocoSerializable>(serializedObject.Array, serializedObject.Offset, serializedObject.Count);
      Helper.ComparePoco(poco, newPoco);
      BufferManager.Shared.Return(serializedObject.Array);
    }

    [Fact]
    public async Task SerializeToByteArraySegmentAsyncTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = await _formatter.SerializeToByteArraySegmentAsync(poco);
      var newPoco = await _formatter.DeserializeFromBytesAsync<SerializerPocoSerializable>(serializedObject.Array, serializedObject.Offset, serializedObject.Count);
      Helper.ComparePoco(poco, newPoco);
      BufferManager.Shared.Return(serializedObject.Array);
    }
  }

  public class JsonMessageFormatterTest : SerializeTestBase
  {
    public JsonMessageFormatterTest() : base(JsonMessageFormatter.DefaultInstance) { }
  }

  public class ProtoBufMessageFormatterTest : SerializeTestBase
  {
    public ProtoBufMessageFormatterTest() : base(ProtoBufMessageFormatter.DefaultInstance) { }
  }

  public class HyperioneMessageFormatterTest : SerializeTestBase
  {
    public HyperioneMessageFormatterTest() : base(HyperionMessageFormatter.DefaultInstance) { }
  }

  public class ServiceStackMessageFormatterTest : SerializeTestBase
  {
    public ServiceStackMessageFormatterTest() : base(ServiceStackMessageFormatter.DefaultInstance) { }
  }

  public class CsvMessageFormatterTest : SerializeTestBase
  {
    public CsvMessageFormatterTest() : base(CsvMessageFormatter.DefaultInstance, true) { }
  }

  public class JsvMessageFormatterTest : SerializeTestBase
  {
    public JsvMessageFormatterTest() : base(JsvMessageFormatter.DefaultInstance) { }
  }

  public class XmlMessageFormatterTest : SerializeTestBase
  {
    public XmlMessageFormatterTest() : base(XmlMessageFormatter.DefaultInstance) { }
  }

  public class BinaryMessageFormatterTest : SerializeTestBase
  {
    public BinaryMessageFormatterTest() : base(BinaryMessageFormatter.DefaultInstance) { }
  }
}