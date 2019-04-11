using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using Xunit;

namespace MessagePack.Tests.ExtensionTests
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

    public class Bar
    {
        public IFoo Foo { get; set; }
    }

    public class Bar0
    {
        [MessagePackFormatter(typeof(MessagePack.Formatters.TypelessFormatter))]
        public IFoo Foo { get; set; }
    }

    public class Bar00
    {
        [MessagePackFormatter(typeof(MessagePack.Formatters.TypelessFormatter))]
        public IFoo0 Foo { get; set; }
    }

    public class Bar1
    {
        [MessagePackFormatter(typeof(MessagePack.Formatters.TypelessFormatter))]
        public object Foo { get; set; }
    }

    public class Bar2
    {
        [MessagePackFormatter(typeof(MessagePack.Formatters.TypelessFormatter))]
        public object Foo { get; set; }
    }

    public interface IFoo0
    {
        int A { get; set; }
        //string B { get; set; }
        string Fact();
        string Fact(string input);
        string Fact1<T>(T input);
    }

    [Union(0, typeof(Foo))]
    public interface IFoo
    {
        int A { get; set; }
        //string B { get; set; }
        string Fact();
        string Fact(string input);
        string Fact1<T>(T input);
    }

    [MessagePackObject]
    public class Foo : IFoo, IFoo0
    {
        [Key(0)]
        public int A { get; set; }
        [Key(1)]
        public string B { get; set; }
        public string Fact() => "Hello";
        public string Fact(string input) => input;
        public string Fact1<T>(T input) => input.ToString();
    }


    [MessagePackObject]
    [DataContract]
    public class FooBar
    {
        [Key(0), DataMember(Name = "foo")]
        public int FooProperty { get; set; }

        [IgnoreMember, IgnoreDataMember]
        public string BarProperty { get; set; }
    }

    [MessagePackObject]
    [DataContract]
    public class FooBar1
    {
        [Key("fo"), DataMember(Name = "foo")]
        public int FooProperty { get; set; }

        [IgnoreMember, IgnoreDataMember]
        public string BarProperty { get; set; }
    }

    [DataContract]
    public class FooBar2
    {
        [DataMember(Name = "f")]
        public int FooProperty { get; set; }

        [IgnoreDataMember]
        public string BarProperty { get; set; }
    }

    [MessagePackObject(true)]
    [DataContract]
    public class FooBar3
    {
        [Key("fo"), DataMember(Name = "foo")]
        public int FooProperty { get; set; }

        [IgnoreMember, IgnoreDataMember]
        public string BarProperty { get; set; }
    }

    [Serializable]
    [MessagePackObject]
    public class SerializerPocoSerializable
    {
        [Key(0)]
        public string StringProperty { get; set; }

        [Key(1)]
        public int IntProperty { get; set; }

        [Key(2)]
        public string[] StringArrayProperty { get; set; }

        [Key(3)]
        public List<string> StringListProperty { get; set; }

        [Key(4)]
        public Dictionary<string, ChildPocco> ChildDictionaryProperty { get; set; }

        public static SerializerPocoSerializable Create()
        {
            return new SerializerPocoSerializable()
            {
                StringProperty = "a",
                IntProperty = 2018,
                StringArrayProperty = new string[] { "foo", "bar" },
                StringListProperty = new List<string>(new string[] { "风雨送春归", "飞雪迎春到", "已是悬崖百丈冰", "犹有花枝俏", "俏也不争春", "只把春来报", "待到山花烂漫时", "她在丛中笑" }),
                ChildDictionaryProperty = new Dictionary<string, ChildPocco>()
        {
          { "king", new ChildPocco() { StringProperty = "good" } },
        }
            };
        }
    }

    [Serializable]
    [MessagePackObject]
    public class ChildPocco
    {
        [Key(0)]
        public string StringProperty { get; set; }
    }

    [MessagePack.Union(0, typeof(FooClass))]
    [MessagePack.Union(1, typeof(BarClass))]
    public interface IUnionSample
    {
    }

    [MessagePackObject]
    public class FooClass : IUnionSample
    {
        [Key(0)]
        public int XYZ { get; set; }
    }

    [MessagePackObject]
    internal class BarClass : IUnionSample
    {
        [Key(0)]
        public string OPQ { get; set; }
    }

    [Union(0, typeof(SubUnionType1))]
    [Union(1, typeof(SubUnionType2))]
    [MessagePackObject]
    public abstract class ParentUnionType
    {
        [Key(0)]
        public Guid MyProperty { get; set; }
    }

    [MessagePackObject]
    public class SubUnionType1 : ParentUnionType
    {
        [Key(1)]
        public int MyProperty1 { get; set; }
    }

    [MessagePackObject]
    internal class SubUnionType2 : ParentUnionType
    {
        [Key(1)]
        public int MyProperty2 { get; set; }
    }
}
