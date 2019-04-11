#if DEPENDENT_ON_CUTEANT

using System;
using System.Reflection;
using CuteAnt.Reflection;
using Hyperion;
using Hyperion.Surrogates;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Xunit;

namespace MessagePack.Tests.ExtensionTests
{
    [Collection("Hyperion")]
    public class MessagePackHyperionTests
    {
        [Fact]
        public void Run()
        {
            var msg = new MessageA("foo");

            var bytes = MessagePackSerializer.Serialize(msg, WithTestMessageResolver.Instance);
            var msg1 = MessagePackSerializer.Deserialize<ITestMessage>(bytes, WithTestMessageResolver.Instance);
            Assert.NotNull(msg1);
            Assert.Equal("fooX", msg1.Data);
        }

        [Fact(Skip = "TODO")]
        public void RunTypeless()
        {
            var msg = new MessageA("foo");

            //var bytes = MessagePackSerializer.Serialize<object>(msg, WithTestMessageTypelessResolver.Instance);
            //var msg1 = (ITestMessage)MessagePackSerializer.Deserialize<object>(bytes, WithTestMessageTypelessResolver.Instance);
            var msg1 = (ITestMessage)MessagePackSerializer.Typeless.DeepCopy(msg, WithTestMessageTypelessResolver.Instance);
            Assert.NotNull(msg1);
            Assert.Equal("fooX", msg1.Data);
        }
    }

    public interface ITestMessage
    {
        string Data { get; }
    }
    public class MessageA : ITestMessage
    {
        public string Data { get; }
        public MessageA(string data) => Data = data;
    }
    public class MessageB : ITestMessage
    {
        public string Data { get; }
        public MessageB(string data) => Data = data;
    }

    public sealed class TestMessageSurrogate : StringPayloadSurrogate
    {
        private const string c_nobody = "nobody";

        public static TestMessageSurrogate ToSurrogate(ITestMessage msg)
        {
            return new TestMessageSurrogate() { S = msg.Data };
        }

        public static ITestMessage FromSurrogate(TestMessageSurrogate surrogate)
        {
            var msg = surrogate.S;

            if (string.IsNullOrWhiteSpace(msg)) { return null; }

            return new MessageB(msg + "X");
        }
    }

    internal class TestMessageFormatter<T> : SimpleHyperionFormatter<T>
    {
        public TestMessageFormatter() : base(
            new SerializerOptions(
                versionTolerance: false,
                preserveObjectReferences: true,
                surrogates: new[]
                    {
                        Surrogate.Create<ITestMessage, TestMessageSurrogate>(TestMessageSurrogate.ToSurrogate, TestMessageSurrogate.FromSurrogate),
                    }
                ))
        { }
    }

    internal sealed class TestMessageResolver : FormatterResolver
  {
        public static readonly IFormatterResolver Instance = new TestMessageResolver();

        TestMessageResolver()
        {
        }

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                formatter = (IMessagePackFormatter<T>)TestMessageFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }

    internal static class TestMessageFormatterHelper
    {
        internal static object GetFormatter(Type t)
        {
#if !TEST40
            if (typeof(ITestMessage).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
#else
            if (typeof(ITestMessage).IsAssignableFrom(t))
#endif
            {
                return ActivatorUtils.FastCreateInstance(typeof(TestMessageFormatter<>).GetCachedGenericType(t));
            }

            return null;
        }
    }

    public class WithTestMessageResolver : FormatterResolver
  {
        public static readonly WithTestMessageResolver Instance = new WithTestMessageResolver();

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return (TestMessageResolver.Instance.GetFormatter<T>()
                 ?? ContractlessStandardResolverAllowPrivate.Instance.GetFormatter<T>());
        }
    }

    public sealed class TestMessageTypelessFormatter : TypelessFormatter
    {
        public new static readonly IMessagePackFormatter<object> Instance = new TestMessageTypelessFormatter();

        protected override Type TranslateTypeName(Type actualType)
        {
#if !TEST40
            if (typeof(ITestMessage).GetTypeInfo().IsAssignableFrom(actualType.GetTypeInfo()))
#else
            if (typeof(ITestMessage).IsAssignableFrom(actualType))
#endif
            {
                return typeof(ITestMessage);
            }
            else
            {
                return actualType;
            }
        }
    }

    public sealed class TestMessageTypelessObjectResolver : FormatterResolver
  {
        public static readonly IFormatterResolver Instance = new TestMessageTypelessObjectResolver();

        TestMessageTypelessObjectResolver()
        {

        }

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                formatter = (typeof(T) == typeof(object))
                    ? (IMessagePackFormatter<T>)(object)TestMessageTypelessFormatter.Instance
                    : null;
            }
        }
    }

    public sealed class TestMessageTypelessContractlessStandardResolver : FormatterResolver
  {
        public static readonly IFormatterResolver Instance = new TestMessageTypelessContractlessStandardResolver();

        static readonly IFormatterResolver[] resolvers = new[]
        {
            NativeDateTimeResolver.Instance, // Native c# DateTime format, preserving timezone
            BuiltinResolver.Instance, // Try Builtin
            AttributeFormatterResolver.Instance, // Try use [MessagePackFormatter]
#if !ENABLE_IL2CPP
            DynamicEnumResolver.Instance, // Try Enum
            DynamicGenericResolver.Instance, // Try Array, Tuple, Collection
            DynamicUnionResolver.Instance, // Try Union(Interface)
            DynamicObjectResolver.Instance, // Try Object
#endif
            DynamicContractlessObjectResolverAllowPrivate.Instance, // Serializes keys as strings
            TestMessageTypelessObjectResolver.Instance
        };

        TestMessageTypelessContractlessStandardResolver()
        {
        }

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                foreach (var item in resolvers)
                {
                    var f = item.GetFormatter<T>();
                    if (f != null)
                    {
                        formatter = f;
                        return;
                    }
                }
            }
        }
    }

    public class WithTestMessageTypelessResolver : FormatterResolver
  {
        public static readonly WithTestMessageTypelessResolver Instance = new WithTestMessageTypelessResolver();

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return (TestMessageResolver.Instance.GetFormatter<T>()
                 ?? TestMessageTypelessContractlessStandardResolver.Instance.GetFormatter<T>());
        }
    }
}

#endif
