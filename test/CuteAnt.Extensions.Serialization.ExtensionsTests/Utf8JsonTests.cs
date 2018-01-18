using System;
using System.Collections.Immutable;
using Utf8Json;
using Utf8Json.ImmutableCollection;
using Utf8Json.Resolvers;
using Xunit;

namespace CuteAnt.Extensions.Serialization.Tests
{
  public class Utf8JsonTests
  {
    static Utf8JsonTests()
    {
      CompositeResolver.RegisterAndSetAsDefault(ImmutableCollectionResolver.Instance, StandardResolver.AllowPrivate);
    }

    [Fact]
    public void SerializeInterfaceTest()
    {
      IUnionSample foo = new FooClass { XYZ = 9999 };

      var bytes = JsonSerializer.Serialize(foo); // IUnionSample
      Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<IUnionSample>(bytes));
      var newFoo = JsonSerializer.Deserialize<FooClass>(bytes);
      Assert.NotNull(newFoo);
      Assert.IsType<FooClass>(newFoo);
      // 已出错，XYZ按正常逻辑应为9999
      Assert.Equal(0, newFoo.XYZ);
    }

    [Fact]
    public void SerializeClassTest()
    {
      var guid = Guid.NewGuid();
      Console.WriteLine(guid);
      ParentUnionType subUnionType1 = new SubUnionType1 { MyProperty = guid, MyProperty1 = 20 };
      var bytes = JsonSerializer.Serialize(subUnionType1);
      Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<ParentUnionType>(bytes));
      var newSubUnionType1 = JsonSerializer.Deserialize<SubUnionType1>(bytes);
      Assert.NotNull(newSubUnionType1);
      Assert.IsType<SubUnionType1>(newSubUnionType1);
      Assert.Equal(guid, newSubUnionType1.MyProperty);
      Assert.Equal(0, newSubUnionType1.MyProperty1); // 说明 MyProperty1 并没有序列化
    }

    [Fact]
    public void SerializeImmutableCollectionTest()
    {
      var imList = ImmutableList<int>.Empty.AddRange(new[] { 1, 2 });
      var bytes = JsonSerializer.Serialize(imList);
      var newList = JsonSerializer.Deserialize<ImmutableList<int>>(bytes);
      Assert.Equal(imList, newList);

      // 此时如果序列化 Object 对象，则无法正确序列化，说明官方的 CompositeResolver 还是有问题的
      Assert.Throws<System.Reflection.TargetInvocationException>(() => JsonSerializer.Serialize((object)imList));
    }

  }
}
