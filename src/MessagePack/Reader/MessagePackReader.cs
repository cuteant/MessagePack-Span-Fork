namespace MessagePack
{
    using System;
    using System.Buffers;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public ref partial struct MessagePackReader
    {
        const uint TooBigOrNegative = int.MaxValue;

        internal readonly bool _isSingleSegment;

        internal ReadOnlySpan<byte> _currentSpan;
        internal int _currentSpanIndex;

        private readonly SequenceWrapper _sequenceWrapper;

        /// <summary>Create a <see cref="MessagePackReader" /> over the given <see cref="ReadOnlySpan{Byte}"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MessagePackReader(ReadOnlySpan<byte> buffer)
        {
            _currentSpanIndex = 0;
            _currentSpan = buffer;
            _isSingleSegment = true;
            _sequenceWrapper = SequenceWrapper.Default;
        }

        /// <summary>Create a <see cref="MessagePackReader" /> over the given <see cref="ReadOnlySequence{Byte}"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MessagePackReader(ReadOnlySequence<byte> buffer)
        {
            _currentSpanIndex = 0;
            _currentSpan = default;
            _isSingleSegment = buffer.IsSingleSegment;

            _sequenceWrapper = new SequenceWrapper(buffer);

            _sequenceWrapper.Init(ref this);
        }

        /// <summary>IsSingleSegment</summary>
        public bool IsSingleSegment => _isSingleSegment;

        /// <summary>The current segment in the reader.</summary>
        public ReadOnlySpan<byte> CurrentSpan => _currentSpan;

        /// <summary>The index in the <see cref="CurrentSpan"/>.</summary>
        public int CurrentSpanIndex => _currentSpanIndex;

        /// <summary>The unread portion of the <see cref="CurrentSpan"/>.</summary>
        public ReadOnlySpan<byte> UnreadSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _currentSpan.Slice(_currentSpanIndex);
        }

        /// <summary>Only call this helper if you know that you are advancing in the current span
        /// with valid count and there is no need to fetch the next one.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AdvanceWithinSpan(int count)
        {
            Debug.Assert(count >= 0);

            _currentSpanIndex += count;
        }

        /// <summary>Unchecked helper to avoid unnecessary checks where you know count is valid.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AdvanceCurrentSpan(int count)
        {
            Debug.Assert(count >= 0);

            _currentSpanIndex += count;
            if ((uint)_currentSpanIndex >= (uint)_currentSpan.Length) { _sequenceWrapper.GetNextSpan(ref this); }
        }

        /// <summary>Move the reader ahead the specified number of items.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            Debug.Assert(count >= 0);
            //if (count < 0) { ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count); }

            if (_isSingleSegment)
            {
                _currentSpanIndex += count;
                //if ((uint)_currentSpanIndex >= (uint)_currentSpan.Length) { _noMoreData = true; }
                return;
            }

            AdvanceMultisegment(count);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AdvanceMultisegment(int count)
        {
            if ((uint)(_currentSpan.Length - _currentSpanIndex) > (uint)count)
            {
                _currentSpanIndex += count;
            }
            else
            {
                // Can't satisfy from the current span
                _sequenceWrapper.AdvanceToNextSpan(ref this, count);
            }
        }

        /// <summary>Read the next value and advance the reader.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Read()
        {
            //if (_noMoreData) { ThrowHelper.ThrowArgumentException_NeedMoreData(); }

            var value = _currentSpan[_currentSpanIndex];
            _currentSpanIndex++;

            if ((uint)_currentSpanIndex >= (uint)_currentSpan.Length) { _sequenceWrapper.GetNextSpan(ref this); }

            return value;
        }

        /// <summary>Peeks at the next value without advancing the reader.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Peek()
        {
            //if (_noMoreData) { ThrowHelper.ThrowArgumentException_NeedMoreData(); }

            return _currentSpan[_currentSpanIndex];
        }

        /// <summary>Peek forward up to the number of positions specified by <paramref name="minimumSize"/>.</summary>
        /// <returns>Span over the peeked data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> Peek(int minimumSize)
        {
            if ((uint)(_currentSpan.Length - _currentSpanIndex) >= (uint)minimumSize)
            {
                return _currentSpan.Slice(_currentSpanIndex);
            }

            // Not enough contiguous Ts, allocate and copy what we can get
            return _sequenceWrapper.PeekMultisegment(ref this, minimumSize);
        }
    }
}
