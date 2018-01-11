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
      var newPoco = _formatter.DeepCopy(poco);
      Helper.ComparePoco(poco, newPoco);
      newPoco = (SerializerPocoSerializable)_formatter.DeepCopyObject((object)poco);
      Helper.ComparePoco(poco, newPoco);
    }

    [Fact]
    public void SerializeWithoutTypeTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var ms = new MemoryStream();
      _formatter.WriteToStream(null, poco, ms);
      ms.Position = 0;
      var newpoco = (SerializerPocoSerializable)_formatter.ReadFromStream(typeof(SerializerPocoSerializable), ms);
      Helper.ComparePoco(poco, newpoco);
      ms.Position = 0;
      newpoco = _formatter.ReadFromStream<SerializerPocoSerializable>(ms);
      Helper.ComparePoco(poco, newpoco);
    }

    [Fact]
    public void SerializeWithoutTypeTest1()
    {
      var poco = SerializerPocoSerializable.Create();
      var ms = new MemoryStream();
      _formatter.WriteToStream((object)poco, ms);
      ms.Position = 0;
      var newpoco = (SerializerPocoSerializable)_formatter.ReadFromStream(typeof(SerializerPocoSerializable), ms);
      Helper.ComparePoco(poco, newpoco);
      ms.Position = 0;
      newpoco = _formatter.ReadFromStream<SerializerPocoSerializable>(ms);
      Helper.ComparePoco(poco, newpoco);
    }

    [Fact]
    public void SerializeWithoutTypeTest2()
    {
      var poco = SerializerPocoSerializable.Create();
      var ms = new MemoryStream();
      _formatter.WriteToStream(poco, ms);
      ms.Position = 0;
      var newpoco = (SerializerPocoSerializable)_formatter.ReadFromStream(typeof(SerializerPocoSerializable), ms);
      Helper.ComparePoco(poco, newpoco);
      ms.Position = 0;
      newpoco = _formatter.ReadFromStream<SerializerPocoSerializable>(ms);
      Helper.ComparePoco(poco, newpoco);
    }

#if !TEST40
    [Fact]
    public async Task SerializeWithoutTypeAsyncTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var ms = new MemoryStream();
      await _formatter.WriteToStreamAsync(null, poco, ms, null);
      ms.Position = 0;
      var newpoco = await _formatter.ReadFromStreamAsync<SerializerPocoSerializable>(ms, null);
      Helper.ComparePoco(poco, newpoco);
    }
#endif

    [Fact]
    public void EmptyTypeAndPocoTest()
    {
      var ms = new MemoryStream();
      _formatter.WriteToStream(null, default(SerializerPocoSerializable), ms);
      ms.Position = 0;
      var obj = _formatter.ReadFromStream(null, ms);
      Assert.Null(obj);
    }

#if !TEST40
    [Fact]
    public async Task EmptyTypeAndPocoTestAsync()
    {
      var ms = new MemoryStream();
      await _formatter.WriteToStreamAsync(null, default(SerializerPocoSerializable), ms, null);
      ms.Position = 0;
      var obj = await _formatter.ReadFromStreamAsync(null, ms, null);
      Assert.Null(obj);
    }
#endif

    [Fact]
    public void EmptyStreamTest()
    {
      InternalEmptyStreamTest();
    }

    protected virtual void InternalEmptyStreamTest()
    {
      var ms = new MemoryStream();
      _formatter.WriteToStream(typeof(SerializerPocoSerializable), default(SerializerPocoSerializable), ms);
      ms.Position = 0;
      var obj = _formatter.ReadFromStream(typeof(SerializerPocoSerializable), ms);
      Assert.Null(obj);

      var emptyBytes = _formatter.SerializeObject(default(SerializerPocoSerializable));
      Assert.Empty(emptyBytes);

      var emptySegment = _formatter.WriteToMemoryPool(default(SerializerPocoSerializable));
      Assert.True(0 == emptySegment.Count);
      Assert.Empty(emptySegment.Array);
    }

#if !TEST40
    [Fact]
    public Task EmptyStreamAsyncTest()
    {
      return InternalEmptyStreamAsyncTest();
    }

    public virtual async Task InternalEmptyStreamAsyncTest()
    {
      var ms = new MemoryStream();
      await _formatter.WriteToStreamAsync(typeof(SerializerPocoSerializable), default(SerializerPocoSerializable), ms, null);
      ms.Position = 0;
      var obj = await _formatter.ReadFromStreamAsync(typeof(SerializerPocoSerializable), ms, null);
      Assert.Null(obj);

      var emptyBytes = await _formatter.SerializeObjectAsync(default(SerializerPocoSerializable));
      Assert.Empty(emptyBytes);

      var emptySegment = await _formatter.WriteToMemoryPoolAsync(default(SerializerPocoSerializable));
      Assert.True(0 == emptySegment.Count);
      Assert.Empty(emptySegment.Array);
    }
#endif

    [Fact]
    public void SerializeToBytesTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = _formatter.SerializeObject(poco);
      var newPoco = _formatter.Deserialize<SerializerPocoSerializable>(serializedObject);
      Helper.ComparePoco(poco, newPoco);
    }

    [Fact]
    public async Task SerializeToBytesAsyncTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = await _formatter.SerializeObjectAsync(poco);
      var newPoco = await _formatter.DeserializeAsync<SerializerPocoSerializable>(serializedObject);
      Helper.ComparePoco(poco, newPoco);
    }

    [Fact]
    public void SerializeToByteArraySegmentTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = _formatter.WriteToMemoryPool(poco);
      var newPoco = _formatter.Deserialize<SerializerPocoSerializable>(serializedObject.Array, serializedObject.Offset, serializedObject.Count);
      Helper.ComparePoco(poco, newPoco);
      BufferManager.Shared.Return(serializedObject.Array);
    }

    [Fact]
    public async Task SerializeToByteArraySegmentAsyncTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = await _formatter.WriteToMemoryPoolAsync(poco);
      var newPoco = await _formatter.DeserializeAsync<SerializerPocoSerializable>(serializedObject.Array, serializedObject.Offset, serializedObject.Count);
      Helper.ComparePoco(poco, newPoco);
      BufferManager.Shared.Return(serializedObject.Array);
    }
  }

  public class JsonMessageFormatterTest : SerializeTestBase
  {
    public JsonMessageFormatterTest() : base(JsonMessageFormatter.DefaultInstance) { }

    protected override void InternalEmptyStreamTest()
    {
      var ms = new MemoryStream();
      _formatter.WriteToStream(typeof(SerializerPocoSerializable), default(SerializerPocoSerializable), ms);
      ms.Position = 0;
      var obj = _formatter.ReadFromStream(typeof(SerializerPocoSerializable), ms);
      Assert.Null(obj);
    }

#if !TEST40
    public override async Task InternalEmptyStreamAsyncTest()
    {
      var ms = new MemoryStream();
      await _formatter.WriteToStreamAsync(typeof(SerializerPocoSerializable), default(SerializerPocoSerializable), ms, null);
      ms.Position = 0;
      var obj = await _formatter.ReadFromStreamAsync(typeof(SerializerPocoSerializable), ms, null);
      Assert.Null(obj);
    }
#endif
  }

  public class MessagePackMessageFormatterTest : SerializeTestBase
  {
    public MessagePackMessageFormatterTest() : base(MessagePackMessageFormatter.DefaultInstance) { }
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

  public class BinaryMessageFormatterTest : SerializeTestBase
  {
    public BinaryMessageFormatterTest() : base(BinaryMessageFormatter.DefaultInstance) { }
  }
}