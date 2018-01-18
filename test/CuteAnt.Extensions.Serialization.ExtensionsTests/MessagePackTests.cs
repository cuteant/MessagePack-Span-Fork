using System;
using System.Collections.Immutable;
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
      MessagePackMessageFormatter.Register(ImmutableCollectionResolver.Instance);
      MessagePackTypelessMessageFormatter.Register(ImmutableCollectionResolver.Instance);
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
    }

    [Fact]
    public void SerializeClassTest()
    {
      var guid = Guid.NewGuid();
      Console.WriteLine(guid);
      ParentUnionType subUnionType1 = new SubUnionType1 { MyProperty = guid, MyProperty1 = 20 };
      var bytes = MessagePackSerializer.Serialize(subUnionType1);
      var newSubUnionType1 = MessagePackSerializer.Deserialize<ParentUnionType>(bytes);
      Assert.NotNull(newSubUnionType1);
      Assert.IsType<SubUnionType1>(newSubUnionType1);
      Assert.Equal(guid, newSubUnionType1.MyProperty);

      // 实际应用中，无法确定消费者是按哪个类型（ParentUnionType or SubUnionType1）来反序列化的
      Assert.Throws<InvalidOperationException>(() => MessagePackSerializer.Deserialize<SubUnionType1>(bytes));
    }

    [Fact]
    public void SerializeImmutableCollectionTest()
    {
      var imList = ImmutableList<int>.Empty.AddRange(new[] { 1, 2 });
      var bytes = MessagePackSerializer.Serialize(imList);
      var newList = MessagePackSerializer.Deserialize<ImmutableList<int>>(bytes);
      Assert.Equal(imList, newList);

      // 此时如果序列化 Object 对象，则无法正确序列化，说明官方的 CompositeResolver 还是有问题的
      Assert.Throws<System.Reflection.TargetInvocationException>(() => MessagePackSerializer.Serialize((object)imList));

      bytes = MessagePackMessageFormatter.DefaultInstance.Serialize(imList);
      newList = MessagePackMessageFormatter.DefaultInstance.Deserialize<ImmutableList<int>>(bytes);
      Assert.Equal(imList, newList);

      bytes = MessagePackMessageFormatter.DefaultInstance.SerializeObject(imList);
      newList = MessagePackMessageFormatter.DefaultInstance.Deserialize<ImmutableList<int>>(bytes);
      Assert.Equal(imList, newList);

      bytes = MessagePackTypelessMessageFormatter.DefaultInstance.Serialize(imList);
      newList = MessagePackTypelessMessageFormatter.DefaultInstance.Deserialize<ImmutableList<int>>(bytes);
      Assert.Equal(imList, newList);

      bytes = MessagePackTypelessMessageFormatter.DefaultInstance.SerializeObject(imList);
      newList = MessagePackTypelessMessageFormatter.DefaultInstance.Deserialize<ImmutableList<int>>(bytes);
      Assert.Equal(imList, newList);
    }
  }
}
