namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using MessagePack.Internal;

    public static partial class MessagePackWriterExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteNamedType(this ref MessagePackWriter writer, Type type, ref int idx)
        {
            var typeName = MessagePackBinary.GetEncodedTypeName(type);
            UnsafeMemory.WriteRaw(ref writer, typeName, ref idx);
        }
    }
}
