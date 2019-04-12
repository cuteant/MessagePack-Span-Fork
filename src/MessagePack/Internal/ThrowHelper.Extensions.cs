using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using MessagePack.Internal;

namespace MessagePack
{
    #region -- ExceptionArgument --

    /// <summary>The convention for this enum is using the argument name as the enum name</summary>
    internal enum ExceptionArgument
    {
        array,
        arrayPool,
        assembly,
        buffer,
        destination,
        key,
        obj,
        s,
        str,
        source,
        bufferWriter,
        type,
        types,
        value,
        values,
        valueFactory,
        name,
        item,
        options,
        list,
        output,
        ts,
        other,
        pool,
        inner,
        policy,
        offset,
        count,
        path,
        typeInfo,
        method,
        qualifiedTypeName,
        fullName,
        feature,
        manager,
        directories,
        dirEnumArgs,
        asm,
        includedAssemblies,
        func,
        defaultFn,
        returnType,
        propertyInfo,
        parameterTypes,
        fieldInfo,
        memberInfo,
        attributeType,
        pi,
        fi,
        invoker,
        instanceType,
        target,
        member,
        typeName,
        predicate,
        assemblyPredicate,
        collection,
        capacity,
        match,
        index,
        length,
        startIndex,
        newSize,
        typelessFormatter,
        expression,
    }

    #endregion

    #region -- ExceptionResource --

    /// <summary>The convention for this enum is using the resource name as the enum name</summary>
    internal enum ExceptionResource
    {
        MessagePack_Register_Err,
    }

    #endregion

