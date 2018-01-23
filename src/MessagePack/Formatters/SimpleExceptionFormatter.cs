using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CuteAnt;
using CuteAnt.Collections;
using CuteAnt.Reflection;

namespace MessagePack.Formatters
{
    public sealed class SimpleExceptionFormatter : SimpleExceptionFormatter<Exception>
    {
        public static readonly IMessagePackFormatter<Exception> Instance = new SimpleExceptionFormatter();

    }
    public class SimpleExceptionFormatter<TException> : IMessagePackFormatter<TException>
        where TException : Exception
    {
        public TException Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            var startOffset = offset;
            var hashCode = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
            offset += readSize;
            var typeName = MessagePackBinary.ReadBytes(bytes, offset, out readSize);
            offset += readSize;
            var actualType = TypeSerializer.GetTypeFromTypeKey(new TypeKey(hashCode, typeName), true);
            var exc = ActivatorUtils.FastCreateInstance(actualType);
            var fields = s_filedCache.GetOrAdd(actualType, s_getFieldsFunc);
            foreach (var (field, getter, setter) in fields)
            {
                var hasValue = MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
                offset += readSize;
                if (hasValue)
                {
                    var v = TypelessFormatter.Instance.Deserialize(bytes, offset, formatterResolver, out readSize);
                    offset += readSize;
                    setter(exc, v);
                }
            }
            readSize = offset - startOffset;
            return (TException)exc;
        }

        public int Serialize(ref byte[] bytes, int offset, TException value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var startOffset = offset;
            var actualType = value.GetType();
            var typeKey = TypeSerializer.GetTypeKeyFromType(actualType);
            offset += MessagePackBinary.WriteInt32(ref bytes, offset, typeKey.HashCode);
            var typeName = typeKey.TypeName;
            offset += MessagePackBinary.WriteBytes(ref bytes, offset, typeName, 0, typeName.Length);

            var fields = s_filedCache.GetOrAdd(actualType, s_getFieldsFunc);
            foreach (var (field, getter, setter) in fields)
            {
                var v = getter(value);
                if (v != null)
                {
                    offset += MessagePackBinary.WriteBoolean(ref bytes, offset, true);
                    offset += TypelessFormatter.Instance.Serialize(ref bytes, offset, getter(value), formatterResolver);
                }
                else
                {
                    offset += MessagePackBinary.WriteBoolean(ref bytes, offset, false);
                }
            }

            return offset - startOffset;
        }

        private static readonly CachedReadConcurrentDictionary<Type, List<(FieldInfo field, MemberGetter getter, MemberSetter setter)>> s_filedCache =
            new CachedReadConcurrentDictionary<Type, List<(FieldInfo field, MemberGetter getter, MemberSetter setter)>>();
        private static readonly Func<Type, List<(FieldInfo field, MemberGetter getter, MemberSetter setter)>> s_getFieldsFunc = GetFields;
        /// <summary>Returns a sorted list of the fields of the provided type.</summary>
        /// <param name="type">The type.</param>
        /// <returns>A sorted list of the fields of the provided type.</returns>
        private static List<(FieldInfo field, MemberGetter getter, MemberSetter setter)> GetFields(Type type)
        {
            var result =
                GetAllFields(type)
                    .Where(
                        field =>
                            !field.IsStatic
                            && IsSupportedFieldType(field.FieldType)
                            && ExceptionFieldFilter(field))
                    .ToList();
            result.Sort(ExceptionFieldInfoComparer.Instance);
            return result.Select(_ => (_, _.GetValueGetter(), _.GetValueSetter())).ToList();
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            const BindingFlags AllFields =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            var current = type;
            while ((current != TypeConstants.ObjectType) && (current != null))
            {
                var fields = current.GetFields(AllFields);
                foreach (var field in fields)
                {
                    yield return field;
                }

                current = current.GetTypeInfo().BaseType;
            }
        }

        /// <summary>Exceptions are a special type in .NET because of the way they are handled by the runtime.
        /// Only certain fields can be safely serialized.</summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private static bool ExceptionFieldFilter(FieldInfo field)
        {
            // Any field defined below Exception is acceptable.
            if (field.DeclaringType != TypeConstants.ExceptionType) return true;

            // Certain fields from the Exception base class are acceptable.
            return field.FieldType == TypeConstants.StringType || field.FieldType == TypeConstants.ExceptionType;
        }

        private static readonly RuntimeTypeHandle IntPtrTypeHandle = typeof(IntPtr).TypeHandle;
        private static readonly RuntimeTypeHandle UIntPtrTypeHandle = typeof(UIntPtr).TypeHandle;
        private static readonly Type DelegateType = typeof(Delegate);
        /// <summary>Returns a value indicating whether the provided type is supported as a field by this class.</summary>
        /// <param name="type">The type.</param>
        /// <returns>A value indicating whether the provided type is supported as a field by this class.</returns>
        private static bool IsSupportedFieldType(Type type)
        {
            if (type.IsPointer || type.IsByRef) return false;

            var handle = type.TypeHandle;
            if (handle.Equals(IntPtrTypeHandle)) return false;
            if (handle.Equals(UIntPtrTypeHandle)) return false;
            if (DelegateType.IsAssignableFrom(type)) return false;

            return true;
        }

        /// <summary>Field comparer which sorts fields on the Exception class higher than fields on sub classes.</summary>
        private sealed class ExceptionFieldInfoComparer : IComparer<FieldInfo>
        {
            /// <summary>Gets the singleton instance of this class.</summary>
            public static ExceptionFieldInfoComparer Instance { get; } = new ExceptionFieldInfoComparer();

            public int Compare(FieldInfo left, FieldInfo right)
            {
                var l = left?.DeclaringType == TypeConstants.ExceptionType ? 1 : 0;
                var r = right?.DeclaringType == TypeConstants.ExceptionType ? 1 : 0;

                // First compare based on whether or not the field is from the Exception base class.
                var compareBaseClass = r - l;
                if (compareBaseClass != 0) return compareBaseClass;

                // Secondarily compare the field names.
                return string.Compare(left?.Name, right?.Name, StringComparison.Ordinal);
            }
        }
    }
}
