using System;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Xunit;

namespace CuteAnt.Extensions.Serialization.Tests
{
  public class ExceptionTests
  {
    [Fact]
    public void ExceptionSerializer_SimpleException()
    {
      var expected = GetNewException();

      var actual = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expected);
      Assert.Equal(expected.Message, actual.Message);
      Assert.Equal(expected.InnerException.Message, actual.InnerException.Message);
      Assert.Equal(expected.BaseField, actual.BaseField);
      Assert.Equal(expected.SubClassField, actual.SubClassField);
      Assert.Equal(expected.OtherField, actual.OtherField);

      actual = (TestException)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expected);
      Assert.Equal(expected.Message, actual.Message);
      Assert.Equal(expected.InnerException.Message, actual.InnerException.Message);
      Assert.Equal(expected.BaseField, actual.BaseField);
      Assert.Equal(expected.SubClassField, actual.SubClassField);
      Assert.Equal(expected.OtherField, actual.OtherField);

      var bytes = MessagePackSerializer.Serialize(expected, ContractlessStandardResolverAllowPrivate.Instance);
      actual = MessagePackSerializer.Deserialize<TestException>(bytes, ContractlessStandardResolverAllowPrivate.Instance);
      Assert.Equal(expected.Message, actual.Message);
      Assert.Equal(expected.InnerException.Message, actual.InnerException.Message);
      Assert.Equal(expected.BaseField, actual.BaseField);
      Assert.Equal(expected.SubClassField, actual.SubClassField);
      Assert.Equal(expected.OtherField, actual.OtherField);

    }

    [Fact]
    public void ExceptionSerializer_SimpleException1()
    {
      var expected = GetNewException();

      var bytes = MessagePackSerializer.Serialize(expected, HyperionExceptionResolver.Instance);
      var actual = MessagePackSerializer.Deserialize<TestException>(bytes, HyperionExceptionResolver.Instance);

      Assert.Equal(expected.Message, actual.Message);
      Assert.Equal(expected.InnerException.Message, actual.InnerException.Message);
      Assert.Equal(expected.BaseField, actual.BaseField);
      Assert.Equal(expected.SubClassField, actual.SubClassField);
      Assert.Equal(expected.OtherField, actual.OtherField);
    }

    private static TestException GetNewException()
    {
      TestException expected;
      try
      {
        var res = new TestException("err", new ApplicationException("app err"))
        {
          BaseField = 10,
          SubClassField = Guid.NewGuid().ToString(),
          OtherField = 20
        };
        throw res;
      }
      catch (TestException exception)
      {
        expected = exception;
      }
      return expected;
    }

    public class BaseException : Exception
    {
      public BaseException(string msg) : base(msg) { }
      public BaseException(string msg, Exception innerException) : base(msg, innerException) { }
      public byte A { get; set; }
      public int BaseField { get; set; }
      public string S { get; set; }
    }

    public class TestException : BaseException
    {
      public TestException(string msg) : base(msg) { }
      public TestException(string msg, Exception innerException) : base(msg, innerException) { }
      public bool IsSuccess { get; set; }
      public string SubClassField { get; set; }
      public long OtherField { get; set; }
    }
  }

  public class WithExceptionResolver : IFormatterResolver
  {
    public static readonly WithExceptionResolver Instance = new WithExceptionResolver();

    public IMessagePackFormatter<T> GetFormatter<T>()
    {
      return (HyperionExceptionResolver.Instance.GetFormatter<T>()
           ?? StandardResolver.Instance.GetFormatter<T>());
    }
  }
}
