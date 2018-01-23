using System;
using System.Linq;
using System.Reflection;
using CuteAnt.Reflection;

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
            var hashCode = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
            offset += readSize;
            var typeName = MessagePackBinary.ReadBytes(bytes, offset, out readSize);
            offset += readSize;
            var declaringType = TypeSerializer.GetTypeFromTypeKey(new TypeKey(hashCode, typeName), _throwOnError);
            var argumentCount = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
            offset += readSize;
            var parameterTypes = Type.EmptyTypes;
            if (argumentCount > 0)
            {
                parameterTypes = new Type[argumentCount];
                for (var idx = 0; idx < argumentCount; idx++)
                {
                    hashCode = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                    offset += readSize;
                    typeName = MessagePackBinary.ReadBytes(bytes, offset, out readSize);
                    offset += readSize;
                    parameterTypes[idx] = TypeSerializer.GetTypeFromTypeKey(new TypeKey(hashCode, typeName), _throwOnError);
                }
            }
            readSize = offset - startOffset;
#if NET40
            var ctor = declaringType.GetConstructor(parameterTypes);
#else
            var ctor = declaringType.GetTypeInfo().GetConstructor(parameterTypes);
#endif
            return (TConstructor)ctor;
        }

        public int Serialize(ref byte[] bytes, int offset, TConstructor value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var declaringType = value.DeclaringType;
            var startOffset = offset;
            var typeKey = TypeSerializer.GetTypeKeyFromType(declaringType);
            offset += MessagePackBinary.WriteInt32(ref bytes, offset, typeKey.HashCode);
            var typeName = typeKey.TypeName;
            offset += MessagePackBinary.WriteBytes(ref bytes, offset, typeName, 0, typeName.Length);
            var arguments = value.GetParameters().Select(p => p.ParameterType).ToArray();
            offset += MessagePackBinary.WriteInt32(ref bytes, offset, arguments.Length);
            for (int idx = 0; idx < arguments.Length; idx++)
            {
                typeKey = TypeSerializer.GetTypeKeyFromType(arguments[idx]);
                offset += MessagePackBinary.WriteInt32(ref bytes, offset, typeKey.HashCode);
                typeName = typeKey.TypeName;
                offset += MessagePackBinary.WriteBytes(ref bytes, offset, typeName, 0, typeName.Length);
            }
            return offset - startOffset;
        }
    }
}
