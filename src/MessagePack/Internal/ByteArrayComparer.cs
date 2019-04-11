namespace MessagePack.Internal
{
    using System;
    using System.Runtime.CompilerServices;

    public static class ByteArrayComparer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(byte[] bytes, int offset, int count)
        {
            if (UnsafeMemory.Is64BitProcess)
            {
                return unchecked((int)FarmHash.Hash64(bytes, offset, count));
            }

            return unchecked((int)FarmHash.Hash32(bytes, offset, count));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(ReadOnlySpan<byte> span)
        {
            if (UnsafeMemory.Is64BitProcess)
            {
                return unchecked((int)FarmHash.Hash64(span));
            }

            return unchecked((int)FarmHash.Hash32(span));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Equals(byte[] xs, int xsOffset, int xsCount, byte[] ys)
        {
            return Equals(xs, xsOffset, xsCount, ys, 0, ys.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Equals(byte[] xs, int xsOffset, int xsCount, byte[] ys, int ysOffset, int ysCount)
        {
            if (xs == null || ys == null || xsCount != ysCount) { return false; }

            return new ReadOnlySpan<byte>(xs, xsOffset, xsCount).SequenceEqual(new ReadOnlySpan<byte>(ys, ysOffset, ysCount));
        }
    }
}