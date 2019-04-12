namespace MessagePack
{
    using System;
    using MessagePack.Formatters;

    public ref partial struct MessagePackReader
    {
        public Type ReadNamedType(bool throwOnError)
        {
            var typeName = ReadUtf8Span();
            return MessagePackBinary.ResolveType(typeName, throwOnError);
        }

    }
}
