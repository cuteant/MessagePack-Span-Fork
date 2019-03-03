using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuteAnt.Buffers;
using Newtonsoft.Json;
using Xunit;

namespace CuteAnt.Extensions.Serialization.Tests
{
  public class JsonMessageFormatterTests
  {
    protected IJsonMessageFormatter _formatter;
    public JsonMessageFormatterTests()
    {
      _formatter = JsonMessageFormatter.DefaultInstance;
    }

    [Fact]
    public void SerializeToBytesTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = _formatter.Serialize(poco, Encoding.UTF8);
      var newPoco = _formatter.Deserialize<SerializerPocoSerializable>(serializedObject, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
      serializedObject = _formatter.Serialize(poco, Encoding.UTF8);
      newPoco = _formatter.Deserialize<SerializerPocoSerializable>(serializedObject, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
    }

    [Fact]
    public async Task SerializeToBytesAsyncTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = await _formatter.SerializeAsync(poco, Encoding.UTF8);
      var newPoco = await _formatter.DeserializeAsync<SerializerPocoSerializable>(serializedObject, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
      serializedObject = await _formatter.SerializeAsync(poco, Encoding.UTF8);
      newPoco = await _formatter.DeserializeAsync<SerializerPocoSerializable>(serializedObject, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
    }

    [Fact]
    public void SerializeToByteArraySegmentTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = _formatter.WriteToMemoryPool(poco, Encoding.UTF8);
      var newPoco = _formatter.Deserialize<SerializerPocoSerializable>(serializedObject.Array, serializedObject.Offset, serializedObject.Count, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
      BufferManager.Shared.Return(serializedObject.Array);
      serializedObject = _formatter.WriteToMemoryPool(poco, Encoding.UTF8);
      newPoco = _formatter.Deserialize<SerializerPocoSerializable>(serializedObject.Array, serializedObject.Offset, serializedObject.Count, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
      BufferManager.Shared.Return(serializedObject.Array);
    }

    [Fact]
    public async Task SerializeToByteArraySegmentAsyncTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = await _formatter.WriteToMemoryPoolAsync(poco, Encoding.UTF8);
      var newPoco = await _formatter.DeserializeAsync<SerializerPocoSerializable>(serializedObject.Array, serializedObject.Offset, serializedObject.Count, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
      BufferManager.Shared.Return(serializedObject.Array);
      serializedObject = await _formatter.WriteToMemoryPoolAsync(poco, Encoding.UTF8);
      newPoco = await _formatter.DeserializeAsync<SerializerPocoSerializable>(serializedObject.Array, serializedObject.Offset, serializedObject.Count, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
      BufferManager.Shared.Return(serializedObject.Array);
    }
  }
}