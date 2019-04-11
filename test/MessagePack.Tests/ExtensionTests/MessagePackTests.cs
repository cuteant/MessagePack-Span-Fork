using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Net;
using MessagePack.ImmutableCollection;
using MessagePack.Resolvers;
using Xunit;

namespace MessagePack.Tests.ExtensionTests
{
    public class MessagePackTests
    {
        static MessagePackTests()
        {
            MessagePackStandardResolver.TryRegister(ImmutableCollectionResolver.Instance);
        }

        [Fact]
        public void SerializeInterfaceTest()
        {
            IUnionSample foo = new FooClass { XYZ = 9999 };

            var bytes = MessagePackSerializer.Serialize(foo);
            // 实际开发中，是无法为接口配置所有实现类的 Union 特性的，虚拟基类也是如此
            var newfoo1 = MessagePackSerializer.Deserialize<IUnionSample>(bytes);
            Assert.NotNull(newfoo1);
            Assert.IsType<FooClass>(newfoo1);
            Assert.Equal(9999, ((FooClass)newfoo1).XYZ);
            // 实际应用中，无法确定消费者是按哪个类型（IUnionSample or FooClass）来反序列化的
            var newFoo = MessagePackSerializer.Deserialize<FooClass>(bytes);
            // 已出错，XYZ按正常逻辑应为9999
            Assert.Equal(0, newFoo.XYZ);

            bytes = MessagePackSerializer.Serialize<object>(foo, DefaultResolver.Instance);
            newFoo = MessagePackSerializer.Deserialize<FooClass>(bytes, DefaultResolver.Instance);
            Assert.Equal(9999, newFoo.XYZ);

            bytes = MessagePackSerializer.Serialize<object>(foo, DefaultResolver.Instance);
            newFoo = (FooClass)MessagePackSerializer.NonGeneric.Deserialize(typeof(FooClass), bytes, DefaultResolver.Instance);
            Assert.Equal(9999, newFoo.XYZ);

            bytes = MessagePackSerializer.Typeless.Serialize(foo);
            newFoo = (FooClass)MessagePackSerializer.Typeless.Deserialize(bytes);
            Assert.Equal(9999, newFoo.XYZ);

            var bar = new BarClass { OPQ = "t" };
            bytes = MessagePackSerializer.Serialize(bar, ContractlessStandardResolverAllowPrivate.Instance);
            var newBar = MessagePackSerializer.Deserialize<BarClass>(bytes, ContractlessStandardResolverAllowPrivate.Instance);
            Assert.Equal(bar.OPQ, newBar.OPQ);

            Assert.Throws<InvalidOperationException>(() => MessagePackStandardResolver.Register(ImmutableCollectionResolver.Instance));
        }

        [Fact]
        public void SerializeClassTest()
        {
            var guid = Guid.NewGuid();
            Console.WriteLine(guid);
            ParentUnionType subUnionType1 = new SubUnionType1 { MyProperty = guid, MyProperty1 = 20 };
            var bytes = MessagePackSerializer.Serialize(subUnionType1);
            var newSubUnionType = MessagePackSerializer.Deserialize<ParentUnionType>(bytes);
            Assert.NotNull(newSubUnionType);
            Assert.IsType<SubUnionType1>(newSubUnionType);
            Assert.Equal(guid, newSubUnionType.MyProperty);

            // 实际应用中，无法确定消费者是按哪个类型（ParentUnionType or SubUnionType1）来反序列化的
            Assert.Throws<InvalidOperationException>(() => MessagePackSerializer.Deserialize<SubUnionType1>(bytes));

            bytes = MessagePackSerializer.Serialize<object>(subUnionType1, DefaultResolver.Instance);
            var newSubUnionType1 = MessagePackSerializer.Deserialize<SubUnionType1>(bytes);
            Assert.Equal(guid, newSubUnionType1.MyProperty); Assert.Equal(20, newSubUnionType1.MyProperty1);

            bytes = MessagePackSerializer.Typeless.Serialize(subUnionType1);
            newSubUnionType1 = (SubUnionType1)MessagePackSerializer.Typeless.Deserialize(bytes);
            Assert.Equal(guid, newSubUnionType1.MyProperty); Assert.Equal(20, newSubUnionType1.MyProperty1);

            var copy = (SubUnionType1)MessagePackSerializer.DeepCopy<object>(subUnionType1);
            Assert.Equal(guid, copy.MyProperty); Assert.Equal(20, copy.MyProperty1);
            copy = (SubUnionType1)MessagePackSerializer.Typeless.DeepCopy(subUnionType1);
            Assert.Equal(guid, copy.MyProperty); Assert.Equal(20, copy.MyProperty1);
        }

