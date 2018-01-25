using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace MessagePack.Formatters
{
    public sealed class MemberAssignmentFormatter : MemberBindingFormatter<MemberAssignment>
    {
        public static readonly IMessagePackFormatter<MemberAssignment> Instance = new MemberAssignmentFormatter();
    }
    public sealed class MemberListBindingFormatter : MemberBindingFormatter<MemberListBinding>
    {
        public static readonly IMessagePackFormatter<MemberListBinding> Instance = new MemberListBindingFormatter();
    }
    public sealed class MemberMemberBindingFormatter : MemberBindingFormatter<MemberMemberBinding>
    {
        public static readonly IMessagePackFormatter<MemberMemberBinding> Instance = new MemberMemberBindingFormatter();
    }
    public sealed class MemberBindingFormatter : MemberBindingFormatter<MemberBinding>
    {
        public static readonly IMessagePackFormatter<MemberBinding> Instance = new MemberBindingFormatter();
    }

    public class MemberBindingFormatter<TExpression> : DynamicObjectTypeFormatterBase<TExpression>, IMessagePackFormatter<TExpression>
        where TExpression : MemberBinding
    {
        public MemberBindingFormatter() : base(null, ExpressionFieldInfoComparer.Instance) { }

        private static readonly Type ExpressionType = typeof(MemberBinding);

        /// <summary>Field comparer which sorts fields on the Expression class higher than fields on sub classes.</summary>
        private sealed class ExpressionFieldInfoComparer : IComparer<FieldInfo>
        {
            /// <summary>Gets the singleton instance of this class.</summary>
            public static ExpressionFieldInfoComparer Instance { get; } = new ExpressionFieldInfoComparer();

            public int Compare(FieldInfo left, FieldInfo right)
            {
                var l = left?.DeclaringType == ExpressionType ? 1 : 0;
                var r = right?.DeclaringType == ExpressionType ? 1 : 0;

                // First compare based on whether or not the field is from the Expression base class.
                var compareBaseClass = r - l;
                if (compareBaseClass != 0) return compareBaseClass;

                // Secondarily compare the field names.
                return string.Compare(left?.Name, right?.Name, StringComparison.Ordinal);
            }
        }
    }
}
