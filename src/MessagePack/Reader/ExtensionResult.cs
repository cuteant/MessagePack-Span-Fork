namespace MessagePack
{
    using System;

    public readonly ref struct ExtensionResult
    {
        public readonly sbyte TypeCode;
        public readonly ReadOnlySpan<byte> Data;

        public ExtensionResult(sbyte typeCode, ReadOnlySpan<byte> data)
        {
            TypeCode = typeCode;
            Data = data;
        }
    }

    public readonly struct ExtensionHeader
    {
        public readonly sbyte TypeCode;
        public readonly uint Length;

        public ExtensionHeader(sbyte typeCode, uint length)
        {
            TypeCode = typeCode;
            Length = length;
        }
    }
}
