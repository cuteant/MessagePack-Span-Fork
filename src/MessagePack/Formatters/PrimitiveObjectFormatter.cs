using System;
using System.Reflection;
using System.Collections.Generic;

namespace MessagePack.Formatters
{
    public sealed class PrimitiveObjectFormatter : IMessagePackFormatter<object>
    {
        public static readonly IMessagePackFormatter<object> Instance = new PrimitiveObjectFormatter();

        static readonly Dictionary<Type, int> typeToJumpCode = new Dictionary<Type, int>()
        {
            { typeof(Boolean), 0 },
            { typeof(Char), 1 },
            { typeof(SByte), 2 },
            { typeof(Byte), 3 },
            { typeof(Int16), 4 },
            { typeof(UInt16), 5 },
            { typeof(Int32), 6 },
            { typeof(UInt32), 7 },
            { typeof(Int64), 8 },
            { typeof(UInt64),9  },
            { typeof(Single), 10 },
            { typeof(Double), 11 },
            { typeof(DateTime), 12 },
            { typeof(string), 13 },
            { typeof(byte[]), 14 }
        };

        PrimitiveObjectFormatter()
        {

        }

        public static bool IsSupportedType(Type type, TypeInfo typeInfo, object value)
        {
            if (value == null) return true;
            if (typeToJumpCode.ContainsKey(type)) return true;
            if (typeInfo.IsEnum) return true;

            if (value is System.Collections.IDictionary) return true;
            if (value is System.Collections.ICollection) return true;

            return false;
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, object value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var t = value.GetType();

            if (typeToJumpCode.TryGetValue(t, out var code))
            {
                switch (code)
                {
                    case 0:
                        writer.WriteBoolean((bool)value, ref idx); return;
                    case 1:
                        writer.WriteChar((char)value, ref idx); return;
                    case 2:
                        writer.WriteSByteForceSByteBlock((sbyte)value, ref idx); return;
                    case 3:
                        writer.WriteByteForceByteBlock((byte)value, ref idx); return;
                    case 4:
                        writer.WriteInt16ForceInt16Block((Int16)value, ref idx); return;
                    case 5:
                        writer.WriteUInt16ForceUInt16Block((UInt16)value, ref idx); return;
                    case 6:
                        writer.WriteInt32ForceInt32Block((Int32)value, ref idx); return;
                    case 7:
                        writer.WriteUInt32ForceUInt32Block((UInt32)value, ref idx); return;
                    case 8:
                        writer.WriteInt64ForceInt64Block((Int64)value, ref idx); return;
                    case 9:
                        writer.WriteUInt64ForceUInt64Block((UInt64)value, ref idx); return;
                    case 10:
                        writer.WriteSingle((Single)value, ref idx); return;
                    case 11:
                        writer.WriteDouble((double)value, ref idx); return;
                    case 12:
                        writer.WriteDateTime((DateTime)value, ref idx); return;
                    case 13:
                        writer.WriteString((string)value, ref idx); return;
                    case 14:
                        writer.WriteBytes((byte[])value, ref idx); return;
                    default:
                        ThrowHelper.ThrowInvalidOperationException_NotSupported(t); return;
                }
            }
            else
            {
                if (t.IsEnum)
                {
                    var underlyingType = Enum.GetUnderlyingType(t);
                    var code2 = typeToJumpCode[underlyingType];
                    switch (code2)
                    {
                        case 2:
                            writer.WriteSByteForceSByteBlock((sbyte)value, ref idx); return;
                        case 3:
                            writer.WriteByteForceByteBlock((byte)value, ref idx); return;
                        case 4:
                            writer.WriteInt16ForceInt16Block((Int16)value, ref idx); return;
                        case 5:
                            writer.WriteUInt16ForceUInt16Block((UInt16)value, ref idx); return;
                        case 6:
                            writer.WriteInt32ForceInt32Block((Int32)value, ref idx); return;
                        case 7:
                            writer.WriteUInt32ForceUInt32Block((UInt32)value, ref idx); return;
                        case 8:
                            writer.WriteInt64ForceInt64Block((Int64)value, ref idx); return;
                        case 9:
                            writer.WriteUInt64ForceUInt64Block((UInt64)value, ref idx); return;
                    }
                }
                else if (value is System.Collections.IDictionary) // check IDictionary first
                {
                    var d = value as System.Collections.IDictionary;
                    writer.WriteMapHeader(d.Count, ref idx);
                    foreach (System.Collections.DictionaryEntry item in d)
                    {
                        Serialize(ref writer, ref idx, item.Key, formatterResolver);
                        Serialize(ref writer, ref idx, item.Value, formatterResolver);
                    }
                    return;
                }
                else if (value is System.Collections.ICollection)
                {
                    var c = value as System.Collections.ICollection;
                    writer.WriteArrayHeader(c.Count, ref idx);
                    foreach (var item in c)
                    {
                        Serialize(ref writer, ref idx, item, formatterResolver);
                    }
                    return;
                }
            }

            ThrowHelper.ThrowInvalidOperationException_NotSupported(t);
        }

