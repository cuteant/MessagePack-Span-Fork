namespace MessagePack
{
    using System;
    using System.Buffers;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal static class MessagePackReaderHelper
    {
        private const int FlagBitMask = 1 << 31;
        private const int IndexBitMask = ~FlagBitMask;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void GetFirstSpan(in ReadOnlySequence<byte> buffer, out ReadOnlySpan<byte> first, out SequencePosition next)
        {
            first = default;
            next = default;
            SequencePosition start = buffer.Start;
            int startIndex = start.GetInteger();
            object startObject = start.GetObject();

            if (startObject != null)
            {
                SequencePosition end = buffer.End;
                int endIndex = end.GetInteger();
                bool isMultiSegment = startObject != end.GetObject();

                // A == 0 && B == 0 means SequenceType.MultiSegment
                if (startIndex >= 0)
                {
                    if (endIndex >= 0)  // SequenceType.MultiSegment
                    {
                        ReadOnlySequenceSegment<byte> segment = (ReadOnlySequenceSegment<byte>)startObject;
                        next = new SequencePosition(segment.Next, 0);
                        first = segment.Memory.Span;
                        if (isMultiSegment)
                        {
                            first = first.Slice(startIndex);
                        }
                        else
                        {
                            first = first.Slice(startIndex, endIndex - startIndex);
                        }
                    }
                    else
                    {
                        if (isMultiSegment) { ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached(); }

                        first = new ReadOnlySpan<byte>((byte[])startObject, startIndex, (endIndex & IndexBitMask) - startIndex);
                    }
                }
                else
                {
                    first = GetFirstSpanSlow(startObject, startIndex, endIndex, isMultiSegment);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ReadOnlySpan<byte> GetFirstSpanSlow(object startObject, int startIndex, int endIndex, bool isMultiSegment)
        {
            Debug.Assert(startIndex < 0 || endIndex < 0);
            if (isMultiSegment) { ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached(); }

            // The type == char check here is redundant. However, we still have it to allow
            // the JIT to see when that the code is unreachable and eliminate it.
            // A == 1 && B == 1 means SequenceType.String
            //if (typeof(T) == typeof(char) && endIndex < 0)
            //{
            //    var memory = (ReadOnlyMemory<T>)(object)((string)startObject).AsMemory();

            //    // No need to remove the FlagBitMask since (endIndex - startIndex) == (endIndex & ReadOnlySequence.IndexBitMask) - (startIndex & ReadOnlySequence.IndexBitMask)
            //    return memory.Span.Slice(startIndex & IndexBitMask, endIndex - startIndex);
            //}
            //else // endIndex >= 0, A == 1 && B == 0 means SequenceType.MemoryManager
            {
                startIndex &= IndexBitMask;
                return ((MemoryManager<byte>)startObject).Memory.Span.Slice(startIndex, endIndex - startIndex);
            }
        }
    }
}
