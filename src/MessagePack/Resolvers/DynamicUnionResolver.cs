namespace MessagePack.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text.RegularExpressions;
    using System.Threading;
    using MessagePack.Formatters;
    using MessagePack.Internal;
#if DEPENDENT_ON_CUTEANT
    using CuteAnt.Reflection;
#endif

    /// <summary>UnionResolver by dynamic code generation.</summary>
    public sealed class DynamicUnionResolver : FormatterResolver
    {
        public static readonly DynamicUnionResolver Instance = new DynamicUnionResolver();

        const string ModuleName = "MessagePack.Resolvers.DynamicUnionResolver";

        static readonly DynamicAssembly assembly;
        static readonly Regex SubtractFullNameRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=\w+, PublicKeyToken=\w+", RegexOptions.Compiled);

        static int nameSequence = 0;

        DynamicUnionResolver() { }

        static DynamicUnionResolver() => assembly = new DynamicAssembly(ModuleName);

#if NETFRAMEWORK
        public AssemblyBuilder Save() => assembly.Save();
#endif

        public override IMessagePackFormatter<T> GetFormatter<T>() => FormatterCache<T>.formatter;

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                var ti = typeof(T);
                if (ti.IsNullable())
                {
                    ti = ti.GenericTypeArguments[0];

                    var innerFormatter = DynamicUnionResolver.Instance.GetFormatterDynamic(ti);
                    if (innerFormatter == null)
                    {
                        return;
                    }
                    formatter = (IMessagePackFormatter<T>)ActivatorUtils.CreateInstance(typeof(StaticNullableFormatter<>).GetCachedGenericType(ti), new object[] { innerFormatter });
                    return;
                }

                var formatterTypeInfo = BuildType(typeof(T));
                if (formatterTypeInfo == null) return;

                formatter = (IMessagePackFormatter<T>)ActivatorUtils.FastCreateInstance(formatterTypeInfo.AsType());
            }
        }

        static TypeInfo BuildType(Type type)
        {
            // order by key(important for use jump-table of switch)
#if DEPENDENT_ON_CUTEANT
            var unionAttrs = type.GetAllAttributes<UnionAttribute>().OrderBy(x => x.Key).ToArray();
#else
            var unionAttrs = type.GetCustomAttributes<UnionAttribute>().OrderBy(x => x.Key).ToArray();
#endif

            if (0u >= (uint)unionAttrs.Length) { return null; }
            if (!type.IsInterface && !type.IsAbstract)
            {
                ThrowHelper.ThrowDynamicUnionResolverException_InterfaceOrAbstract(type);
            }

            var checker1 = new HashSet<int>();
            var checker2 = new HashSet<Type>();
            foreach (var item in unionAttrs)
            {
                if (!checker1.Add(item.Key)) ThrowHelper.ThrowDynamicUnionResolverException_Key(type, item);
                if (!checker2.Add(item.SubType)) ThrowHelper.ThrowDynamicUnionResolverException_Subtype(type, item);
            }

            var formatterType = typeof(IMessagePackFormatter<>).GetCachedGenericType(type);
            var typeBuilder = assembly.DefineType("MessagePack.Formatters." + SubtractFullNameRegex.Replace(type.FullName, "").Replace(".", "_") + "Formatter" + Interlocked.Increment(ref nameSequence), TypeAttributes.Public | TypeAttributes.Sealed, null, new[] { formatterType });

            FieldBuilder typeToKeyAndJumpMap = null; // Dictionary<RuntimeTypeHandle, KeyValuePair<int, int>>
            FieldBuilder keyToJumpMap = null; // Dictionary<int, int>

            // create map dictionary
            {
                var method = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
                typeToKeyAndJumpMap = typeBuilder.DefineField("typeToKeyAndJumpMap", typeof(Dictionary<RuntimeTypeHandle, KeyValuePair<int, int>>), FieldAttributes.Private | FieldAttributes.InitOnly);
                keyToJumpMap = typeBuilder.DefineField("keyToJumpMap", typeof(Dictionary<int, int>), FieldAttributes.Private | FieldAttributes.InitOnly);

                var il = method.GetILGenerator();
                BuildConstructor(type, unionAttrs, method, typeToKeyAndJumpMap, keyToJumpMap, il);
            }

            {
                var method = typeBuilder.DefineMethod("Serialize", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
                    typeof(void),
                    new Type[] { s_refWriter, s_refInt, type, typeof(IFormatterResolver) });

                var il = method.GetILGenerator();
                BuildSerialize(type, unionAttrs, method, typeToKeyAndJumpMap, il);
            }
            {
                var method = typeBuilder.DefineMethod("Deserialize", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
                    type,
                    new Type[] { s_refReader, typeof(IFormatterResolver) });

                var il = method.GetILGenerator();
                BuildDeserialize(type, unionAttrs, method, keyToJumpMap, il);
            }

            return typeBuilder.CreateTypeInfo();
        }

        static void BuildConstructor(Type type, UnionAttribute[] infos, ConstructorInfo method, FieldBuilder typeToKeyAndJumpMap, FieldBuilder keyToJumpMap, ILGenerator il)
        {
            il.EmitLdarg(0);
            il.Emit(OpCodes.Call, s_objectCtor);

            {
                il.EmitLdarg(0);
                il.EmitLdc_I4(infos.Length);
                il.Emit(OpCodes.Ldsfld, s_runtimeTypeHandleEqualityComparer);
                il.Emit(OpCodes.Newobj, s_typeMapDictionaryConstructor);

                var index = 0;
                foreach (var item in infos)
                {
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Ldtoken, item.SubType);
                    il.EmitLdc_I4(item.Key);
                    il.EmitLdc_I4(index);
                    il.Emit(OpCodes.Newobj, s_intIntKeyValuePairConstructor);
                    il.EmitCall(s_typeMapDictionaryAdd);

                    index++;
                }

                il.Emit(OpCodes.Stfld, typeToKeyAndJumpMap);
            }
            {
                il.EmitLdarg(0);
                il.EmitLdc_I4(infos.Length);
                il.Emit(OpCodes.Newobj, s_keyMapDictionaryConstructor);

                var index = 0;
                foreach (var item in infos)
                {
                    il.Emit(OpCodes.Dup);
                    il.EmitLdc_I4(item.Key);
                    il.EmitLdc_I4(index);
                    il.EmitCall(s_keyMapDictionaryAdd);

                    index++;
                }
                il.Emit(OpCodes.Stfld, keyToJumpMap);
            }

            il.Emit(OpCodes.Ret);
        }


        // int Serialize([arg:1]ref MessagePackWriter writer, [arg:2]ref int idx, [arg:3]T value, [arg:4]IFormatterResolver formatterResolver);
        static void BuildSerialize(Type type, UnionAttribute[] infos, MethodBuilder method, FieldBuilder typeToKeyAndJumpMap, ILGenerator il)
        {
            // if(value == null) { writer.WriteNil(ref idx); return; }
            var elseBody = il.DefineLabel();
            var notFoundType = il.DefineLabel();

            il.EmitLdarg(3);
            il.Emit(OpCodes.Brtrue_S, elseBody);
            il.Emit(OpCodes.Br, notFoundType);
            il.MarkLabel(elseBody);

            var keyPair = il.DeclareLocal(typeof(KeyValuePair<int, int>));

            il.EmitLoadThis();
            il.EmitLdfld(typeToKeyAndJumpMap);
            il.EmitLdarg(3);
            il.EmitCall(s_objectGetType);
            il.EmitCall(s_getTypeHandle);
            il.EmitLdloca(keyPair);
            il.EmitCall(s_typeMapDictionaryTryGetValue);
            il.Emit(OpCodes.Brfalse, notFoundType);

            // writer.WriteFixedArrayHeaderUnsafe(2, ref idx);
            il.EmitLdarg(1);
            il.EmitLdc_I4(2);
            il.EmitLdarg(2);
            il.EmitCall(MessagePackBinaryTypeInfo.WriteFixedArrayHeaderUnsafe);

            // writer.WriteInt32(keyPair.Key, ref idx);
            il.EmitLdarg(1);
            il.EmitLdloca(keyPair);
            il.EmitCall(s_intIntKeyValuePairGetKey);
            il.EmitLdarg(2);
            il.EmitCall(MessagePackBinaryTypeInfo.WriteInt32);

            var loopEnd = il.DefineLabel();

            // switch-case (resolver.GetFormatter.Serialize(with cast)
            var switchLabels = infos.Select(x => new { Label = il.DefineLabel(), Attr = x }).ToArray();
            il.EmitLdloca(keyPair);
            il.EmitCall(s_intIntKeyValuePairGetValue);
            il.Emit(OpCodes.Switch, switchLabels.Select(x => x.Label).ToArray());
            il.Emit(OpCodes.Br, loopEnd); // default

            foreach (var item in switchLabels)
            {
                il.MarkLabel(item.Label);

                il.EmitLdarg(4);
                il.Emit(OpCodes.Call, s_getFormatterWithVerify.MakeGenericMethod(item.Attr.SubType));

                il.EmitLdarg(1);
                il.EmitLdarg(2);
                il.EmitLdarg(3);
                if (item.Attr.SubType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, item.Attr.SubType);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, item.Attr.SubType);
                }
                il.EmitLdarg(4);
                il.Emit(OpCodes.Callvirt, s_getSerialize(item.Attr.SubType));

                il.Emit(OpCodes.Br, loopEnd);
            }

            // return
            il.MarkLabel(loopEnd);
            il.Emit(OpCodes.Ret);

            // else, writer.WriteNil(ref idx); return
            il.MarkLabel(notFoundType);
            il.EmitLdarg(1);
            il.EmitLdarg(2);
            il.EmitCall(MessagePackBinaryTypeInfo.WriteNil);
            il.Emit(OpCodes.Ret);
        }

        // T Deserialize([arg:1]ref MessagePackReader, [arg:2]IFormatterResolver formatterResolver);
        static void BuildDeserialize(Type type, UnionAttribute[] infos, MethodBuilder method, FieldBuilder keyToJumpMap, ILGenerator il)
        {
            // if(reader.IsNil()) return null;
            var falseLabel = il.DefineLabel();
            il.EmitLdarg(1);
            il.EmitCall(MessagePackBinaryTypeInfo.IsNil);
            il.Emit(OpCodes.Brfalse_S, falseLabel);

            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ret);

            // read-array header and validate, ReadArrayHeader() != 2) throw;
            il.MarkLabel(falseLabel);

            var rightLabel = il.DefineLabel();
            il.EmitLdarg(1);
            il.EmitCall(MessagePackBinaryTypeInfo.ReadArrayHeader);
            il.EmitLdc_I4(2);
            il.Emit(OpCodes.Beq_S, rightLabel);
            il.Emit(OpCodes.Ldstr, "Invalid Union data was detected. Type:" + type.FullName);
            il.Emit(OpCodes.Newobj, s_invalidOperationExceptionConstructor);
            il.Emit(OpCodes.Throw);

            il.MarkLabel(rightLabel);

            // read key
            var key = il.DeclareLocal(typeof(int));
            il.EmitLdarg(1);
            il.EmitCall(MessagePackBinaryTypeInfo.ReadInt32);
            il.EmitStloc(key);

            // is-sequential don't need else convert key to jump-table value
            if (!IsZeroStartSequential(infos))
            {
                var endKeyMapGet = il.DefineLabel();
                il.EmitLdarg(0);
                il.EmitLdfld(keyToJumpMap);
                il.EmitLdloc(key);
                il.EmitLdloca(key);
                il.EmitCall(s_keyMapDictionaryTryGetValue);
                il.Emit(OpCodes.Brtrue_S, endKeyMapGet);
                il.EmitLdc_I4(-1);
                il.EmitStloc(key);

                il.MarkLabel(endKeyMapGet);
            }

            // switch->read
            var result = il.DeclareLocal(type);
            var loopEnd = il.DefineLabel();
            il.Emit(OpCodes.Ldnull);
            il.EmitStloc(result);
            il.Emit(OpCodes.Ldloc, key);

            var switchLabels = infos.Select(x => new { Label = il.DefineLabel(), Attr = x }).ToArray();
            il.Emit(OpCodes.Switch, switchLabels.Select(x => x.Label).ToArray());

            // default
            il.EmitLdarg(1);
            il.EmitCall(MessagePackBinaryTypeInfo.ReadNextBlock);
            il.Emit(OpCodes.Br, loopEnd);

            foreach (var item in switchLabels)
            {
                il.MarkLabel(item.Label);
                il.EmitLdarg(2);
                il.EmitCall(s_getFormatterWithVerify.MakeGenericMethod(item.Attr.SubType));
                il.EmitLdarg(1);
                il.EmitLdarg(2);
                il.EmitCall(s_getDeserialize(item.Attr.SubType));
                if (item.Attr.SubType.IsValueType)
                {
                    il.Emit(OpCodes.Box, item.Attr.SubType);
                }
                il.Emit(OpCodes.Stloc, result);
                il.Emit(OpCodes.Br, loopEnd);
            }

            il.MarkLabel(loopEnd);

            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ret);
        }

        static bool IsZeroStartSequential(UnionAttribute[] infos)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].Key != i) return false;
            }
            return true;
        }

        // EmitInfos...

        static readonly Type s_refWriter = typeof(MessagePackWriter).MakeByRefType();
        static readonly Type s_refReader = typeof(MessagePackReader).MakeByRefType();
        static readonly Type s_refInt = typeof(int).MakeByRefType();
        static readonly Type s_refKvp = typeof(KeyValuePair<int, int>).MakeByRefType();
        static readonly MethodInfo s_getFormatterWithVerify = typeof(FormatterResolverExtensions).GetRuntimeMethods().First(x => x.Name == nameof(FormatterResolverExtensions.GetFormatterWithVerify));

        static readonly Func<Type, MethodInfo> s_getSerialize = t => typeof(IMessagePackFormatter<>).GetCachedGenericType(t).GetRuntimeMethod(nameof(IMessagePackFormatter<int>.Serialize), new[] { s_refWriter, s_refInt, t, typeof(IFormatterResolver) });
        static readonly Func<Type, MethodInfo> s_getDeserialize = t => typeof(IMessagePackFormatter<>).GetCachedGenericType(t).GetRuntimeMethod(nameof(IMessagePackFormatter<int>.Deserialize), new[] { s_refReader, typeof(IFormatterResolver) });

        static readonly FieldInfo s_runtimeTypeHandleEqualityComparer = typeof(RuntimeTypeHandleEqualityComparer).GetRuntimeField(nameof(RuntimeTypeHandleEqualityComparer.Default));
        static readonly ConstructorInfo s_intIntKeyValuePairConstructor = typeof(KeyValuePair<int, int>).GetTypeInfo().DeclaredConstructors.First(x => x.GetParameters().Length == 2);
        static readonly ConstructorInfo s_typeMapDictionaryConstructor = typeof(Dictionary<RuntimeTypeHandle, KeyValuePair<int, int>>).GetTypeInfo().DeclaredConstructors.First(x => { var p = x.GetParameters(); return p.Length == 2 && p[0].ParameterType == typeof(int); });
        static readonly MethodInfo s_typeMapDictionaryAdd = typeof(Dictionary<RuntimeTypeHandle, KeyValuePair<int, int>>).GetRuntimeMethod(nameof(Dictionary<RuntimeTypeHandle, KeyValuePair<int, int>>.Add), new[] { typeof(RuntimeTypeHandle), typeof(KeyValuePair<int, int>) });
        static readonly MethodInfo s_typeMapDictionaryTryGetValue = typeof(Dictionary<RuntimeTypeHandle, KeyValuePair<int, int>>).GetRuntimeMethod(nameof(Dictionary<RuntimeTypeHandle, KeyValuePair<int, int>>.TryGetValue), new[] { typeof(RuntimeTypeHandle), s_refKvp });

        static readonly ConstructorInfo s_keyMapDictionaryConstructor = typeof(Dictionary<int, int>).GetTypeInfo().DeclaredConstructors.First(x => { var p = x.GetParameters(); return p.Length == 1 && p[0].ParameterType == typeof(int); });
        static readonly MethodInfo s_keyMapDictionaryAdd = typeof(Dictionary<int, int>).GetRuntimeMethod(nameof(Dictionary<int, int>.Add), new[] { typeof(int), typeof(int) });
        static readonly MethodInfo s_keyMapDictionaryTryGetValue = typeof(Dictionary<int, int>).GetRuntimeMethod(nameof(Dictionary<int, int>.TryGetValue), new[] { typeof(int), s_refInt });

        static readonly MethodInfo s_objectGetType = typeof(object).GetRuntimeMethod(nameof(object.GetType), Type.EmptyTypes);
        static readonly MethodInfo s_getTypeHandle = typeof(Type).GetRuntimeProperty(nameof(Type.TypeHandle)).GetGetMethod();

        static readonly MethodInfo s_intIntKeyValuePairGetKey = typeof(KeyValuePair<int, int>).GetRuntimeProperty(nameof(KeyValuePair<int, int>.Key)).GetGetMethod();
        static readonly MethodInfo s_intIntKeyValuePairGetValue = typeof(KeyValuePair<int, int>).GetRuntimeProperty(nameof(KeyValuePair<int, int>.Value)).GetGetMethod();

        static readonly ConstructorInfo s_invalidOperationExceptionConstructor = typeof(System.InvalidOperationException).GetTypeInfo().DeclaredConstructors.First(x => { var p = x.GetParameters(); return p.Length == 1 && p[0].ParameterType == typeof(string); });
        static readonly ConstructorInfo s_objectCtor = typeof(object).GetTypeInfo().DeclaredConstructors.First(x => 0u >= (uint)x.GetParameters().Length);

        static class MessagePackBinaryTypeInfo
        {
            public static readonly MethodInfo WriteFixedMapHeaderUnsafe = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteFixedMapHeaderUnsafe), new[] { s_refWriter, typeof(int), s_refInt });
            public static readonly MethodInfo WriteFixedArrayHeaderUnsafe = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteFixedArrayHeaderUnsafe), new[] { s_refWriter, typeof(int), s_refInt });
            public static readonly MethodInfo WriteMapHeader = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteMapHeader), new[] { s_refWriter, typeof(int), s_refInt });
            public static readonly MethodInfo WriteArrayHeader = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteArrayHeader), new[] { s_refWriter, typeof(int), s_refInt });
            public static readonly MethodInfo WritePositiveFixedIntUnsafe = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WritePositiveFixedIntUnsafe), new[] { s_refWriter, typeof(int), s_refInt });
            public static readonly MethodInfo WriteInt32 = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteInt32), new[] { s_refWriter, typeof(int), s_refInt });
            public static readonly MethodInfo WriteBytes = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteBytes), new[] { s_refWriter, typeof(byte[]), s_refInt });
            public static readonly MethodInfo WriteNil = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteNil), new[] { s_refWriter, s_refInt });
            public static readonly MethodInfo WriteStringUnsafe = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteStringUnsafe), new[] { s_refWriter, typeof(string), typeof(int), s_refInt });

            public static readonly MethodInfo ReadBytes = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.ReadBytes), Type.EmptyTypes);
            public static readonly MethodInfo ReadInt32 = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.ReadInt32), Type.EmptyTypes);
            public static readonly MethodInfo ReadString = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.ReadString), Type.EmptyTypes);
            public static readonly MethodInfo IsNil = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.IsNil), Type.EmptyTypes);
            public static readonly MethodInfo ReadNextBlock = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.ReadNextBlock), Type.EmptyTypes);
            public static readonly MethodInfo ReadArrayHeader = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.ReadArrayHeader), Type.EmptyTypes);
            public static readonly MethodInfo ReadMapHeader = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.ReadMapHeader), Type.EmptyTypes);

            static MessagePackBinaryTypeInfo() { }
        }
    }
}

namespace MessagePack.Internal
{
    using System;
    using System.Collections.Generic;

    // RuntimeTypeHandle can embed directly by OpCodes.Ldtoken
    // It does not implements IEquatable<T>(but GetHashCode and Equals is implemented) so needs this to avoid boxing.
    public class RuntimeTypeHandleEqualityComparer : IEqualityComparer<RuntimeTypeHandle>
    {
        public static IEqualityComparer<RuntimeTypeHandle> Default = new RuntimeTypeHandleEqualityComparer();

        RuntimeTypeHandleEqualityComparer() { }

        public bool Equals(RuntimeTypeHandle x, RuntimeTypeHandle y) => x.Equals(y);

        public int GetHashCode(RuntimeTypeHandle obj) => obj.GetHashCode();
    }

    internal class MessagePackDynamicUnionResolverException : Exception
    {
        public MessagePackDynamicUnionResolverException(string message) : base(message) { }
    }
}