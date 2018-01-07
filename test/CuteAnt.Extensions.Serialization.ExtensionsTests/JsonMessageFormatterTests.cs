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
      var serializedObject = _formatter.SerializeToBytes(poco, Encoding.UTF8);
      var newPoco = _formatter.DeserializeFromBytes<SerializerPocoSerializable>(serializedObject, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
      serializedObject = _formatter.SerializeToBytes(poco, JsonConvertX.IncludeTypeNameSettings, Encoding.UTF8);
      newPoco = _formatter.DeserializeFromBytes<SerializerPocoSerializable>(serializedObject, JsonConvertX.IncludeTypeNameSettings, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
    }

    [Fact]
    public async Task SerializeToBytesAsyncTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = await _formatter.SerializeToBytesAsync(poco, Encoding.UTF8);
      var newPoco = await _formatter.DeserializeFromBytesAsync<SerializerPocoSerializable>(serializedObject, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
      serializedObject = await _formatter.SerializeToBytesAsync(poco, JsonConvertX.IncludeTypeNameSettings, Encoding.UTF8);
      newPoco = await _formatter.DeserializeFromBytesAsync<SerializerPocoSerializable>(serializedObject, JsonConvertX.IncludeTypeNameSettings, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
    }

    [Fact]
    public void SerializeToByteArraySegmentTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = _formatter.SerializeToByteArraySegment(poco, Encoding.UTF8);
      var newPoco = _formatter.DeserializeFromBytes<SerializerPocoSerializable>(serializedObject.Array, serializedObject.Offset, serializedObject.Count, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
      BufferManager.Shared.Return(serializedObject.Array);
      serializedObject = _formatter.SerializeToByteArraySegment(poco, JsonConvertX.IncludeTypeNameSettings, Encoding.UTF8);
      newPoco = _formatter.DeserializeFromBytes<SerializerPocoSerializable>(serializedObject.Array, serializedObject.Offset, serializedObject.Count, JsonConvertX.IncludeTypeNameSettings, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
      BufferManager.Shared.Return(serializedObject.Array);
    }

    [Fact]
    public async Task SerializeToByteArraySegmentAsyncTest()
    {
      var poco = SerializerPocoSerializable.Create();
      var serializedObject = await _formatter.SerializeToByteArraySegmentAsync(poco, Encoding.UTF8);
      var newPoco = await _formatter.DeserializeFromBytesAsync<SerializerPocoSerializable>(serializedObject.Array, serializedObject.Offset, serializedObject.Count, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
      BufferManager.Shared.Return(serializedObject.Array);
      serializedObject = await _formatter.SerializeToByteArraySegmentAsync(poco, JsonConvertX.IncludeTypeNameSettings, Encoding.UTF8);
      newPoco = await _formatter.DeserializeFromBytesAsync<SerializerPocoSerializable>(serializedObject.Array, serializedObject.Offset, serializedObject.Count, JsonConvertX.IncludeTypeNameSettings, Encoding.UTF8);
      Helper.ComparePoco(poco, newPoco);
      BufferManager.Shared.Return(serializedObject.Array);
    }
  }
}