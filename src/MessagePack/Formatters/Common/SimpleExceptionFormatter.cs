using System;
using System.Collections.Generic;
using System.Reflection;
using CuteAnt;
using CuteAnt.Reflection;

namespace MessagePack.Formatters
{
    public sealed class SimpleExceptionFormatter : SimpleExceptionFormatter<Exception>
    {
        public static readonly IMessagePackFormatter<Exception> Instance = new SimpleExceptionFormatter();

    }
    public class SimpleExceptionFormatter<TException> : DynamicObjectTypeFormatterBase<TException>
        where TException : Exception
    {
        public SimpleExceptionFormatter() : base(ExceptionFieldFilter, ExceptionFieldInfoComparer.Instance) { }

        /// <inheritdoc />
        protected override object GetFieldValue(object obj, FieldInfo field, MemberGetter getter)
        {
            switch (field.Name)
            {
                case "_source":
                    return ((Exception)obj).Source;
                case "_stackTraceString":
                    return ((Exception)obj).StackTrace;
                default:
                    return base.GetFieldValue(obj, field, getter);
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
