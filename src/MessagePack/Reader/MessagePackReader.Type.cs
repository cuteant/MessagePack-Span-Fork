namespace MessagePack
{
    using System;
    using MessagePack.Formatters;

    public ref partial struct MessagePackReader
    {
        public Type ReadNamedType(bool throwOnError)
        {
            var typeName = ReadStringSegment();
            return MessagePackBinary.ResolveType(typeName, throwOnError);
        }

    }
}
