namespace MessagePack
{
    using MessagePack.Decoders;

    public ref partial struct MessagePackReader
    {
        const int MaxSize = 256; // [0] ~ [255]
        const int ArrayMaxSize = 0x7FFFFFC7; // https://msdn.microsoft.com/en-us/library/system.array

        static readonly IMapHeaderDecoder[] mapHeaderDecoders;
        static readonly IArrayHeaderDecoder[] arrayHeaderDecoders;
        static readonly IBooleanDecoder[] booleanDecoders;
        static readonly IByteDecoder[] byteDecoders;
        static readonly IBytesDecoder[] bytesDecoders;
        static readonly IBytesLengthDecoder[] bytesLengthDecoders;
        static readonly ISpanDecoder[] spanDecoders;
        static readonly ISByteDecoder[] sbyteDecoders;
        static readonly ISingleDecoder[] singleDecoders;
        static readonly IDoubleDecoder[] doubleDecoders;
        static readonly IInt16Decoder[] int16Decoders;
        static readonly IInt32Decoder[] int32Decoders;
        static readonly IInt32LengthDecoder[] int32LengthDecoders;
        static readonly IInt64Decoder[] int64Decoders;
        static readonly IUInt16Decoder[] uint16Decoders;
        static readonly IUInt32Decoder[] uint32Decoders;
        static readonly IUInt64Decoder[] uint64Decoders;
        static readonly IStringDecoder[] stringDecoders;
        static readonly IStringLengthDecoder[] stringLengthDecoders;
        static readonly IUtf8SpanDecoder[] stringSpanDecoders;
        static readonly IExtDecoder[] extDecoders;
        static readonly IExtHeaderDecoder[] extHeaderDecoders;
        static readonly IExtTypeCodeDecoder[] extTypeCodeDecoders;
        static readonly IDateTimeDecoder[] dateTimeDecoders;
        static readonly IReadNextDecoder[] readNextDecoders;

        static MessagePackReader()
        {
            mapHeaderDecoders = new IMapHeaderDecoder[MaxSize];
            arrayHeaderDecoders = new IArrayHeaderDecoder[MaxSize];
            booleanDecoders = new IBooleanDecoder[MaxSize];
            byteDecoders = new IByteDecoder[MaxSize];
            bytesDecoders = new IBytesDecoder[MaxSize];
            bytesLengthDecoders = new IBytesLengthDecoder[MaxSize];
            spanDecoders = new ISpanDecoder[MaxSize];
            sbyteDecoders = new ISByteDecoder[MaxSize];
            singleDecoders = new ISingleDecoder[MaxSize];
            doubleDecoders = new IDoubleDecoder[MaxSize];
            int16Decoders = new IInt16Decoder[MaxSize];
            int32Decoders = new IInt32Decoder[MaxSize];
            int32LengthDecoders = new IInt32LengthDecoder[MaxSize];
            int64Decoders = new IInt64Decoder[MaxSize];
            uint16Decoders = new IUInt16Decoder[MaxSize];
            uint32Decoders = new IUInt32Decoder[MaxSize];
            uint64Decoders = new IUInt64Decoder[MaxSize];
            stringDecoders = new IStringDecoder[MaxSize];
            stringLengthDecoders = new IStringLengthDecoder[MaxSize];
            stringSpanDecoders = new IUtf8SpanDecoder[MaxSize];
            extDecoders = new IExtDecoder[MaxSize];
            extHeaderDecoders = new IExtHeaderDecoder[MaxSize];
            extTypeCodeDecoders = new IExtTypeCodeDecoder[MaxSize];
            dateTimeDecoders = new IDateTimeDecoder[MaxSize];
            readNextDecoders = new IReadNextDecoder[MaxSize];

            // Init LookupTable.
            for (int i = 0; i < MaxSize; i++)
            {
                mapHeaderDecoders[i] = Decoders.InvalidMapHeader.Instance;
                arrayHeaderDecoders[i] = Decoders.InvalidArrayHeader.Instance;
                booleanDecoders[i] = Decoders.InvalidBoolean.Instance;
                byteDecoders[i] = Decoders.InvalidByte.Instance;
                bytesDecoders[i] = Decoders.InvalidBytes.Instance;
                bytesLengthDecoders[i] = Decoders.InvalidBytesLength.Instance;
                spanDecoders[i] = Decoders.InvalidSpan.Instance;
                sbyteDecoders[i] = Decoders.InvalidSByte.Instance;
                singleDecoders[i] = Decoders.InvalidSingle.Instance;
                doubleDecoders[i] = Decoders.InvalidDouble.Instance;
                int16Decoders[i] = Decoders.InvalidInt16.Instance;
                int32Decoders[i] = Decoders.InvalidInt32.Instance;
                int32LengthDecoders[i] = Decoders.InvalidInt32Length.Instance;
                int64Decoders[i] = Decoders.InvalidInt64.Instance;
                uint16Decoders[i] = Decoders.InvalidUInt16.Instance;
                uint32Decoders[i] = Decoders.InvalidUInt32.Instance;
                uint64Decoders[i] = Decoders.InvalidUInt64.Instance;
                stringDecoders[i] = Decoders.InvalidString.Instance;
                stringLengthDecoders[i] = Decoders.InvalidStringLength.Instance;
                stringSpanDecoders[i] = Decoders.InvalidUtf8Span.Instance;
                extDecoders[i] = Decoders.InvalidExt.Instance;
                extHeaderDecoders[i] = Decoders.InvalidExtHeader.Instance;
                extTypeCodeDecoders[i] = Decoders.InvalidExtTypeCode.Instance;
                dateTimeDecoders[i] = Decoders.InvalidDateTime.Instance;
            }

            // Number
            for (int i = MessagePackCode.MinNegativeFixInt; i <= MessagePackCode.MaxNegativeFixInt; i++)
            {
                sbyteDecoders[i] = Decoders.FixSByte.Instance;
                int16Decoders[i] = Decoders.FixNegativeInt16.Instance;
                int32Decoders[i] = Decoders.FixNegativeInt32.Instance;
                int32LengthDecoders[i] = Decoders.FixNegativeInt32Length.Instance;
                int64Decoders[i] = Decoders.FixNegativeInt64.Instance;
                singleDecoders[i] = Decoders.FixNegativeFloat.Instance;
                doubleDecoders[i] = Decoders.FixNegativeDouble.Instance;
                readNextDecoders[i] = Decoders.ReadNext1.Instance;
            }
            for (int i = MessagePackCode.MinFixInt; i <= MessagePackCode.MaxFixInt; i++)
            {
                byteDecoders[i] = Decoders.FixByte.Instance;
                sbyteDecoders[i] = Decoders.FixSByte.Instance;
                int16Decoders[i] = Decoders.FixInt16.Instance;
                int32Decoders[i] = Decoders.FixInt32.Instance;
                int32LengthDecoders[i] = Decoders.FixInt32Length.Instance;
                int64Decoders[i] = Decoders.FixInt64.Instance;
                uint16Decoders[i] = Decoders.FixUInt16.Instance;
                uint32Decoders[i] = Decoders.FixUInt32.Instance;
                uint64Decoders[i] = Decoders.FixUInt64.Instance;
                singleDecoders[i] = Decoders.FixFloat.Instance;
                doubleDecoders[i] = Decoders.FixDouble.Instance;
                readNextDecoders[i] = Decoders.ReadNext1.Instance;
            }

            byteDecoders[MessagePackCode.UInt8] = Decoders.UInt8Byte.Instance;
            sbyteDecoders[MessagePackCode.Int8] = Decoders.Int8SByte.Instance;
            int16Decoders[MessagePackCode.UInt8] = Decoders.UInt8Int16.Instance;
            int16Decoders[MessagePackCode.UInt16] = Decoders.UInt16Int16.Instance;
            int16Decoders[MessagePackCode.Int8] = Decoders.Int8Int16.Instance;
            int16Decoders[MessagePackCode.Int16] = Decoders.Int16Int16.Instance;
            int32Decoders[MessagePackCode.UInt8] = Decoders.UInt8Int32.Instance;
            int32Decoders[MessagePackCode.UInt16] = Decoders.UInt16Int32.Instance;
            int32Decoders[MessagePackCode.UInt32] = Decoders.UInt32Int32.Instance;
            int32Decoders[MessagePackCode.Int8] = Decoders.Int8Int32.Instance;
            int32Decoders[MessagePackCode.Int16] = Decoders.Int16Int32.Instance;
            int32Decoders[MessagePackCode.Int32] = Decoders.Int32Int32.Instance;
            int32LengthDecoders[MessagePackCode.UInt8] = Decoders.UInt8Int32Length.Instance;
            int32LengthDecoders[MessagePackCode.UInt16] = Decoders.UInt16Int32Length.Instance;
            int32LengthDecoders[MessagePackCode.UInt32] = Decoders.UInt32Int32Length.Instance;
            int32LengthDecoders[MessagePackCode.Int8] = Decoders.Int8Int32Length.Instance;
            int32LengthDecoders[MessagePackCode.Int16] = Decoders.Int16Int32Length.Instance;
            int32LengthDecoders[MessagePackCode.Int32] = Decoders.Int32Int32Length.Instance;
            int64Decoders[MessagePackCode.UInt8] = Decoders.UInt8Int64.Instance;
            int64Decoders[MessagePackCode.UInt16] = Decoders.UInt16Int64.Instance;
            int64Decoders[MessagePackCode.UInt32] = Decoders.UInt32Int64.Instance;
            int64Decoders[MessagePackCode.UInt64] = Decoders.UInt64Int64.Instance;
            int64Decoders[MessagePackCode.Int8] = Decoders.Int8Int64.Instance;
            int64Decoders[MessagePackCode.Int16] = Decoders.Int16Int64.Instance;
            int64Decoders[MessagePackCode.Int32] = Decoders.Int32Int64.Instance;
            int64Decoders[MessagePackCode.Int64] = Decoders.Int64Int64.Instance;
            uint16Decoders[MessagePackCode.UInt8] = Decoders.UInt8UInt16.Instance;
            uint16Decoders[MessagePackCode.UInt16] = Decoders.UInt16UInt16.Instance;
            uint32Decoders[MessagePackCode.UInt8] = Decoders.UInt8UInt32.Instance;
            uint32Decoders[MessagePackCode.UInt16] = Decoders.UInt16UInt32.Instance;
            uint32Decoders[MessagePackCode.UInt32] = Decoders.UInt32UInt32.Instance;
            uint64Decoders[MessagePackCode.UInt8] = Decoders.UInt8UInt64.Instance;
            uint64Decoders[MessagePackCode.UInt16] = Decoders.UInt16UInt64.Instance;
            uint64Decoders[MessagePackCode.UInt32] = Decoders.UInt32UInt64.Instance;
            uint64Decoders[MessagePackCode.UInt64] = Decoders.UInt64UInt64.Instance;

            singleDecoders[MessagePackCode.Float32] = Decoders.Float32Single.Instance;
            singleDecoders[MessagePackCode.Int8] = Decoders.Int8Single.Instance;
            singleDecoders[MessagePackCode.Int16] = Decoders.Int16Single.Instance;
            singleDecoders[MessagePackCode.Int32] = Decoders.Int32Single.Instance;
            singleDecoders[MessagePackCode.Int64] = Decoders.Int64Single.Instance;
            singleDecoders[MessagePackCode.UInt8] = Decoders.UInt8Single.Instance;
            singleDecoders[MessagePackCode.UInt16] = Decoders.UInt16Single.Instance;
            singleDecoders[MessagePackCode.UInt32] = Decoders.UInt32Single.Instance;
            singleDecoders[MessagePackCode.UInt64] = Decoders.UInt64Single.Instance;

            doubleDecoders[MessagePackCode.Float32] = Decoders.Float32Double.Instance;
            doubleDecoders[MessagePackCode.Float64] = Decoders.Float64Double.Instance;
            doubleDecoders[MessagePackCode.Int8] = Decoders.Int8Double.Instance;
            doubleDecoders[MessagePackCode.Int16] = Decoders.Int16Double.Instance;
            doubleDecoders[MessagePackCode.Int32] = Decoders.Int32Double.Instance;
            doubleDecoders[MessagePackCode.Int64] = Decoders.Int64Double.Instance;
            doubleDecoders[MessagePackCode.UInt8] = Decoders.UInt8Double.Instance;
            doubleDecoders[MessagePackCode.UInt16] = Decoders.UInt16Double.Instance;
            doubleDecoders[MessagePackCode.UInt32] = Decoders.UInt32Double.Instance;
            doubleDecoders[MessagePackCode.UInt64] = Decoders.UInt64Double.Instance;

            readNextDecoders[MessagePackCode.Int8] = Decoders.ReadNext2.Instance;
            readNextDecoders[MessagePackCode.Int16] = Decoders.ReadNext3.Instance;
            readNextDecoders[MessagePackCode.Int32] = Decoders.ReadNext5.Instance;
            readNextDecoders[MessagePackCode.Int64] = Decoders.ReadNext9.Instance;
            readNextDecoders[MessagePackCode.UInt8] = Decoders.ReadNext2.Instance;
            readNextDecoders[MessagePackCode.UInt16] = Decoders.ReadNext3.Instance;
            readNextDecoders[MessagePackCode.UInt32] = Decoders.ReadNext5.Instance;
            readNextDecoders[MessagePackCode.UInt64] = Decoders.ReadNext9.Instance;
            readNextDecoders[MessagePackCode.Float32] = Decoders.ReadNext5.Instance;
            readNextDecoders[MessagePackCode.Float64] = Decoders.ReadNext9.Instance;

            // Map
            for (int i = MessagePackCode.MinFixMap; i <= MessagePackCode.MaxFixMap; i++)
            {
                mapHeaderDecoders[i] = Decoders.FixMapHeader.Instance;
                readNextDecoders[i] = Decoders.ReadNext1.Instance;
            }
            mapHeaderDecoders[MessagePackCode.Map16] = Decoders.Map16Header.Instance;
            mapHeaderDecoders[MessagePackCode.Map32] = Decoders.Map32Header.Instance;
            readNextDecoders[MessagePackCode.Map16] = Decoders.ReadNextMap.Instance;
            readNextDecoders[MessagePackCode.Map32] = Decoders.ReadNextMap.Instance;

            // Array
            for (int i = MessagePackCode.MinFixArray; i <= MessagePackCode.MaxFixArray; i++)
            {
                arrayHeaderDecoders[i] = Decoders.FixArrayHeader.Instance;
                readNextDecoders[i] = Decoders.ReadNext1.Instance;
            }
            arrayHeaderDecoders[MessagePackCode.Array16] = Decoders.Array16Header.Instance;
            arrayHeaderDecoders[MessagePackCode.Array32] = Decoders.Array32Header.Instance;
            readNextDecoders[MessagePackCode.Array16] = Decoders.ReadNextArray.Instance;
            readNextDecoders[MessagePackCode.Array32] = Decoders.ReadNextArray.Instance;

            // Str
            for (int i = MessagePackCode.MinFixStr; i <= MessagePackCode.MaxFixStr; i++)
            {
                stringDecoders[i] = Decoders.FixString.Instance;
                stringLengthDecoders[i] = Decoders.FixStringLength.Instance;
                stringSpanDecoders[i] = Decoders.FixUtf8Span.Instance;
                readNextDecoders[i] = Decoders.ReadNextFixStr.Instance;
            }

            stringDecoders[MessagePackCode.Str8] = Decoders.Str8String.Instance;
            stringDecoders[MessagePackCode.Str16] = Decoders.Str16String.Instance;
            stringDecoders[MessagePackCode.Str32] = Decoders.Str32String.Instance;
            stringLengthDecoders[MessagePackCode.Str8] = Decoders.Str8StringLength.Instance;
            stringLengthDecoders[MessagePackCode.Str16] = Decoders.Str16StringLength.Instance;
            stringLengthDecoders[MessagePackCode.Str32] = Decoders.Str32StringLength.Instance;
            stringSpanDecoders[MessagePackCode.Str8] = Decoders.Str8Utf8Span.Instance;
            stringSpanDecoders[MessagePackCode.Str16] = Decoders.Str16Utf8Span.Instance;
            stringSpanDecoders[MessagePackCode.Str32] = Decoders.Str32Utf8Span.Instance;
            readNextDecoders[MessagePackCode.Str8] = Decoders.ReadNextStr8.Instance;
            readNextDecoders[MessagePackCode.Str16] = Decoders.ReadNextStr16.Instance;
            readNextDecoders[MessagePackCode.Str32] = Decoders.ReadNextStr32.Instance;

            // Others
            stringDecoders[MessagePackCode.Nil] = Decoders.NilString.Instance;
            stringLengthDecoders[MessagePackCode.Nil] = Decoders.NilStringLength.Instance;
            stringSpanDecoders[MessagePackCode.Nil] = Decoders.NilUtf8Span.Instance;
            bytesDecoders[MessagePackCode.Nil] = Decoders.NilBytes.Instance;
            bytesLengthDecoders[MessagePackCode.Nil] = Decoders.NilBytesLength.Instance;
            spanDecoders[MessagePackCode.Nil] = Decoders.NilBytesSpan.Instance;
            readNextDecoders[MessagePackCode.Nil] = Decoders.ReadNext1.Instance;

            booleanDecoders[MessagePackCode.False] = Decoders.False.Instance;
            booleanDecoders[MessagePackCode.True] = Decoders.True.Instance;
            readNextDecoders[MessagePackCode.False] = Decoders.ReadNext1.Instance;
            readNextDecoders[MessagePackCode.True] = Decoders.ReadNext1.Instance;

            bytesDecoders[MessagePackCode.Bin8] = Decoders.Bin8Bytes.Instance;
            bytesDecoders[MessagePackCode.Bin16] = Decoders.Bin16Bytes.Instance;
            bytesDecoders[MessagePackCode.Bin32] = Decoders.Bin32Bytes.Instance;
            bytesLengthDecoders[MessagePackCode.Bin8] = Decoders.Bin8BytesLength.Instance;
            bytesLengthDecoders[MessagePackCode.Bin16] = Decoders.Bin16BytesLength.Instance;
            bytesLengthDecoders[MessagePackCode.Bin32] = Decoders.Bin32BytesLength.Instance;
            spanDecoders[MessagePackCode.Bin8] = Decoders.Bin8Span.Instance;
            spanDecoders[MessagePackCode.Bin16] = Decoders.Bin16Span.Instance;
            spanDecoders[MessagePackCode.Bin32] = Decoders.Bin32Span.Instance;
            readNextDecoders[MessagePackCode.Bin8] = Decoders.ReadNextBin8.Instance;
            readNextDecoders[MessagePackCode.Bin16] = Decoders.ReadNextBin16.Instance;
            readNextDecoders[MessagePackCode.Bin32] = Decoders.ReadNextBin32.Instance;

            // Ext
            extDecoders[MessagePackCode.FixExt1] = Decoders.FixExt1.Instance;
            extDecoders[MessagePackCode.FixExt2] = Decoders.FixExt2.Instance;
            extDecoders[MessagePackCode.FixExt4] = Decoders.FixExt4.Instance;
            extDecoders[MessagePackCode.FixExt8] = Decoders.FixExt8.Instance;
            extDecoders[MessagePackCode.FixExt16] = Decoders.FixExt16.Instance;
            extDecoders[MessagePackCode.Ext8] = Decoders.Ext8.Instance;
            extDecoders[MessagePackCode.Ext16] = Decoders.Ext16.Instance;
            extDecoders[MessagePackCode.Ext32] = Decoders.Ext32.Instance;

            extHeaderDecoders[MessagePackCode.FixExt1] = Decoders.FixExt1Header.Instance;
            extHeaderDecoders[MessagePackCode.FixExt2] = Decoders.FixExt2Header.Instance;
            extHeaderDecoders[MessagePackCode.FixExt4] = Decoders.FixExt4Header.Instance;
            extHeaderDecoders[MessagePackCode.FixExt8] = Decoders.FixExt8Header.Instance;
            extHeaderDecoders[MessagePackCode.FixExt16] = Decoders.FixExt16Header.Instance;
            extHeaderDecoders[MessagePackCode.Ext8] = Decoders.Ext8Header.Instance;
            extHeaderDecoders[MessagePackCode.Ext16] = Decoders.Ext16Header.Instance;
            extHeaderDecoders[MessagePackCode.Ext32] = Decoders.Ext32Header.Instance;

            extTypeCodeDecoders[MessagePackCode.FixExt1] = Decoders.FixExt1TypeCode.Instance;
            extTypeCodeDecoders[MessagePackCode.FixExt2] = Decoders.FixExt2TypeCode.Instance;
            extTypeCodeDecoders[MessagePackCode.FixExt4] = Decoders.FixExt4TypeCode.Instance;
            extTypeCodeDecoders[MessagePackCode.FixExt8] = Decoders.FixExt8TypeCode.Instance;
            extTypeCodeDecoders[MessagePackCode.FixExt16] = Decoders.FixExt16TypeCode.Instance;
            extTypeCodeDecoders[MessagePackCode.Ext8] = Decoders.Ext8TypeCode.Instance;
            extTypeCodeDecoders[MessagePackCode.Ext16] = Decoders.Ext16TypeCode.Instance;
            extTypeCodeDecoders[MessagePackCode.Ext32] = Decoders.Ext32TypeCode.Instance;

            readNextDecoders[MessagePackCode.FixExt1] = Decoders.ReadNext3.Instance;
            readNextDecoders[MessagePackCode.FixExt2] = Decoders.ReadNext4.Instance;
            readNextDecoders[MessagePackCode.FixExt4] = Decoders.ReadNext6.Instance;
            readNextDecoders[MessagePackCode.FixExt8] = Decoders.ReadNext10.Instance;
            readNextDecoders[MessagePackCode.FixExt16] = Decoders.ReadNext18.Instance;
            readNextDecoders[MessagePackCode.Ext8] = Decoders.ReadNextExt8.Instance;
            readNextDecoders[MessagePackCode.Ext16] = Decoders.ReadNextExt16.Instance;
            readNextDecoders[MessagePackCode.Ext32] = Decoders.ReadNextExt32.Instance;

            // DateTime
            dateTimeDecoders[MessagePackCode.FixExt4] = Decoders.FixExt4DateTime.Instance;
            dateTimeDecoders[MessagePackCode.FixExt8] = Decoders.FixExt8DateTime.Instance;
            dateTimeDecoders[MessagePackCode.Ext8] = Decoders.Ext8DateTime.Instance;
        }
    }
}
