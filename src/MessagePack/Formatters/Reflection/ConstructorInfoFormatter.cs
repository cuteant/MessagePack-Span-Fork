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

        public ConstructorInfoFormatter(bool throwOnError)
        {
            _throwOnError = throwOnError;
        }

        public TConstructor Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            var startOffset = offset;
            var declaringType = MessagePackBinary.ReadNamedType(bytes, offset, out readSize, _throwOnError);
            offset += readSize;
            var argumentCount = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;
            var parameterTypes = Type.EmptyTypes;
            if (argumentCount > 0)
            {
                parameterTypes = new Type[argumentCount];
                for (var idx = 0; idx < argumentCount; idx++)
                {
                    parameterTypes[idx] = MessagePackBinary.ReadNamedType(bytes, offset, out readSize, _throwOnError);
                    offset += readSize;
                }
            }
            readSize = offset - startOffset;
            var ctor = declaringType.GetConstructor(parameterTypes);
            return (TConstructor)ctor;
        }

        public int Serialize(ref byte[] bytes, int offset, TConstructor value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var startOffset = offset;
            offset += MessagePackBinary.WriteNamedType(ref bytes, offset, value.DeclaringType);
            var arguments = value.GetParameters().Select(p => p.ParameterType).ToArray();
            offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, arguments.Length);
            for (int idx = 0; idx < arguments.Length; idx++)
            {
                offset += MessagePackBinary.WriteNamedType(ref bytes, offset, arguments[idx]);
            }
            return offset - startOffset;
        }
    }
}
