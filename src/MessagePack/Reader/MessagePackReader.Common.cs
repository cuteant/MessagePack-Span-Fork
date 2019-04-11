namespace MessagePack
{
    using System.Runtime.CompilerServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNil()
        {
            if (_currentSpan[_currentSpanIndex] == MessagePackCode.Nil)
            {
                AdvanceWithinSpan(1);
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Nil ReadNil()
        {
            var code = _currentSpan[_currentSpanIndex];
            if (code == MessagePackCode.Nil)
            {
                AdvanceWithinSpan(1);
                return Nil.Default;
            }
            else
            {
                ThrowHelper.ThrowInvalidOperationException_Code(code);
                return default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MessagePackType GetMessagePackType()
        {
            return MessagePackCode.ToMessagePackType(_currentSpan[_currentSpanIndex]);
        }
    }
}
