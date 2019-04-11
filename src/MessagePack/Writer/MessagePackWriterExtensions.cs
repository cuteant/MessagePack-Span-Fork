namespace MessagePack
{
    using System.Runtime.CompilerServices;

    public static partial class MessagePackWriterExtensions
    {
        const uint IntMaxValue = int.MaxValue;
        const uint MaxFixPositiveInt = MessagePackRange.MaxFixPositiveInt;
        const uint ByteMaxValue = byte.MaxValue;
        const uint UShortMaxValue = ushort.MaxValue;
        const uint MinFixNegativeInt = unchecked((uint)MessagePackRange.MinFixNegativeInt);
        const uint SByteMinValue = unchecked((uint)sbyte.MinValue);
        const uint ShortMinValue = unchecked((uint)short.MinValue);
        const uint IntMinValue = unchecked((uint)int.MinValue);
    }
}
