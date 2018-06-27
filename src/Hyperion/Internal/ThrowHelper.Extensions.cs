using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Hyperion
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
        args,
        typeId,
    }

    #endregion

    #region -- ExceptionResource --

    /// <summary>The convention for this enum is using the resource name as the enum name</summary>
    internal enum ExceptionResource
    {
        Variable_Not_Found,
        Parameter_Count_Mismatch,
    }

    #endregion

    partial class ThrowHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowException_Variable()
        {
            throw GetException();
            Exception GetException()
            {
                return new Exception("Variable not found.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowException_Method(MethodInfo method)
        {
            throw GetException();
            ArgumentException GetException()
            {
                return new ArgumentException($"Method {method.Name} should be static.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowException_Method(MethodInfo method, Type expectedReturnType)
        {
            throw GetException();
            ArgumentException GetException()
            {
                return new ArgumentException($"Method {method.Name} should return {expectedReturnType.Name}.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowException_Tracking(Exception x)
        {
            throw GetException();
            Exception GetException()
            {
                return new Exception("Error tracking object ", x);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowNotSupportedException_StackDepth()
        {
            throw GetException();
            NotSupportedException GetException()
            {
                return new NotSupportedException("Stack depth can not be less than 0");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowEndOfStreamException()
        {
            throw GetException();
            EndOfStreamException GetException()
            {
                return new EndOfStreamException();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowInvalidDataException()
        {
            throw GetException();
            InvalidDataException GetException()
            {
                return new InvalidDataException();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowNotSupportedException()
        {
            throw GetException();
            NotSupportedException GetException()
            {
                return new NotSupportedException();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowAmbiguousMatchException()
        {
            throw GetException();
            AmbiguousMatchException GetException()
            {
                return new AmbiguousMatchException("More than one matching method found!");
            }
        }
    }
}
