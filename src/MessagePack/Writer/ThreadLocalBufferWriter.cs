
namespace MessagePack
{
    using System;
    using System.Buffers;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal sealed class ThreadLocalBufferWriter : ThreadLocalBufferWriter<ThreadLocalBufferWriter>
    {
        public ThreadLocalBufferWriter() : base(MessagePackBinary.Shared) { }

        public ThreadLocalBufferWriter(int initialCapacity) : base(MessagePackBinary.Shared, initialCapacity) { }

        public ThreadLocalBufferWriter(ArrayPool<byte> arrayPool) : base(arrayPool) { }

        public ThreadLocalBufferWriter(ArrayPool<byte> arrayPool, int initialCapacity) : base(arrayPool, initialCapacity) { }
    }

    public abstract class ThreadLocalBufferWriter<TWriter> : ThreadLocalBufferWriter<byte, ThreadLocalBufferWriter<TWriter>>
        where TWriter : ThreadLocalBufferWriter<TWriter>
    {
        public ThreadLocalBufferWriter(ArrayPool<byte> arrayPool) : base(arrayPool) { }

        public ThreadLocalBufferWriter(ArrayPool<byte> arrayPool, int initialCapacity) : base(arrayPool, initialCapacity) { }

        public override byte[] ToArray()
        {
            uint nLen = (uint)_writerIndex;
            if (0u >= nLen) { return MessagePackBinary.Empty; }

            var destination = new byte[_writerIndex];
            MessagePackBinary.CopyMemory(_borrowedBuffer, 0, destination, 0, _writerIndex);
            return destination;
        }
    }

    public abstract class ThreadLocalBufferWriter<T, TWriter> : ArrayBufferWriter<T, TWriter>
        where TWriter : ThreadLocalBufferWriter<T, TWriter>
    {
        private bool _useThreadLocal;

        public ThreadLocalBufferWriter(ArrayPool<T> arrayPool) : this(arrayPool, s_defaultBufferSize) { }

        public ThreadLocalBufferWriter(ArrayPool<T> arrayPool, int initialCapacity) : base()
        {
            if (null == arrayPool) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.arrayPool); }
            //if (initialCapacity <= 0) ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.initialCapacity);
            if (((uint)(initialCapacity - 1)) > c_maxBufferSize) { initialCapacity = s_defaultBufferSize; }

            _arrayPool = arrayPool;
            var buffer = ((uint)initialCapacity <= InternalMemoryPool.InitialCapacity) ? InternalMemoryPool.GetBuffer() : null;
            if (buffer != null)
            {
                _useThreadLocal = true;
            }
            else
            {
                _useThreadLocal = false;
                buffer = _arrayPool.Rent(initialCapacity);
            }
            _borrowedBuffer = buffer;
        }

        // Returns the rented buffer back to the pool
        protected override void Dispose(bool disposing)
        {
            //ClearHelper();
            var borrowedBuffer = _borrowedBuffer;
            _borrowedBuffer = null;
            if (_useThreadLocal)
            {
                Release();
            }
            else
            {
                _arrayPool.Return(borrowedBuffer);
            }
            _arrayPool = null;
        }

        protected override void CheckAndResizeBuffer(int sizeHint)
        {
            Debug.Assert(_borrowedBuffer != null);

            //if (sizeHint < 0) ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.sizeHint);
            //if (sizeHint == 0)
            if (unchecked((uint)(sizeHint - 1)) > c_maxBufferSize)
            {
                sizeHint = c_minimumBufferSize;
            }

            int availableSpace = _borrowedBuffer.Length - _writerIndex;

            if (sizeHint > availableSpace)
            {
                var growBy = Math.Max(sizeHint, _borrowedBuffer.Length);
                var newSize = checked(_borrowedBuffer.Length + growBy);

                T[] oldBuffer = _borrowedBuffer;

                _borrowedBuffer = _arrayPool.Rent(newSize);

                Debug.Assert(oldBuffer.Length >= _writerIndex);
                Debug.Assert(_borrowedBuffer.Length >= _writerIndex);

                Span<T> previousBuffer = oldBuffer.AsSpan(0, _writerIndex);
                previousBuffer.CopyTo(_borrowedBuffer);
                previousBuffer.Clear();

                if (_useThreadLocal)
                {
                    Release();
                }
                else
                {
                    _arrayPool.Return(oldBuffer);
                }
            }

            Debug.Assert(_borrowedBuffer.Length - _writerIndex > 0);
            Debug.Assert(_borrowedBuffer.Length - _writerIndex >= sizeHint);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Release()
        {
            _useThreadLocal = false;
            InternalMemoryPool.Free();
        }

        #region == InternalMemoryPool ==

        sealed class InternalWrappingBuffer
        {
            public T[] Buffer;
            public bool Idle;
        }
        static class InternalMemoryPool
        {
            private static readonly int s_initialCapacity;
            internal static readonly uint InitialCapacity;

            static InternalMemoryPool()
            {
                s_initialCapacity = 1 + ((64 * 1024 - 1) / Unsafe.SizeOf<T>());
                InitialCapacity = (uint)s_initialCapacity;
            }

            [ThreadStatic]
            static InternalWrappingBuffer s_wrappingBuffer = null;

            public static T[] GetBuffer()
            {
                if (s_wrappingBuffer == null)
                {
                    s_wrappingBuffer = new InternalWrappingBuffer { Buffer = new T[s_initialCapacity], Idle = true };
                }
                if (s_wrappingBuffer.Idle)
                {
                    s_wrappingBuffer.Idle = false;
                    return s_wrappingBuffer.Buffer;
                }
                return null;
            }

            public static void Free()
            {
                Debug.Assert(s_wrappingBuffer != null);
                s_wrappingBuffer.Idle = true;
            }
        }

        #endregion
    }
}
