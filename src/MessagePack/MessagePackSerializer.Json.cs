namespace MessagePack
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using MessagePack.Formatters;

    // JSON API
    public static partial class MessagePackSerializer
    {
        /// <summary>Dump to JSON string.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToJson<T>(T obj)
        {
            using (var serializedData = SerializeUnsafe(obj))
            {
                return ToJson(serializedData.Span);
            }
        }

        /// <summary>Dump to JSON string.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToJson<T>(T obj, IFormatterResolver resolver)
        {
            using (var serializedData = SerializeUnsafe(obj, resolver))
            {
                return ToJson(serializedData.Span);
            }
        }

        /// <summary>Dump message-pack binary to JSON string.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToJson(byte[] messagePackBinary)
        {
            return ToJson(new ReadOnlySpan<byte>(messagePackBinary));
        }

        /// <summary>Dump message-pack binary to JSON string.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToJson(ArraySegment<byte> messagePackBinary)
        {
            return ToJson(new ReadOnlySpan<byte>(messagePackBinary.Array, messagePackBinary.Offset, messagePackBinary.Count));
        }

        /// <summary>Dump message-pack binary to JSON string.</summary>
        public static string ToJson(ReadOnlySpan<byte> messagePackBinary)
        {
            if (messagePackBinary.IsEmpty) { return ""; }

            var sb = new StringBuilder();
            var reader = new MessagePackReader(messagePackBinary);
            ToJsonCore(ref reader, sb);
            return sb.ToString();
        }

        /// <summary>Dump message-pack binary to JSON string.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToJson(ReadOnlySpan<byte> messagePackBinary, StringBuilder output)
        {
            if (null == output) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.output); }
            if (messagePackBinary.IsEmpty) { return; }

            var reader = new MessagePackReader(messagePackBinary);
            ToJsonCore(ref reader, output);
        }

        /// <summary>From Json String to LZ4MessagePack binary</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] FromJson(string str)
        {
            using (var sr = new StringReader(str))
            {
                return FromJson(sr);
            }
        }

        /// <summary>From Json String to MessagePack binary</summary>
        public static byte[] FromJson(TextReader reader)
        {
            using (var jr = new TinyJsonReader(reader, false))
            {
                var idx = 0;
                var writer = new MessagePackWriter(true);
                FromJsonCore(jr, ref writer, ref idx);
                return writer.ToArray(idx);
            }
        }

        /// <summary>return buffer is from memory pool, be careful to use.</summary>
        internal static IOwnedBuffer<byte> FromJsonUnsafe(TextReader reader)
        {
            using (var jr = new TinyJsonReader(reader, false))
            {
                var idx = 0;
                var writer = new MessagePackWriter(true);
                FromJsonCore(jr, ref writer, ref idx);
                return writer.ToOwnedBuffer(idx);
            }
        }

        static uint FromJsonCore(TinyJsonReader jr, ref MessagePackWriter writer, ref int idx)
        {
            uint count = 0;
            while (jr.Read())
            {
                switch (jr.TokenType)
                {
                    case TinyJsonToken.None:
                        break;
                    case TinyJsonToken.StartObject:
                        {
                            var startOffset = idx;
                            idx += 5;
                            var mapCount = FromJsonCore(jr, ref writer, ref idx);
                            mapCount = mapCount / 2; // remove propertyname string count.
                            MessagePackBinary.WriteMapHeaderForceMap32Block(ref writer.PinnableAddress, startOffset, mapCount);
                            count++;
                            break;
                        }
                    case TinyJsonToken.EndObject:
                        return count; // break
                    case TinyJsonToken.StartArray:
                        {
                            var startOffset = idx;
                            idx += 5;
                            var arrayCount = FromJsonCore(jr, ref writer, ref idx);
                            MessagePackBinary.WriteArrayHeaderForceArray32Block(ref writer.PinnableAddress, startOffset, arrayCount);
                            count++;
                            break;
                        }
                    case TinyJsonToken.EndArray:
                        return count; // break
                    case TinyJsonToken.Number:
                        var v = jr.ValueType;
                        if (v == ValueType.Double)
                        {
                            writer.WriteDouble(jr.DoubleValue, ref idx);
                        }
                        else if (v == ValueType.Long)
                        {
                            writer.WriteInt64(jr.LongValue, ref idx);
                        }
                        else if (v == ValueType.ULong)
                        {
                            writer.WriteUInt64(jr.ULongValue, ref idx);
                        }
                        else if (v == ValueType.Decimal)
                        {
                            DecimalFormatter.Instance.Serialize(ref writer, ref idx, jr.DecimalValue, null);
                        }
                        count++;
                        break;
                    case TinyJsonToken.String:
                        writer.WriteString(jr.StringValue, ref idx);
                        count++;
                        break;
                    case TinyJsonToken.True:
                        writer.WriteBoolean(true, ref idx);
                        count++;
                        break;
                    case TinyJsonToken.False:
                        writer.WriteBoolean(false, ref idx);
                        count++;
                        break;
                    case TinyJsonToken.Null:
                        writer.WriteNil(ref idx);
                        count++;
                        break;

                    default: break;
                }
            }
            return count;
        }

        static void ToJsonCore(ref MessagePackReader reader, StringBuilder builder)
        {
            var type = reader.GetMessagePackType();
            switch (type)
            {
                case MessagePackType.Integer:
                    var code = reader.Peek();
                    if (MessagePackCode.MinNegativeFixInt <= code && code <= MessagePackCode.MaxNegativeFixInt)
                    {
                        builder.Append(reader.ReadSByte().ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (MessagePackCode.MinFixInt <= code && code <= MessagePackCode.MaxFixInt)
                    {
                        builder.Append(reader.ReadByte().ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        switch (code)
                        {
                            case MessagePackCode.Int8:
                                builder.Append(reader.ReadSByte().ToString(System.Globalization.CultureInfo.InvariantCulture));
                                break;
                            case MessagePackCode.Int16:
                                builder.Append(reader.ReadInt16().ToString(System.Globalization.CultureInfo.InvariantCulture));
                                break;
                            case MessagePackCode.Int32:
                                builder.Append(reader.ReadInt32().ToString(System.Globalization.CultureInfo.InvariantCulture));
                                break;
                            case MessagePackCode.Int64:
                                builder.Append(reader.ReadInt64().ToString(System.Globalization.CultureInfo.InvariantCulture));
                                break;
                            case MessagePackCode.UInt8:
                                builder.Append(reader.ReadByte().ToString(System.Globalization.CultureInfo.InvariantCulture));
                                break;
                            case MessagePackCode.UInt16:
                                builder.Append(reader.ReadUInt16().ToString(System.Globalization.CultureInfo.InvariantCulture));
                                break;
                            case MessagePackCode.UInt32:
                                builder.Append(reader.ReadUInt32().ToString(System.Globalization.CultureInfo.InvariantCulture));
                                break;
                            case MessagePackCode.UInt64:
                                builder.Append(reader.ReadUInt64().ToString(System.Globalization.CultureInfo.InvariantCulture));
                                break;
                        }
                    }
                    break;
                case MessagePackType.Boolean:
                    builder.Append(reader.ReadBoolean() ? "true" : "false");
                    break;
                case MessagePackType.Float:
                    var floatCode = reader.Peek();
                    if (floatCode == MessagePackCode.Float32)
                    {
                        builder.Append(reader.ReadSingle().ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        builder.Append(reader.ReadDouble().ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    break;
                case MessagePackType.String:
                    WriteJsonString(reader.ReadString(), builder);
                    break;
                case MessagePackType.Binary:
                    builder.Append("\"" + Convert.ToBase64String(reader.ReadBytes()) + "\"");
                    break;
                case MessagePackType.Array:
                    {
                        var length = reader.ReadArrayHeaderRaw();
                        builder.Append("[");
                        for (int i = 0; i < length; i++)
                        {
                            ToJsonCore(ref reader, builder);

                            if (i != length - 1)
                            {
                                builder.Append(",");
                            }
                        }
                        builder.Append("]");

                        return;
                    }
                case MessagePackType.Map:
                    {
                        var length = reader.ReadMapHeaderRaw();
                        builder.Append("{");
                        for (int i = 0; i < length; i++)
                        {
                            // write key
                            {
                                var keyType = reader.GetMessagePackType();
                                if (keyType == MessagePackType.String || keyType == MessagePackType.Binary)
                                {
                                    ToJsonCore(ref reader, builder);
                                }
                                else
                                {
                                    builder.Append("\"");
                                    ToJsonCore(ref reader, builder);
                                    builder.Append("\"");
                                }
                            }

                            builder.Append(":");

                            // write body
                            {
                                ToJsonCore(ref reader, builder);
                            }

                            if (i != length - 1)
                            {
                                builder.Append(",");
                            }
                        }
                        builder.Append("}");

                        return;
                    }
                case MessagePackType.Extension:
                    WriteExtensionFormat(ref reader, builder);
                    break;
                case MessagePackType.Unknown:
                case MessagePackType.Nil:
                default:
                    reader.Advance(1);
                    builder.Append("null");
                    break;
            }

            return;
        }

        private static void WriteExtensionFormat(ref MessagePackReader reader, StringBuilder builder)
        {
            var extHeaderTypeCode = reader.GetExtensionFormatTypeCode();

            switch (extHeaderTypeCode)
            {
                case ReservedMessagePackExtensionTypeCode.DateTime:
                    var dt = reader.ReadDateTime();
                    builder.Append("\"");
                    builder.Append(dt.ToString("o", CultureInfo.InvariantCulture));
                    builder.Append("\"");
                    break;

                case ReservedMessagePackExtensionTypeCode.DateTimeOffset:
                    var dts = ExtDateTimeOffsetFormatter.Instance.Deserialize(ref reader, null);
                    builder.Append("\"");
                    builder.Append(dts.ToString("u", CultureInfo.InvariantCulture));
                    builder.Append("\"");
                    break;

                case ReservedMessagePackExtensionTypeCode.Guid:
                    var guid = ExtBinaryGuidFormatter.Instance.Deserialize(ref reader, null);
                    builder.Append("\"");
                    builder.Append(guid.ToString("D"));
                    builder.Append("\"");
                    break;

#if DEPENDENT_ON_CUTEANT
                case ReservedMessagePackExtensionTypeCode.ComgGuid:
                    var comb = CombGuidFormatter.Instance.Deserialize(ref reader, null);
                    builder.Append("\"");
                    builder.Append(comb.ToString(CuteAnt.CombGuidFormatStringType.Comb));
                    builder.Append("\"");
                    break;
#endif

                case ReservedMessagePackExtensionTypeCode.Decimal:
                    var decimalValue = ExtBinaryDecimalFormatter.Instance.Deserialize(ref reader, null);
                    builder.Append(decimalValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    break;

                case ReservedMessagePackExtensionTypeCode.Typeless:
                    var extHeader = reader.ReadExtensionFormatHeader();

                    // prepare type name token
                    var typeNameLen = reader.GetEncodedStringLength();
                    var typeNameToken = new StringBuilder();
                    ToJsonCore(ref reader, typeNameToken);
                    int startBuilderLength = builder.Length;
                    if (extHeader.Length > typeNameLen)
                    {
                        // object map or array
                        var typeInside = reader.GetMessagePackType();
                        if (typeInside != MessagePackType.Array && typeInside != MessagePackType.Map)
                        {
                            builder.Append("{");
                        }

                        ToJsonCore(ref reader, builder);
                        // insert type name token to start of object map or array
                        if (typeInside != MessagePackType.Array)
                        {
                            typeNameToken.Insert(0, "\"$type\":");
                        }
                        if (typeInside != MessagePackType.Array && typeInside != MessagePackType.Map)
                        {
                            builder.Append("}");
                        }
                        if (builder.Length - startBuilderLength > 2)
                        {
                            typeNameToken.Append(",");
                        }
                        builder.Insert(startBuilderLength + 1, typeNameToken.ToString());
                    }
                    else
                    {
                        builder.Append("{\"$type\":\"" + typeNameToken.ToString() + "}");
                    }
                    break;

                default:
                    var ext = reader.ReadExtensionFormat();
                    builder.Append("[");
                    builder.Append(ext.TypeCode);
                    builder.Append(",");
                    builder.Append("\"");
#if NETCOREAPP
                    builder.Append(Convert.ToBase64String(ext.Data));
#else
                    builder.Append(Convert.ToBase64String(ext.Data.ToArray()));
#endif
                    builder.Append("\"");
                    builder.Append("]");
                    break;
            }
        }

        // escape string
        private static void WriteJsonString(string value, StringBuilder builder)
        {
            builder.Append('\"');

            var len = value.Length;
            for (int i = 0; i < len; i++)
            {
                var c = value[i];
                switch (c)
                {
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\b':
                        builder.Append("\\b");
                        break;
                    case '\f':
                        builder.Append("\\f");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }

            builder.Append('\"');
        }
    }
}