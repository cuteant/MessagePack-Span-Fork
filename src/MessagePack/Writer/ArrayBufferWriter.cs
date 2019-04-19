#if DEPENDENT_ON_CUTEANT

namespace MessagePack
{
    using System.Buffers;
    using CuteAnt.Buffers;

    public sealed class ArrayBufferWriter : ArrayBufferWriter<ArrayBufferWriter>
    {
        public ArrayBufferWriter() : base(MessagePackBinary.Shared) { }

        public ArrayBufferWriter(int initialCapacity) : base(MessagePackBinary.Shared, initialCapacity) { }

        public ArrayBufferWriter(ArrayPool<byte> arrayPool) : base(arrayPool) { }

        public ArrayBufferWriter(ArrayPool<byte> arrayPool, int initialCapacity) : base(arrayPool, initialCapacity) { }
    }
}

#endif