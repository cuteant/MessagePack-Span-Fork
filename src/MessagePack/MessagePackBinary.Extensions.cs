using System;
using CuteAnt.Reflection;

namespace MessagePack
{
    partial class MessagePackBinary
    {
        public static Type ReadNamedType(byte[] bytes, int offset, out int readSize, bool throwOnError = true)
        {
            var hashCode = ReadInt32(bytes, offset, out var hashCodeSize);
            var typeName = ReadBytes(bytes, offset + hashCodeSize, out readSize);
            readSize += hashCodeSize;
            return TypeSerializer.GetTypeFromTypeKey(new TypeKey(hashCode, typeName), throwOnError);
        }

        public static int WriteNamedType(ref byte[] bytes, int offset, Type type)
        {
            var typeKey = TypeSerializer.GetTypeKeyFromType(type);
            var hashCodeSize = WriteInt32(ref bytes, offset, typeKey.HashCode);
            var typeName = typeKey.TypeName;
            var nameSize = WriteBytes(ref bytes, offset + hashCodeSize, typeName, 0, typeName.Length);
            return hashCodeSize + nameSize;
        }
    }
}
