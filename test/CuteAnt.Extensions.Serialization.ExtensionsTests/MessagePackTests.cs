using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Net;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.ImmutableCollection;
using MessagePack.Resolvers;
using Xunit;

namespace CuteAnt.Extensions.Serialization.Tests
{
  public class MessagePackTests
  {
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

      bytes = MessagePackMessageFormatter.DefaultInstance.Serialize(foo);
      newFoo = MessagePackMessageFormatter.DefaultInstance.Deserialize<FooClass>(bytes);
      Assert.Equal(9999, newFoo.XYZ);

      bytes = MessagePackMessageFormatter.DefaultInstance.SerializeObject(foo);
      newFoo = (FooClass)MessagePackMessageFormatter.DefaultInstance.Deserialize(typeof(FooClass), bytes);
      Assert.Equal(9999, newFoo.XYZ);
      newFoo = (FooClass)MessagePackMessageFormatter.DefaultInstance.Deserialize<IUnionSample>(typeof(FooClass), bytes);
      Assert.Equal(9999, newFoo.XYZ);

      bytes = TypelessMessagePackMessageFormatter.DefaultInstance.Serialize(foo);
      newFoo = TypelessMessagePackMessageFormatter.DefaultInstance.Deserialize<FooClass>(bytes);
      Assert.Equal(9999, newFoo.XYZ);

      bytes = TypelessMessagePackMessageFormatter.DefaultInstance.SerializeObject(foo);
      newFoo = (FooClass)TypelessMessagePackMessageFormatter.DefaultInstance.Deserialize(typeof(FooClass), bytes);
      Assert.Equal(9999, newFoo.XYZ);
      newFoo = (FooClass)TypelessMessagePackMessageFormatter.DefaultInstance.Deserialize<IUnionSample>(typeof(FooClass), bytes);
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

      bytes = MessagePackMessageFormatter.DefaultInstance.Serialize(subUnionType1);
      var newSubUnionType1 = MessagePackMessageFormatter.DefaultInstance.Deserialize<SubUnionType1>(bytes);
      Assert.Equal(guid, newSubUnionType1.MyProperty); Assert.Equal(20, newSubUnionType1.MyProperty1);

      bytes = MessagePackMessageFormatter.DefaultInstance.SerializeObject(subUnionType1);
      newSubUnionType1 = (SubUnionType1)MessagePackMessageFormatter.DefaultInstance.Deserialize(typeof(SubUnionType1), bytes);
      Assert.Equal(guid, newSubUnionType1.MyProperty); Assert.Equal(20, newSubUnionType1.MyProperty1);
      newSubUnionType1 = (SubUnionType1)MessagePackMessageFormatter.DefaultInstance.Deserialize<ParentUnionType>(typeof(SubUnionType1), bytes);
      Assert.Equal(guid, newSubUnionType1.MyProperty); Assert.Equal(20, newSubUnionType1.MyProperty1);

      bytes = TypelessMessagePackMessageFormatter.DefaultInstance.Serialize(subUnionType1);
      newSubUnionType1 = TypelessMessagePackMessageFormatter.DefaultInstance.Deserialize<SubUnionType1>(bytes);
      Assert.Equal(guid, newSubUnionType1.MyProperty); Assert.Equal(20, newSubUnionType1.MyProperty1);

      bytes = TypelessMessagePackMessageFormatter.DefaultInstance.SerializeObject(subUnionType1);
      newSubUnionType1 = (SubUnionType1)TypelessMessagePackMessageFormatter.DefaultInstance.Deserialize(typeof(SubUnionType1), bytes);
      Assert.Equal(guid, newSubUnionType1.MyProperty); Assert.Equal(20, newSubUnionType1.MyProperty1);
      newSubUnionType1 = (SubUnionType1)TypelessMessagePackMessageFormatter.DefaultInstance.Deserialize<ParentUnionType>(typeof(SubUnionType1), bytes);
      Assert.Equal(guid, newSubUnionType1.MyProperty); Assert.Equal(20, newSubUnionType1.MyProperty1);

      var copy = (SubUnionType1)MessagePackMessageFormatter.DefaultInstance.DeepCopy(subUnionType1);
      Assert.Equal(guid, copy.MyProperty); Assert.Equal(20, copy.MyProperty1);
      copy = (SubUnionType1)LZ4MessagePackMessageFormatter.DefaultInstance.DeepCopy(subUnionType1);
      Assert.Equal(guid, copy.MyProperty); Assert.Equal(20, copy.MyProperty1);
      copy = (SubUnionType1)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(subUnionType1);
      Assert.Equal(guid, copy.MyProperty); Assert.Equal(20, copy.MyProperty1);
      copy = (SubUnionType1)LZ4TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(subUnionType1);
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

