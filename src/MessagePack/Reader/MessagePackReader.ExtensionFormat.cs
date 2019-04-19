namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ExtensionResult ReadExtensionFormat()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return extDecoders[position].Read(ref this, ref position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte GetExtensionFormatTypeCode()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return extTypeCodeDecoders[position].Read(ref this, ref position);
        }

        /// <summary>return byte length of ExtensionFormat.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ExtensionHeader ReadExtensionFormatHeader()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return extHeaderDecoders[position].Read(ref this, ref position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsLZ4Binary()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return (MessagePackCode.ToMessagePackType(position) == MessagePackType.Extension &&
                    extTypeCodeDecoders[position].Read(ref this, ref position) == ReservedMessagePackExtensionTypeCode.LZ4)
                    ? true : false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsTypelessFormat(out bool isNil)
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);

            if (MessagePackCode.Nil == position)
            {
                isNil = true;
                Advance(1);
                return false;
            }

            var isTypeless = (MessagePackCode.ToMessagePackType(position) == MessagePackType.Extension &&
                    extTypeCodeDecoders[position].Read(ref this, ref position) == ReservedMessagePackExtensionTypeCode.Typeless)
                    ? true : false;
            if (isTypeless) { Advance(6); }
            isNil = false;
            return isTypeless;
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal interface IExtDecoder
    {
        ExtensionResult Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixExt1 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new FixExt1();

        FixExt1() { }

        public ExtensionResult Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionResult ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            var valueSpan = reader.PeekFast(2, 1);
            reader.AdvanceWithinSpan(3);
            return new ExtensionResult(typeCode, valueSpan);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionResult ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(3);
            reader.Advance(3);
            var typeCode = unchecked((sbyte)valueSpan[1]);
            return new ExtensionResult(typeCode, valueSpan.Slice(2, 1));
        }
    }

    internal sealed class FixExt2 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new FixExt2();

        FixExt2() { }

        public ExtensionResult Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionResult ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            var valueSpan = reader.PeekFast(2, 2);
            reader.AdvanceWithinSpan(4);
            return new ExtensionResult(typeCode, valueSpan);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionResult ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(4);
            reader.Advance(4);
            var typeCode = unchecked((sbyte)valueSpan[1]);
            return new ExtensionResult(typeCode, valueSpan.Slice(2, 2));
        }
    }

    internal sealed class FixExt4 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new FixExt4();

        FixExt4() { }

        public ExtensionResult Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionResult ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            var valueSpan = reader.PeekFast(2, 4);
            reader.AdvanceWithinSpan(6);
            return new ExtensionResult(typeCode, valueSpan);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionResult ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(6);
            reader.Advance(6);
            var typeCode = unchecked((sbyte)valueSpan[1]);
            return new ExtensionResult(typeCode, valueSpan.Slice(2, 4));
        }
    }

    internal sealed class FixExt8 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new FixExt8();

        FixExt8() { }

        public ExtensionResult Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionResult ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            var valueSpan = reader.PeekFast(2, 8);
            reader.AdvanceWithinSpan(10);
            return new ExtensionResult(typeCode, valueSpan);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionResult ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(10);
            reader.Advance(10);
            var typeCode = unchecked((sbyte)valueSpan[1]);
            return new ExtensionResult(typeCode, valueSpan.Slice(2, 8));
        }
    }

    internal sealed class FixExt16 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new FixExt16();

        FixExt16() { }

        public ExtensionResult Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionResult ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            var valueSpan = reader.PeekFast(2, 16);
            reader.AdvanceWithinSpan(18);
            return new ExtensionResult(typeCode, valueSpan);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionResult ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(18);
            reader.Advance(18);
            var typeCode = unchecked((sbyte)valueSpan[1]);
            return new ExtensionResult(typeCode, valueSpan.Slice(2, 16));
        }
    }

    internal sealed class Ext8 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new Ext8();

        Ext8() { }

        public ExtensionResult Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionResult ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var length = Unsafe.AddByteOffset(ref position, offset); // pos 1
            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref position, offset + 1)); // pos 2

            if (0u >= length)
            {
                reader.AdvanceWithinSpan(3);
                return new ExtensionResult(typeCode, ReadOnlySpan<byte>.Empty);
            }

            var valueSpan = reader.PeekFast(3, length);
            reader.AdvanceWithinSpan(length + 3);
            return new ExtensionResult(typeCode, valueSpan);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionResult ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(3);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            var length = Unsafe.AddByteOffset(ref b, offset); // pos 1
            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref b, offset + 1)); // pos 2

            if (0u >= length)
            {
                reader.Advance(3);
                return new ExtensionResult(typeCode, ReadOnlySpan<byte>.Empty);
            }

            var readSize = length + 3;
            var valueSpan = reader.Peek(readSize);
            reader.Advance(readSize);
            return new ExtensionResult(typeCode, valueSpan.Slice(3, length));
        }
    }

    internal sealed class Ext16 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new Ext16();

        Ext16() { }

        public ExtensionResult Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionResult ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length = (UInt16)(Unsafe.AddByteOffset(ref position, offset) << 8) | (UInt16)Unsafe.AddByteOffset(ref position, offset + 1);
                var typeCode = (sbyte)Unsafe.AddByteOffset(ref position, offset + 2);

                var valueSpan = reader.PeekFast(4, length);
                reader.AdvanceWithinSpan(length + 4);
                return new ExtensionResult(typeCode, valueSpan);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionResult ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(4);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length = (UInt16)(Unsafe.AddByteOffset(ref b, offset) << 8) | (UInt16)Unsafe.AddByteOffset(ref b, offset + 1);
                var typeCode = (sbyte)Unsafe.AddByteOffset(ref b, offset + 2);

                var readSize = length + 4;
                var valueSpan = reader.Peek(readSize);
                reader.Advance(readSize);
                return new ExtensionResult(typeCode, valueSpan.Slice(4, length));
            }
        }
    }

    internal sealed class Ext32 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new Ext32();

        Ext32() { }

        public ExtensionResult Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionResult ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length = (int)(
                    (UInt32)(Unsafe.AddByteOffset(ref position, offset) << 24) |
                    (UInt32)(Unsafe.AddByteOffset(ref position, offset + 1) << 16) |
                    (UInt32)(Unsafe.AddByteOffset(ref position, offset + 2) << 8) |
                    (UInt32)Unsafe.AddByteOffset(ref position, offset + 3));
                var typeCode = (sbyte)Unsafe.AddByteOffset(ref position, offset + 4);

                var valueSpan = reader.PeekFast(6, length);
                reader.AdvanceWithinSpan(length + 6);
                return new ExtensionResult(typeCode, valueSpan);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionResult ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(6);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length = (int)(
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset) << 24) |
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 1) << 16) |
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 2) << 8) |
                    (UInt32)Unsafe.AddByteOffset(ref b, offset + 3));
                var typeCode = (sbyte)Unsafe.AddByteOffset(ref b, offset + 4);

                var readSize = length + 6;
                var valueSpan = reader.Peek(readSize);
                reader.Advance(readSize);
                return new ExtensionResult(typeCode, valueSpan.Slice(6, length));
            }
        }
    }

    internal sealed class InvalidExt : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new InvalidExt();

        InvalidExt() { }

        public ExtensionResult Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }




    internal interface IExtHeaderDecoder
    {
        ExtensionHeader Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixExt1Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new FixExt1Header();

        FixExt1Header() { }

        public ExtensionHeader Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionHeader ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            reader.AdvanceWithinSpan(2);
            return new ExtensionHeader(typeCode, 1u);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionHeader ReadMultisegment(ref MessagePackReader reader)
        {
            var typeCode = unchecked((sbyte)reader.GetRawByte(1));
            reader.Advance(2);
            return new ExtensionHeader(typeCode, 1u);
        }
    }

    internal sealed class FixExt2Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new FixExt2Header();

        FixExt2Header() { }

        public ExtensionHeader Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionHeader ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            reader.AdvanceWithinSpan(2);
            return new ExtensionHeader(typeCode, 2u);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionHeader ReadMultisegment(ref MessagePackReader reader)
        {
            var typeCode = unchecked((sbyte)reader.GetRawByte(1));
            reader.Advance(2);
            return new ExtensionHeader(typeCode, 2u);
        }
    }

    internal sealed class FixExt4Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new FixExt4Header();

        FixExt4Header() { }

        public ExtensionHeader Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionHeader ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            reader.AdvanceWithinSpan(2);
            return new ExtensionHeader(typeCode, 4u);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionHeader ReadMultisegment(ref MessagePackReader reader)
        {
            var typeCode = unchecked((sbyte)reader.GetRawByte(1));
            reader.Advance(2);
            return new ExtensionHeader(typeCode, 4u);
        }
    }

    internal sealed class FixExt8Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new FixExt8Header();

        FixExt8Header() { }

        public ExtensionHeader Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionHeader ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            reader.AdvanceWithinSpan(2);
            return new ExtensionHeader(typeCode, 8u);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionHeader ReadMultisegment(ref MessagePackReader reader)
        {
            var typeCode = unchecked((sbyte)reader.GetRawByte(1));
            reader.Advance(2);
            return new ExtensionHeader(typeCode, 8u);
        }
    }

    internal sealed class FixExt16Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new FixExt16Header();

        FixExt16Header() { }

        public ExtensionHeader Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionHeader ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var typeCode = unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            reader.AdvanceWithinSpan(2);
            return new ExtensionHeader(typeCode, 16u);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionHeader ReadMultisegment(ref MessagePackReader reader)
        {
            var typeCode = unchecked((sbyte)reader.GetRawByte(1));
            reader.Advance(2);
            return new ExtensionHeader(typeCode, 16u);
        }
    }

    internal sealed class Ext8Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new Ext8Header();

        Ext8Header() { }

        public ExtensionHeader Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionHeader ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length = Unsafe.AddByteOffset(ref position, offset);
                var typeCode = (sbyte)Unsafe.AddByteOffset(ref position, offset + 1);
                reader.AdvanceWithinSpan(3);
                return new ExtensionHeader(typeCode, length);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionHeader ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(3);
            reader.Advance(3);
            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length = Unsafe.AddByteOffset(ref b, offset);
                var typeCode = (sbyte)Unsafe.AddByteOffset(ref b, offset + 1);
                return new ExtensionHeader(typeCode, length);
            }
        }
    }

    internal sealed class Ext16Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new Ext16Header();

        Ext16Header() { }

        public ExtensionHeader Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExtensionHeader ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length = (UInt32)((UInt16)(Unsafe.AddByteOffset(ref position, offset) << 8) | Unsafe.AddByteOffset(ref position, offset + 1));
                var typeCode = (sbyte)Unsafe.AddByteOffset(ref position, offset + 2);
                reader.AdvanceWithinSpan(4);
                return new ExtensionHeader(typeCode, length);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExtensionHeader ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(4);
            reader.Advance(4);
            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length = (UInt32)((UInt16)(Unsafe.AddByteOffset(ref b, offset) << 8) | Unsafe.AddByteOffset(ref b, offset + 1));
                var typeCode = (sbyte)Unsafe.AddByteOffset(ref b, offset + 2);
                return new ExtensionHeader(typeCode, length);
            }
        }
    }

    internal sealed class Ext32Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new Ext32Header();

        Ext32Header() { }

        public ExtensionHeader Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ExtensionHeader ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length =
                    (UInt32)(Unsafe.AddByteOffset(ref position, offset) << 24) |
                    (UInt32)(Unsafe.AddByteOffset(ref position, offset + 1) << 16) |
                    (UInt32)(Unsafe.AddByteOffset(ref position, offset + 2) << 8) |
                    (UInt32)Unsafe.AddByteOffset(ref position, offset + 3);
                var typeCode = (sbyte)Unsafe.AddByteOffset(ref position, offset + 4);
                reader.AdvanceWithinSpan(6);
                return new ExtensionHeader(typeCode, length);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ExtensionHeader ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(6);
            reader.Advance(6);
            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length =
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset) << 24) |
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 1) << 16) |
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 2) << 8) |
                    (UInt32)Unsafe.AddByteOffset(ref b, offset + 3);
                var typeCode = (sbyte)Unsafe.AddByteOffset(ref b, offset + 4);
                return new ExtensionHeader(typeCode, length);
            }
        }
    }

    internal sealed class InvalidExtHeader : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new InvalidExtHeader();

        InvalidExtHeader() { }

        public ExtensionHeader Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }


    internal interface IExtTypeCodeDecoder
    {
        sbyte Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixExt1TypeCode : IExtTypeCodeDecoder
    {
        internal static readonly IExtTypeCodeDecoder Instance = new FixExt1TypeCode();

        FixExt1TypeCode() { }

        public sbyte Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static sbyte ReadMultisegment(ref MessagePackReader reader)
        {
            return unchecked((sbyte)reader.GetRawByte(1));
        }
    }

    internal sealed class FixExt2TypeCode : IExtTypeCodeDecoder
    {
        internal static readonly IExtTypeCodeDecoder Instance = new FixExt2TypeCode();

        FixExt2TypeCode() { }

        public sbyte Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static sbyte ReadMultisegment(ref MessagePackReader reader)
        {
            return unchecked((sbyte)reader.GetRawByte(1));
        }
    }

    internal sealed class FixExt4TypeCode : IExtTypeCodeDecoder
    {
        internal static readonly IExtTypeCodeDecoder Instance = new FixExt4TypeCode();

        FixExt4TypeCode() { }

        public sbyte Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static sbyte ReadMultisegment(ref MessagePackReader reader)
        {
            return unchecked((sbyte)reader.GetRawByte(1));
        }
    }

    internal sealed class FixExt8TypeCode : IExtTypeCodeDecoder
    {
        internal static readonly IExtTypeCodeDecoder Instance = new FixExt8TypeCode();

        FixExt8TypeCode() { }

        public sbyte Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static sbyte ReadMultisegment(ref MessagePackReader reader)
        {
            return unchecked((sbyte)reader.GetRawByte(1));
        }
    }

    internal sealed class FixExt16TypeCode : IExtTypeCodeDecoder
    {
        internal static readonly IExtTypeCodeDecoder Instance = new FixExt16TypeCode();

        FixExt16TypeCode() { }

        public sbyte Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static sbyte ReadMultisegment(ref MessagePackReader reader)
        {
            return unchecked((sbyte)reader.GetRawByte(1));
        }
    }

    internal sealed class Ext8TypeCode : IExtTypeCodeDecoder
    {
        internal static readonly IExtTypeCodeDecoder Instance = new Ext8TypeCode();

        Ext8TypeCode() { }

        public sbyte Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)2));
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static sbyte ReadMultisegment(ref MessagePackReader reader)
        {
            return unchecked((sbyte)reader.GetRawByte(2));
        }
    }

    internal sealed class Ext16TypeCode : IExtTypeCodeDecoder
    {
        internal static readonly IExtTypeCodeDecoder Instance = new Ext16TypeCode();

        Ext16TypeCode() { }

        public sbyte Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)3));
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static sbyte ReadMultisegment(ref MessagePackReader reader)
        {
            return unchecked((sbyte)reader.GetRawByte(3));
        }
    }

    internal sealed class Ext32TypeCode : IExtTypeCodeDecoder
    {
        internal static readonly IExtTypeCodeDecoder Instance = new Ext32TypeCode();

        Ext32TypeCode() { }

        public sbyte Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)5));
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static sbyte ReadMultisegment(ref MessagePackReader reader)
        {
            return unchecked((sbyte)reader.GetRawByte(5));
        }
    }

    internal sealed class InvalidExtTypeCode : IExtTypeCodeDecoder
    {
        internal static readonly IExtTypeCodeDecoder Instance = new InvalidExtTypeCode();

        InvalidExtTypeCode() { }

        public sbyte Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}