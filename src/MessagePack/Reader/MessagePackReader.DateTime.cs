namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime ReadDateTime()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return dateTimeDecoders[position].Read(ref this, ref position);
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using MessagePack.Internal;

    internal interface IDateTimeDecoder
    {
        DateTime Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixExt4DateTime : IDateTimeDecoder
    {
        internal static readonly IDateTimeDecoder Instance = new FixExt4DateTime();

        FixExt4DateTime() { }

        public DateTime Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DateTime ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;

            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref position, offset));
            if (typeCode != ReservedMessagePackExtensionTypeCode.DateTime)
            {
                ThrowHelper.ThrowInvalidOperationException_TypeCode(typeCode);
            }

            unchecked
            {
                var seconds =
                    (UInt32)(Unsafe.AddByteOffset(ref position, offset + 1) << 24) |
                    (UInt32)(Unsafe.AddByteOffset(ref position, offset + 2) << 16) |
                    (UInt32)(Unsafe.AddByteOffset(ref position, offset + 3) << 8) |
                    (UInt32)Unsafe.AddByteOffset(ref position, offset + 4);

                reader.AdvanceWithinSpan(6);
                return DateTimeConstants.UnixEpoch.AddSeconds(seconds);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static DateTime ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(6);
            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;

            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref b, offset));
            if (typeCode != ReservedMessagePackExtensionTypeCode.DateTime)
            {
                ThrowHelper.ThrowInvalidOperationException_TypeCode(typeCode);
            }

            unchecked
            {
                var seconds =
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 1) << 24) |
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 2) << 16) |
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 3) << 8) |
                    (UInt32)Unsafe.AddByteOffset(ref b, offset + 4);

                reader.Advance(6);
                return DateTimeConstants.UnixEpoch.AddSeconds(seconds);
            }
        }
    }

    internal sealed class FixExt8DateTime : IDateTimeDecoder
    {
        internal static readonly IDateTimeDecoder Instance = new FixExt8DateTime();

        FixExt8DateTime() { }

        public DateTime Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DateTime ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;

            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref position, offset));
            if (typeCode != ReservedMessagePackExtensionTypeCode.DateTime)
            {
                ThrowHelper.ThrowInvalidOperationException_TypeCode(typeCode);
            }

            var data64 = (UInt64)Unsafe.AddByteOffset(ref position, offset + 1) << 56
                       | (UInt64)Unsafe.AddByteOffset(ref position, offset + 2) << 48
                       | (UInt64)Unsafe.AddByteOffset(ref position, offset + 3) << 40
                       | (UInt64)Unsafe.AddByteOffset(ref position, offset + 4) << 32
                       | (UInt64)Unsafe.AddByteOffset(ref position, offset + 5) << 24
                       | (UInt64)Unsafe.AddByteOffset(ref position, offset + 6) << 16
                       | (UInt64)Unsafe.AddByteOffset(ref position, offset + 7) << 8
                       | (UInt64)Unsafe.AddByteOffset(ref position, offset + 8);

            var nanoseconds = (long)(data64 >> 34);
            var seconds = data64 & 0x00000003ffffffffL;

            reader.AdvanceWithinSpan(10);
            return DateTimeConstants.UnixEpoch.AddSeconds(seconds).AddTicks(nanoseconds / DateTimeConstants.NanosecondsPerTick);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static DateTime ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(10);
            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;

            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref b, offset));
            if (typeCode != ReservedMessagePackExtensionTypeCode.DateTime)
            {
                ThrowHelper.ThrowInvalidOperationException_TypeCode(typeCode);
            }

            var data64 = (UInt64)Unsafe.AddByteOffset(ref b, offset + 1) << 56
                       | (UInt64)Unsafe.AddByteOffset(ref b, offset + 2) << 48
                       | (UInt64)Unsafe.AddByteOffset(ref b, offset + 3) << 40
                       | (UInt64)Unsafe.AddByteOffset(ref b, offset + 4) << 32
                       | (UInt64)Unsafe.AddByteOffset(ref b, offset + 5) << 24
                       | (UInt64)Unsafe.AddByteOffset(ref b, offset + 6) << 16
                       | (UInt64)Unsafe.AddByteOffset(ref b, offset + 7) << 8
                       | (UInt64)Unsafe.AddByteOffset(ref b, offset + 8);

            var nanoseconds = (long)(data64 >> 34);
            var seconds = data64 & 0x00000003ffffffffL;

            reader.Advance(10);
            return DateTimeConstants.UnixEpoch.AddSeconds(seconds).AddTicks(nanoseconds / DateTimeConstants.NanosecondsPerTick);
        }
    }

    internal sealed class Ext8DateTime : IDateTimeDecoder
    {
        internal static readonly IDateTimeDecoder Instance = new Ext8DateTime();

        Ext8DateTime() { }

        public DateTime Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DateTime ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;

            var length = checked((byte)Unsafe.AddByteOffset(ref position, offset));
            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref position, offset + 1));
            if (length != 12 || typeCode != ReservedMessagePackExtensionTypeCode.DateTime)
            {
                ThrowHelper.ThrowInvalidOperationException_TypeCode(typeCode);
            }

            var nanoseconds =
                (UInt32)(Unsafe.AddByteOffset(ref position, offset + 2) << 24) |
                (UInt32)(Unsafe.AddByteOffset(ref position, offset + 3) << 16) |
                (UInt32)(Unsafe.AddByteOffset(ref position, offset + 4) << 8) |
                (UInt32)Unsafe.AddByteOffset(ref position, offset + 5);
            unchecked
            {
                var seconds =
                    (long)Unsafe.AddByteOffset(ref position, offset + 6) << 56 |
                    (long)Unsafe.AddByteOffset(ref position, offset + 7) << 48 |
                    (long)Unsafe.AddByteOffset(ref position, offset + 8) << 40 |
                    (long)Unsafe.AddByteOffset(ref position, offset + 9) << 32 |
                    (long)Unsafe.AddByteOffset(ref position, offset + 10) << 24 |
                    (long)Unsafe.AddByteOffset(ref position, offset + 11) << 16 |
                    (long)Unsafe.AddByteOffset(ref position, offset + 12) << 8 |
                    (long)Unsafe.AddByteOffset(ref position, offset + 13);

                reader.AdvanceWithinSpan(15);
                return DateTimeConstants.UnixEpoch.AddSeconds(seconds).AddTicks(nanoseconds / DateTimeConstants.NanosecondsPerTick);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static DateTime ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(15);
            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;

            var length = checked((byte)Unsafe.AddByteOffset(ref b, offset));
            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref b, offset + 1));
            if (length != 12 || typeCode != ReservedMessagePackExtensionTypeCode.DateTime)
            {
                ThrowHelper.ThrowInvalidOperationException_TypeCode(typeCode);
            }

            var nanoseconds =
                (UInt32)(Unsafe.AddByteOffset(ref b, offset + 2) << 24) |
                (UInt32)(Unsafe.AddByteOffset(ref b, offset + 3) << 16) |
                (UInt32)(Unsafe.AddByteOffset(ref b, offset + 4) << 8) |
                (UInt32)Unsafe.AddByteOffset(ref b, offset + 5);
            unchecked
            {
                var seconds =
                    (long)Unsafe.AddByteOffset(ref b, offset + 6) << 56 |
                    (long)Unsafe.AddByteOffset(ref b, offset + 7) << 48 |
                    (long)Unsafe.AddByteOffset(ref b, offset + 8) << 40 |
                    (long)Unsafe.AddByteOffset(ref b, offset + 9) << 32 |
                    (long)Unsafe.AddByteOffset(ref b, offset + 10) << 24 |
                    (long)Unsafe.AddByteOffset(ref b, offset + 11) << 16 |
                    (long)Unsafe.AddByteOffset(ref b, offset + 12) << 8 |
                    (long)Unsafe.AddByteOffset(ref b, offset + 13);

                reader.Advance(15);
                return DateTimeConstants.UnixEpoch.AddSeconds(seconds).AddTicks(nanoseconds / DateTimeConstants.NanosecondsPerTick);
            }
        }
    }

    internal sealed class InvalidDateTime : IDateTimeDecoder
    {
        internal static readonly IDateTimeDecoder Instance = new InvalidDateTime();

        InvalidDateTime() { }

        public DateTime Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}