      bytes = MessagePackMessageFormatter.DefaultInstance.Serialize(imList);
      newList = MessagePackMessageFormatter.DefaultInstance.Deserialize<ImmutableList<int>>(bytes);
      Assert.Equal(imList, newList);

      bytes = MessagePackMessageFormatter.DefaultInstance.SerializeObject(imList);
      newList = MessagePackMessageFormatter.DefaultInstance.Deserialize<ImmutableList<int>>(bytes);
      Assert.Equal(imList, newList);

      bytes = TypelessMessagePackMessageFormatter.DefaultInstance.Serialize(imList);
      newList = TypelessMessagePackMessageFormatter.DefaultInstance.Deserialize<ImmutableList<int>>(bytes);
      Assert.Equal(imList, newList);

      bytes = TypelessMessagePackMessageFormatter.DefaultInstance.SerializeObject(imList);
      newList = TypelessMessagePackMessageFormatter.DefaultInstance.Deserialize<ImmutableList<int>>(bytes);
      Assert.Equal(imList, newList);
    }

    [Fact]
    public void DataContractTest()
    {
      var foobar = new FooBar { FooProperty = 2018, BarProperty = "good" };
      var bytes = MessagePackSerializer.Serialize(foobar);
      Assert.Equal(@"[2018]", MessagePackSerializer.ToJson(bytes));
      Assert.Equal(@"{""foo"":2018}", Utf8Json.JsonSerializer.ToJsonString(foobar));

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
      var copy = MessagePackMessageFormatter.DefaultInstance.DeepCopy(fooType);
      Assert.Equal(fooType, copy);
      copy = (Type)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(fooType);
      Assert.Equal(fooType, copy);
    }

    [Fact]
    public void CanSerializeCultureInfo()
    {
      var culture = CultureInfo.InvariantCulture;
      var bytes = MessagePackSerializer.Serialize(culture);
      Assert.Equal(culture, MessagePackSerializer.Deserialize<CultureInfo>(bytes));
      var copy = MessagePackMessageFormatter.DefaultInstance.DeepCopy(culture);
      Assert.Equal(culture, copy);
      copy = (CultureInfo)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(culture);
      Assert.Equal(culture, copy);
    }

