using System;

namespace MessagePack.Formatters
{
    /// <summary>Serialize by .NET native DateTime binary format.</summary>
    public sealed class NativeDateTimeFormatter : IMessagePackFormatter<DateTime>
    {
        public static readonly NativeDateTimeFormatter Instance = new NativeDateTimeFormatter();

        public void Serialize(ref MessagePackWriter writer, ref int idx, DateTime value, IFormatterResolver formatterResolver)
        {
            var dateData = value.ToBinary();
            writer.WriteInt64(dateData, ref idx);
        }

        public DateTime Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.GetMessagePackType() == MessagePackType.Extension)
            {
                return DateTimeFormatter.Instance.Deserialize(ref reader, formatterResolver);
            }

            var dateData = reader.ReadInt64();
            return DateTime.FromBinary(dateData);
        }
    }

    public sealed class NativeDateTimeArrayFormatter : IMessagePackFormatter<DateTime[]>
    {
        public static readonly NativeDateTimeArrayFormatter Instance = new NativeDateTimeArrayFormatter();

        public void Serialize(ref MessagePackWriter writer, ref int idx, DateTime[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteInt64(value[i].ToBinary(), ref idx);
            }
        }

        public DateTime[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new DateTime[len];
            for (int i = 0; i < array.Length; i++)
            {
                var dateData = reader.ReadInt64();
                array[i] = DateTime.FromBinary(dateData);
            }
            return array;
        }
    }

    //// Old-Spec
    //// bin 8, bin 16, bin 32, str 8, str 16, str 32 -> fixraw or raw 16 or raw 32
    //// fixraw -> fixstr, raw16 -> str16, raw32 -> str32
    //// https://github.com/msgpack/msgpack/blob/master/spec-old.md

    ///// <summary>
    ///// Old-MessagePack spec's string formatter.
    ///// </summary>
    //public sealed class OldSpecStringFormatter : IMessagePackFormatter<string>
    //{
    //    public static readonly OldSpecStringFormatter Instance = new OldSpecStringFormatter();

    //    // Old spec does not exists str 8 format.
    //    public void Serialize(ref MessagePackWriter writer, ref int idx, string value, IFormatterResolver formatterResolver)
    //    {
    //        if (value == null) { writer.WriteNil(ref idx); return; }

    //        MessagePackBinary.EnsureCapacity(ref bytes, offset, StringEncoding.UTF8.GetMaxByteCount(value.Length) + 5);

    //        int useOffset;
    //        if (value.Length <= MessagePackRange.MaxFixStringLength)
    //        {
    //            useOffset = 1;
    //        }
    //        else if (value.Length <= ushort.MaxValue)
    //        {
    //            useOffset = 3;
    //        }
    //        else
    //        {
    //            useOffset = 5;
    //        }

    //        // skip length area
    //        var writeBeginOffset = offset + useOffset;
    //        var byteCount = StringEncoding.UTF8.GetBytes(value, 0, value.Length, bytes, writeBeginOffset);

    //        // move body and write prefix
    //        if (byteCount <= MessagePackRange.MaxFixStringLength)
    //        {
    //            if (useOffset != 1)
    //            {
    //                Buffer.BlockCopy(bytes, writeBeginOffset, bytes, offset + 1, byteCount);
    //            }
    //            bytes[offset] = (byte)(MessagePackCode.MinFixStr | byteCount);
    //            return byteCount + 1;
    //        }
    //        else if (byteCount <= ushort.MaxValue)
    //        {
    //            if (useOffset != 3)
    //            {
    //                Buffer.BlockCopy(bytes, writeBeginOffset, bytes, offset + 3, byteCount);
    //            }

    //            bytes[offset] = MessagePackCode.Str16;
    //            bytes[offset + 1] = unchecked((byte)(byteCount >> 8));
    //            bytes[offset + 2] = unchecked((byte)byteCount);
    //            return byteCount + 3;
    //        }
    //        else
    //        {
    //            if (useOffset != 5)
    //            {
    //                Buffer.BlockCopy(bytes, writeBeginOffset, bytes, offset + 5, byteCount);
    //            }

    //            bytes[offset] = MessagePackCode.Str32;
    //            bytes[offset + 1] = unchecked((byte)(byteCount >> 24));
    //            bytes[offset + 2] = unchecked((byte)(byteCount >> 16));
    //            bytes[offset + 3] = unchecked((byte)(byteCount >> 8));
    //            bytes[offset + 4] = unchecked((byte)byteCount);
    //            return byteCount + 5;
    //        }
    //    }

    //    public string Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
    //    {
    //        return MessagePackBinary.ReadString();
    //    }
    //}

    ///// <summary>
    ///// Old-MessagePack spec's binary formatter.
    ///// </summary>
    //public sealed class OldSpecBinaryFormatter : IMessagePackFormatter<byte[]>
    //{
    //    public static readonly OldSpecBinaryFormatter Instance = new OldSpecBinaryFormatter();

    //    public void Serialize(ref MessagePackWriter writer, ref int idx, byte[] value, IFormatterResolver formatterResolver)
    //    {
    //        if (value == null) writer.WriteNil(ref idx); return;

    //        var byteCount = value.Length;

    //        if (byteCount <= MessagePackRange.MaxFixStringLength)
    //        {
    //            MessagePackBinary.EnsureCapacity(ref bytes, offset, byteCount + 1);

    //            bytes[offset] = (byte)(MessagePackCode.MinFixStr | byteCount);
    //            Buffer.BlockCopy(value, 0, bytes, offset + 1, byteCount);
    //            return byteCount + 1;
    //        }
    //        else if (byteCount <= ushort.MaxValue)
    //        {
    //            MessagePackBinary.EnsureCapacity(ref bytes, offset, byteCount + 3);

    //            bytes[offset] = MessagePackCode.Str16;
    //            bytes[offset + 1] = unchecked((byte)(byteCount >> 8));
    //            bytes[offset + 2] = unchecked((byte)byteCount);
    //            Buffer.BlockCopy(value, 0, bytes, offset + 3, byteCount);
    //            return byteCount + 3;
    //        }
    //        else
    //        {
    //            MessagePackBinary.EnsureCapacity(ref bytes, offset, byteCount + 5);

    //            bytes[offset] = MessagePackCode.Str32;
    //            bytes[offset + 1] = unchecked((byte)(byteCount >> 24));
    //            bytes[offset + 2] = unchecked((byte)(byteCount >> 16));
    //            bytes[offset + 3] = unchecked((byte)(byteCount >> 8));
    //            bytes[offset + 4] = unchecked((byte)byteCount);
    //            Buffer.BlockCopy(value, 0, bytes, offset + 5, byteCount);
    //            return byteCount + 5;
    //        }
    //    }

    //    public byte[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
    //    {
    //        var type = MessagePackBinary.GetMessagePackType(bytes, offset);
    //        switch (type)
    //        {
    //            case MessagePackType.Nil:
    //                readSize = 1;
    //                return null;
    //            case MessagePackType.Binary:
    //                return MessagePackBinary.ReadBytes();
    //            case MessagePackType.String:
    //                var code = bytes[offset];
    //                unchecked
    //                {
    //                    if (MessagePackCode.MinFixStr <= code && code <= MessagePackCode.MaxFixStr)
    //                    {
    //                        var length = bytes[offset] & 0x1F;
    //                        readSize = length + 1;
    //                        var result = new byte[length];
    //                        Buffer.BlockCopy(bytes, offset + 1, result, 0, result.Length);
    //                        return result;
    //                    }
    //                    else
    //                    {
    //                        switch (code)
    //                        {
    //                            case MessagePackCode.Str8:
    //                                var length0 = (int)bytes[offset + 1];
    //                                readSize = length0 + 2;
    //                                var result0 = new byte[length0];
    //                                Buffer.BlockCopy(bytes, offset + 2, result0, 0, result0.Length);
    //                                return result0;
    //                            case MessagePackCode.Str16:
    //                                var length1 = (bytes[offset + 1] << 8) + (bytes[offset + 2]);
    //                                readSize = length1 + 3;
    //                                var result1 = new byte[length1];
    //                                Buffer.BlockCopy(bytes, offset + 3, result1, 0, result1.Length);
    //                                return result1;
    //                            case MessagePackCode.Str32:
    //                                var length2 = (int)((uint)(bytes[offset + 1] << 24) | (uint)(bytes[offset + 2] << 16) | (uint)(bytes[offset + 3] << 8) | (uint)bytes[offset + 4]);
    //                                readSize = length2 + 5;
    //                                var result2 = new byte[length2];
    //                                Buffer.BlockCopy(bytes, offset + 5, result2, 0, result2.Length);
    //                                return result2;
    //                            default:
    //                                break;
    //                        }
    //                    }
    //                }
    //                break;
    //            default:
    //                break;
    //        }
    //        ThrowHelper.ThrowInvalidOperationException_Code(bytes[offset]);
    //        readSize = default; return null;
    //    }
    //}
}
