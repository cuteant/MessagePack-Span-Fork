﻿namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using MessagePack.Internal;

    public static partial class MessagePackWriterExtensions
    {
        const uint MaxFixStringLength = (uint)MessagePackRange.MaxFixStringLength;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteStringBytes(this ref MessagePackWriter writer, byte[] utf8stringBytes, ref int idx)
        {
            var byteCount = utf8stringBytes.Length;
            uint nLen = (uint)byteCount;

            writer.Ensure(idx, byteCount + 5);

            ref byte pinnableAddr = ref writer.PinnableAddress;
            if (nLen <= MaxFixStringLength)
            {
                Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = unchecked((byte)(MessagePackCode.MinFixStr | byteCount));
                idx++;
            }
            else if (nLen <= ByteMaxValue)
            {
                IntPtr offset = (IntPtr)idx;
                Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.Str8;
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)byteCount);
                idx += 2;
            }
            else if (nLen <= UShortMaxValue)
            {
                WriteShort(ref pinnableAddr, MessagePackCode.Str16, (ushort)byteCount, ref idx);
            }
            else
            {
                WriteInt(ref pinnableAddr, MessagePackCode.Str32, nLen, ref idx);
            }

            UnsafeMemory.WriteRaw(ref pinnableAddr, ref utf8stringBytes[0], byteCount, ref idx);
        }

        public static void WriteString(this ref MessagePackWriter writer, string value, ref int idx)
        {
            if (value == null) { WriteNil(ref writer, ref idx); return; }

            var utf16Source = MemoryMarshal.AsBytes(value.AsSpan());
            EncodingUtils.ToUtf8Length(utf16Source, out int byteCount);
            writer.Ensure(idx, byteCount + 5);

            ref byte pinnableAddr = ref writer.PinnableAddress;

            uint nLen = (uint)byteCount;
            // move body and write prefix
            if (nLen <= MaxFixStringLength)
            {
                Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = unchecked((byte)(MessagePackCode.MinFixStr | byteCount));
                idx++;
            }
            else if (nLen <= ByteMaxValue)
            {
                IntPtr offset = (IntPtr)idx;
                Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.Str8;
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)byteCount);
                idx += 2;
            }
            else if (nLen <= UShortMaxValue)
            {
                WriteShort(ref pinnableAddr, MessagePackCode.Str16, (ushort)byteCount, ref idx);
            }
            else
            {
                WriteInt(ref pinnableAddr, MessagePackCode.Str32, nLen, ref idx);
            }

            EncodingUtils.ToUtf8(ref MemoryMarshal.GetReference(utf16Source), utf16Source.Length,
                ref Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx), writer._capacity - idx, out _, out _);
            idx += byteCount;
        }

        public static void WriteStringForceStr32Block(this ref MessagePackWriter writer, string value, ref int idx)
        {
            if (value == null) { WriteNil(ref writer, ref idx); return; }

            writer.Ensure(idx, EncodingUtils.Utf8MaxBytes(value) + 5);

            ref byte pinnableAddr = ref writer.PinnableAddress;

            var startOffset = idx;
            idx += 5;

            var utf16Source = MemoryMarshal.AsBytes(value.AsSpan());
            EncodingUtils.ToUtf8(ref MemoryMarshal.GetReference(utf16Source), utf16Source.Length,
                ref Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx), writer._capacity - idx, out _, out int written);

            uint count = (uint)written;
            IntPtr offset = (IntPtr)startOffset;
            unchecked
            {
                Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.Str32;
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = (byte)(count >> 24);
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 2) = (byte)(count >> 16);
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 3) = (byte)(count >> 8);
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 4) = (byte)count;
            }

            idx += written;
        }

        /// <summary>Unsafe. If pre-calculated byteCount of target string, can use this method.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteStringUnsafe(this ref MessagePackWriter writer, string value, int byteCount, ref int idx)
        {
            if (value == null) { WriteNil(ref writer, ref idx); return; }

            writer.Ensure(idx, byteCount + 5);

            ref byte pinnableAddr = ref writer.PinnableAddress;

            uint nLen = (uint)byteCount;
            // move body and write prefix
            if (nLen <= MaxFixStringLength)
            {
                Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = unchecked((byte)(MessagePackCode.MinFixStr | byteCount));
                idx++;
            }
            else if (nLen <= ByteMaxValue)
            {
                IntPtr offset = (IntPtr)idx;
                Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.Str8;
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)byteCount);
                idx += 2;
            }
            else if (nLen <= UShortMaxValue)
            {
                WriteShort(ref pinnableAddr, MessagePackCode.Str16, (ushort)byteCount, ref idx);
            }
            else
            {
                WriteInt(ref pinnableAddr, MessagePackCode.Str32, nLen, ref idx);
            }

            var utf16Source = MemoryMarshal.AsBytes(value.AsSpan());
            EncodingUtils.ToUtf8(ref MemoryMarshal.GetReference(utf16Source), utf16Source.Length,
                ref Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx), writer._capacity - idx, out _, out int written);
            idx += written;
        }

        /// <summary>Unsafe. If value is guranteed length is 0 ~ 31, can use this method.</summary>
        [Obsolete("=> WriteStringWithCache")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFixedStringUnsafe(this ref MessagePackWriter writer, string value, int byteCount, ref int idx)
        {
            writer.Ensure(idx, byteCount + 1);

            ref byte pinnableAddr = ref writer.PinnableAddress;

            Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = unchecked((byte)(MessagePackCode.MinFixStr | byteCount));
            idx++;

            var utf16Source = MemoryMarshal.AsBytes(value.AsSpan());
            EncodingUtils.ToUtf8(ref MemoryMarshal.GetReference(utf16Source), utf16Source.Length,
                ref Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx), writer._capacity - idx, out _, out int written);
            idx += written;
        }

        /// <summary>For short strings use only.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteStringWithCache(this ref MessagePackWriter writer, string value, ref int idx)
        {
            if (value == null) { WriteNil(ref writer, ref idx); return; }

            var encodedStr = MessagePackBinary.GetEncodedStringBytes(value);
            UnsafeMemory.WriteRaw(ref writer, encodedStr, ref idx);
        }
    }
}
