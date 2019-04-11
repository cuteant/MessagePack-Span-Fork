namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;

    public static partial class MessagePackWriterExtensions
    {
        /// <summary>Write map count.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteMapHeader(this ref MessagePackWriter writer, int count, ref int idx)
        {
            writer.WriteMapHeader((uint)count, ref idx);
        }

        /// <summary>Write map count.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteMapHeader(this ref MessagePackWriter writer, uint count, ref int idx)
        {
            writer.Ensure(idx, 5);
            if (count <= MessagePackRange.MaxFixMapCount)
            {
                Unsafe.AddByteOffset(ref writer.PinnableAddress, (IntPtr)idx) = unchecked((byte)(MessagePackCode.MinFixMap | count));
                idx++;
            }
            else if (count <= UShortMaxValue)
            {
                WriteShort(ref writer.PinnableAddress, MessagePackCode.Map16, unchecked((ushort)count), ref idx);
            }
            else
            {
                WriteInt(ref writer.PinnableAddress, MessagePackCode.Map32, count, ref idx);
            }
        }

        /// <summary>Write map format header, always use map32 format(length is fixed, 5).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteMapHeaderForceMap32Block(this ref MessagePackWriter writer, uint count, ref int idx)
        {
            writer.Ensure(idx, 5);
            WriteInt(ref writer.PinnableAddress, MessagePackCode.Map32, count, ref idx);
        }

        /// <summary>Unsafe. If value is guranteed 0 ~ MessagePackRange.MaxFixMapCount(15), can use this method.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFixedMapHeaderUnsafe(this ref MessagePackWriter writer, int count, ref int idx)
        {
            writer.Ensure(idx, 1);
            Unsafe.AddByteOffset(ref writer.PinnableAddress, (IntPtr)idx) = (byte)(MessagePackCode.MinFixMap | count);
            idx++;
        }
    }
}
