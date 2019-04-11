namespace MessagePack.Internal
{
    using System;
    using System.Runtime.CompilerServices;

    // for string key property name write optimization.

    public static class UnsafeMemory
    {
        public static readonly bool Is64BitProcess = IntPtr.Size >= 8;

        // 直接引用 byte[], 节省 byte[] => ReadOnlySpan<byte> 的转换
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRaw(ref MessagePackWriter writer, byte[] source, ref int idx)
        {
            if (Is64BitProcess)
            {
                UnsafeMemory64.WriteRaw(ref writer, source, ref idx);
            }
            else
            {
                UnsafeMemory32.WriteRaw(ref writer, source, ref idx);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRaw(ref MessagePackWriter writer, ref byte source, int sourceBytesToCopy, ref int idx)
        {
            if (Is64BitProcess)
            {
                UnsafeMemory64.WriteRaw(ref writer, ref source, sourceBytesToCopy, ref idx);
            }
            else
            {
                UnsafeMemory32.WriteRaw(ref writer, ref source, sourceBytesToCopy, ref idx);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawBytes(ref MessagePackWriter writer, byte[] source, ref int idx)
        {
            var count = source.Length;
            writer.Ensure(idx, count);
            MessagePackBinary.CopyMemory(source, 0, writer._borrowedBuffer, idx, count);
            idx += count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WriteRawBytes(ref MessagePackWriter writer, ref byte source, int sourceBytesToCopy, ref int idx)
        {
            //if (0u >= (uint)sourceBytesToCopy) { return; }

            MessagePackBinary.CopyMemory(ref source, ref Unsafe.AddByteOffset(ref writer.PinnableAddress, (IntPtr)idx), sourceBytesToCopy);
            idx += sourceBytesToCopy;
        }
    }

    public static partial class UnsafeMemory32
    {
        // 直接引用 byte[], 节省 byte[] => ReadOnlySpan<byte> 的转换
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRaw1(ref MessagePackWriter writer, byte[] source, ref int idx)
        {
            writer.Ensure(idx, 1);

            Unsafe.Add(ref writer.PinnableAddress, (IntPtr)(uint)idx) = source[0];

            idx += 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRaw1(ref MessagePackWriter writer, ref byte source, int sourceBytesToCopy, ref int idx)
        {
            Unsafe.Add(ref writer.PinnableAddress, (IntPtr)(uint)idx) = source;

            idx += 1;
        }
    }

    public static partial class UnsafeMemory64
    {
        // 直接引用 byte[], 节省 byte[] => ReadOnlySpan<byte> 的转换
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRaw1(ref MessagePackWriter writer, byte[] source, ref int idx)
        {
            writer.Ensure(idx, 1);

            Unsafe.Add(ref writer.PinnableAddress, (IntPtr)(uint)idx) = source[0];

            idx += 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRaw1(ref MessagePackWriter writer, ref byte source, int sourceBytesToCopy, ref int idx)
        {
            Unsafe.Add(ref writer.PinnableAddress, (IntPtr)(uint)idx) = source;

            idx += 1;
        }
    }
}
