#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ReflectionEx.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using CuteAnt;

namespace Hyperion.Extensions
{
    internal static class BindingFlagsEx
    {
        public const BindingFlags All = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    }

    internal static class ReflectionEx
    {
        public static readonly Assembly CoreAssembly = typeof(int).GetTypeInfo().Assembly;

        public static FieldInfo[] GetFieldInfosForType(this Type type,
            Func<FieldInfo, bool> serializationFieldsFilter = null,
            IComparer<FieldInfo> fieldInfoComparer = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            //            var fieldInfos = new List<FieldInfo>();
            //            var current = type;
            //            while (current != null)
            //            {
            //                var tfields =
            //                    current
            //#if !NET40
            //                .GetTypeInfo()
            //#endif
            //                .GetFields(BindingFlagsEx.All)
            //                        //#if SERIALIZATION
            //                        .Where(f => !f.IsDefined(typeof(NonSerializedAttribute)))
            //                        //#endif
            //                        .Where(f => !f.IsDefined(typeof(IgnoreDataMemberAttribute)))
            //                        .Where(f => !f.IsDefined(typeof(IgnoreAttribute)))
            //                        .Where(f => !f.IsStatic)
            //                        .Where(f => f.FieldType != typeof(IntPtr))
            //                        .Where(f => f.FieldType != typeof(UIntPtr))
            //                        .Where(f => f.Name != "_syncRoot"); //HACK: ignore these 

            //                fieldInfos.AddRange(tfields);
            //                current = current.GetTypeInfo().BaseType;
            //            }
            //            var fields = fieldInfos.OrderBy(f => f.Name).ToArray();
            //            return fields;
            bool fieldFilter(FieldInfo field) => DefaultFieldFilter(field) && (serializationFieldsFilter?.Invoke(field) ?? true);

            return GetFields(type, fieldFilter, fieldInfoComparer).ToArray();
        }

        /// <summary>Returns a sorted list of the fields of the provided type.</summary>
        /// <param name="type">The type.</param>
        /// <param name="fieldFilter">The predicate used in addition to the default logic to select which fields are included.</param>
        /// <param name="fieldInfoComparer">The comparer used to sort fields, or <see langword="null"/> to use the default.</param>
        /// <returns>A sorted list of the fields of the provided type.</returns>
        private static List<FieldInfo> GetFields(Type type,
            Func<FieldInfo, bool> fieldFilter = null,
            IComparer<FieldInfo> fieldInfoComparer = null)
        {
            var result =
                GetAllFields(type)
                    .Where(
                        field =>
                            !field.IsStatic
                            && IsSupportedFieldType(field.FieldType)
                            && (fieldFilter == null || fieldFilter(field)))
                    .ToList();
            result.Sort(fieldInfoComparer ?? FieldInfoComparer.Instance);
            return result;
        }

        private const string _syncRoot = "_syncRoot";
        private static bool DefaultFieldFilter(FieldInfo f)
        {
            if (f.IsNotSerialized) { return false; }
            if (f.IsDefined(typeof(IgnoreDataMemberAttribute))) { return false; }
            if (f.IsDefined(typeof(IgnoreAttribute))) { return false; }
            if (f.HasAttributeNamed("IgnoreMember", false)) { return false; }

            return !string.Equals(_syncRoot, f.Name, StringComparison.Ordinal);
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