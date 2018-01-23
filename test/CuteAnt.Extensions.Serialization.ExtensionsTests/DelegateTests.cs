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
      public int A { get; set; }
      public Func<int> Del;
      public string W { get; set; }

      public void Create()
      {
        A = 10;
        W = "test";
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

      //var json = bytes.ToHex();
      var json = "83A1410AA157A474657374A344656CC90000015E64D9BC4D6573736167655061636B2E466F726D6174746572732E44656C65676174655368696D60315B5B43757465416E742E457874656E73696F6E732E53657269616C697A6174696F6E2E54657374732E44656C656761746554657374732B486173436C6F737572652B3C3E635F5F446973706C6179436C617373395F302C2043757465416E742E457874656E73696F6E732E53657269616C697A6174696F6E2E457874656E73696F6E7354657374735D5D2C204D6573736167655061636B9281A16103AC3C4372656174653E625F5F30D284E4A3CCC48643757465416E742E457874656E73696F6E732E53657269616C697A6174696F6E2E54657374732E44656C656761746554657374732B486173436C6F737572652B3C3E635F5F446973706C6179436C617373395F302C2043757465416E742E457874656E73696F6E732E53657269616C697A6174696F6E2E457874656E73696F6E73546573747300";
      bytes = json.ToHex();
      res = MessagePackSerializer.Deserialize<HasClosure>(bytes, ContractlessStandardResolverAllowPrivate.Instance);
      Assert.NotNull(res);
      actual = res.Del();
      Assert.Equal(4, actual);
    }
  }
}