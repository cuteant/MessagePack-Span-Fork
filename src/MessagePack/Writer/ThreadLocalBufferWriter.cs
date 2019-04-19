#if DEPENDENT_ON_CUTEANT

namespace MessagePack
{
    using System.Buffers;
    using CuteAnt.Buffers;

    internal sealed class ThreadLocalBufferWriter : ThreadLocalBufferWriter<ThreadLocalBufferWriter>
    {
        public ThreadLocalBufferWriter() : base(MessagePackBinary.Shared) { }

        public ThreadLocalBufferWriter(int initialCapacity) : base(MessagePackBinary.Shared, initialCapacity) { }

        public ThreadLocalBufferWriter(ArrayPool<byte> arrayPool) : base(arrayPool) { }

        public ThreadLocalBufferWriter(ArrayPool<byte> arrayPool, int initialCapacity) : base(arrayPool, initialCapacity) { }
    }
}

#endif