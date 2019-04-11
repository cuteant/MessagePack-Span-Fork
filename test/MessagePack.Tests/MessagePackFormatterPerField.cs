using System;
using System.Reflection;
using MessagePack.Formatters;
using Xunit;

namespace MessagePack.Tests
{
    public class MessagePackFormatterPerFieldTest
    {
        [MessagePackObject]
        public class MyClass
        {
            [Key(0)]
            [MessagePackFormatter(typeof(Int_x10Formatter))]
            public int MyProperty1 { get; set; }
            [Key(1)]
            public int MyProperty2 { get; set; }
            [Key(2)]
            [MessagePackFormatter(typeof(String_x2Formatter))]
            public string MyProperty3 { get; set; }
            [Key(3)]
            public string MyProperty4 { get; set; }
        }

        [MessagePackObject]
        public struct MyStruct
        {
            [Key(0)]
            [MessagePackFormatter(typeof(Int_x10Formatter))]
            public int MyProperty1 { get; set; }
            [Key(1)]
            public int MyProperty2 { get; set; }
            [Key(2)]
            [MessagePackFormatter(typeof(String_x2Formatter))]
            public string MyProperty3 { get; set; }
            [Key(3)]
            public string MyProperty4 { get; set; }
        }

        public class Int_x10Formatter : IMessagePackFormatter<int>
        {
            public int Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
            {
                return reader.ReadInt32() * 10;
            }

            public void Serialize(ref MessagePackWriter writer, ref int idx, int value, IFormatterResolver formatterResolver)
            {
                writer.WriteInt32(value * 10, ref idx);
            }
        }

        public class String_x2Formatter : IMessagePackFormatter<string>
        {
            public string Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
            {
                var s = reader.ReadString();
                return s + s;
            }

            public void Serialize(ref MessagePackWriter writer, ref int idx, string value, IFormatterResolver formatterResolver)
            {
                writer.WriteString(value + value, ref idx);
            }
        }


        [Fact]
        public void FooBar()
        {
            {
                var bin = MessagePack.MessagePackSerializer.Serialize(new MyClass { MyProperty1 = 100, MyProperty2 = 9, MyProperty3 = "foo", MyProperty4 = "bar" });
                var json = MessagePackSerializer.ToJson(bin);
                json.Is("[1000,9,\"foofoo\",\"bar\"]");

                var r2 = MessagePackSerializer.Deserialize<MyClass>(bin);
                r2.MyProperty1.Is(10000);
                r2.MyProperty2.Is(9);
                r2.MyProperty3.Is("foofoofoofoo");
                r2.MyProperty4.Is("bar");
            }
            {
                var bin = MessagePack.MessagePackSerializer.Serialize(new MyStruct { MyProperty1 = 100, MyProperty2 = 9, MyProperty3 = "foo", MyProperty4 = "bar" });
                var json = MessagePackSerializer.ToJson(bin);
                json.Is("[1000,9,\"foofoo\",\"bar\"]");

                var r2 = MessagePackSerializer.Deserialize<MyStruct>(bin);
                r2.MyProperty1.Is(10000);
                r2.MyProperty2.Is(9);
                r2.MyProperty3.Is("foofoofoofoo");
                r2.MyProperty4.Is("bar");
            }
        }

        [Fact]
        public void CreateNestedClass()
        {
            var obj = (MyClass)Activator.CreateInstance(typeof(MyClass));
            Assert.NotNull(obj);
            var attr = typeof(MyClass).GetCustomAttribute<MessagePackObjectAttribute>();
            Assert.NotNull(attr);
            var attr1 = typeof(MyClass).GetCustomAttribute<MessagePackObjectAttribute>();
            Assert.NotNull(attr1);
        }
    }
}
