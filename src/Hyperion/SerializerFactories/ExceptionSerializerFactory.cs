#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ExceptionSerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CuteAnt;
using CuteAnt.Collections;
using CuteAnt.Reflection;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    internal sealed class ExceptionSerializerFactory : ValueSerializerFactory
    {
#if NET40
        private static readonly Type ExceptionTypeInfo = typeof(Exception);
#else
        private static readonly TypeInfo ExceptionTypeInfo = typeof(Exception).GetTypeInfo();
#endif
        /// <summary>The field filter used for generating serializers for subclasses of <see cref="Exception"/>.</summary>
        private static readonly Func<FieldInfo, bool> _exceptionFieldFilter;

        static ExceptionSerializerFactory()
        {
            // Exceptions are a special type in .NET because of the way they are handled by the runtime.
            // Only certain fields can be safely serialized.
            _exceptionFieldFilter = field =>
            {
                // Any field defined below Exception is acceptable.
                if (field.DeclaringType != TypeConstants.ExceptionType) return true;

                // Certain fields from the Exception base class are acceptable.
                return field.FieldType == TypeConstants.StringType || field.FieldType == TypeConstants.ExceptionType;
            };
        }
        public ExceptionSerializerFactory()
        {
        }

#if NET40
        public override bool CanSerialize(Serializer serializer, Type type) => ExceptionTypeInfo.IsAssignableFrom(type);
#else
        public override bool CanSerialize(Serializer serializer, Type type) => ExceptionTypeInfo.IsAssignableFrom(type.GetTypeInfo());
#endif

        public override bool CanDeserialize(Serializer serializer, Type type) => CanSerialize(serializer, type);

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
          CachedReadConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var exceptionSerializer = new ObjectSerializer(type);

            exceptionSerializer.Initialize((stream, session) =>
            {
                var exception = ActivatorUtils.FastCreateInstance(type);
                var sessionSerializer = session.Serializer;
                if (sessionSerializer.Options.PreserveObjectReferences)
                {
                    session.TrackDeserializedObject(exception);
                }

                var fields = s_filedCache.GetOrAdd(type, s_getFieldsFunc);
                foreach (var (field, getter, setter) in fields)
                {
                    var fieldType = field.FieldType;
                    object fieldValue;
                    if (!sessionSerializer.Options.VersionTolerance && fieldType.IsHyperionPrimitive())
                    {
                        var valueSerializer = sessionSerializer.GetSerializerByType(fieldType);
                        fieldValue = valueSerializer.ReadValue(stream, session);
                    }
                    else
                    {
                        var valueType = fieldType;
                        if (fieldType.IsNullableType())
                        {
                            valueType = Nullable.GetUnderlyingType(fieldType);
                        }
                        var valueSerializer = sessionSerializer.GetSerializerByType(valueType);
                        fieldValue = stream.ReadObject(session);
                    }
                    setter(exception, fieldValue);
                }

                return exception;
            }, (stream, exception, session) =>
            {
                var sessionSerializer = session.Serializer;
                if (sessionSerializer.Options.PreserveObjectReferences)
                {
                    session.TrackSerializedObject(exception);
                }
                var fields = s_filedCache.GetOrAdd(type,s_getFieldsFunc);
                foreach (var (field, getter, setter) in fields)
                {
                    var fieldType = field.FieldType;
                    var v = getter(exception);
                    //if the type is one of our special primitives, ignore manifest as the content will always only be of this type
                    if (!sessionSerializer.Options.VersionTolerance && fieldType.IsHyperionPrimitive())
                    {
                        var valueSerializer = sessionSerializer.GetSerializerByType(fieldType);
                        valueSerializer.WriteValue(stream, v, session);
                    }
                    else
                    {
                        var valueType = fieldType;
                        if (fieldType.IsNullableType())
                        {
                            valueType = Nullable.GetUnderlyingType(fieldType);
                        }
                        var valueSerializer = sessionSerializer.GetSerializerByType(valueType);
                        stream.WriteObject(v, valueType, valueSerializer, true, session);
                    }
                }
            });
            typeMapping.TryAdd(type, exceptionSerializer);
            return exceptionSerializer;
        }

        private static readonly CachedReadConcurrentDictionary<Type, List<(FieldInfo field, MemberGetter getter, MemberSetter setter)>> s_filedCache =
            new CachedReadConcurrentDictionary<Type, List<(FieldInfo field, MemberGetter getter, MemberSetter setter)>>();
        private static readonly Func<Type, List<(FieldInfo field, MemberGetter getter, MemberSetter setter)>> s_getFieldsFunc = GetFields;

        private static List<(FieldInfo field, MemberGetter getter, MemberSetter setter)> GetFields(Type type)
        {
            var result = type.GetFieldInfosForType(_exceptionFieldFilter, ExceptionFieldInfoComparer.Instance);
            return result.Select(_ => (_, _.GetValueGetter(), _.GetValueSetter())).ToList();
        }

        /// <summary>Field comparer which sorts fields on the Exception class higher than fields on sub classes.</summary>
        private class ExceptionFieldInfoComparer : IComparer<FieldInfo>
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