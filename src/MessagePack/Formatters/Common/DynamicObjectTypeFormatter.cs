using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
#if DEPENDENT_ON_CUTEANT
using CuteAnt;
using CuteAnt.Collections;
using CuteAnt.Reflection;
#else
using MessagePack.Internal;
#endif

namespace MessagePack.Formatters
{
    using MessagePack.Formatters.Internal;

    public abstract class DynamicObjectTypeFormatterBase<T> : IMessagePackFormatter<T>
    {
        const int c_count = 2;

        private const string _syncRoot = "_syncRoot";
        private static readonly Func<FieldInfo, bool> DefaultFieldFilter = f =>
        {
            if (f.IsNotSerialized) { return false; }
            if (f.IsDefined(typeof(IgnoreDataMemberAttribute))) { return false; }
            if (f.IsDefined(typeof(IgnoreMemberAttribute))) { return false; }
            if (f.HasAttributeNamed("Ignore", false)) { return false; }

            return !string.Equals(_syncRoot, f.Name, StringComparison.Ordinal);
        };

        private readonly IComparer<FieldInfo> _fieldInfoComparer;
        private readonly Func<FieldInfo, bool> _fieldFilter;
        private readonly Func<Type, bool> _isSupportedFieldType;

        protected DynamicObjectTypeFormatterBase(Func<FieldInfo, bool> fieldFilter = null,
            IComparer<FieldInfo> fieldInfoComparer = null, Func<Type, bool> isSupportedFieldType = null)
        {
            _fieldFilter = (FieldInfo field) => DefaultFieldFilter(field) && (fieldFilter?.Invoke(field) ?? true);
            _fieldInfoComparer = fieldInfoComparer ?? FieldInfoComparer.Instance;
            _isSupportedFieldType = isSupportedFieldType ?? IsSupportedFieldType;
        }

        public virtual T Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return default; }

            var count = reader.ReadArrayHeader();
            if (count != c_count) { ThrowHelper.ThrowInvalidOperationException_DynamicObject_Format(); }

            var actualType = reader.ReadNamedType(true);
            var obj = ActivatorUtils.FastCreateInstance(actualType);

            var fields = _filedCache.GetOrAdd(actualType, s_getFieldsFunc, _fieldFilter, _fieldInfoComparer, _isSupportedFieldType);

            count = reader.ReadArrayHeader();
            if (count != fields.Count) { ThrowHelper.ThrowInvalidOperationException_DynamicObject_Format(); }

            foreach (var (field, getter, setter) in fields)
            {
                var fieldType = field.FieldType;
                object fieldValue;
                if (fieldType != TypeConstants.ObjectType)
                {
                    var fieldFormatter = DynamicObjectTypeFormatter.GetObjectTypeFormatter(fieldType);
                    fieldValue = fieldFormatter.Deserialize(ref reader, formatterResolver);
                }
                else
                {
                    fieldValue = MessagePackSerializer.Typeless.TypelessFormatter.Deserialize(ref reader, formatterResolver);
                }
                setter(obj, fieldValue);
            }

