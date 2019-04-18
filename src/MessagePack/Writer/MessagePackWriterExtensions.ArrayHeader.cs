namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;

    public static partial class MessagePackWriterExtensions
    {
        /// <summary>Write array count.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteArrayHeader(this ref MessagePackWriter writer, int count, ref int idx)
        {
            checked
            {
                writer.WriteArrayHeader((uint)count, ref idx);
            }
        }

        /// <summary>Write array count.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteArrayHeader(this ref MessagePackWriter writer, uint count, ref int idx)
        {
            writer.Ensure(idx, 5);
            if (count <= MaxFixArrayCount)
            {
                Unsafe.AddByteOffset(ref writer.PinnableAddress, (IntPtr)idx) = unchecked((byte)(MessagePackCode.MinFixArray | count));
                idx++;
            }
            else if (count <= UShortMaxValue)
            {
                WriteShort(ref writer.PinnableAddress, MessagePackCode.Array16, unchecked((ushort)count), ref idx);
            }
            else
            {
                WriteInt(ref writer.PinnableAddress, MessagePackCode.Array32, count, ref idx);
            }
        }

        /// <summary>Write array format header, always use array32 format(length is fixed, 5).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteArrayHeaderForceArray32Block(this ref MessagePackWriter writer, uint count, ref int idx)
        {
            writer.Ensure(idx, 5);
            WriteInt(ref writer.PinnableAddress, MessagePackCode.Array32, count, ref idx);
        }

        /// <summary>Unsafe. If value is guranteed 0 ~ MessagePackRange.MaxFixArrayCount(15), can use this method.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFixedArrayHeaderUnsafe(this ref MessagePackWriter writer, int count, ref int idx)
        {
            writer.Ensure(idx, 1);
            Unsafe.AddByteOffset(ref writer.PinnableAddress, (IntPtr)idx) = (byte)(MessagePackCode.MinFixArray | count);
            idx++;
        }
    }
}
