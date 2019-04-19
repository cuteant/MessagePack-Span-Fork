namespace MessagePack
{
    using System.Buffers;
#if DEPENDENT_ON_CUTEANT
    using CuteAnt.Buffers;
#endif

    internal sealed class LZ4ThreadLocalBufferWriter : ThreadLocalBufferWriter<LZ4ThreadLocalBufferWriter>
    {
        public LZ4ThreadLocalBufferWriter() : base(MessagePackBinary.Shared) { }

        public LZ4ThreadLocalBufferWriter(int initialCapacity) : base(MessagePackBinary.Shared, initialCapacity) { }

        public LZ4ThreadLocalBufferWriter(ArrayPool<byte> arrayPool) : base(arrayPool) { }

        public LZ4ThreadLocalBufferWriter(ArrayPool<byte> arrayPool, int initialCapacity) : base(arrayPool, initialCapacity) { }
    }
}
