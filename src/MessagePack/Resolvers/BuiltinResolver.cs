﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using MessagePack.Formatters;
using MessagePack.Internal;
#if DEPENDENT_ON_CUTEANT
using CuteAnt;
#endif

namespace MessagePack.Resolvers
{
    public sealed class BuiltinResolver : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new BuiltinResolver();

        BuiltinResolver()
        {

        }

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                // Reduce IL2CPP code generate size(don't write long code in <T>)
                formatter = (IMessagePackFormatter<T>)BuiltinResolverGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }
}

namespace MessagePack.Internal
{
    internal static class BuiltinResolverGetFormatterHelper
    {
        static readonly Dictionary<Type, object> formatterMap = new Dictionary<Type, object>()
        {
            // Primitive
            {typeof(Int16), Int16Formatter.Instance},
            {typeof(Int32), Int32Formatter.Instance},
            {typeof(Int64), Int64Formatter.Instance},
            {typeof(UInt16), UInt16Formatter.Instance},
            {typeof(UInt32), UInt32Formatter.Instance},
            {typeof(UInt64), UInt64Formatter.Instance},
            {typeof(Single), SingleFormatter.Instance},
            {typeof(Double), DoubleFormatter.Instance},
            {typeof(bool), BooleanFormatter.Instance},
            {typeof(byte), ByteFormatter.Instance},
            {typeof(sbyte), SByteFormatter.Instance},
            {typeof(DateTime), DateTimeFormatter.Instance},
            {typeof(char), CharFormatter.Instance},
            
            // Nulllable Primitive
            {typeof(Nullable<Int16>), NullableInt16Formatter.Instance},
            {typeof(Nullable<Int32>), NullableInt32Formatter.Instance},
            {typeof(Nullable<Int64>), NullableInt64Formatter.Instance},
            {typeof(Nullable<UInt16>), NullableUInt16Formatter.Instance},
            {typeof(Nullable<UInt32>), NullableUInt32Formatter.Instance},
            {typeof(Nullable<UInt64>), NullableUInt64Formatter.Instance},
            {typeof(Nullable<Single>), NullableSingleFormatter.Instance},
            {typeof(Nullable<Double>), NullableDoubleFormatter.Instance},
            {typeof(Nullable<bool>), NullableBooleanFormatter.Instance},
            {typeof(Nullable<byte>), NullableByteFormatter.Instance},
            {typeof(Nullable<sbyte>), NullableSByteFormatter.Instance},
            {typeof(Nullable<DateTime>), NullableDateTimeFormatter.Instance},
            {typeof(Nullable<char>), NullableCharFormatter.Instance},
            
            // StandardClassLibraryFormatter
            {typeof(string), NullableStringFormatter.Instance},
            {typeof(decimal), DecimalFormatter.Instance},
            {typeof(decimal?), new StaticNullableFormatter<decimal>(DecimalFormatter.Instance)},
            {typeof(TimeSpan), TimeSpanFormatter.Instance},
            {typeof(TimeSpan?), new StaticNullableFormatter<TimeSpan>(TimeSpanFormatter.Instance)},
            {typeof(DateTimeOffset), DateTimeOffsetFormatter.Instance},
            {typeof(DateTimeOffset?), new StaticNullableFormatter<DateTimeOffset>(DateTimeOffsetFormatter.Instance)},
            {typeof(Guid), GuidFormatter.Instance},
            {typeof(Guid?), new StaticNullableFormatter<Guid>(GuidFormatter.Instance)},
#if DEPENDENT_ON_CUTEANT
            {typeof(CombGuid), CombGuidFormatter.Instance},
            {typeof(CombGuid?), new StaticNullableFormatter<CombGuid>(CombGuidFormatter.Instance)},
#endif
            {typeof(Uri), UriFormatter.Instance},
            {typeof(Version), VersionFormatter.Instance},
            {typeof(StringBuilder), StringBuilderFormatter.Instance},
            {typeof(BitArray), BitArrayFormatter.Instance},

            {typeof(Delegate), DelegateFormatter.Instance},
            {typeof(Type), SimpleTypeFormatter.Instance},
            {typeof(ConstructorInfo), ConstructorInfoFormatter.Instance},
            {typeof(EventInfo), EventInfoFormatter.Instance},
            {typeof(FieldInfo), FieldInfoFormatter.Instance},
            {typeof(PropertyInfo), PropertyInfoFormatter.Instance},
            {typeof(MethodInfo), MethodInfoFormatter.Instance},
            {typeof(MemberInfo), MemberInfoFormatter.Instance},
            {typeof(CultureInfo), CultureInfoFormatter.Instance},
            {typeof(IPAddress), IPAddressFormatter.Instance},
            {typeof(IPEndPoint), IPEndPointFormatter.Instance},
            {typeof(Exception), SimpleExceptionFormatter.Instance},
            {typeof(Expression), SimpleExpressionFormatter.Instance},
            {typeof(SymbolDocumentInfo), SymbolDocumentInfoFormatter.Instance},
            {typeof(MemberBinding), MemberBindingFormatter.Instance},
            {typeof(MemberAssignment), MemberAssignmentFormatter.Instance},
            {typeof(MemberListBinding), MemberListBindingFormatter.Instance},
            {typeof(MemberMemberBinding), MemberMemberBindingFormatter.Instance},
            {typeof(CatchBlock), CatchBlockFormatter.Instance},
            {typeof(ElementInit), ElementInitFormatter.Instance},
            {typeof(LabelTarget), LabelTargetFormatter.Instance},
            
            // special primitive
            {typeof(byte[]), ByteArrayFormatter.Instance},
            
            // Nil
            {typeof(Nil), NilFormatter.Instance},
            {typeof(Nil?), NullableNilFormatter.Instance},
            
            // otpmitized primitive array formatter
            {typeof(Int16[]), Int16ArrayFormatter.Instance},
            {typeof(Int32[]), Int32ArrayFormatter.Instance},
            {typeof(Int64[]), Int64ArrayFormatter.Instance},
            {typeof(UInt16[]), UInt16ArrayFormatter.Instance},
            {typeof(UInt32[]), UInt32ArrayFormatter.Instance},
            {typeof(UInt64[]), UInt64ArrayFormatter.Instance},
            {typeof(Single[]), SingleArrayFormatter.Instance},
            {typeof(Double[]), DoubleArrayFormatter.Instance},
            {typeof(Boolean[]), BooleanArrayFormatter.Instance},
            {typeof(SByte[]), SByteArrayFormatter.Instance},
            {typeof(DateTime[]), DateTimeArrayFormatter.Instance},
            {typeof(Char[]), CharArrayFormatter.Instance},
            {typeof(string[]), NullableStringArrayFormatter.Instance},

            // well known collections
            {typeof(List<Int16>), new ListFormatter<Int16>()},
            {typeof(List<Int32>), new ListFormatter<Int32>()},
            {typeof(List<Int64>), new ListFormatter<Int64>()},
            {typeof(List<UInt16>), new ListFormatter<UInt16>()},
            {typeof(List<UInt32>), new ListFormatter<UInt32>()},
            {typeof(List<UInt64>), new ListFormatter<UInt64>()},
            {typeof(List<Single>), new ListFormatter<Single>()},
            {typeof(List<Double>), new ListFormatter<Double>()},
            {typeof(List<Boolean>), new ListFormatter<Boolean>()},
            {typeof(List<byte>), new ListFormatter<byte>()},
            {typeof(List<SByte>), new ListFormatter<SByte>()},
            {typeof(List<DateTime>), new ListFormatter<DateTime>()},
            {typeof(List<Char>), new ListFormatter<Char>()},
            {typeof(List<string>), new ListFormatter<string>()},

            { typeof(ArraySegment<byte>), ByteArraySegmentFormatter.Instance },
            { typeof(ArraySegment<byte>?),new StaticNullableFormatter<ArraySegment<byte>>(ByteArraySegmentFormatter.Instance) },

            {typeof(System.Numerics.BigInteger), BigIntegerFormatter.Instance},
            {typeof(System.Numerics.BigInteger?), new StaticNullableFormatter<System.Numerics.BigInteger>(BigIntegerFormatter.Instance)},
            {typeof(System.Numerics.Complex), ComplexFormatter.Instance},
            {typeof(System.Numerics.Complex?), new StaticNullableFormatter<System.Numerics.Complex>(ComplexFormatter.Instance)},
            {typeof(System.Threading.Tasks.Task), TaskUnitFormatter.Instance},
        };

        internal static object GetFormatter(Type t)
        {
            if (formatterMap.TryGetValue(t, out object formatter))
            {
                return formatter;
            }

            return null;
        }
    }
}