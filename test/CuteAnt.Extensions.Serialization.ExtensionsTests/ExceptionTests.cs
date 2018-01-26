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
      var expected = GetNewException(true);

      var actual = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expected);
      Assert.Equal(expected.Message, actual.Message);
      Assert.Equal(expected.InnerException.Message, actual.InnerException.Message);
      Assert.Equal(expected.BaseField.Value, actual.BaseField.Value, StringComparer.Ordinal);
      Assert.Equal(expected.SubClassField, actual.SubClassField, StringComparer.Ordinal);
      Assert.Equal(expected.OtherField.Value, actual.OtherField.Value, StringComparer.Ordinal);

      actual = (TestException)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expected);
      Assert.Equal(expected.Message, actual.Message);
      Assert.Equal(expected.InnerException.Message, actual.InnerException.Message);
      Assert.Equal(expected.BaseField.Value, actual.BaseField.Value, StringComparer.Ordinal);
      Assert.Equal(expected.SubClassField, actual.SubClassField, StringComparer.Ordinal);
      Assert.Equal(expected.OtherField.Value, actual.OtherField.Value, StringComparer.Ordinal);

      var bytes = MessagePackSerializer.Serialize(expected, ContractlessStandardResolverAllowPrivate.Instance);
      actual = MessagePackSerializer.Deserialize<TestException>(bytes, ContractlessStandardResolverAllowPrivate.Instance);
      Assert.Equal(expected.Message, actual.Message);
      Assert.Equal(expected.InnerException.Message, actual.InnerException.Message);
      Assert.Equal(expected.BaseField.Value, actual.BaseField.Value, StringComparer.Ordinal);
      Assert.Equal(expected.SubClassField, actual.SubClassField, StringComparer.Ordinal);
      Assert.Equal(expected.OtherField.Value, actual.OtherField.Value, StringComparer.Ordinal);

    }

    [Fact]
    public void ExceptionSerializer_SimpleException1()
    {
      var expected = GetNewException(true);

      var bytes = MessagePackSerializer.Serialize(expected, WithExceptionResolver.Instance);
      var actual = MessagePackSerializer.Deserialize<TestException>(bytes, WithExceptionResolver.Instance);

      Assert.Equal(expected.Message, actual.Message);
      Assert.Equal(expected.InnerException.Message, actual.InnerException.Message);
      Assert.Equal(expected.BaseField.Value, actual.BaseField.Value, StringComparer.Ordinal);
      Assert.Equal(expected.SubClassField, actual.SubClassField, StringComparer.Ordinal);
      Assert.Equal(expected.OtherField.Value, actual.OtherField.Value, StringComparer.Ordinal);
    }

    [Fact]
    public void ExceptionSerializer_ReferenceCycle()
    {
      // Throw an exception so that is has a stack trace.
      var expected = GetNewException();

      // Create a reference cycle at the top level.
      expected.SomeObject = expected;

      var actual = this.TestExceptionSerialization(expected);
      Assert.Equal(actual, actual.SomeObject);
    }

    [Fact]
    public void ExceptionSerializer_NestedReferenceCycle()
    {
      // Throw an exception so that is has a stack trace.
      var exception = GetNewException();
      var expected = new Outer
      {
        SomeFunObject = exception.OtherField,
        Object = exception,
      };

      // Create a reference cycle.
      exception.SomeObject = expected;

      var bytes = MessagePackSerializer.Serialize(expected, WithExceptionResolver.Instance);
      var actual = MessagePackSerializer.Deserialize<Outer>(bytes, WithExceptionResolver.Instance);

      Assert.Equal(expected.Object.BaseField.Value, actual.Object.BaseField.Value, StringComparer.Ordinal);
      Assert.Equal(expected.Object.SubClassField, actual.Object.SubClassField, StringComparer.Ordinal);
      Assert.Equal(expected.Object.OtherField.Value, actual.Object.OtherField.Value, StringComparer.Ordinal);

      // Check for referential equality in the fields which happened to be reference-equals.
      Assert.Equal(actual.Object.BaseField, actual.Object.OtherField, ReferenceEqualsComparer.Instance);
      Assert.Equal(actual, actual.Object.SomeObject, ReferenceEqualsComparer.Instance);
      Assert.Equal(actual.SomeFunObject, actual.Object.OtherField, ReferenceEqualsComparer.Instance);
    }

    private TestException TestExceptionSerialization(TestException expected)
    {
      var bytes = MessagePackSerializer.Serialize(expected, WithExceptionResolver.Instance);
      var actual = MessagePackSerializer.Deserialize<TestException>(bytes, WithExceptionResolver.Instance);

      Assert.Equal(expected.BaseField.Value, actual.BaseField.Value, StringComparer.Ordinal);
      Assert.Equal(expected.SubClassField, actual.SubClassField, StringComparer.Ordinal);
      Assert.Equal(expected.OtherField.Value, actual.OtherField.Value, StringComparer.Ordinal);

      // Check for referential equality in the two fields which happened to be reference-equals.
      Assert.Equal(actual.BaseField, actual.OtherField, ReferenceEqualsComparer.Instance);

      return actual;
    }

    private static TestException GetNewException(bool includeInnerException = false)
    {
      TestException expected;
      try
      {
        var baseField = new SomeFunObject
        {
          Value = Guid.NewGuid().ToString()
        };
        TestException res;
        if (includeInnerException)
        {
          res = new TestException("err", new ApplicationException("app err"))
          {
            BaseField = baseField,
            SubClassField = Guid.NewGuid().ToString(),
            OtherField = baseField,
          };
        }
        else
        {
          res = new TestException()
          {
            BaseField = baseField,
            SubClassField = Guid.NewGuid().ToString(),
            OtherField = baseField,
          };
        }
        throw res;
      }
      catch (TestException exception)
      {
        expected = exception;
      }
      return expected;
    }

    private class Outer : IObjectReferences
    {
      public SomeFunObject SomeFunObject { get; set; }
      public TestException Object { get; set; }
    }

    private class SomeFunObject
    {
      public string Value { get; set; }
    }

    private class BaseException : Exception
    {
      public BaseException() : base() { }
      public BaseException(string msg) : base(msg) { }
      public BaseException(string msg, Exception innerException) : base(msg, innerException) { }
      public SomeFunObject BaseField { get; set; }
    }

    private class TestException : BaseException
    {
      public TestException() : base() { }
      public TestException(string msg) : base(msg) { }
      public TestException(string msg, Exception innerException) : base(msg, innerException) { }
      public string SubClassField { get; set; }
      public SomeFunObject OtherField { get; set; }
      public object SomeObject { get; set; }
    }
  }

  public class WithExceptionResolver : IFormatterResolver
  {
    public static readonly WithExceptionResolver Instance = new WithExceptionResolver();

    public IMessagePackFormatter<T> GetFormatter<T>()
    {
      return (HyperionExceptionResolver.Instance.GetFormatter<T>()
           ?? HyperionResolver.Instance.GetFormatter<T>()
           ?? ContractlessStandardResolverAllowPrivate.Instance.GetFormatter<T>());
    }
  }
}