    partial class ThrowHelper
    {
        #region -- InvalidOperationException --

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_ValueTuple_Nil()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Data is Nil, ValueTuple can not be null.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Reached_MaximumSize()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("byte[] size reached maximum size of array(0x7FFFFFC7), can not write to single byte[]. Details: https://msdn.microsoft.com/en-us/library/system.array");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_ValueTuple_Count()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid ValueTuple count");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Primitive_Bytes()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid primitive bytes.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Grouping_Format()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid Grouping format.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_ConstructorInfo_Format()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid ConstructorInfo format");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_EventInfo_Format()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid EventInfo format");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_FieldInfo_Format()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid FieldInfo format");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_MemberInfo_Format()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid MemberInfo format");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_MethodInfo_Format()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid MethodInfo format");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_PropertyInfo_Format()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid PropertyInfo format");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_DynamicObject_Format()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid DynamicObject format");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Delegate_Format()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid Delegate format");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_IPEndPoint_Format()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid IPEndPoint format");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Tuple_Count()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid Tuple count");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Input()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid input");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Complex_Format()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid Complex format.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_KeyValuePair_Format()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid KeyValuePair format.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_DateTimeOffset_Format()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid DateTimeOffset format.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Decimal_Format()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid Decimal format.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Guid_Size()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid Guid Size.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_T_Format1()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid T[,] format");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_T_Format2()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid T[,,] format");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_T_Format3()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid T[,,,] format");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Dict()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("DictionaryFormatterBase's TDictionary supports only ICollection<KVP> or IReadOnlyCollection<KVP>");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Code_Detected(int byteCode)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid MessagePack code was detected, code:" + byteCode);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_MessagePackType(byte code)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid MessagePackType:" + MessagePackCode.ToFormatName(code));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Code(byte code)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException($"code is invalid. code:{code} format:{MessagePackCode.ToFormatName(code)}");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_TypeCode(sbyte typeCode)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException($"typeCode is invalid. typeCode:{typeCode}");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_NotSupported(Type type)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Not supported primitive object resolver. type:" + type.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_NotSupport_Serialize(Type type)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException(type.Name + " does not support Serialize.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_NotSupport_Deserialize(Type type)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException(type.Name + " does not support Deserialize.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_InterfaceOrAbstract(Type actualType)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException($"Type '{actualType}' is an interface or abstract class and cannot be serialized.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Blacklist(Type type)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Type is in blacklist:" + type.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Register_Resolvers()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Register must call on startup(before use GetFormatter<T>).");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Not_Supported_Length()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Not Supported Length");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_EndPositionNotReached() { throw CreateInvalidOperationException_EndPositionNotReached(); }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateInvalidOperationException_EndPositionNotReached()
        {
            return new InvalidOperationException("EndPositionNotReached");
        }

        #endregion

        #region -- TinyJsonException --

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowTinyJsonException_Token()
        {
            throw GetException();
            TinyJsonException GetException()
            {
                return new TinyJsonException("Invalid Token");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowTinyJsonException_Json()
        {
            throw GetException();
            TinyJsonException GetException()
            {
                return new TinyJsonException("Invalid Json String");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowTinyJsonException_String(char c)
        {
            throw GetException();
            TinyJsonException GetException()
            {
                return new TinyJsonException($"Invalid String:{c}");
            }
        }

        #endregion

        #region -- ArgumentException --

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArgumentException_NeedMoreData()
        {
            throw GetException();
            ArgumentException GetException()
            {
                return new ArgumentException("Need more data.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArgumentException_DestinationTooShort()
        {
            throw GetException();
            ArgumentException GetException()
            {
                return new ArgumentException("Destination is too short.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArgumentException_Guid_Pattern()
        {
            throw GetArgumentException();
            ArgumentException GetArgumentException()
            {
                return new ArgumentException("Invalid Guid Pattern.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArgumentException_Key(string key)
        {
            throw GetArgumentException();
            ArgumentException GetArgumentException()
            {
                return new ArgumentException("Key was already exists. Key:" + key);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArgumentException_Key(byte[] key)
        {
            throw GetArgumentException();
            ArgumentException GetArgumentException()
            {
                return new ArgumentException("Key was already exists. Key:" + Encoding.UTF8.GetString(key));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArgumentException_FailedToGetLargerSpan()
        {
            throw GetArgumentOutOfRangeException();

            ArgumentException GetArgumentOutOfRangeException()
            {
                return new ArgumentException("The 'IBufferWriter' could not provide an output buffer that is large enough to continue writing.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArgumentException_FailedToGetMinimumSizeSpan(int minimumSize)
        {
            throw GetArgumentOutOfRangeException();

            ArgumentException GetArgumentOutOfRangeException()
            {
                return new ArgumentException($"The 'IBufferWriter' could not provide an output buffer that is large enough to continue writing. Need at least {minimumSize} bytes.");
            }
        }

        #endregion

        #region -- Exception --

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowException_Guid_Little_Endian()
        {
            throw GetException();
            Exception GetException()
            {
                return new Exception("BinaryGuidFormatter only allows on little endian env.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowException_Decimal_Little_Endian()
        {
            throw GetException();
            Exception GetException()
            {
                return new Exception("BinaryDecimalFormatter only allows on little endian env.");
            }
        }

        #endregion

        #region -- AmbiguousMatchException --

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowAmbiguousMatchException()
        {
            throw GetException();
            Exception GetException()
            {
                return new AmbiguousMatchException("More than one matching method found!");
            }
        }

        #endregion

        #region -- FormatterNotRegisteredException --

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowFormatterNotRegisteredException(Type type, IFormatterResolver formatterResolver)
        {
            throw GetException();
            Exception GetException()
            {
                return new FormatterNotRegisteredException(type.FullName + " is not registered in this resolver. resolver:" + formatterResolver.GetType().Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowFormatterNotRegisteredException(Type type, IFormatterResolver[] innerResolvers)
        {
            throw GetException();
            Exception GetException()
            {
                return new FormatterNotRegisteredException(type.FullName + " is not registered in this resolver. resolvers:" + string.Join(", ", innerResolvers.Select(x => x.GetType().Name).ToArray()));
            }
        }

        #endregion

        #region -- MessagePackDynamicUnionResolverException --

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicUnionResolverException_InterfaceOrAbstract(Type type)
        {
            throw GetException();
            MessagePackDynamicUnionResolverException GetException()
            {
                return new MessagePackDynamicUnionResolverException("Union can only be interface or abstract class. Type:" + type.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicUnionResolverException_Key(Type type, UnionAttribute item)
        {
            throw GetException();
            MessagePackDynamicUnionResolverException GetException()
            {
                return new MessagePackDynamicUnionResolverException("Same union key has found. Type:" + type.Name + " Key:" + item.Key);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicUnionResolverException_Subtype(Type type, UnionAttribute item)
        {
            throw GetException();
            MessagePackDynamicUnionResolverException GetException()
            {
                return new MessagePackDynamicUnionResolverException("Same union subType has found. Type:" + type.Name + " SubType: " + item.SubType);
            }
        }

        #endregion

        #region -- MessagePackDynamicObjectResolverException --

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Property_DataMem(Type type, PropertyInfo item)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("all public members must mark DataMemberAttribute or IgnoreMemberAttribute." + " type: " + type.FullName + " member:" + item.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Property_Key(Type type, PropertyInfo item)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("all public members must mark KeyAttribute or IgnoreMemberAttribute." + " type: " + type.FullName + " member:" + item.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Property_KeyAreNull(Type type, PropertyInfo item)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("both IntKey and StringKey are null." + " type: " + type.FullName + " member:" + item.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Property_Same(Type type, PropertyInfo item)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("all members key type must be same." + " type: " + type.FullName + " member:" + item.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Property_Duplicated(Type type, PropertyInfo item)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("key is duplicated, all members key must be unique." + " type: " + type.FullName + " member:" + item.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Field_DataMem(Type type, FieldInfo item)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("all public members must mark DataMemberAttribute or IgnoreMemberAttribute." + " type: " + type.FullName + " member:" + item.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Field_Key(Type type, FieldInfo item)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("all public members must mark KeyAttribute or IgnoreMemberAttribute." + " type: " + type.FullName + " member:" + item.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Field_KeyAreNull(Type type, FieldInfo item)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("both IntKey and StringKey are null." + " type: " + type.FullName + " member:" + item.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Field_Same(Type type, FieldInfo item)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("all members key type must be same." + " type: " + type.FullName + " member:" + item.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Field_Duplicated(Type type, FieldInfo item)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("key is duplicated, all members key must be unique." + " type: " + type.FullName + " member:" + item.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Ctor_None(Type type)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("can't find public constructor. type:" + type.FullName);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Ctor_NonMatched(Type type)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("can't find matched constructor. type:" + type.FullName);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Ctor_ParamType_Mismatch(Type type, int ctorParamIndex, ParameterInfo item)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("can't find matched constructor parameter, parameterType mismatch. type:" + type.FullName + " parameterIndex:" + ctorParamIndex + " paramterType:" + item.ParameterType.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Ctor_Index_NotFound(Type type, int ctorParamIndex)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("can't find matched constructor parameter, index not found. type:" + type.FullName + " parameterIndex:" + ctorParamIndex);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Ctor_Duplicate_Matched(Type type, ParameterInfo item)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("duplicate matched constructor parameter name:" + type.FullName + " parameterName:" + item.Name + " paramterType:" + item.ParameterType.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Ctor_NonMatched_Param(Type type, ParameterInfo item)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("can't find matched constructor parameter, parameterType mismatch. type:" + type.FullName + " parameterName:" + item.Name + " paramterType:" + item.ParameterType.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowDynamicObjectResolverException_Ctor_Index_NotFound(Type type, ParameterInfo item)
        {
            throw GetException();
            MessagePackDynamicObjectResolverException GetException()
            {
                return new MessagePackDynamicObjectResolverException("can't find matched constructor parameter, index not found. type:" + type.FullName + " parameterName:" + item.Name);
            }
        }

        #endregion
    }
}
