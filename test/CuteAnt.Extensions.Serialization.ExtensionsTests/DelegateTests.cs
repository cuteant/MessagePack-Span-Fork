using System;
using System.IO;
using MessagePack;
using MessagePack.Resolvers;
using Xunit;

namespace CuteAnt.Extensions.Serialization.Tests
{
  public class DelegateTests
  {
    public class Dummy
    {
      public int Prop { get; set; }
    }

    public class HasClosure
    {
      public Func<int> Del;

      public void Create()
      {
        var a = 3;
        Del = () => a + 1;
      }
    }

    [Fact]
    public void CanSerializeMemberMethod()
    {
      Func<string> a = 123.ToString;
      var bytes = MessagePackSerializer.Serialize(a);
      var res = MessagePackSerializer.Deserialize<Func<string>>(bytes);
      Assert.NotNull(res);
      var actual = res();
      Assert.Equal("123", actual);

      res = MessagePackMessageFormatter.DefaultInstance.DeepCopy(a);
      Assert.NotNull(res);
      actual = res();
      Assert.Equal("123", actual);

      res = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(a);
      Assert.NotNull(res);
      actual = res();
      Assert.Equal("123", actual);
    }

    [Fact]
    public void CanSerializeDelegate()
    {
      Action<Dummy> a = dummy => dummy.Prop = 1;
      var bytes = MessagePackSerializer.Serialize(a, ContractlessStandardResolverAllowPrivate.Instance);
      var res = MessagePackSerializer.Deserialize<Action<Dummy>>(bytes, ContractlessStandardResolverAllowPrivate.Instance);
      Assert.NotNull(res);

      var d = new Dummy { Prop = 0 };
      res(d);
      Assert.Equal(1, d.Prop);
    }

    private static int StaticFunc(int a)
    {
      return a + 1;
    }

    [Fact]
    public void CanSerializeStaticDelegate()
    {
      Func<int, int> fun = StaticFunc;

      var bytes = MessagePackSerializer.Serialize(fun);
      var res = MessagePackSerializer.Deserialize<Func<int, int>>(bytes);
      Assert.NotNull(res);
      var actual = res(4);

      Assert.Equal(5, actual);
    }

    [Fact]
    public void CanSerializeObjectWithClosure()
    {
      var hasClosure = new HasClosure();
      hasClosure.Create();

      var bytes = MessagePackSerializer.Serialize(hasClosure, ContractlessStandardResolverAllowPrivate.Instance);
      var res = MessagePackSerializer.Deserialize<HasClosure>(bytes, ContractlessStandardResolverAllowPrivate.Instance);
      Assert.NotNull(res);
      var actual = res.Del();
      Assert.Equal(4, actual);
    }
  }
}