        [Fact]
        public void SerializeImmutableCollectionTest()
        {
            var imList = ImmutableList<int>.Empty.AddRange(new[] { 1, 2 });
            var bytes = MessagePackSerializer.Serialize(imList, WithImmutableDefaultResolver.Instance);
            var newList = MessagePackSerializer.Deserialize<ImmutableList<int>>(bytes, WithImmutableDefaultResolver.Instance);
            Assert.Equal(imList, newList);

            // 此时如果序列化 Object 对象，则无法正确序列化，说明官方的 CompositeResolver 所采用的策略还是有问题的
            Assert.Throws<System.Reflection.TargetInvocationException>(() => MessagePackSerializer.Serialize((object)imList));

            bytes = MessagePackSerializer.Serialize<object>(imList, DefaultResolver.Instance);
            newList = MessagePackSerializer.Deserialize<ImmutableList<int>>(bytes, DefaultResolver.Instance);
            Assert.Equal(imList, newList);

            bytes = MessagePackSerializer.Serialize((object)imList, DefaultResolver.Instance);
            newList = MessagePackSerializer.Deserialize<ImmutableList<int>>(bytes, DefaultResolver.Instance);
            Assert.Equal(imList, newList);

            newList = (ImmutableList<int>)MessagePackSerializer.Typeless.DeepCopy(imList, TypelessDefaultResolver.Instance);
            Assert.Equal(imList, newList);
        }

        [Fact]
        public void DataContractTest()
        {
            var foobar = new FooBar { FooProperty = 2018, BarProperty = "good" };
            var bytes = MessagePackSerializer.Serialize(foobar);
            Assert.Equal(@"[2018]", MessagePackSerializer.ToJson(bytes));
            //Assert.Equal(@"{""foo"":2018}", Utf8Json.JsonSerializer.ToJsonString(foobar));

            var foobar1 = new FooBar1 { FooProperty = 2018, BarProperty = "good" };
            bytes = MessagePackSerializer.Serialize(foobar1);
            Assert.Equal(@"{""fo"":2018}", MessagePackSerializer.ToJson(bytes));

            var foobar2 = new FooBar2 { FooProperty = 2018, BarProperty = "good" };
            bytes = MessagePackSerializer.Serialize(foobar2);
            Assert.Equal(@"{""f"":2018}", MessagePackSerializer.ToJson(bytes));

            var foobar3 = new FooBar3 { FooProperty = 2018, BarProperty = "good" };
            bytes = MessagePackSerializer.Serialize(foobar3);
            Assert.Equal(@"{""FooProperty"":2018}", MessagePackSerializer.ToJson(bytes));
        }

        [Fact]
        public void CanSerializeType()
        {
            var fooType = typeof(FooClass);
            var bytes = MessagePackSerializer.Serialize(fooType);
            Assert.Equal(fooType, MessagePackSerializer.Deserialize<Type>(bytes));
            var copy = (Type)MessagePackSerializer.DeepCopy<object>(fooType);
            Assert.Equal(fooType, copy);
        }

        [Fact]
        public void CanSerializeCultureInfo()
        {
            var culture = CultureInfo.InvariantCulture;
            var bytes = MessagePackSerializer.Serialize(culture);
            Assert.Equal(culture, MessagePackSerializer.Deserialize<CultureInfo>(bytes));
            var copy = (CultureInfo)MessagePackSerializer.DeepCopy<object>(culture);
            Assert.Equal(culture, copy);
        }

        [Fact]
        public void CanSerializeIPAddress()
        {
            var ip = IPAddress.Parse("192.168.0.108");
            var bytes = MessagePackSerializer.Serialize(ip);
            Assert.Equal(ip, MessagePackSerializer.Deserialize<IPAddress>(bytes));
            var copy = (IPAddress)MessagePackSerializer.DeepCopy<object>(ip);
            Assert.Equal(ip, copy);

            var endPoint = new IPEndPoint(ip, 8080);
            bytes = MessagePackSerializer.Serialize(endPoint);
            Assert.Equal(endPoint, MessagePackSerializer.Deserialize<IPEndPoint>(bytes));
            var copy1 = (IPEndPoint)MessagePackSerializer.DeepCopy<object>(endPoint);
            Assert.Equal(endPoint, copy1);
        }

