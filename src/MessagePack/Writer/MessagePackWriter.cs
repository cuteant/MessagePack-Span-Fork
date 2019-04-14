namespace MessagePack
{
    using System;
    using System.Buffers;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public ref partial struct MessagePackWriter
    {
        private const uint c_maximumBufferSize = int.MaxValue;
        private const int c_minimumBufferSize = 256;
        private const int c_defaultBufferSize = 64 * 1024;

        private ArrayPool<byte> _arrayPool;
        private bool _useThreadLocal;

        internal byte[] _borrowedBuffer;
        internal int _capacity;

        /// <summary>TBD</summary>
        public ref byte PinnableAddress
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _borrowedBuffer[0];
        }

        public byte[] Buffer => _borrowedBuffer;

        public int Capacity => _capacity;

        /// <summary>Constructs a new <see cref="MessagePackWriter"/> instance.</summary>
        public MessagePackWriter(bool useThreadLocalBuffer)
        {
            if (useThreadLocalBuffer)
            {
                _useThreadLocal = true;
                _arrayPool = null;
                _borrowedBuffer = InternalMemoryPool.GetBuffer();
            }
            else
            {
                _useThreadLocal = false;
                _arrayPool = MessagePackBinary.Shared;
                _borrowedBuffer = _arrayPool.Rent(c_defaultBufferSize);
            }
            _capacity = _borrowedBuffer.Length;
        }

        /// <summary>Constructs a new <see cref="MessagePackWriter"/> instance.</summary>
        public MessagePackWriter(int initialCapacity)
        {
            if (((uint)(initialCapacity - 1)) > c_maximumBufferSize) { initialCapacity = c_defaultBufferSize; }

            _useThreadLocal = false;
            _arrayPool = MessagePackBinary.Shared;
            _borrowedBuffer = _arrayPool.Rent(c_defaultBufferSize);
            _capacity = _borrowedBuffer.Length;
        }

        public byte[] ToArray(int alreadyWritten)
        {
            Debug.Assert(alreadyWritten >= 0);
            uint nLen = (uint)alreadyWritten;
            if (0u >= nLen) { return MessagePackBinary.Empty; }

            var destination = new byte[alreadyWritten];
            MessagePackBinary.CopyMemory(_borrowedBuffer, 0, destination, 0, alreadyWritten);
            if (_arrayPool != null)
            {
                _arrayPool.Return(_borrowedBuffer);
                _arrayPool = null;
            }
            return destination;
        }

        public IOwnedBuffer<byte> ToOwnedBuffer(int alreadyWritten)
        {
            Debug.Assert(alreadyWritten >= 0);
            var arrayPool = _arrayPool;
            _arrayPool = null;
            return new ArrayWrittenBuffer(arrayPool, _borrowedBuffer, alreadyWritten);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Ensure(int alreadyWritten, int sizeHintt)
        {
            if ((uint)sizeHintt >= (uint)(_capacity - alreadyWritten)) { CheckAndResizeBuffer(alreadyWritten, sizeHintt); }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void CheckAndResizeBuffer(int alreadyWritten, int sizeHint)
        {
            Debug.Assert(_borrowedBuffer != null);

            //if (sizeHint < 0) ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.sizeHint);
            //if (sizeHint == 0)
            if (unchecked((uint)(sizeHint - 1)) > c_maximumBufferSize)
            {
                sizeHint = c_minimumBufferSize;
            }

            int availableSpace = _capacity - alreadyWritten;

            if (sizeHint > availableSpace)
            {
                int growBy = Math.Max(sizeHint, _capacity);

                int newSize = checked(_capacity + growBy);

                var oldBuffer = _borrowedBuffer;

                if (null == _arrayPool) { _arrayPool = MessagePackBinary.Shared; }
                _borrowedBuffer = _arrayPool.Rent(newSize);

                Debug.Assert(oldBuffer.Length >= alreadyWritten);
                Debug.Assert(_borrowedBuffer.Length >= alreadyWritten);

                //var previousBuffer = oldBuffer.AsSpan(0, alreadyWritten);
                //previousBuffer.CopyTo(_borrowedBuffer);
                //previousBuffer.Clear();
                MessagePackBinary.CopyMemory(oldBuffer, 0, _borrowedBuffer, 0, alreadyWritten);

                _capacity = _borrowedBuffer.Length;

                if (_useThreadLocal)
                {
                    _useThreadLocal = false;
                }
                else
                {
                    _arrayPool.Return(oldBuffer);
                }
            }
        }
    }

    #region ** ArrayWrittenBuffer **

    sealed class ArrayWrittenBuffer : IOwnedBuffer<byte>
    {
        private ArrayPool<byte> _arrayPool;
        private readonly byte[] _buffer;
        private readonly int _alreadyWritten;

        public ArrayWrittenBuffer(ArrayPool<byte> arrayPool, byte[] buffer, int alreadyWritten)
        {
            _arrayPool = arrayPool;
            _buffer = buffer;
            _alreadyWritten = alreadyWritten;
        }

        public int Count => _alreadyWritten;

        public ArraySegment<byte> Buffer => new ArraySegment<byte>(_buffer, 0, _alreadyWritten);

        public ReadOnlyMemory<byte> Memory => new ReadOnlyMemory<byte>(_buffer, 0, _alreadyWritten);

        public ReadOnlySpan<byte> Span => new ReadOnlySpan<byte>(_buffer, 0, _alreadyWritten);

        public void Dispose()
        {
            var arrayPool = _arrayPool;
            if (arrayPool != null)
            {
                arrayPool.Return(_buffer);
                _arrayPool = null;
            }
        }
    }

    #endregion

    #region ** InternalMemoryPool **

    static class InternalMemoryPool
    {
        private static readonly int s_initialCapacity;
        internal static readonly uint InitialCapacity;

        static InternalMemoryPool()
        {
            s_initialCapacity = 64 * 1024;
            InitialCapacity = (uint)s_initialCapacity;
        }

        [ThreadStatic]
        static byte[] s_buffer = null;

        public static byte[] GetBuffer()
        {
            if (s_buffer == null) { s_buffer = new byte[s_initialCapacity]; }
            return s_buffer;
        }
    }

    #endregion
}
