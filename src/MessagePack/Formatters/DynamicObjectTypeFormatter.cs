using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CuteAnt;
using CuteAnt.Collections;
using CuteAnt.Reflection;

namespace MessagePack.Formatters
{
    public abstract class DynamicObjectTypeFormatterBase<T> : IMessagePackFormatter<T>
    {
        private static readonly Func<FieldInfo, bool> s_defaultFieldFilter = f => true;
        private readonly IComparer<FieldInfo> _fieldInfoComparer;
        private readonly Func<FieldInfo, bool> _fieldFilter;

        protected DynamicObjectTypeFormatterBase(Func<FieldInfo, bool> fieldFilter = null, IComparer<FieldInfo> fieldInfoComparer = null)
        {
            _fieldFilter = fieldFilter ?? s_defaultFieldFilter;
            _fieldInfoComparer = fieldInfoComparer ?? FieldInfoComparer.Instance;
        }

        public T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            return (T)DeserializeObject(bytes, offset, formatterResolver, out readSize);
        }

        public int Serialize(ref byte[] bytes, int offset, T value, IFormatterResolver formatterResolver)
        {
            return SerializeObject(ref bytes, offset, value, formatterResolver);
        }

        protected virtual object DeserializeObject(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            var startOffset = offset;

            var actualType = MessagePackBinary.ReadNamedType(bytes, offset, out readSize, true);
            offset += readSize;
            var obj = ActivatorUtils.FastCreateInstance(actualType);

            var fields = s_filedCache.GetOrAdd(actualType, s_getFieldsFunc, _fieldFilter, _fieldInfoComparer);
            foreach (var (field, getter, setter) in fields)
            {
                var fieldFormatter = s_objectFormatterCache.GetOrAdd(field.FieldType, s_createObjectTypeFormatterFunc);
                var fieldValue = fieldFormatter.Deserialize(bytes, offset, formatterResolver, out readSize);
                offset += readSize;
                setter(obj, fieldValue);
            }

            readSize = offset - startOffset;
            return obj;
        }

        protected virtual int SerializeObject(ref byte[] bytes, int offset, object value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var actualType = value.GetType();
            if (!IsSupportedType(actualType))
            {
                throw new InvalidOperationException($"Type '{actualType}' is an interface or abstract class and cannot be serialized.");
            }

            var startOffset = offset;

            offset += MessagePackBinary.WriteNamedType(ref bytes, offset, actualType);

            var fields = s_filedCache.GetOrAdd(actualType, s_getFieldsFunc, _fieldFilter, _fieldInfoComparer);
            foreach (var (field, getter, setter) in fields)
            {
                var fieldFormatter = s_objectFormatterCache.GetOrAdd(field.FieldType, s_createObjectTypeFormatterFunc);
                var v = getter(value);
                offset += fieldFormatter.Serialize(ref bytes, offset, v, formatterResolver);
            }

            return offset - startOffset;
        }

        #region ++ IsSupportedType ++

        /// <summary>Returns a value indicating whether the provided <paramref name="type"/> is supported.</summary>
        /// <param name="type">The type.</param>
        /// <returns>A value indicating whether the provided <paramref name="type"/> is supported.</returns>
        protected bool IsSupportedType(Type type)
        {
            return !type.IsAbstract && !type.IsInterface && !type.IsArray && !type.IsEnum && IsSupportedFieldType(type);
        }

        #endregion

        #region ++ GetObjectTypeFormatter ++

        private static readonly CachedReadConcurrentDictionary<Type, IDynamicObjectTypeFormatter> s_objectFormatterCache =
            new CachedReadConcurrentDictionary<Type, IDynamicObjectTypeFormatter>(DictionaryCacheConstants.SIZE_MEDIUM);
        private static readonly Func<Type, IDynamicObjectTypeFormatter> s_createObjectTypeFormatterFunc = CreateObjectTypeFormatter;

        private static IDynamicObjectTypeFormatter CreateObjectTypeFormatter(Type type)
        {
            var formatterType = typeof(DynamicObjectTypeFormatter<>).GetCachedGenericType(type);
            return ActivatorUtils.FastCreateInstance<IDynamicObjectTypeFormatter>(formatterType);
        }

        protected static IDynamicObjectTypeFormatter GetObjectTypeFormatter(Type type)
        {
            if (null == type) { throw new ArgumentNullException(nameof(type)); }
            return s_objectFormatterCache.GetOrAdd(type, s_createObjectTypeFormatterFunc);
        }

        #endregion

        #region ++ GetFields ++

        private readonly CachedReadConcurrentDictionary<Type, List<(FieldInfo field, MemberGetter getter, MemberSetter setter)>> s_filedCache =
            new CachedReadConcurrentDictionary<Type, List<(FieldInfo field, MemberGetter getter, MemberSetter setter)>>();
        private readonly Func<Type, Func<FieldInfo, bool>, IComparer<FieldInfo>, List<(FieldInfo field, MemberGetter getter, MemberSetter setter)>> s_getFieldsFunc = GetFields;
        /// <summary>Returns a sorted list of the fields of the provided type.</summary>
        /// <param name="type">The type.</param>
        /// <param name="fieldFilter">The predicate used in addition to the default logic to select which fields are included.</param>
        /// <param name="fieldInfoComparer">The comparer used to sort fields.</param>
        /// <returns>A sorted list of the fields of the provided type.</returns>
        private static List<(FieldInfo field, MemberGetter getter, MemberSetter setter)> GetFields(Type type,
            Func<FieldInfo, bool> fieldFilter, IComparer<FieldInfo> fieldInfoComparer)
        {
            var result =
                GetAllFields(type)
                    .Where(
                        field =>
                            !field.IsStatic
                            && IsSupportedFieldType(field.FieldType)
                            && fieldFilter(field))
                    .ToList();
            result.Sort(fieldInfoComparer);
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

        #endregion

        #region ** class FieldInfoComparer **

        /// <summary>A comparer for <see cref="FieldInfo"/> which compares by name.</summary>
        private sealed class FieldInfoComparer : IComparer<FieldInfo>
        {
            /// <summary>Gets the singleton instance of this class.</summary>
            public static readonly FieldInfoComparer Instance = new FieldInfoComparer();

            public int Compare(FieldInfo x, FieldInfo y)
            {
                return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            }
        }

        #endregion
    }

    public class DynamicObjectTypeFormatter<T> : IDynamicObjectTypeFormatter
    {
        public object Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            var formatter = formatterResolver.GetFormatterWithVerify<T>();
            return formatter.Deserialize(bytes, offset, formatterResolver, out readSize);
        }

        public int Serialize(ref byte[] bytes, int offset, object value, IFormatterResolver formatterResolver)
        {
            var formatter = formatterResolver.GetFormatterWithVerify<T>();
            return formatter.Serialize(ref bytes, offset, (T)value, formatterResolver);
        }
    }

    public interface IDynamicObjectTypeFormatter : IMessagePackFormatter
    {
        int Serialize(ref byte[] bytes, int offset, object value, IFormatterResolver formatterResolver);
        object Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize);
    }
}
