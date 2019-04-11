namespace MessagePack.Resolvers
{
    using System;
    using MessagePack.Formatters;
    using MessagePack.Internal;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
#if DEPENDENT_ON_CUTEANT
    using CuteAnt.Reflection;
#endif

    /// <summary>EnumResolver by dynamic code generation, serialized underlying type.</summary>
    public sealed class DynamicEnumResolver : FormatterResolver
    {
        public static readonly DynamicEnumResolver Instance = new DynamicEnumResolver();

        const string ModuleName = "MessagePack.Resolvers.DynamicEnumResolver";

        static readonly DynamicAssembly assembly;

        static int nameSequence = 0;

        DynamicEnumResolver() { }

        static DynamicEnumResolver()
        {
            assembly = new DynamicAssembly(ModuleName);
        }


#if NETFRAMEWORK
        public AssemblyBuilder Save()
        {
            return assembly.Save();
        }
#endif

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                var ti = typeof(T);
                if (ti.IsNullable())
                {
                    // build underlying type and use wrapped formatter.
                    ti = ti.GenericTypeArguments[0];
                    if (!ti.IsEnum)
                    {
                        return;
                    }

                    var innerFormatter = DynamicEnumResolver.Instance.GetFormatterDynamic(ti);
                    if (innerFormatter == null)
                    {
                        return;
                    }
                    formatter = (IMessagePackFormatter<T>)ActivatorUtils.CreateInstance(typeof(StaticNullableFormatter<>).GetCachedGenericType(ti), new object[] { innerFormatter });
                    return;
                }
                else if (!ti.IsEnum)
                {
                    return;
                }

                var formatterTypeInfo = BuildType(typeof(T));
                formatter = (IMessagePackFormatter<T>)ActivatorUtils.FastCreateInstance(formatterTypeInfo.AsType());
            }
        }

        static TypeInfo BuildType(Type enumType)
        {
            var underlyingType = Enum.GetUnderlyingType(enumType);
            var formatterType = typeof(IMessagePackFormatter<>).GetCachedGenericType(enumType);

            var typeBuilder = assembly.DefineType("MessagePack.Formatters." + enumType.FullName.Replace(".", "_") + "Formatter" + Interlocked.Increment(ref nameSequence), TypeAttributes.Public | TypeAttributes.Sealed, null, new[] { formatterType });

            // void Serialize(ref MessagePackWriter writer, ref int idx, T value, IFormatterResolver formatterResolver);
            {
                var method = typeBuilder.DefineMethod("Serialize", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
                    typeof(void),
                    new Type[] { typeof(MessagePackWriter).MakeByRefType(), typeof(int).MakeByRefType(), enumType, typeof(IFormatterResolver) });

                var il = method.GetILGenerator();
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_3);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Call, typeof(MessagePackWriterExtensions).GetRuntimeMethod("Write" + underlyingType.Name, new[] { typeof(MessagePackWriter).MakeByRefType(), underlyingType, typeof(int).MakeByRefType() }));
                il.Emit(OpCodes.Ret);
            }

            // T Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver);
            {
                var method = typeBuilder.DefineMethod("Deserialize", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
                    enumType,
                    new Type[] { typeof(MessagePackReader).MakeByRefType(), typeof(IFormatterResolver) });

                var il = method.GetILGenerator();
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, typeof(MessagePackReader).GetRuntimeMethod("Read" + underlyingType.Name, Type.EmptyTypes));
                il.Emit(OpCodes.Ret);
            }

            return typeBuilder.CreateTypeInfo();
        }
    }
}
