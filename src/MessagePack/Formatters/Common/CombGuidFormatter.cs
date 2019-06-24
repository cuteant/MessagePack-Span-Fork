#if DEPENDENT_ON_CUTEANT

namespace MessagePack.Formatters
{
    using System;
    using System.Runtime.CompilerServices;
    using CuteAnt;
#if !NETCOREAPP
    using MessagePack.Internal;
#endif

    public sealed class CombGuidFormatter : IMessagePackFormatter<CombGuid>
    {
        public static readonly IMessagePackFormatter<CombGuid> Instance = new CombGuidFormatter();

        CombGuidFormatter() { }

        const int c_totalSize = 18;

        public void Serialize(ref MessagePackWriter writer, ref int idx, CombGuid value, IFormatterResolver formatterResolver)
        {
            writer.Ensure(idx, c_totalSize);

            ref byte pinnableAddr = ref writer.PinnableAddress;
            IntPtr offset = (IntPtr)idx;
            Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.FixExt16;
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)ReservedMessagePackExtensionTypeCode.ComgGuid);
            idx += 2;

#if NETCOREAPP
            value.TryWriteBytes(writer.Buffer.Slice(idx));
            idx += 16;
#else
            var buffer = value.GetByteArray(CombGuidSequentialSegmentType.Guid);
            if (UnsafeMemory.Is64BitProcess)
            {
                UnsafeMemory64.WriteRaw16(ref pinnableAddr, ref buffer[0], ref idx);
            }
            else
            {
                UnsafeMemory32.WriteRaw16(ref pinnableAddr, ref buffer[0], ref idx);
            }
#endif
        }

        public CombGuid Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            var result = reader.ReadExtensionFormat();
            var typeCode = result.TypeCode;
            if (typeCode != ReservedMessagePackExtensionTypeCode.ComgGuid)
            {
                ThrowHelper.ThrowInvalidOperationException_TypeCode(typeCode);
            }
#if NETCOREAPP
            return new CombGuid(result.Data, CombGuidSequentialSegmentType.Guid);
#else
            return new CombGuid(result.Data.ToArray(), CombGuidSequentialSegmentType.Guid, true);
#endif
        }
    }
}

#endif