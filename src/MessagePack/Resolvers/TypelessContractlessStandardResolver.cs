﻿using MessagePack.Formatters;

namespace MessagePack.Resolvers
{
    /// <summary>
    /// Embed c# type names for `object` typed fields/collection items
    /// Preserve c# DateTime timezone
    /// </summary>
    public sealed class TypelessContractlessStandardResolver : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new TypelessContractlessStandardResolver();

        static readonly IFormatterResolver[] resolvers = new[]
        {
            NativeDateTimeResolver.Instance, // Native c# DateTime format, preserving timezone
            BuiltinResolver.Instance, // Try Builtin
            AttributeFormatterResolver.Instance, // Try use [MessagePackFormatter]
            DynamicEnumResolver.Instance, // Try Enum
            DynamicGenericResolver.Instance, // Try Array, Tuple, Collection
            DynamicUnionResolver.Instance, // Try Union(Interface)
            DynamicObjectResolver.Instance, // Try Object
            DynamicContractlessObjectResolverAllowPrivate.Instance, // Serializes keys as strings
            TypelessObjectResolver.Instance
        };

        TypelessContractlessStandardResolver() { }

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                foreach (var item in resolvers)
                {
                    var f = item.GetFormatter<T>();
                    if (f != null)
                    {
                        formatter = f;
                        return;
                    }
                }
            }
        }
    }
}
