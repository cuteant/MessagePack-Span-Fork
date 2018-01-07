using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using Xunit;

namespace CuteAnt.Extensions.Serialization.Tests
{
  internal class Helper
  {
    public static void ComparePoco(SerializerPocoSerializable expected, SerializerPocoSerializable actual)
    {
      Assert.Equal(expected.StringProperty, actual.StringProperty);
      Assert.Equal(expected.IntProperty, actual.IntProperty);
      Assert.Equal(expected.StringArrayProperty, actual.StringArrayProperty);
      Assert.Equal(expected.StringListProperty, actual.StringListProperty);
      Assert.Equal(expected.ChildDictionaryProperty.Keys.ToArray(), actual.ChildDictionaryProperty.Keys.ToArray());
      Assert.Equal((expected.ChildDictionaryProperty.Values.ToArray())[0].StringProperty, (actual.ChildDictionaryProperty.Values.ToArray())[0].StringProperty);
    }
  }

  [Serializable]
  [ProtoContract]
  public class SerializerPocoSerializable
  {
    [ProtoMember(1)]
    public string StringProperty { get; set; }

    [ProtoMember(2)]
    public int IntProperty { get; set; }

    [ProtoMember(3)]
    public string[] StringArrayProperty { get; set; }

    [ProtoMember(4)]
    public List<string> StringListProperty { get; set; }

    [ProtoMember(5)]
    public Dictionary<string, ChildPocco> ChildDictionaryProperty { get; set; }

    public static SerializerPocoSerializable Create()
    {
      return new SerializerPocoSerializable()
      {
        StringProperty = "a",
        IntProperty = 2018,
        StringArrayProperty = new string[] { "foo", "bar" },
        StringListProperty = new List<string>(new string[] { "hello", "world" }),
        ChildDictionaryProperty = new Dictionary<string, ChildPocco>()
        {
          { "king", new ChildPocco() { StringProperty = "good" } },
        }
      };
    }
  }

  [Serializable]
  [ProtoContract]
  public class ChildPocco
  {
    [ProtoMember(1)]
    public string StringProperty { get; set; }
  }
}