    [Fact]
    public void CanSerializeIPAddress()
    {
      var ip = IPAddress.Parse("192.168.0.108");
      var bytes = MessagePackSerializer.Serialize(ip);
      Assert.Equal(ip, MessagePackSerializer.Deserialize<IPAddress>(bytes));
      var copy = MessagePackMessageFormatter.DefaultInstance.DeepCopy(ip);
      Assert.Equal(ip, copy);
      copy = (IPAddress)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(ip);
      Assert.Equal(ip, copy);

      var endPoint = new IPEndPoint(ip, 8080);
      bytes = MessagePackSerializer.Serialize(endPoint);
      Assert.Equal(endPoint, MessagePackSerializer.Deserialize<IPEndPoint>(bytes));
      var copy1 = MessagePackMessageFormatter.DefaultInstance.DeepCopy(endPoint);
      Assert.Equal(endPoint, copy1);
      copy1 = (IPEndPoint)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(endPoint);
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
      var bytes = MessagePackMessageFormatter.DefaultInstance.Serialize(b);
      var json = MessagePackSerializer.ToJson(bytes);
      Assert.Equal(@"{""Foo"":[0,[123,""hello""]]}", json);
      var copy = MessagePackMessageFormatter.DefaultInstance.DeepCopy(b);
      Assert.NotNull(copy);
      Assert.IsAssignableFrom<IFoo>(copy.Foo);
      Assert.Equal(b.Foo.A, copy.Foo.A);
      Assert.Equal(((Foo)b.Foo).B, ((Foo)copy.Foo).B);
      bytes = TypelessMessagePackMessageFormatter.DefaultInstance.Serialize(b);
      json = MessagePackSerializer.ToJson(bytes);
      Assert.Equal(@"{""$type"":""CuteAnt.Extensions.Serialization.Tests.Bar, CuteAnt.Extensions.Serialization.ExtensionsTests"",""Foo"":[0,[123,""hello""]]}", json);
      copy = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(b);
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
      var bytes = MessagePackMessageFormatter.DefaultInstance.Serialize(b);
      var json = MessagePackSerializer.ToJson(bytes);
      Assert.Equal(@"{""Foo"":[""CuteAnt.Extensions.Serialization.Tests.Foo, CuteAnt.Extensions.Serialization.ExtensionsTests"",123,""hello""]}", json);

      var copy = MessagePackMessageFormatter.DefaultInstance.DeepCopy(b);
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


      copy = (Bar1)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(b);
      Assert.NotNull(copy);
      Assert.IsType<Foo>(copy.Foo);
      Assert.NotNull(copy);
      Assert.IsType<Foo>(copy.Foo);
      foo = (Foo)copy.Foo;
      Assert.Equal(123, foo.A);
      Assert.Equal("hello", foo.B);

      bytes = TypelessMessagePackMessageFormatter.DefaultInstance.Serialize(b);
      json = MessagePackSerializer.ToJson(bytes);
      Assert.Equal(@"{""$type"":""CuteAnt.Extensions.Serialization.Tests.Bar1, CuteAnt.Extensions.Serialization.ExtensionsTests"",""Foo"":[""CuteAnt.Extensions.Serialization.Tests.Foo, CuteAnt.Extensions.Serialization.ExtensionsTests"",123,""hello""]}", json);
      copy = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(b);
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

      var copy = MessagePackMessageFormatter.DefaultInstance.DeepCopy(b);
      Assert.NotNull(copy);
      Assert.IsType<SerializerPocoSerializable>(copy.Foo);
      Helper.ComparePoco((SerializerPocoSerializable)b.Foo, (SerializerPocoSerializable)copy.Foo);

      copy = (Bar2)LZ4MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(b);
      Assert.NotNull(copy);
      Assert.IsType<SerializerPocoSerializable>(copy.Foo);
      Helper.ComparePoco((SerializerPocoSerializable)b.Foo, (SerializerPocoSerializable)copy.Foo);

      copy = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(b);
      Assert.NotNull(copy);
      Assert.IsType<SerializerPocoSerializable>(copy.Foo);
      Helper.ComparePoco((SerializerPocoSerializable)b.Foo, (SerializerPocoSerializable)copy.Foo);

      copy = (Bar2)LZ4TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(b);
      Assert.NotNull(copy);
      Assert.IsType<SerializerPocoSerializable>(copy.Foo);
      Helper.ComparePoco((SerializerPocoSerializable)b.Foo, (SerializerPocoSerializable)copy.Foo);
    }

    [Fact]
    public void FormatterResolverTest()
    {
      var resolver = new DefaultResolver();
      resolver.Context.Add("A", 1);
      Assert.Equal(1, resolver.Context.Count);
      var resolver1 = new TypelessDefaultResolver();
      resolver1.Context.Add("A", 1);
      Assert.Equal(1, resolver.Context.Count);
    }

    [Fact]
    public void CanSerializeCMD()
    {
      var cmd = new Cmd("a");
      var copy = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(cmd);
      Assert.NotNull(copy);
      Assert.Equal("a", (string)copy.Data);

      var trans = new Transition<string>("a", "b", "c");
      var trans1 = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(trans);
      Assert.NotNull(trans1);
      Assert.Equal("a", trans1.FsmRef);
      Assert.Equal("b", trans1.From);
      Assert.Equal("c", trans1.To);
    }

    [Fact]
    public void CanSerializeGenericInterface()
    {
      var trans = new Transition<ITestMessage>("a", new MessageA("b"), new MessageB("c"));
      var trans1 = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(trans);
      Assert.NotNull(trans1);
      Assert.Equal("a", trans1.FsmRef);
      Assert.Equal("b", trans1.From.Data);
      Assert.Equal("c", trans1.To.Data);

      var wrappedTrans = new WrappedTransition<Transition<ITestMessage>>
      {
        Payload = trans
      };
      var wrappedTrans1 = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(wrappedTrans);
      Assert.NotNull(wrappedTrans1);
      Assert.Equal("a", wrappedTrans1.Payload.FsmRef);
      Assert.Equal("b", wrappedTrans1.Payload.From.Data);
      Assert.Equal("c", wrappedTrans1.Payload.To.Data);
    }
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

  public class WithImmutableDefaultResolver : FormatterResolver
  {
    public static readonly WithImmutableDefaultResolver Instance = new WithImmutableDefaultResolver();

    public override IMessagePackFormatter<T> GetFormatter<T>()
    {
      return (ImmutableCollectionResolver.Instance.GetFormatter<T>()
           ?? StandardResolver.Instance.GetFormatter<T>());
    }
  }
}
