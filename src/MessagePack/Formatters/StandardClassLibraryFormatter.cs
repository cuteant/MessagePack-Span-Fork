﻿namespace MessagePack.Formatters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using MessagePack.Internal;

    // NET40 -> BigInteger, Complex, Tuple

    // byte[] is special. represents bin type.
    public sealed class ByteArrayFormatter : IMessagePackFormatter<byte[]>
    {
        public static readonly ByteArrayFormatter Instance = new ByteArrayFormatter();

        ByteArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, byte[] value, IFormatterResolver formatterResolver)
        {
            writer.WriteBytes(value, ref idx);
        }

        public byte[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return reader.ReadBytes();
        }
    }

    public sealed class NullableStringFormatter : IMessagePackFormatter<String>
    {
        public static readonly NullableStringFormatter Instance = new NullableStringFormatter();

        NullableStringFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, String value, IFormatterResolver typeResolver)
        {
            writer.WriteString(value, ref idx);
        }

        public String Deserialize(ref MessagePackReader reader, IFormatterResolver typeResolver)
        {
            return reader.ReadString();
        }
    }

    public sealed class NullableStringArrayFormatter : IMessagePackFormatter<String[]>
    {
        public static readonly NullableStringArrayFormatter Instance = new NullableStringArrayFormatter();

        NullableStringArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, String[] value, IFormatterResolver typeResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(value.Length, ref idx);
            for (int i = 0; i < value.Length; i++)
            {
                writer.WriteString(value[i], ref idx);
            }
        }

        public String[] Deserialize(ref MessagePackReader reader, IFormatterResolver typeResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new String[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = reader.ReadString();
            }
            return array;
        }
    }

    public sealed class DecimalFormatter : IMessagePackFormatter<Decimal>
    {
        public static readonly DecimalFormatter Instance = new DecimalFormatter();

        DecimalFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, decimal value, IFormatterResolver formatterResolver)
        {
            writer.WriteString(value.ToString(CultureInfo.InvariantCulture), ref idx);
        }

        public decimal Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return decimal.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
        }
    }

    public sealed class TimeSpanFormatter : IMessagePackFormatter<TimeSpan>
    {
        public static readonly IMessagePackFormatter<TimeSpan> Instance = new TimeSpanFormatter();

        TimeSpanFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, TimeSpan value, IFormatterResolver formatterResolver)
        {
            writer.WriteInt64(value.Ticks, ref idx);
        }

        public TimeSpan Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return new TimeSpan(reader.ReadInt64());
        }
    }

    public sealed class DateTimeOffsetFormatter : IMessagePackFormatter<DateTimeOffset>
    {
        public static readonly IMessagePackFormatter<DateTimeOffset> Instance = new DateTimeOffsetFormatter();

        DateTimeOffsetFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, DateTimeOffset value, IFormatterResolver formatterResolver)
        {
            writer.WriteFixedArrayHeaderUnsafe(2, ref idx);
            writer.WriteDateTime(new DateTime(value.Ticks, DateTimeKind.Utc), ref idx); // current ticks as is
            writer.WriteInt16((short)value.Offset.TotalMinutes, ref idx); // offset is normalized in minutes
        }

        public DateTimeOffset Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            var count = reader.ReadArrayHeader();
            if (count != 2) ThrowHelper.ThrowInvalidOperationException_DateTimeOffset_Format();

            var utc = reader.ReadDateTime();
            var dtOffsetMinutes = reader.ReadInt16();
            return new DateTimeOffset(utc.Ticks, TimeSpan.FromMinutes(dtOffsetMinutes));
        }
    }

    public sealed class GuidFormatter : IMessagePackFormatter<Guid>
    {
        public static readonly IMessagePackFormatter<Guid> Instance = new GuidFormatter();

        GuidFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Guid value, IFormatterResolver formatterResolver)
        {
            writer.Ensure(idx, 38);

            var buffer = value.ToByteArray();

            ref byte pinnableAddr = ref writer.PinnableAddress;
            IntPtr offset = (IntPtr)idx;
            Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.Str8;
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)36);
            idx += 2;
            new GuidBits(ref value).Write(ref pinnableAddr, ref idx);
        }

        public Guid Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            var utf8Span = reader.ReadUtf8Span();
            return new GuidBits(utf8Span).Value;
        }
    }

    public sealed class UriFormatter : IMessagePackFormatter<Uri>
    {
        public static readonly IMessagePackFormatter<Uri> Instance = new UriFormatter();

        UriFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Uri value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }
            writer.WriteString(value.ToString(), ref idx);
        }

        public Uri Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return new Uri(reader.ReadString(), UriKind.RelativeOrAbsolute);
        }
    }

    public sealed class VersionFormatter : IMessagePackFormatter<Version>
    {
        public static readonly IMessagePackFormatter<Version> Instance = new VersionFormatter();

        VersionFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Version value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }
            writer.WriteString(value.ToString(), ref idx);
        }

        public Version Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return new Version(reader.ReadString());
        }
    }

    public sealed class KeyValuePairFormatter<TKey, TValue> : IMessagePackFormatter<KeyValuePair<TKey, TValue>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, KeyValuePair<TKey, TValue> value, IFormatterResolver formatterResolver)
        {
            writer.WriteFixedArrayHeaderUnsafe(2, ref idx);
            formatterResolver.GetFormatterWithVerify<TKey>().Serialize(ref writer, ref idx, value.Key, formatterResolver);
            formatterResolver.GetFormatterWithVerify<TValue>().Serialize(ref writer, ref idx, value.Value, formatterResolver);
        }

        public KeyValuePair<TKey, TValue> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            var count = reader.ReadArrayHeader();
            if (count != 2) ThrowHelper.ThrowInvalidOperationException_KeyValuePair_Format();

            var key = formatterResolver.GetFormatterWithVerify<TKey>().Deserialize(ref reader, formatterResolver);
            var value = formatterResolver.GetFormatterWithVerify<TValue>().Deserialize(ref reader, formatterResolver);
            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }

    public sealed class StringBuilderFormatter : IMessagePackFormatter<StringBuilder>
    {
        public static readonly IMessagePackFormatter<StringBuilder> Instance = new StringBuilderFormatter();

        StringBuilderFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, StringBuilder value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }
            writer.WriteString(value.ToString(), ref idx);
        }

        public StringBuilder Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }
            return new StringBuilder(reader.ReadString());
        }
    }

    public sealed class BitArrayFormatter : IMessagePackFormatter<BitArray>
    {
        public static readonly IMessagePackFormatter<BitArray> Instance = new BitArrayFormatter();

        BitArrayFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, BitArray value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var len = value.Length;
            writer.WriteArrayHeader(len, ref idx);
            for (int i = 0; i < len; i++)
            {
                writer.WriteBoolean(value.Get(i), ref idx);
            }
        }

        public BitArray Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var len = reader.ReadArrayHeader();
            var array = new BitArray(len);
            for (int i = 0; i < len; i++)
            {
                array[i] = reader.ReadBoolean();
            }
            return array;
        }
    }

    public sealed class BigIntegerFormatter : IMessagePackFormatter<System.Numerics.BigInteger>
    {
        public static readonly IMessagePackFormatter<System.Numerics.BigInteger> Instance = new BigIntegerFormatter();

        BigIntegerFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, System.Numerics.BigInteger value, IFormatterResolver formatterResolver)
        {
            writer.WriteBytes(value.ToByteArray(), ref idx);
        }

        public System.Numerics.BigInteger Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return new System.Numerics.BigInteger(reader.ReadBytes());
        }
    }

    public sealed class ComplexFormatter : IMessagePackFormatter<System.Numerics.Complex>
    {
        public static readonly IMessagePackFormatter<System.Numerics.Complex> Instance = new ComplexFormatter();

        ComplexFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, System.Numerics.Complex value, IFormatterResolver formatterResolver)
        {
            writer.WriteFixedArrayHeaderUnsafe(2, ref idx);
            writer.WriteDouble(value.Real, ref idx);
            writer.WriteDouble(value.Imaginary, ref idx);
        }

        public System.Numerics.Complex Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            var count = reader.ReadArrayHeader();
            if (count != 2) ThrowHelper.ThrowInvalidOperationException_Complex_Format();

            var real = reader.ReadDouble();
            var imaginary = reader.ReadDouble();
            return new System.Numerics.Complex(real, imaginary);
        }
    }

    public sealed class LazyFormatter<T> : IMessagePackFormatter<Lazy<T>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, Lazy<T> value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }
            formatterResolver.GetFormatterWithVerify<T>().Serialize(ref writer, ref idx, value.Value, formatterResolver);
        }

        public Lazy<T> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            // deserialize immediately(no delay, because capture byte[] causes memory leak)
            var v = formatterResolver.GetFormatterWithVerify<T>().Deserialize(ref reader, formatterResolver);
            return new Lazy<T>(() => v);
        }
    }

    public sealed class TaskUnitFormatter : IMessagePackFormatter<Task>
    {
        public static readonly IMessagePackFormatter<Task> Instance = new TaskUnitFormatter();
        static readonly Task CompletedTask = Task.FromResult<object>(null);

        TaskUnitFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, Task value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            value.Wait(); // wait...!
            writer.WriteNil(ref idx);
        }

        public Task Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return CompletedTask; }
            ThrowHelper.ThrowInvalidOperationException_Input(); return null;
        }
    }

    public sealed class TaskValueFormatter<T> : IMessagePackFormatter<Task<T>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, Task<T> value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            // value.Result -> wait...!
            formatterResolver.GetFormatterWithVerify<T>().Serialize(ref writer, ref idx, value.Result, formatterResolver);
        }

        public Task<T> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var v = formatterResolver.GetFormatterWithVerify<T>().Deserialize(ref reader, formatterResolver);
            return Task.FromResult(v);
        }
    }

    public sealed class ValueTaskFormatter<T> : IMessagePackFormatter<ValueTask<T>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, ValueTask<T> value, IFormatterResolver formatterResolver)
        {
            formatterResolver.GetFormatterWithVerify<T>().Serialize(ref writer, ref idx, value.Result, formatterResolver);
        }

        public ValueTask<T> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            var v = formatterResolver.GetFormatterWithVerify<T>().Deserialize(ref reader, formatterResolver);
            return new ValueTask<T>(v);
        }
    }
}