using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Net;
using MessagePack;
using MessagePack.ImmutableCollection;
using MessagePack.Resolvers;
using Xunit;

namespace CuteAnt.Extensions.Serialization.Tests
{
  public class MessagePackTests
  {
    static MessagePackTests()
    {
      CompositeResolver.RegisterAndSetAsDefault(ImmutableCollectionResolver.Instance, ContractlessStandardResolverAllowPrivate.Instance);
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
      bytes = MessagePackSerializer.Serialize(bar);
      var newBar = MessagePackSerializer.Deserialize<BarClass>(bytes);
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
      var bytes = MessagePackSerializer.Serialize(imList);
      var newList = MessagePackSerializer.Deserialize<ImmutableList<int>>(bytes);
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
    }

    [Fact]
    public void CanSerializeCultureInfo()
    {
      var culture = CultureInfo.InvariantCulture;
      var bytes = MessagePackSerializer.Serialize(culture);
      Assert.Equal(culture, MessagePackSerializer.Deserialize<CultureInfo>(bytes));
    }

    [Fact]
    public void CanSerializeIPAddress()
    {
      var ip = IPAddress.Parse("192.168.0.108");
      var bytes = MessagePackSerializer.Serialize(ip);
      Assert.Equal(ip, MessagePackSerializer.Deserialize<IPAddress>(bytes));
      var endPoint = new IPEndPoint(ip, 8080);
      bytes = MessagePackSerializer.Serialize(endPoint);
      Assert.Equal(endPoint, MessagePackSerializer.Deserialize<IPEndPoint>(bytes));
    }
  }
}
