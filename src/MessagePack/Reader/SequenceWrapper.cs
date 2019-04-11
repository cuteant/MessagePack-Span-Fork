namespace MessagePack
{
    using System;
    using System.Buffers;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    sealed class SequenceWrapper
    {
        internal static readonly SequenceWrapper Default = new SequenceWrapper();

        private readonly ReadOnlySequence<byte> _sequence;
        private SequencePosition _currentPosition;
        private SequencePosition _nextPosition;

        private bool _noMoreData;

        private SequenceWrapper()
        {
            _sequence = default;
            _currentPosition = default;
            _nextPosition = default;
        }

        /// <summary>Create a <see cref="MessagePackReader" /> over the given <see cref="ReadOnlySequence{Byte}"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SequenceWrapper(ReadOnlySequence<byte> buffer)
        {
            _sequence = buffer;
            _currentPosition = buffer.Start;
        }

        /// <summary>Return true if we're in the last segment.</summary>
        public bool IsLastSegment => _nextPosition.GetObject() == null;

        /// <summary>True when there is no more data in the reader's buffer.</summary>
        public bool End => _noMoreData;

        internal void Init(ref MessagePackReader reader)
        {
            reader._currentSpanIndex = 0;
            MessagePackReaderHelper.GetFirstSpan(_sequence, out ReadOnlySpan<byte> first, out _nextPosition);
            reader._currentSpan = first;
            _noMoreData = first.IsEmpty;

            if (!reader._isSingleSegment && _noMoreData)
            {
                _noMoreData = false;
                GetNextSpan(ref reader);
            }
        }

        internal bool TryGetRawMultisegment(ref MessagePackReader reader, int index, out byte value)
        {
            index = index - (reader._currentSpan.Length - reader._currentSpanIndex);

            SequencePosition next = _nextPosition;
            while (_sequence.TryGet(ref next, out ReadOnlyMemory<byte> nextSegment, true))
            {
                uint nLen = (uint)nextSegment.Length;
                if (nLen > 0u)
                {
                    if (nLen > (uint)index)
                    {
                        value = nextSegment.Span[index];
                        return true;
                    }
                    index -= (int)nLen;
                }
            }

            value = default; return false;
        }

        /// <summary>Get the next segment with available data, if any.</summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void GetNextSpan(ref MessagePackReader reader)
        {
            if (reader._isSingleSegment) { goto NoMoreData; }

            SequencePosition previousNextPosition = _nextPosition;
            while (_sequence.TryGet(ref _nextPosition, out ReadOnlyMemory<byte> memory, advance: true))
            {
                _currentPosition = previousNextPosition;
                if (memory.Length > 0)
                {
                    reader._currentSpan = memory.Span;
                    reader._currentSpanIndex = 0;
                    return;
                }
                else
                {
                    reader._currentSpan = default;
                    reader._currentSpanIndex = 0;
                    previousNextPosition = _nextPosition;
                }
            }

        NoMoreData:
            _noMoreData = true;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void AdvanceToNextSpan(ref MessagePackReader reader, int count)
        {
            if (reader._isSingleSegment)
            {
                if ((reader._currentSpan.Length - reader._currentSpanIndex) <= count)
                {
                    reader.AdvanceCurrentSpan(count);
                    return;
                }
                else
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count);
                }
            }

            while (!_noMoreData)
            {
                int remaining = reader._currentSpan.Length - reader._currentSpanIndex;

                if ((uint)remaining > (uint)count)
                {
                    reader._currentSpanIndex += count;
                    count = 0;
                    break;
                }

                // As there may not be any further segments we need to
                // push the current index to the end of the span.
                reader._currentSpanIndex += remaining;
                count -= remaining;
                Debug.Assert(count >= 0);

                GetNextSpan(ref reader);

                if (0u >= (uint)count) { break; }
            }

            if (count != 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal ReadOnlySpan<byte> PeekMultisegment(ref MessagePackReader reader, int minimumSize)
        {
            Span<byte> destination = new byte[minimumSize];

            ReadOnlySpan<byte> firstSpan = reader.UnreadSpan;
            firstSpan.CopyTo(destination);
            int copied = firstSpan.Length;

            SequencePosition next = _nextPosition;
            while (_sequence.TryGet(ref next, out ReadOnlyMemory<byte> nextSegment, true))
            {
                ReadOnlySpan<byte> nextSpan = nextSegment.Span;
                if (nextSpan.Length > 0)
                {
                    int toCopy = Math.Min(nextSpan.Length, destination.Length - copied);
                    nextSpan.Slice(0, toCopy).CopyTo(destination.Slice(copied));
                    copied += toCopy;
                    if ((uint)copied >= (uint)destination.Length) { break; }
                }
            }

            return destination.Slice(0, copied);
        }
    }
}
