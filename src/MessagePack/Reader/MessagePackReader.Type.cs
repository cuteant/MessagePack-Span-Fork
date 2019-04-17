namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type ReadNamedType(bool throwOnError)
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);

            if (MessagePackCode.Nil == position) { Advance(1); return null; }

            var utf8Span = stringSpanDecoders[position].Read(ref this, ref position);
            return MessagePackBinary.ResolveType(utf8Span, throwOnError);
        }
    }
}
