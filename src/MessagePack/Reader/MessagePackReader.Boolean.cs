namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBoolean()
        {
            //var value = booleanDecoders[_currentSpan[_currentSpanIndex]].Read();
            //Advance(1);
            //return value;
            var code = _currentSpan[_currentSpanIndex];
            switch (code)
            {
                case MessagePackCode.False:
                    Advance(1);
                    return false;
                case MessagePackCode.True:
                    Advance(1);
                    return true;

                default:
                    throw GetInvalidOperationException(code);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static InvalidOperationException GetInvalidOperationException(byte code)
        {
            return new InvalidOperationException($"code is invalid. code:{code} format:{MessagePackCode.ToFormatName(code)}");
        }
    }
}

namespace MessagePack.Decoders
{
    using System;

    internal interface IBooleanDecoder
    {
        bool Read();
    }

    internal sealed class True : IBooleanDecoder
    {
        internal static readonly IBooleanDecoder Instance = new True();

        True() { }

        public bool Read() => true;
    }

    internal sealed class False : IBooleanDecoder
    {
        internal static readonly IBooleanDecoder Instance = new False();

        False() { }

        public bool Read() => false;
    }

    internal sealed class InvalidBoolean : IBooleanDecoder
    {
        internal static readonly IBooleanDecoder Instance = new InvalidBoolean();

        InvalidBoolean() { }

        public bool Read() => throw new InvalidOperationException("code is invalid. code.");
    }
}