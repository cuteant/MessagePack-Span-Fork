using System;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Reflection;
using System.Text;
using Utf8Json.Internal;
using Utf8Json.Internal.Emit;

namespace Utf8Json
{
    #region -- ExceptionArgument --

    /// <summary>The convention for this enum is using the argument name as the enum name</summary>
    internal enum ExceptionArgument
    {
        array,
        assembly,
        buffer,
        destination,
        key,
        obj,
        s,
        str,
        source,
        type,
        types,
        value,
        values,
        valueFactory,
        name,
        item,
        options,
        list,
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
        expression,
    }

    #endregion

    #region -- ExceptionResource --

    /// <summary>The convention for this enum is using the resource name as the enum name</summary>
    internal enum ExceptionResource
    {
    }

    #endregion

    partial class ThrowHelper
    {
        #region -- Exception --

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowException_UnreachedCode()
        {
            throw GetException();
            Exception GetException()
            {
                return new Exception("unreached code.");
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowException_UnreachableCode()
        {
            throw GetException();
            Exception GetException()
            {
                return new Exception("Unreachable code.");
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowException_InvalidMode()
        {
            throw GetException();
            Exception GetException()
            {
                return new Exception("Invalid Mode.");
            }
        }

        #endregion

        #region -- ArgumentException --

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowException_Length()
        {
            throw GetException();
            ArgumentException GetException()
            {
                return new ArgumentException("length < newSize");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArgumentException_Guid_Pattern()
        {
            throw GetException();
            ArgumentException GetException()
            {
                return new ArgumentException("Invalid Guid Pattern.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArgumentException_Key(string key)
        {
            throw GetException();
            ArgumentException GetException()
            {
                return new ArgumentException("Key was already exists. Key:" + key);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowArgumentException_Key(byte[] key)
        {
            throw GetException();
            ArgumentException GetException()
            {
                return new ArgumentException("Key was already exists. Key:" + Encoding.UTF8.GetString(key));
            }
        }

        #endregion

        #region -- InvalidOperationException --

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
        internal static void ThrowInvalidOperationException_NotSupport_Value(float value)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("not support float value:" + value);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_NotSupport_Value(double value)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("not support double value:" + value);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Buffer()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("return buffer is not from pool");
            }
        }

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
        internal static void ThrowInvalidOperationException_Bool()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("value is not boolean.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Bool_T()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("value is not boolean(true).");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Bool_F()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("value is not boolean(false).");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_ParseJSON()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Can't parse JSON to Enum format.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_KeyValuePair()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Data is Nil, KeyValuePair can not be null.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Datetime(ArraySegment<byte> str)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("invalid datetime format. value:" + StringEncoding.UTF8.GetString(str.Array, str.Offset, str.Count));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Dict_Key<TKey>()
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException(typeof(TKey) + " does not support dictionary key deserialize.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Same(Type type, MetaMember member)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("same (custom)name is in type. Type:" + type.Name + " Name:" + member.Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_Ctor_DuplicateMatched(Type type, ParameterInfo item)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("duplicate matched constructor parameter name:" + type.FullName + " parameterName:" + item.Name + " paramterType:" + item.ParameterType.Name);
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
        internal static void ThrowInvalidOperationException_JsonToken_Dec(JsonToken token)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid Json Token for DecimalFormatter:" + token);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidOperationException_JsonToken(JsonToken token)
        {
            throw GetException();
            InvalidOperationException GetException()
            {
                return new InvalidOperationException("Invalid Json Token:" + token);
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

        #endregion

        #region -- FormatterNotRegisteredException --

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowFormatterNotRegisteredException<T>(IJsonFormatterResolver formatterResolver)
        {
            throw GetException();
            Exception GetException()
            {
                return new FormatterNotRegisteredException(typeof(T).FullName + " is not registered in this resolver. resolver:" + formatterResolver.GetType().Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowFormatterNotRegisteredException(Type type, IJsonFormatterResolver[] innerResolvers)
        {
            throw GetException();
            Exception GetException()
            {
                return new FormatterNotRegisteredException(type.FullName + " is not registered in this resolver. resolvers:" + string.Join(", ", innerResolvers.Select(x => x.GetType().Name).ToArray()));
            }
        }

        #endregion

        #region -- JsonParsingException --

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowJsonParsingException_Single()
        {
            throw GetException();
            JsonParsingException GetException()
            {
                return new JsonParsingException("Can not find end token of single line comment(\r or \n).");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowJsonParsingException_Multi()
        {
            throw GetException();
            JsonParsingException GetException()
            {
                return new JsonParsingException("Can not find end token of multi line comment(*/).");
            }
        }

        #endregion
    }
}