        [Fact]
        public void CanSerializeInterfaceField()
        {
            var b = new Bar
            {
                Foo = new Foo()
                {
                    A = 123,
                    B = "hello"
                }
            };
            var bytes = MessagePackSerializer.Serialize(b, DefaultResolver.Instance);
            var json = MessagePackSerializer.ToJson(bytes);
            Assert.Equal(@"{""Foo"":[0,[123,""hello""]]}", json);
            var copy = MessagePackSerializer.DeepCopy(b, DefaultResolver.Instance);
            Assert.NotNull(copy);
            Assert.IsAssignableFrom<IFoo>(copy.Foo);
            Assert.Equal(b.Foo.A, copy.Foo.A);
            Assert.Equal(((Foo)b.Foo).B, ((Foo)copy.Foo).B);
            bytes = MessagePackSerializer.Typeless.Serialize(b);
            json = MessagePackSerializer.ToJson(bytes);
            Assert.Equal(@"{""$type"":""MessagePack.Tests.ExtensionTests.Bar, MessagePack.Tests"",""Foo"":[0,[123,""hello""]]}", json);
            copy = (Bar)MessagePackSerializer.Typeless.DeepCopy(b);
            Assert.NotNull(copy);
            Assert.IsAssignableFrom<IFoo>(copy.Foo);
            Assert.Equal(b.Foo.A, copy.Foo.A);
            Assert.Equal(((Foo)b.Foo).B, ((Foo)copy.Foo).B);
        }

        [Fact]
        public void CanSerializeInterfaceField1()
        {
            var b = new Bar0
            {
                Foo = new Foo()
                {
                    A = 123,
                    B = "hello"
                }
            };
            Assert.Throws<EntryPointNotFoundException>(() => MessagePackSerializer.Serialize(b, ContractlessStandardResolverAllowPrivate.Instance));
        }

        [Fact]
        public void CanSerializeInterfaceField2()
        {
            var b = new Bar00
            {
                Foo = new Foo()
                {
                    A = 123,
                    B = "hello"
                }
            };
            Assert.Throws<EntryPointNotFoundException>(() => MessagePackSerializer.Serialize(b, ContractlessStandardResolverAllowPrivate.Instance));
        }

        [Fact]
        public void CanSerializeObjectField()
        {
            var b = new Bar1
            {
                Foo = new Foo()
                {
                    A = 123,
                    B = "hello"
                }
            };
            var bytes = MessagePackSerializer.Serialize(b, DefaultResolver.Instance);
            var json = MessagePackSerializer.ToJson(bytes);
            Assert.Equal(@"{""Foo"":[""MessagePack.Tests.ExtensionTests.Foo, MessagePack.Tests"",123,""hello""]}", json);

            var copy = MessagePackSerializer.DeepCopy(b, DefaultResolver.Instance);
            Assert.NotNull(copy);
            Assert.IsType<Foo>(copy.Foo);
            Assert.NotNull(copy);
            Assert.IsType<Foo>(copy.Foo);
            var foo = (Foo)copy.Foo;
            Assert.Equal(123, foo.A);
            Assert.Equal("hello", foo.B);

            bytes = MessagePackSerializer.Serialize(b, ContractlessStandardResolverAllowPrivate.Instance);
            copy = MessagePackSerializer.Deserialize<Bar1>(bytes, ContractlessStandardResolverAllowPrivate.Instance);
            Assert.NotNull(copy);
            Assert.IsType<Foo>(copy.Foo);
            Assert.NotNull(copy);
            Assert.IsType<Foo>(copy.Foo);
            foo = (Foo)copy.Foo;
            Assert.Equal(123, foo.A);
            Assert.Equal("hello", foo.B);


            copy = MessagePackSerializer.DeepCopy(b, DefaultResolver.Instance);
            Assert.NotNull(copy);
            Assert.IsType<Foo>(copy.Foo);
            Assert.NotNull(copy);
            Assert.IsType<Foo>(copy.Foo);
            foo = (Foo)copy.Foo;
            Assert.Equal(123, foo.A);
            Assert.Equal("hello", foo.B);

            bytes = MessagePackSerializer.Typeless.Serialize(b);
            json = MessagePackSerializer.ToJson(bytes);
            Assert.Equal(@"{""$type"":""MessagePack.Tests.ExtensionTests.Bar1, MessagePack.Tests"",""Foo"":[""MessagePack.Tests.ExtensionTests.Foo, MessagePack.Tests"",123,""hello""]}", json);
            copy = (Bar1)MessagePackSerializer.Typeless.DeepCopy(b);
            Assert.NotNull(copy);
            Assert.IsType<Foo>(copy.Foo);
            foo = (Foo)copy.Foo;
            Assert.Equal(123, foo.A);
            Assert.Equal("hello", foo.B);
        }

