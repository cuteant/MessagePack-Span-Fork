using System;

namespace MessagePack.Formatters
{
    public sealed class ForceInt16BlockFormatter : IMessagePackFormatter<Int16>
    {
        public static readonly ForceInt16BlockFormatter Instance = new ForceInt16BlockFormatter();

        ForceInt16BlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int16 value, IFormatterResolver formatterResolver)
        {
            writer.WriteInt16ForceInt16Block(value, ref idx);
        }

        public Int16 Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadInt16();
        }
    }

    public sealed class NullableForceInt16BlockFormatter : IMessagePackFormatter<Int16?>
    {
        public static readonly NullableForceInt16BlockFormatter Instance = new NullableForceInt16BlockFormatter();

        NullableForceInt16BlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int16? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteInt16ForceInt16Block(value.Value, ref idx);
        }

        public Int16? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadInt16();
        }
    }

    public sealed class ForceInt16BlockArrayFormatter : IMessagePackFormatter<Int16[]>
    {
        public static readonly ForceInt16BlockArrayFormatter Instance = new ForceInt16BlockArrayFormatter();

        ForceInt16BlockArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int16[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteInt16ForceInt16Block(value[i], ref idx);
            }
        }

        public Int16[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new Int16[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.ReadInt16();
            }
            return array;
        }
    }

    public sealed class ForceInt32BlockFormatter : IMessagePackFormatter<Int32>
    {
        public static readonly ForceInt32BlockFormatter Instance = new ForceInt32BlockFormatter();

        ForceInt32BlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int32 value, IFormatterResolver formatterResolver)
        {
            writer.WriteInt32ForceInt32Block(value, ref idx);
        }

        public Int32 Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadInt32();
        }
    }

    public sealed class NullableForceInt32BlockFormatter : IMessagePackFormatter<Int32?>
    {
        public static readonly NullableForceInt32BlockFormatter Instance = new NullableForceInt32BlockFormatter();

        NullableForceInt32BlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int32? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteInt32ForceInt32Block(value.Value, ref idx);
        }

        public Int32? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadInt32();
        }
    }

    public sealed class ForceInt32BlockArrayFormatter : IMessagePackFormatter<Int32[]>
    {
        public static readonly ForceInt32BlockArrayFormatter Instance = new ForceInt32BlockArrayFormatter();

        ForceInt32BlockArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int32[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteInt32ForceInt32Block(value[i], ref idx);
            }
        }

        public Int32[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new Int32[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.ReadInt32();
            }
            return array;
        }
    }

    public sealed class ForceInt64BlockFormatter : IMessagePackFormatter<Int64>
    {
        public static readonly ForceInt64BlockFormatter Instance = new ForceInt64BlockFormatter();

        ForceInt64BlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int64 value, IFormatterResolver formatterResolver)
        {
            writer.WriteInt64ForceInt64Block(value, ref idx);
        }

        public Int64 Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadInt64();
        }
    }

    public sealed class NullableForceInt64BlockFormatter : IMessagePackFormatter<Int64?>
    {
        public static readonly NullableForceInt64BlockFormatter Instance = new NullableForceInt64BlockFormatter();

        NullableForceInt64BlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int64? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteInt64ForceInt64Block(value.Value, ref idx);
        }

        public Int64? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadInt64();
        }
    }

    public sealed class ForceInt64BlockArrayFormatter : IMessagePackFormatter<Int64[]>
    {
        public static readonly ForceInt64BlockArrayFormatter Instance = new ForceInt64BlockArrayFormatter();

        ForceInt64BlockArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int64[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteInt64ForceInt64Block(value[i], ref idx);
            }
        }

        public Int64[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new Int64[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.ReadInt64();
            }
            return array;
        }
    }

    public sealed class ForceUInt16BlockFormatter : IMessagePackFormatter<UInt16>
    {
        public static readonly ForceUInt16BlockFormatter Instance = new ForceUInt16BlockFormatter();

        ForceUInt16BlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt16 value, IFormatterResolver formatterResolver)
        {
            writer.WriteUInt16ForceUInt16Block(value, ref idx);
        }

        public UInt16 Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadUInt16();
        }
    }

    public sealed class NullableForceUInt16BlockFormatter : IMessagePackFormatter<UInt16?>
    {
        public static readonly NullableForceUInt16BlockFormatter Instance = new NullableForceUInt16BlockFormatter();

        NullableForceUInt16BlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt16? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteUInt16ForceUInt16Block(value.Value, ref idx);
        }

        public UInt16? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadUInt16();
        }
    }

    public sealed class ForceUInt16BlockArrayFormatter : IMessagePackFormatter<UInt16[]>
    {
        public static readonly ForceUInt16BlockArrayFormatter Instance = new ForceUInt16BlockArrayFormatter();

        ForceUInt16BlockArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt16[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteUInt16ForceUInt16Block(value[i], ref idx);
            }
        }

        public UInt16[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new UInt16[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.ReadUInt16();
            }
            return array;
        }
    }

    public sealed class ForceUInt32BlockFormatter : IMessagePackFormatter<UInt32>
    {
        public static readonly ForceUInt32BlockFormatter Instance = new ForceUInt32BlockFormatter();

        ForceUInt32BlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt32 value, IFormatterResolver formatterResolver)
        {
            writer.WriteUInt32ForceUInt32Block(value, ref idx);
        }

        public UInt32 Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadUInt32();
        }
    }

    public sealed class NullableForceUInt32BlockFormatter : IMessagePackFormatter<UInt32?>
    {
        public static readonly NullableForceUInt32BlockFormatter Instance = new NullableForceUInt32BlockFormatter();

        NullableForceUInt32BlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt32? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteUInt32ForceUInt32Block(value.Value, ref idx);
        }

        public UInt32? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadUInt32();
        }
    }

    public sealed class ForceUInt32BlockArrayFormatter : IMessagePackFormatter<UInt32[]>
    {
        public static readonly ForceUInt32BlockArrayFormatter Instance = new ForceUInt32BlockArrayFormatter();

        ForceUInt32BlockArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt32[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteUInt32ForceUInt32Block(value[i], ref idx);
            }
        }

        public UInt32[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new UInt32[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.ReadUInt32();
            }
            return array;
        }
    }

    public sealed class ForceUInt64BlockFormatter : IMessagePackFormatter<UInt64>
    {
        public static readonly ForceUInt64BlockFormatter Instance = new ForceUInt64BlockFormatter();

        ForceUInt64BlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt64 value, IFormatterResolver formatterResolver)
        {
            writer.WriteUInt64ForceUInt64Block(value, ref idx);
        }

        public UInt64 Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadUInt64();
        }
    }

    public sealed class NullableForceUInt64BlockFormatter : IMessagePackFormatter<UInt64?>
    {
        public static readonly NullableForceUInt64BlockFormatter Instance = new NullableForceUInt64BlockFormatter();

        NullableForceUInt64BlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt64? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteUInt64ForceUInt64Block(value.Value, ref idx);
        }

        public UInt64? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadUInt64();
        }
    }

    public sealed class ForceUInt64BlockArrayFormatter : IMessagePackFormatter<UInt64[]>
    {
        public static readonly ForceUInt64BlockArrayFormatter Instance = new ForceUInt64BlockArrayFormatter();

        ForceUInt64BlockArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt64[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteUInt64ForceUInt64Block(value[i], ref idx);
            }
        }

        public UInt64[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new UInt64[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.ReadUInt64();
            }
            return array;
        }
    }

    public sealed class ForceByteBlockFormatter : IMessagePackFormatter<Byte>
    {
        public static readonly ForceByteBlockFormatter Instance = new ForceByteBlockFormatter();

        ForceByteBlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Byte value, IFormatterResolver formatterResolver)
        {
            writer.WriteByteForceByteBlock(value, ref idx);
        }

        public Byte Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadByte();
        }
    }

    public sealed class NullableForceByteBlockFormatter : IMessagePackFormatter<Byte?>
    {
        public static readonly NullableForceByteBlockFormatter Instance = new NullableForceByteBlockFormatter();

        NullableForceByteBlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Byte? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteByteForceByteBlock(value.Value, ref idx);
        }

        public Byte? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadByte();
        }
    }


    public sealed class ForceSByteBlockFormatter : IMessagePackFormatter<SByte>
    {
        public static readonly ForceSByteBlockFormatter Instance = new ForceSByteBlockFormatter();

        ForceSByteBlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, SByte value, IFormatterResolver formatterResolver)
        {
            writer.WriteSByteForceSByteBlock(value, ref idx);
        }

        public SByte Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadSByte();
        }
    }

    public sealed class NullableForceSByteBlockFormatter : IMessagePackFormatter<SByte?>
    {
        public static readonly NullableForceSByteBlockFormatter Instance = new NullableForceSByteBlockFormatter();

        NullableForceSByteBlockFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, SByte? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteSByteForceSByteBlock(value.Value, ref idx);
        }

        public SByte? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadSByte();
        }
    }

    public sealed class ForceSByteBlockArrayFormatter : IMessagePackFormatter<SByte[]>
    {
        public static readonly ForceSByteBlockArrayFormatter Instance = new ForceSByteBlockArrayFormatter();

        ForceSByteBlockArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, SByte[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteSByteForceSByteBlock(value[i], ref idx);
            }
        }

        public SByte[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new SByte[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.ReadSByte();
            }
            return array;
        }
    }

}