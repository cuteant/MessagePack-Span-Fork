using System.IO;
using CuteAnt.Buffers;
using Newtonsoft.Json;
using Xunit;

namespace CuteAnt.Extensions.Serialization.Tests
{
  public class JsonConvertXTests
  {
    [Fact]
    public void SerializeToBytesTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = JsonConvertX.SerializeToByteArray(poco);
      var newPoco = JsonConvertX.DeserializeFromByteArray<SerializerPocoSerializable>(serializedObject);
      Helper.ComparePoco(poco, newPoco);
    }

    [Fact]
    public void SerializeToArraySegmentTest1()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = JsonConvertX.SerializeToMemoryPool(poco);
      var newPoco = JsonConvertX.DeserializeFromByteArray<SerializerPocoSerializable>(serializedObject.Array, serializedObject.Offset, serializedObject.Count);
      BufferManager.Shared.Return(serializedObject.Array);
      Helper.ComparePoco(poco, newPoco);
    }

    [Fact]
    public void SerializeToStreamTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var ms = new MemoryStream();
      JsonConvertX.SerializeToStream(ms, poco);
      ms.Seek(0, SeekOrigin.Begin);
      var newPoco = JsonConvertX.DeserializeFromStream<SerializerPocoSerializable>(ms);
      Helper.ComparePoco(poco, newPoco);
    }

    [Fact]
    public void SerializeToTextWriterTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var sw = new StringWriter();
      JsonConvertX.SerializeToWriter(sw, poco);
      var sr = new StringReader(sw.ToString());
      var newPoco = JsonConvertX.DeserializeFromReader<SerializerPocoSerializable>(sr);
      Helper.ComparePoco(poco, newPoco);
    }
  }
}