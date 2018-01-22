using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Net;
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

      bytes = Utf8JsonMessageFormatter.DefaultInstance.Serialize(foo);
      newFoo = Utf8JsonMessageFormatter.DefaultInstance.Deserialize<FooClass>(bytes);
      Assert.Equal(9999, newFoo.XYZ);

      bytes = Utf8JsonMessageFormatter.DefaultInstance.SerializeObject(foo);
      newFoo = (FooClass)Utf8JsonMessageFormatter.DefaultInstance.Deserialize(typeof(FooClass), bytes);
      Assert.Equal(9999, newFoo.XYZ);
      newFoo = (FooClass)Utf8JsonMessageFormatter.DefaultInstance.Deserialize<IUnionSample>(typeof(FooClass), bytes);
      Assert.Equal(9999, newFoo.XYZ);

      Assert.Throws<InvalidOperationException>(()=> Utf8JsonStandardResolver.Register(ImmutableCollectionResolver.Instance));
    }

    [Fact]
    public void SerializeClassTest()
    {
      var guid = Guid.NewGuid();
      Console.WriteLine(guid);
      ParentUnionType subUnionType1 = new SubUnionType1 { MyProperty = guid, MyProperty1 = 20 };
      var bytes = JsonSerializer.Serialize(subUnionType1);
      Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<ParentUnionType>(bytes));
      var newSubUnionType = JsonSerializer.Deserialize<SubUnionType1>(bytes);
      Assert.NotNull(newSubUnionType);
      Assert.IsType<SubUnionType1>(newSubUnionType);
      Assert.Equal(guid, newSubUnionType.MyProperty);
      Assert.Equal(0, newSubUnionType.MyProperty1); // 说明 MyProperty1 并没有序列化

      bytes = Utf8JsonMessageFormatter.DefaultInstance.Serialize(subUnionType1);
      var newSubUnionType1 = Utf8JsonMessageFormatter.DefaultInstance.Deserialize<SubUnionType1>(bytes);
      Assert.Equal(guid, newSubUnionType1.MyProperty); Assert.Equal(20, newSubUnionType1.MyProperty1);

      bytes = Utf8JsonMessageFormatter.DefaultInstance.SerializeObject(subUnionType1);
      newSubUnionType1 = (SubUnionType1)Utf8JsonMessageFormatter.DefaultInstance.Deserialize(typeof(SubUnionType1), bytes);
      Assert.Equal(guid, newSubUnionType1.MyProperty); Assert.Equal(20, newSubUnionType1.MyProperty1);
      newSubUnionType1 = (SubUnionType1)Utf8JsonMessageFormatter.DefaultInstance.Deserialize<ParentUnionType>(typeof(SubUnionType1), bytes);
      Assert.Equal(guid, newSubUnionType1.MyProperty); Assert.Equal(20, newSubUnionType1.MyProperty1);

      var copy = (SubUnionType1)Utf8JsonMessageFormatter.DefaultInstance.DeepCopy(subUnionType1);
      Assert.Equal(guid, copy.MyProperty); Assert.Equal(20, copy.MyProperty1);
    }

    [Fact]
    public void SerializeImmutableCollectionTest()
    {
      var imList = ImmutableList<int>.Empty.AddRange(new[] { 1, 2 });
      var bytes = JsonSerializer.Serialize(imList);
      var newList = JsonSerializer.Deserialize<ImmutableList<int>>(bytes);
      Assert.Equal(imList, newList);

      // 此时如果序列化 Object 对象，则无法正确序列化，说明官方的 CompositeResolver 所采用的策略还是有问题的
      Assert.Throws<System.Reflection.TargetInvocationException>(() => JsonSerializer.Serialize((object)imList));

      bytes = Utf8JsonMessageFormatter.DefaultInstance.Serialize(imList);
      newList = Utf8JsonMessageFormatter.DefaultInstance.Deserialize<ImmutableList<int>>(bytes);
      Assert.Equal(imList, newList);

      bytes = Utf8JsonMessageFormatter.DefaultInstance.SerializeObject(imList);
      newList = Utf8JsonMessageFormatter.DefaultInstance.Deserialize<ImmutableList<int>>(bytes);
      Assert.Equal(imList, newList);
    }

    [Fact]
    public void CanSerializeType()
    {
      var fooType = typeof(FooClass);
      var bytes = JsonSerializer.Serialize(fooType);
      Assert.Equal(fooType, JsonSerializer.Deserialize<Type>(bytes));
    }

    [Fact]
    public void CanSerializeCultureInfo()
    {
      var culture = CultureInfo.InvariantCulture;
      var bytes = JsonSerializer.Serialize(culture);
      Assert.Equal(culture, JsonSerializer.Deserialize<CultureInfo>(bytes));
    }

    [Fact]
    public void CanSerializeIPAddress()
    {
      var ip = IPAddress.Parse("192.168.0.108");
      var bytes = JsonSerializer.Serialize(ip);
      Assert.Equal(ip, JsonSerializer.Deserialize<IPAddress>(bytes));
      var endPoint = new IPEndPoint(ip, 8080);
      bytes= JsonSerializer.Serialize(endPoint);
      Assert.Equal(endPoint, JsonSerializer.Deserialize<IPEndPoint>(bytes));
    }
  }
}
