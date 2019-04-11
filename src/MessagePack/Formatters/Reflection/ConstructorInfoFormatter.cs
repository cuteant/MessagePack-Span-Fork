using System;
using System.Linq;
using System.Reflection;

namespace MessagePack.Formatters
{
    public sealed class ConstructorInfoFormatter : ConstructorInfoFormatter<ConstructorInfo>
    {
        public static readonly IMessagePackFormatter<ConstructorInfo> Instance = new ConstructorInfoFormatter();

        public ConstructorInfoFormatter() : base() { }

        public ConstructorInfoFormatter(bool throwOnError) : base(throwOnError) { }
    }

    public class ConstructorInfoFormatter<TConstructor> : IMessagePackFormatter<TConstructor>
        where TConstructor : ConstructorInfo
    {
        private readonly bool _throwOnError;

        public ConstructorInfoFormatter() : this(true) { }

        public ConstructorInfoFormatter(bool throwOnError) => _throwOnError = throwOnError;

        public TConstructor Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var declaringType = reader.ReadNamedType(_throwOnError);
            var argumentCount = reader.ReadArrayHeader();
            var parameterTypes = Type.EmptyTypes;
            if (argumentCount > 0)
            {
                parameterTypes = new Type[argumentCount];
                for (var idx = 0; idx < argumentCount; idx++)
                {
                    parameterTypes[idx] = reader.ReadNamedType(_throwOnError);
                }
            }
            var ctor = declaringType.GetConstructor(parameterTypes);
            return (TConstructor)ctor;
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, TConstructor value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteNamedType(value.DeclaringType, ref idx);
            var arguments = value.GetParameters().Select(p => p.ParameterType).ToArray();
            writer.WriteArrayHeader(arguments.Length, ref idx);
            for (int i = 0; i < arguments.Length; i++)
            {
                writer.WriteNamedType(arguments[i], ref idx);
            }
        }
    }
}