            return (T)obj;
        }

        public virtual void Serialize(ref MessagePackWriter writer, ref int idx, T value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var actualType = value.GetType();
            if (!IsSupportedType(actualType))
            {
                ThrowHelper.ThrowInvalidOperationException_InterfaceOrAbstract(actualType);
            }

            writer.WriteFixedArrayHeaderUnsafe(c_count, ref idx);

            writer.WriteNamedType(actualType, ref idx);

            var fields = _filedCache.GetOrAdd(actualType, s_getFieldsFunc, _fieldFilter, _fieldInfoComparer, _isSupportedFieldType);

            writer.WriteArrayHeader(fields.Count, ref idx);

            foreach (var (field, getter, setter) in fields)
            {
                var fieldType = field.FieldType;
                var v = GetFieldValue(value, field, getter);
                if (fieldType != TypeConstants.ObjectType)
                {
                    var fieldFormatter = DynamicObjectTypeFormatter.GetObjectTypeFormatter(fieldType);
                    fieldFormatter.Serialize(ref writer, ref idx, v, formatterResolver);
                }
                else
                {
                    MessagePackSerializer.Typeless.TypelessFormatter.Serialize(ref writer, ref idx, v, formatterResolver);
                }
            }
        }

        /// <summary>GetFieldValue</summary>
        /// <param name="obj"></param>
        /// <param name="field"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        protected virtual object GetFieldValue(object obj, FieldInfo field, MemberGetter getter)
        {
            return getter(obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected List<(FieldInfo field, MemberGetter getter, MemberSetter setter)> GetFieldsFromCache(Type type)
        {
            return _filedCache.GetOrAdd(type, s_getFieldsFunc, _fieldFilter, _fieldInfoComparer, _isSupportedFieldType); ;
        }

        #region ++ IsSupportedType ++

        /// <summary>Returns a value indicating whether the provided <paramref name="type"/> is supported.</summary>
        /// <param name="type">The type.</param>
        /// <returns>A value indicating whether the provided <paramref name="type"/> is supported.</returns>
        protected bool IsSupportedType(Type type)
        {
            return !type.IsAbstract && !type.IsInterface && !type.IsArray && !type.IsEnum && _isSupportedFieldType(type);
        }

        #endregion

        #region ++ GetFields ++

        private readonly CachedReadConcurrentDictionary<Type, List<(FieldInfo field, MemberGetter getter, MemberSetter setter)>> _filedCache =
            new CachedReadConcurrentDictionary<Type, List<(FieldInfo field, MemberGetter getter, MemberSetter setter)>>();
        private static readonly Func<Type, Func<FieldInfo, bool>, IComparer<FieldInfo>, Func<Type, bool>, List<(FieldInfo field, MemberGetter getter, MemberSetter setter)>> s_getFieldsFunc = GetFields;
        /// <summary>Returns a sorted list of the fields of the provided type.</summary>
        /// <param name="type">The type.</param>
        /// <param name="fieldFilter">The predicate used in addition to the default logic to select which fields are included.</param>
        /// <param name="fieldInfoComparer">The comparer used to sort fields.</param>
        /// <param name="isSupportedFieldType"></param>
        /// <returns>A sorted list of the fields of the provided type.</returns>
        private static List<(FieldInfo field, MemberGetter getter, MemberSetter setter)> GetFields(Type type,
            Func<FieldInfo, bool> fieldFilter, IComparer<FieldInfo> fieldInfoComparer, Func<Type, bool> isSupportedFieldType)
        {
            var result =
                GetAllFields(type)
                    .Where(
                        field =>
                            !field.IsStatic
                            && isSupportedFieldType(field.FieldType)
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

                current = current.BaseType;
            }
        }

        private static readonly RuntimeTypeHandle IntPtrTypeHandle = typeof(IntPtr).TypeHandle;
        private static readonly RuntimeTypeHandle UIntPtrTypeHandle = typeof(UIntPtr).TypeHandle;
        //private static readonly Type DelegateType = typeof(Delegate);
        /// <summary>Returns a value indicating whether the provided type is supported as a field by this class.</summary>
        /// <param name="type">The type.</param>
        /// <returns>A value indicating whether the provided type is supported as a field by this class.</returns>
        private static bool IsSupportedFieldType(Type type)
        {
            if (type.IsPointer || type.IsByRef) return false;

            var handle = type.TypeHandle;
            if (handle.Equals(IntPtrTypeHandle)) return false;
            if (handle.Equals(UIntPtrTypeHandle)) return false;
            //if (DelegateType.IsAssignableFrom(type)) return false;

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
}

namespace MessagePack.Formatters.Internal
{
    public static class DynamicObjectTypeFormatter
    {
        private static readonly CachedReadConcurrentDictionary<Type, IDynamicObjectTypeFormatter> s_objectFormatterCache =
            new CachedReadConcurrentDictionary<Type, IDynamicObjectTypeFormatter>(DictionaryCacheConstants.SIZE_MEDIUM);
        private static readonly Func<Type, IDynamicObjectTypeFormatter> s_createObjectTypeFormatterFunc = CreateObjectTypeFormatter;

        private static IDynamicObjectTypeFormatter CreateObjectTypeFormatter(Type type)
        {
            if (null == type) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.type); }

            var formatterType = typeof(DynamicObjectTypeFormatter<>).GetCachedGenericType(type);
            return ActivatorUtils.FastCreateInstance<IDynamicObjectTypeFormatter>(formatterType);
        }

        public static IDynamicObjectTypeFormatter GetObjectTypeFormatter(Type type)
        {
            if (null == type) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.type); }

            return s_objectFormatterCache.GetOrAdd(type, s_createObjectTypeFormatterFunc);
        }
    }

    public class DynamicObjectTypeFormatter<T> : IDynamicObjectTypeFormatter
    {
        public object Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            var formatter = formatterResolver.GetFormatterWithVerify<T>();
            return formatter.Deserialize(ref reader, formatterResolver);
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, object value, IFormatterResolver formatterResolver)
        {
            var formatter = formatterResolver.GetFormatterWithVerify<T>();
            formatter.Serialize(ref writer, ref idx, (T)value, formatterResolver);
        }
    }

    public interface IDynamicObjectTypeFormatter : IMessagePackFormatter
    {
        void Serialize(ref MessagePackWriter writer, ref int idx, object value, IFormatterResolver formatterResolver);
        object Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver);
    }
}
;