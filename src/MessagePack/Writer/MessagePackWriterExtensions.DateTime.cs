namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using MessagePack.Internal;

    public static partial class MessagePackWriterExtensions
    {
        // Timestamp spec
        // https://github.com/msgpack/msgpack/pull/209
        // FixExt4(-1) => seconds |  [1970-01-01 00:00:00 UTC, 2106-02-07 06:28:16 UTC) range
        // FixExt8(-1) => nanoseconds + seconds | [1970-01-01 00:00:00.000000000 UTC, 2514-05-30 01:53:04.000000000 UTC) range
        // Ext8(12,-1) => nanoseconds + seconds | [-584554047284-02-23 16:59:44 UTC, 584554051223-11-09 07:00:16.000000000 UTC) range

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDateTime(this ref MessagePackWriter writer, DateTime dateTime, ref int idx)
        {
            dateTime = dateTime.ToUniversalTime();

            var secondsSinceBclEpoch = dateTime.Ticks / TimeSpan.TicksPerSecond;
            var seconds = secondsSinceBclEpoch - DateTimeConstants.BclSecondsAtUnixEpoch;
            var nanoseconds = (dateTime.Ticks % TimeSpan.TicksPerSecond) * DateTimeConstants.NanosecondsPerTick;

            // reference pseudo code.
            /*
            struct timespec {
                long tv_sec;  // seconds
                long tv_nsec; // nanoseconds
            } time;
            if ((time.tv_sec >> 34) == 0)
            {
                uint64_t data64 = (time.tv_nsec << 34) | time.tv_sec;
                if (data & 0xffffffff00000000L == 0)
                {
                    // timestamp 32
                    uint32_t data32 = data64;
                    serialize(0xd6, -1, data32)
                }
                else
                {
                    // timestamp 64
                    serialize(0xd7, -1, data64)
                }
            }
            else
            {
                // timestamp 96
                serialize(0xc7, 12, -1, time.tv_nsec, time.tv_sec)
            }
            */

            writer.Ensure(idx, 15);

            ref byte b = ref writer.PinnableAddress;
            IntPtr offset = (IntPtr)idx;
            unchecked
            {
                if ((seconds >> 34) == 0)
                {
                    var data64 = (ulong)((nanoseconds << 34) | seconds);
                    if ((data64 & 0xffffffff00000000L) == 0)
                    {
                        // timestamp 32(seconds in 32-bit unsigned int)
                        var data32 = (UInt32)data64;
                        Unsafe.AddByteOffset(ref b, offset) = MessagePackCode.FixExt4;
                        Unsafe.AddByteOffset(ref b, offset + 1) = (byte)ReservedMessagePackExtensionTypeCode.DateTime;
                        Unsafe.AddByteOffset(ref b, offset + 2) = (byte)(data32 >> 24);
                        Unsafe.AddByteOffset(ref b, offset + 3) = (byte)(data32 >> 16);
                        Unsafe.AddByteOffset(ref b, offset + 4) = (byte)(data32 >> 8);
                        Unsafe.AddByteOffset(ref b, offset + 5) = (byte)data32;
                        idx += 6;
                    }
                    else
                    {
                        // timestamp 64(nanoseconds in 30-bit unsigned int | seconds in 34-bit unsigned int)
                        Unsafe.AddByteOffset(ref b, offset) = MessagePackCode.FixExt8;
                        Unsafe.AddByteOffset(ref b, offset + 1) = (byte)ReservedMessagePackExtensionTypeCode.DateTime;
                        Unsafe.AddByteOffset(ref b, offset + 2) = (byte)(data64 >> 56);
                        Unsafe.AddByteOffset(ref b, offset + 3) = (byte)(data64 >> 48);
                        Unsafe.AddByteOffset(ref b, offset + 4) = (byte)(data64 >> 40);
                        Unsafe.AddByteOffset(ref b, offset + 5) = (byte)(data64 >> 32);
                        Unsafe.AddByteOffset(ref b, offset + 6) = (byte)(data64 >> 24);
                        Unsafe.AddByteOffset(ref b, offset + 7) = (byte)(data64 >> 16);
                        Unsafe.AddByteOffset(ref b, offset + 8) = (byte)(data64 >> 8);
                        Unsafe.AddByteOffset(ref b, offset + 9) = (byte)data64;
                        idx += 10;
                    }
                }
                else
                {
                    // timestamp 96( nanoseconds in 32-bit unsigned int | seconds in 64-bit signed int )
                    Unsafe.AddByteOffset(ref b, offset) = MessagePackCode.Ext8;
                    Unsafe.AddByteOffset(ref b, offset + 1) = (byte)12;
                    Unsafe.AddByteOffset(ref b, offset + 2) = (byte)ReservedMessagePackExtensionTypeCode.DateTime;
                    Unsafe.AddByteOffset(ref b, offset + 3) = (byte)(nanoseconds >> 24);
                    Unsafe.AddByteOffset(ref b, offset + 4) = (byte)(nanoseconds >> 16);
                    Unsafe.AddByteOffset(ref b, offset + 5) = (byte)(nanoseconds >> 8);
                    Unsafe.AddByteOffset(ref b, offset + 6) = (byte)nanoseconds;
                    Unsafe.AddByteOffset(ref b, offset + 7) = (byte)(seconds >> 56);
                    Unsafe.AddByteOffset(ref b, offset + 8) = (byte)(seconds >> 48);
                    Unsafe.AddByteOffset(ref b, offset + 9) = (byte)(seconds >> 40);
                    Unsafe.AddByteOffset(ref b, offset + 10) = (byte)(seconds >> 32);
                    Unsafe.AddByteOffset(ref b, offset + 11) = (byte)(seconds >> 24);
                    Unsafe.AddByteOffset(ref b, offset + 12) = (byte)(seconds >> 16);
                    Unsafe.AddByteOffset(ref b, offset + 13) = (byte)(seconds >> 8);
                    Unsafe.AddByteOffset(ref b, offset + 14) = (byte)seconds;
                    idx += 15;
                }
            }
        }
    }
}