        public object Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            var type = reader.GetMessagePackType();
            switch (type)
            {
                case MessagePackType.Integer:
                    var code = reader.Peek();
                    if (MessagePackCode.MinNegativeFixInt <= code && code <= MessagePackCode.MaxNegativeFixInt)
                    {
                        return reader.ReadSByte();
                    }
                    else if (MessagePackCode.MinFixInt <= code && code <= MessagePackCode.MaxFixInt)
                    {
                        return reader.ReadByte();
                    }
                    else
                    {
                        switch (code)
                        {
                            case MessagePackCode.Int8: return reader.ReadSByte();
                            case MessagePackCode.Int16: return reader.ReadInt16();
                            case MessagePackCode.Int32: return reader.ReadInt32();
                            case MessagePackCode.Int64: return reader.ReadInt64();
                            case MessagePackCode.UInt8: return reader.ReadByte();
                            case MessagePackCode.UInt16: return reader.ReadUInt16();
                            case MessagePackCode.UInt32: return reader.ReadUInt32();
                            case MessagePackCode.UInt64: return reader.ReadUInt64();
                            default: ThrowHelper.ThrowInvalidOperationException_Primitive_Bytes(); return null;
                        }
                    }
                case MessagePackType.Boolean:
                    return reader.ReadBoolean();
                case MessagePackType.Float:
                    if (MessagePackCode.Float32 == reader.Peek())
                    {
                        return reader.ReadSingle();
                    }
                    else
                    {
                        return reader.ReadDouble();
                    }
                case MessagePackType.String:
                    return reader.ReadString();
                case MessagePackType.Binary:
                    return reader.ReadBytes();
                case MessagePackType.Extension:
                    var extTypeCode = reader.GetExtensionFormatTypeCode();
                    if (extTypeCode != ReservedMessagePackExtensionTypeCode.DateTime)
                    {
                        ThrowHelper.ThrowInvalidOperationException_Primitive_Bytes();
                    }
                    return reader.ReadDateTime();
                case MessagePackType.Array:
                    {
                        var length = reader.ReadArrayHeader();

                        var objectFormatter = formatterResolver.GetFormatter<object>();
                        var array = new object[length];
                        for (int i = 0; i < length; i++)
                        {
                            array[i] = objectFormatter.Deserialize(ref reader, formatterResolver);
                        }

                        return array;
                    }
                case MessagePackType.Map:
                    {
                        var length = reader.ReadMapHeader();

                        var objectFormatter = formatterResolver.GetFormatter<object>();
                        var hash = new Dictionary<object, object>(length);
                        for (int i = 0; i < length; i++)
                        {
                            var key = objectFormatter.Deserialize(ref reader, formatterResolver);
                            var value = objectFormatter.Deserialize(ref reader, formatterResolver);

                            hash.Add(key, value);
                        }

                        return hash;
                    }
                case MessagePackType.Nil:
                    reader.Advance(1);
                    return null;
                default:
                    ThrowHelper.ThrowInvalidOperationException_Primitive_Bytes(); return null;
            }
        }
    }
}
