﻿using System;

namespace MessagePack.Formatters
{
    public sealed class Int16Formatter : IMessagePackFormatter<Int16>
    {
        public static readonly Int16Formatter Instance = new Int16Formatter();

        Int16Formatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int16 value, IFormatterResolver formatterResolver)
        {
            writer.WriteInt16(value, ref idx);
        }

        public Int16 Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadInt16();
        }
    }

    public sealed class NullableInt16Formatter : IMessagePackFormatter<Int16?>
    {
        public static readonly NullableInt16Formatter Instance = new NullableInt16Formatter();

        NullableInt16Formatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int16? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteInt16(value.Value, ref idx);
        }

        public Int16? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadInt16();
        }
    }

    public sealed class Int16ArrayFormatter : IMessagePackFormatter<Int16[]>
    {
        public static readonly Int16ArrayFormatter Instance = new Int16ArrayFormatter();

        Int16ArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int16[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteInt16(value[i], ref idx);
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

    public sealed class Int32Formatter : IMessagePackFormatter<Int32>
    {
        public static readonly Int32Formatter Instance = new Int32Formatter();

        Int32Formatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int32 value, IFormatterResolver formatterResolver)
        {
            writer.WriteInt32(value, ref idx);
        }

        public Int32 Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadInt32();
        }
    }

    public sealed class NullableInt32Formatter : IMessagePackFormatter<Int32?>
    {
        public static readonly NullableInt32Formatter Instance = new NullableInt32Formatter();

        NullableInt32Formatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int32? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteInt32(value.Value, ref idx);
        }

        public Int32? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadInt32();
        }
    }

    public sealed class Int32ArrayFormatter : IMessagePackFormatter<Int32[]>
    {
        public static readonly Int32ArrayFormatter Instance = new Int32ArrayFormatter();

        Int32ArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int32[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteInt32(value[i], ref idx);
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

    public sealed class Int64Formatter : IMessagePackFormatter<Int64>
    {
        public static readonly Int64Formatter Instance = new Int64Formatter();

        Int64Formatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int64 value, IFormatterResolver formatterResolver)
        {
            writer.WriteInt64(value, ref idx);
        }

        public Int64 Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadInt64();
        }
    }

    public sealed class NullableInt64Formatter : IMessagePackFormatter<Int64?>
    {
        public static readonly NullableInt64Formatter Instance = new NullableInt64Formatter();

        NullableInt64Formatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int64? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteInt64(value.Value, ref idx);
        }

        public Int64? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadInt64();
        }
    }

    public sealed class Int64ArrayFormatter : IMessagePackFormatter<Int64[]>
    {
        public static readonly Int64ArrayFormatter Instance = new Int64ArrayFormatter();

        Int64ArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Int64[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteInt64(value[i], ref idx);
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

    public sealed class UInt16Formatter : IMessagePackFormatter<UInt16>
    {
        public static readonly UInt16Formatter Instance = new UInt16Formatter();

        UInt16Formatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt16 value, IFormatterResolver formatterResolver)
        {
            writer.WriteUInt16(value, ref idx);
        }

        public UInt16 Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadUInt16();
        }
    }

    public sealed class NullableUInt16Formatter : IMessagePackFormatter<UInt16?>
    {
        public static readonly NullableUInt16Formatter Instance = new NullableUInt16Formatter();

        NullableUInt16Formatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt16? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteUInt16(value.Value, ref idx);
        }

        public UInt16? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadUInt16();
        }
    }

    public sealed class UInt16ArrayFormatter : IMessagePackFormatter<UInt16[]>
    {
        public static readonly UInt16ArrayFormatter Instance = new UInt16ArrayFormatter();

        UInt16ArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt16[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteUInt16(value[i], ref idx);
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

    public sealed class UInt32Formatter : IMessagePackFormatter<UInt32>
    {
        public static readonly UInt32Formatter Instance = new UInt32Formatter();

        UInt32Formatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt32 value, IFormatterResolver formatterResolver)
        {
            writer.WriteUInt32(value, ref idx);
        }

        public UInt32 Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadUInt32();
        }
    }

    public sealed class NullableUInt32Formatter : IMessagePackFormatter<UInt32?>
    {
        public static readonly NullableUInt32Formatter Instance = new NullableUInt32Formatter();

        NullableUInt32Formatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt32? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteUInt32(value.Value, ref idx);
        }

        public UInt32? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadUInt32();
        }
    }

    public sealed class UInt32ArrayFormatter : IMessagePackFormatter<UInt32[]>
    {
        public static readonly UInt32ArrayFormatter Instance = new UInt32ArrayFormatter();

        UInt32ArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt32[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteUInt32(value[i], ref idx);
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

    public sealed class UInt64Formatter : IMessagePackFormatter<UInt64>
    {
        public static readonly UInt64Formatter Instance = new UInt64Formatter();

        UInt64Formatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt64 value, IFormatterResolver formatterResolver)
        {
            writer.WriteUInt64(value, ref idx);
        }

        public UInt64 Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadUInt64();
        }
    }

    public sealed class NullableUInt64Formatter : IMessagePackFormatter<UInt64?>
    {
        public static readonly NullableUInt64Formatter Instance = new NullableUInt64Formatter();

        NullableUInt64Formatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt64? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteUInt64(value.Value, ref idx);
        }

        public UInt64? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadUInt64();
        }
    }

    public sealed class UInt64ArrayFormatter : IMessagePackFormatter<UInt64[]>
    {
        public static readonly UInt64ArrayFormatter Instance = new UInt64ArrayFormatter();

        UInt64ArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, UInt64[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteUInt64(value[i], ref idx);
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

    public sealed class SingleFormatter : IMessagePackFormatter<Single>
    {
        public static readonly SingleFormatter Instance = new SingleFormatter();

        SingleFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Single value, IFormatterResolver formatterResolver)
        {
            writer.WriteSingle(value, ref idx);
        }

        public Single Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadSingle();
        }
    }

    public sealed class NullableSingleFormatter : IMessagePackFormatter<Single?>
    {
        public static readonly NullableSingleFormatter Instance = new NullableSingleFormatter();

        NullableSingleFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Single? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteSingle(value.Value, ref idx);
        }

        public Single? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadSingle();
        }
    }

    public sealed class SingleArrayFormatter : IMessagePackFormatter<Single[]>
    {
        public static readonly SingleArrayFormatter Instance = new SingleArrayFormatter();

        SingleArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Single[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteSingle(value[i], ref idx);
            }
        }

        public Single[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new Single[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.ReadSingle();
            }
            return array;
        }
    }

    public sealed class DoubleFormatter : IMessagePackFormatter<Double>
    {
        public static readonly DoubleFormatter Instance = new DoubleFormatter();

        DoubleFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Double value, IFormatterResolver formatterResolver)
        {
            writer.WriteDouble(value, ref idx);
        }

        public Double Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadDouble();
        }
    }

    public sealed class NullableDoubleFormatter : IMessagePackFormatter<Double?>
    {
        public static readonly NullableDoubleFormatter Instance = new NullableDoubleFormatter();

        NullableDoubleFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Double? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteDouble(value.Value, ref idx);
        }

        public Double? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadDouble();
        }
    }

    public sealed class DoubleArrayFormatter : IMessagePackFormatter<Double[]>
    {
        public static readonly DoubleArrayFormatter Instance = new DoubleArrayFormatter();

        DoubleArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Double[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteDouble(value[i], ref idx);
            }
        }

        public Double[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new Double[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.ReadDouble();
            }
            return array;
        }
    }

    public sealed class BooleanFormatter : IMessagePackFormatter<Boolean>
    {
        public static readonly BooleanFormatter Instance = new BooleanFormatter();

        BooleanFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Boolean value, IFormatterResolver formatterResolver)
        {
            writer.WriteBoolean(value, ref idx);
        }

        public Boolean Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadBoolean();
        }
    }

    public sealed class NullableBooleanFormatter : IMessagePackFormatter<Boolean?>
    {
        public static readonly NullableBooleanFormatter Instance = new NullableBooleanFormatter();

        NullableBooleanFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Boolean? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteBoolean(value.Value, ref idx);
        }

        public Boolean? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadBoolean();
        }
    }

    public sealed class BooleanArrayFormatter : IMessagePackFormatter<Boolean[]>
    {
        public static readonly BooleanArrayFormatter Instance = new BooleanArrayFormatter();

        BooleanArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Boolean[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteBoolean(value[i], ref idx);
            }
        }

        public Boolean[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new Boolean[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.ReadBoolean();
            }
            return array;
        }
    }

    public sealed class ByteFormatter : IMessagePackFormatter<Byte>
    {
        public static readonly ByteFormatter Instance = new ByteFormatter();

        ByteFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Byte value, IFormatterResolver formatterResolver)
        {
            writer.WriteByte(value, ref idx);
        }

        public Byte Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadByte();
        }
    }

    public sealed class NullableByteFormatter : IMessagePackFormatter<Byte?>
    {
        public static readonly NullableByteFormatter Instance = new NullableByteFormatter();

        NullableByteFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Byte? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteByte(value.Value, ref idx);
        }

        public Byte? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadByte();
        }
    }


    public sealed class SByteFormatter : IMessagePackFormatter<SByte>
    {
        public static readonly SByteFormatter Instance = new SByteFormatter();

        SByteFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, SByte value, IFormatterResolver formatterResolver)
        {
            writer.WriteSByte(value, ref idx);
        }

        public SByte Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadSByte();
        }
    }

    public sealed class NullableSByteFormatter : IMessagePackFormatter<SByte?>
    {
        public static readonly NullableSByteFormatter Instance = new NullableSByteFormatter();

        NullableSByteFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, SByte? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteSByte(value.Value, ref idx);
        }

        public SByte? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadSByte();
        }
    }

    public sealed class SByteArrayFormatter : IMessagePackFormatter<SByte[]>
    {
        public static readonly SByteArrayFormatter Instance = new SByteArrayFormatter();

        SByteArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, SByte[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteSByte(value[i], ref idx);
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

    public sealed class CharFormatter : IMessagePackFormatter<Char>
    {
        public static readonly CharFormatter Instance = new CharFormatter();

        CharFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Char value, IFormatterResolver formatterResolver)
        {
            writer.WriteChar(value, ref idx);
        }

        public Char Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadChar();
        }
    }

    public sealed class NullableCharFormatter : IMessagePackFormatter<Char?>
    {
        public static readonly NullableCharFormatter Instance = new NullableCharFormatter();

        NullableCharFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Char? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteChar(value.Value, ref idx);
        }

        public Char? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadChar();
        }
    }

    public sealed class CharArrayFormatter : IMessagePackFormatter<Char[]>
    {
        public static readonly CharArrayFormatter Instance = new CharArrayFormatter();

        CharArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Char[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteChar(value[i], ref idx);
            }
        }

        public Char[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new Char[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.ReadChar();
            }
            return array;
        }
    }

    public sealed class DateTimeFormatter : IMessagePackFormatter<DateTime>
    {
        public static readonly DateTimeFormatter Instance = new DateTimeFormatter();

        DateTimeFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, DateTime value, IFormatterResolver formatterResolver)
        {
            writer.WriteDateTime(value, ref idx);
        }

        public DateTime Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadDateTime();
        }
    }

    public sealed class NullableDateTimeFormatter : IMessagePackFormatter<DateTime?>
    {
        public static readonly NullableDateTimeFormatter Instance = new NullableDateTimeFormatter();

        NullableDateTimeFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, DateTime? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteDateTime(value.Value, ref idx);
        }

        public DateTime? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return reader.ReadDateTime();
        }
    }

    public sealed class DateTimeArrayFormatter : IMessagePackFormatter<DateTime[]>
    {
        public static readonly DateTimeArrayFormatter Instance = new DateTimeArrayFormatter();

        DateTimeArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, DateTime[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteDateTime(value[i], ref idx);
            }
        }

        public DateTime[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new DateTime[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.ReadDateTime();
            }
            return array;
        }
    }

}