        [Fact]
        public void CanSerializeObjectField1()
        {
            var b = new Bar2
            {
                Foo = SerializerPocoSerializable.Create()
            };

            var copy = (Bar2)MessagePackSerializer.DeepCopy<object>(b, DefaultResolver.Instance);
            Assert.NotNull(copy);
            Assert.IsType<SerializerPocoSerializable>(copy.Foo);
            Helper.ComparePoco((SerializerPocoSerializable)b.Foo, (SerializerPocoSerializable)copy.Foo);

            copy = (Bar2)MessagePackSerializer.Typeless.DeepCopy(b, TypelessDefaultResolver.Instance);
            Assert.NotNull(copy);
            Assert.IsType<SerializerPocoSerializable>(copy.Foo);
            Helper.ComparePoco((SerializerPocoSerializable)b.Foo, (SerializerPocoSerializable)copy.Foo);
        }

        class TesttResolver : DefaultResolver, IFormatterResolverContext<int>, IFormatterResolverContext<string>
        {
            private readonly int _age;
            private readonly string _name;
            public TesttResolver(int age, string name) : base()
            {
                _age = age;
                _name = name;
            }

            int IFormatterResolverContext<int>.Value => _age;

            string IFormatterResolverContext<string>.Value => _name;
        }

        [Fact]
        public void FormatterResolverTest()
        {
            var resolver = new TesttResolver(10, "蝉");
            Assert.Equal(10, ((IFormatterResolverContext<int>)resolver).Value);
            Assert.Equal("蝉", ((IFormatterResolverContext<string>)resolver).Value);
        }

        [Fact]
        public void CanSerializeCMD()
        {
            var cmd = new Cmd("a");
            var copy = (Cmd)MessagePackSerializer.Typeless.DeepCopy(cmd, TypelessDefaultResolver.Instance);
            Assert.NotNull(copy);
            Assert.Equal("a", (string)copy.Data);

            var trans = new Transition<string>("a", "b", "c");
            var trans1 = (Transition<string>)MessagePackSerializer.Typeless.DeepCopy(trans, TypelessDefaultResolver.Instance);
            Assert.NotNull(trans1);
            Assert.Equal("a", trans1.FsmRef);
            Assert.Equal("b", trans1.From);
            Assert.Equal("c", trans1.To);
        }

#if DEPENDENT_ON_CUTEANT
        [Fact]
        public void CanSerializeGenericInterface()
        {
            var trans = new Transition<ITestMessage>("a", new MessageA("b"), new MessageB("c"));
            var trans1 = (Transition<ITestMessage>)MessagePackSerializer.Typeless.DeepCopy(trans, TypelessDefaultResolver.Instance);
            Assert.NotNull(trans1);
            Assert.Equal("a", trans1.FsmRef);
            Assert.Equal("b", trans1.From.Data);
            Assert.Equal("c", trans1.To.Data);

            var wrappedTrans = new WrappedTransition<Transition<ITestMessage>>
            {
                Payload = trans
            };
            var wrappedTrans1 = (WrappedTransition<Transition<ITestMessage>>)MessagePackSerializer.Typeless.DeepCopy(wrappedTrans, TypelessDefaultResolver.Instance);
            Assert.NotNull(wrappedTrans1);
            Assert.Equal("a", wrappedTrans1.Payload.FsmRef);
            Assert.Equal("b", wrappedTrans1.Payload.From.Data);
            Assert.Equal("c", wrappedTrans1.Payload.To.Data);
        }
#endif
    }


    internal class Cmd
    {
        public Cmd(object data)
        {
            Data = data;
        }

        public object Data { get; private set; }

        public override string ToString()
        {
            return "Cmd(" + Data + ")";
        }
    }

    public sealed class WrappedTransition<T>
    {
        public T Payload { get; set; }
    }

    public sealed class Transition<TS>
    {
        /// <summary>
        /// Initializes a new instance of the Transition
        /// </summary>
        /// <param name="fsmRef">TBD</param>
        /// <param name="from">TBD</param>
        /// <param name="to">TBD</param>
        public Transition(string fsmRef, TS from, TS to)
        {
            To = to;
            From = from;
            FsmRef = fsmRef;
        }

        /// <summary>
        /// TBD
        /// </summary>
        public string FsmRef { get; }

        /// <summary>
        /// TBD
        /// </summary>
        public TS From { get; }

        /// <summary>
        /// TBD
        /// </summary>
        public TS To { get; }
    }

    //public class WithImmutableDefaultResolver : FormatterResolver
    //{
    //    public static readonly WithImmutableDefaultResolver Instance = new WithImmutableDefaultResolver();

    //    public override IMessagePackFormatter<T> GetFormatter<T>()
    //    {
    //        return (ImmutableCollectionResolver.Instance.GetFormatter<T>()
    //             ?? StandardResolver.Instance.GetFormatter<T>());
    //    }
    //}
}
