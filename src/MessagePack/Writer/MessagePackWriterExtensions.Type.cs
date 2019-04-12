namespace MessagePack
{
    using System;
    using MessagePack.Internal;

    public static partial class MessagePackWriterExtensions
    {
        public static void WriteNamedType(this ref MessagePackWriter writer, Type type, ref int idx)
        {
            var typeName = MessagePackBinary.GetEncodedTypeName(type);
            UnsafeMemory.WriteRaw(ref writer, typeName, ref idx);
        }
    }
}
