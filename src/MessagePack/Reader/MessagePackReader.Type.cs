namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type ReadNamedType(bool throwOnError)
        {
            return MessagePackBinary.ResolveType(ReadUtf8Span(), throwOnError);
        }

    }
}
