namespace MessagePack.Resolvers
{
    using System;
#if NETFRAMEWORK
    using System.Reflection.Emit;
#endif
#if DEPENDENT_ON_CUTEANT
    using CuteAnt.Reflection;
#endif
    using MessagePack.Formatters;
    using MessagePack.Internal;

    /// <summary>ObjectResolver by dynamic code generation.</summary>
    public sealed class DynamicObjectResolver : FormatterResolver
    {
        public static readonly DynamicObjectResolver Instance = new DynamicObjectResolver();

        const string ModuleName = "MessagePack.Resolvers.DynamicObjectResolver";

        internal static readonly DynamicAssembly assembly;

        DynamicObjectResolver() { }

        static DynamicObjectResolver()
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

                if (ti.IsInterface)
                {
                    return;
                }

                if (ti.IsNullable())
                {
                    ti = ti.GenericTypeArguments[0];

                    var innerFormatter = DynamicObjectResolver.Instance.GetFormatterDynamic(ti);
                    if (innerFormatter == null)
                    {
                        return;
                    }
                    formatter = (IMessagePackFormatter<T>)ActivatorUtils.CreateInstance(typeof(StaticNullableFormatter<>).GetCachedGenericType(ti), new object[] { innerFormatter });
                    return;
                }

                if (ti.IsAnonymous())
                {
                    formatter = (IMessagePackFormatter<T>)DynamicObjectTypeBuilder.BuildFormatterToDynamicMethod(typeof(T), true, true, false);
                    return;
                }

                var formatterTypeInfo = DynamicObjectTypeBuilder.BuildType(assembly, typeof(T), false, false);
                if (formatterTypeInfo == null) return;

                formatter = (IMessagePackFormatter<T>)ActivatorUtils.FastCreateInstance(formatterTypeInfo.AsType());
            }
        }
    }

    /// <summary>ObjectResolver by dynamic code generation, allow private member.</summary>
    public sealed class DynamicObjectResolverAllowPrivate : FormatterResolver
    {
        public static readonly DynamicObjectResolverAllowPrivate Instance = new DynamicObjectResolverAllowPrivate();

        DynamicObjectResolverAllowPrivate() { }

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

                if (ti.IsInterface)
                {
                    return;
                }

                if (ti.IsNullable())
                {
                    ti = ti.GenericTypeArguments[0];

                    var innerFormatter = DynamicObjectResolverAllowPrivate.Instance.GetFormatterDynamic(ti);
                    if (innerFormatter == null)
                    {
                        return;
                    }
                    formatter = (IMessagePackFormatter<T>)ActivatorUtils.CreateInstance(typeof(StaticNullableFormatter<>).GetCachedGenericType(ti), new object[] { innerFormatter });
                    return;
                }

                if (ti.IsAnonymous())
                {
                    formatter = (IMessagePackFormatter<T>)DynamicObjectTypeBuilder.BuildFormatterToDynamicMethod(typeof(T), true, true, false);
                }
                else
                {
                    formatter = (IMessagePackFormatter<T>)DynamicObjectTypeBuilder.BuildFormatterToDynamicMethod(typeof(T), false, false, true);
                }
            }
        }
    }

    /// <summary>ObjectResolver by dynamic code generation, no needs MessagePackObject attribute and serialized key as string.</summary>
    public sealed class DynamicContractlessObjectResolver : FormatterResolver
    {
        public static readonly DynamicContractlessObjectResolver Instance = new DynamicContractlessObjectResolver();

        const string ModuleName = "MessagePack.Resolvers.DynamicContractlessObjectResolver";

        static readonly DynamicAssembly assembly;

        DynamicContractlessObjectResolver() { }

        static DynamicContractlessObjectResolver()
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
                if (typeof(T) == typeof(object))
                {
                    return;
                }

                var ti = typeof(T);

                if (ti.IsInterface)
                {
                    return;
                }

                if (ti.IsNullable())
                {
                    ti = ti.GenericTypeArguments[0];

                    var innerFormatter = DynamicContractlessObjectResolver.Instance.GetFormatterDynamic(ti);
                    if (innerFormatter == null)
                    {
                        return;
                    }
                    formatter = (IMessagePackFormatter<T>)ActivatorUtils.CreateInstance(typeof(StaticNullableFormatter<>).GetCachedGenericType(ti), new object[] { innerFormatter });
                    return;
                }

                if (ti.IsAnonymous())
                {
                    formatter = (IMessagePackFormatter<T>)DynamicObjectTypeBuilder.BuildFormatterToDynamicMethod(typeof(T), true, true, false);
                    return;
                }

                var formatterTypeInfo = DynamicObjectTypeBuilder.BuildType(assembly, typeof(T), true, true);
                if (formatterTypeInfo == null) return;

                formatter = (IMessagePackFormatter<T>)ActivatorUtils.FastCreateInstance(formatterTypeInfo.AsType());
            }
        }
    }

    /// <summary>ObjectResolver by dynamic code generation, no needs MessagePackObject attribute and serialized key as string, allow private member.</summary>
    public sealed class DynamicContractlessObjectResolverAllowPrivate : FormatterResolver
    {
        public static readonly DynamicContractlessObjectResolverAllowPrivate Instance = new DynamicContractlessObjectResolverAllowPrivate();

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                if (typeof(T) == typeof(object))
                {
                    return;
                }

                var ti = typeof(T);

                if (ti.IsInterface)
                {
                    return;
                }

                if (ti.IsNullable())
                {
                    ti = ti.GenericTypeArguments[0];

                    var innerFormatter = DynamicContractlessObjectResolverAllowPrivate.Instance.GetFormatterDynamic(ti);
                    if (innerFormatter == null)
                    {
                        return;
                    }
                    formatter = (IMessagePackFormatter<T>)ActivatorUtils.CreateInstance(typeof(StaticNullableFormatter<>).GetCachedGenericType(ti), new object[] { innerFormatter });
                    return;
                }

                if (ti.IsAnonymous())
                {
                    formatter = (IMessagePackFormatter<T>)DynamicObjectTypeBuilder.BuildFormatterToDynamicMethod(typeof(T), true, true, false);
                }
                else
                {
                    formatter = (IMessagePackFormatter<T>)DynamicObjectTypeBuilder.BuildFormatterToDynamicMethod(typeof(T), true, true, true);
                }
            }
        }
    }
}

namespace MessagePack.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using System.Threading;
    using MessagePack.Formatters;
#if DEPENDENT_ON_CUTEANT
    using CuteAnt.Reflection;
#endif

    internal static class DynamicObjectTypeBuilder
    {
        static readonly Regex SubtractFullNameRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=\w+, PublicKeyToken=\w+", RegexOptions.Compiled);

        static int nameSequence = 0;

        static HashSet<Type> ignoreTypes = new HashSet<Type>
        {
            {typeof(object)},
            {typeof(short)},
            {typeof(int)},
            {typeof(long)},
            {typeof(ushort)},
            {typeof(uint)},
            {typeof(ulong)},
            {typeof(float)},
            {typeof(double)},
            {typeof(bool)},
            {typeof(byte)},
            {typeof(sbyte)},
            {typeof(decimal)},
            {typeof(char)},
            {typeof(string)},
            {typeof(System.Guid)},
#if DEPENDENT_ON_CUTEANT
            {typeof(CuteAnt.CombGuid)},
#endif
            {typeof(System.TimeSpan)},
            {typeof(System.DateTime)},
            {typeof(System.DateTimeOffset)},
            {typeof(MessagePack.Nil)},

            {typeof(ConstructorInfo)},
            {typeof(Delegate)},
            {typeof(Action)},
            {typeof(EventInfo)},
            {typeof(FieldInfo)},
            {typeof(PropertyInfo)},
            {typeof(MethodInfo)},
            {typeof(Type)},
            {typeof(MemberInfo)},
            {typeof(System.Globalization.CultureInfo)},
            {typeof(System.Net.IPAddress)},
            {typeof(System.Net.IPEndPoint)},
            {typeof(Expression)},
            {typeof(SymbolDocumentInfo)},
            {typeof(MemberBinding)},
            {typeof(MemberAssignment)},
            {typeof(MemberListBinding)},
            {typeof(MemberMemberBinding)},
        };

        public static TypeInfo BuildType(DynamicAssembly assembly, Type type, bool forceStringKey, bool contractless)
        {
            if (ignoreTypes.Contains(type)) return null;

            var serializationInfo = MessagePack.Internal.ObjectSerializationInfo.CreateOrNull(type, forceStringKey, contractless, false);
            if (serializationInfo == null) return null;

            var formatterType = typeof(IMessagePackFormatter<>).GetCachedGenericType(type);
            var typeBuilder = assembly.DefineType("MessagePack.Formatters." + SubtractFullNameRegex.Replace(type.FullName, "").Replace(".", "_") + "Formatter" + Interlocked.Increment(ref nameSequence), TypeAttributes.Public | TypeAttributes.Sealed, null, new[] { formatterType });

            FieldBuilder stringByteKeysField = null;
            Dictionary<ObjectSerializationInfo.EmittableMember, FieldInfo> customFormatterLookup = null;

            // string key needs string->int mapper for deserialize switch statement
            if (serializationInfo.IsStringKey)
            {
                var method = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
                stringByteKeysField = typeBuilder.DefineField("stringByteKeys", typeof(byte[][]), FieldAttributes.Private | FieldAttributes.InitOnly);

                var il = method.GetILGenerator();
                BuildConstructor(type, serializationInfo, method, stringByteKeysField, il);
                customFormatterLookup = BuildCustomFormatterField(typeBuilder, serializationInfo, il);
                il.Emit(OpCodes.Ret);
            }
            else
            {
                var method = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
                var il = method.GetILGenerator();
                il.EmitLoadThis();
                il.Emit(OpCodes.Call, s_objectCtor);
                customFormatterLookup = BuildCustomFormatterField(typeBuilder, serializationInfo, il);
                il.Emit(OpCodes.Ret);
            }

            {
                var method = typeBuilder.DefineMethod("Serialize", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
                    typeof(void),
                    new Type[] { s_refWriter, s_refInt, type, typeof(IFormatterResolver) });

                var il = method.GetILGenerator();
                BuildSerialize(type, serializationInfo, il, () =>
                {
                    il.EmitLoadThis();
                    il.EmitLdfld(stringByteKeysField);
                }, (index, member) =>
                {
                    if (!customFormatterLookup.TryGetValue(member, out FieldInfo fi)) return null;

                    return () =>
                    {
                        il.EmitLoadThis();
                        il.EmitLdfld(fi);
                    };
                }, 1);
            }

            {
                var method = typeBuilder.DefineMethod("Deserialize", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
                    type,
                    new Type[] { s_refReader, typeof(IFormatterResolver) });

                var il = method.GetILGenerator();
                BuildDeserialize(type, serializationInfo, il, (index, member) =>
                {
                    if (!customFormatterLookup.TryGetValue(member, out FieldInfo fi)) return null;

                    return () =>
                    {
                        il.EmitLoadThis();
                        il.EmitLdfld(fi);
                    };
                }, 1); // firstArgIndex:0 is this.
            }

            return typeBuilder.CreateTypeInfo();
        }

        public static object BuildFormatterToDynamicMethod(Type type, bool forceStringKey, bool contractless, bool allowPrivate)
        {
            // ## 苦竹 添加 ##
            if (ignoreTypes.Contains(type)) return null;

            var serializationInfo = ObjectSerializationInfo.CreateOrNull(type, forceStringKey, contractless, allowPrivate);
            if (serializationInfo == null) return null;

            // internal delegate int AnonymousSerializeFunc<T>(byte[][] stringByteKeysField, object[] customFormatters, ref MessagePackWriter writer, ref int idx, T value, IFormatterResolver resolver);
            var serialize = new DynamicMethod("Serialize", typeof(void), new[] { typeof(byte[][]), typeof(object[]), s_refWriter, s_refInt, type, typeof(IFormatterResolver) }, type, true);
            DynamicMethod deserialize = null;

            List<byte[]> stringByteKeysField = new List<byte[]>();
            List<object> serializeCustomFormatters = new List<object>();
            List<object> deserializeCustomFormatters = new List<object>();

            if (serializationInfo.IsStringKey)
            {
                var i = 0;
                foreach (var item in serializationInfo.Members.Where(x => x.IsReadable))
                {
                    stringByteKeysField.Add(MessagePackBinary.GetEncodedStringBytes(item.StringKey));
                    i++;
                }
            }
            foreach (var item in serializationInfo.Members.Where(x => x.IsReadable))
            {
                var attr = item.GetMessagePackFormatterAttribute();
                if (attr != null)
                {
                    var formatter = ActivatorUtil.CreateInstance(attr.FormatterType, attr.Arguments);
                    serializeCustomFormatters.Add(formatter);
                }
                else
                {
                    serializeCustomFormatters.Add(null);
                }
            }
            foreach (var item in serializationInfo.Members) // not only for writable because for use ctor.
            {
                var attr = item.GetMessagePackFormatterAttribute();
                if (attr != null)
                {
                    var formatter = ActivatorUtil.CreateInstance(attr.FormatterType, attr.Arguments);
                    deserializeCustomFormatters.Add(formatter);
                }
                else
                {
                    deserializeCustomFormatters.Add(null);
                }
            }

            {
                var il = serialize.GetILGenerator();
                BuildSerialize(type, serializationInfo, il, () =>
                {
                    il.EmitLdarg(0);
                }, (index, member) =>
                {
                    if (0u >= (uint)serializeCustomFormatters.Count) return null;
                    if (serializeCustomFormatters[index] == null) return null;

                    return () =>
                    {
                        il.EmitLdarg(1); // read object[]
                        il.EmitLdc_I4(index);
                        il.Emit(OpCodes.Ldelem_Ref); // object
                        il.Emit(OpCodes.Castclass, serializeCustomFormatters[index].GetType());
                    };
                }, 2);  // 0, 1 is parameter.
            }

            if (serializationInfo.IsStruct || serializationInfo.BestmatchConstructor != null)
            {
                // internal delegate T AnonymousDeserializeFunc<T>(object[] customFormatters, ref MessagePackReader reader, IFormatterResolver resolver);
                deserialize = new DynamicMethod("Deserialize", type, new[] { typeof(object[]), s_refReader, typeof(IFormatterResolver) }, type, true);

                var il = deserialize.GetILGenerator();
                BuildDeserialize(type, serializationInfo, il, (index, member) =>
                {
                    if (0u >= (uint)deserializeCustomFormatters.Count) return null;
                    if (deserializeCustomFormatters[index] == null) return null;

                    return () =>
                    {
                        il.EmitLdarg(0); // read object[]
                        il.EmitLdc_I4(index);
                        il.Emit(OpCodes.Ldelem_Ref); // object
                        il.Emit(OpCodes.Castclass, deserializeCustomFormatters[index].GetType());
                    };
                }, 1);
            }

            object serializeDelegate = serialize.CreateDelegate(typeof(AnonymousSerializeFunc<>).GetCachedGenericType(type));
            object deserializeDelegate = (deserialize == null)
                ? (object)null
                : (object)deserialize.CreateDelegate(typeof(AnonymousDeserializeFunc<>).GetCachedGenericType(type));
            var resultFormatter = ActivatorUtils.CreateInstance(typeof(AnonymousSerializableFormatter<>).GetCachedGenericType(type),
                new[] { stringByteKeysField.ToArray(), serializeCustomFormatters.ToArray(), deserializeCustomFormatters.ToArray(), serializeDelegate, deserializeDelegate });
            return resultFormatter;
        }

        static void BuildConstructor(Type type, ObjectSerializationInfo info, ConstructorInfo method, FieldBuilder stringByteKeysField, ILGenerator il)
        {
            il.EmitLoadThis();
            il.Emit(OpCodes.Call, s_objectCtor);

            var writeCount = info.Members.Count(x => x.IsReadable);
            il.EmitLoadThis();
            il.EmitLdc_I4(writeCount);
            il.Emit(OpCodes.Newarr, typeof(byte[]));

            var i = 0;
            foreach (var item in info.Members.Where(x => x.IsReadable))
            {
                il.Emit(OpCodes.Dup);
                il.EmitLdc_I4(i);
                il.Emit(OpCodes.Ldstr, item.StringKey);
                il.EmitCall(MessagePackBinaryTypeInfo.GetEncodedStringBytes);
                il.Emit(OpCodes.Stelem_Ref);
                i++;
            }

            il.Emit(OpCodes.Stfld, stringByteKeysField);
        }

        static Dictionary<ObjectSerializationInfo.EmittableMember, FieldInfo> BuildCustomFormatterField(TypeBuilder builder, ObjectSerializationInfo info, ILGenerator il)
        {
            Dictionary<ObjectSerializationInfo.EmittableMember, FieldInfo> dict = new Dictionary<ObjectSerializationInfo.EmittableMember, FieldInfo>();
            foreach (var item in info.Members.Where(x => x.IsReadable || x.IsWritable))
            {
                var attr = item.GetMessagePackFormatterAttribute();
                if (attr != null)
                {
                    var f = builder.DefineField(item.Name + "_formatter", attr.FormatterType, FieldAttributes.Private | FieldAttributes.InitOnly);

                    var bindingFlags = (int)(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    var attrVar = il.DeclareLocal(typeof(MessagePackFormatterAttribute));

                    il.Emit(OpCodes.Ldtoken, info.Type);
                    il.EmitCall(EmitInfo.GetTypeFromHandle);
                    il.Emit(OpCodes.Ldstr, item.Name);
                    il.EmitLdc_I4(bindingFlags);
                    if (item.IsProperty)
                    {
                        il.EmitCall(EmitInfo.TypeGetProperty);
                    }
                    else
                    {
                        il.EmitCall(EmitInfo.TypeGetField);
                    }

                    il.EmitTrue();
                    il.EmitCall(EmitInfo.GetCustomAttributeMessagePackFormatterAttribute);
                    il.EmitStloc(attrVar);

                    il.EmitLoadThis();

                    il.EmitLdloc(attrVar);
                    il.EmitCall(EmitInfo.MessagePackFormatterAttr.FormatterType);
                    il.EmitLdloc(attrVar);
                    il.EmitCall(EmitInfo.MessagePackFormatterAttr.Arguments);
                    il.EmitCall(EmitInfo.ActivatorCreateInstance);

                    il.Emit(OpCodes.Castclass, attr.FormatterType);
                    il.Emit(OpCodes.Stfld, f);

                    dict.Add(item, f);
                }
            }

            return dict;
        }

        // void Serialize([arg:1]ref MessagePackWriter writer, [arg:2]ref int offset, [arg:3]T value, [arg:4]IFormatterResolver formatterResolver);
        static void BuildSerialize(Type type, ObjectSerializationInfo info, ILGenerator il, Action emitStringByteKeys, Func<int, ObjectSerializationInfo.EmittableMember, Action> tryEmitLoadCustomFormatter, int firstArgIndex)
        {
            var argWriter = new ArgumentField(il, firstArgIndex);
            var argOffset = new ArgumentField(il, firstArgIndex + 1);
            var argValue = new ArgumentField(il, firstArgIndex + 2, type);
            var argResolver = new ArgumentField(il, firstArgIndex + 3);

            // if(value == null) return WriteNil
            if (type.IsClass)
            {
                var elseBody = il.DefineLabel();

                argValue.EmitLoad();
                il.Emit(OpCodes.Brtrue_S, elseBody);
                argWriter.EmitLoad();
                argOffset.EmitLoad();
                il.EmitCall(MessagePackBinaryTypeInfo.WriteNil);
                il.Emit(OpCodes.Ret);

                il.MarkLabel(elseBody);
            }

            // IMessagePackSerializationCallbackReceiver.OnBeforeSerialize()
            if (type.GetTypeInfo().ImplementedInterfaces.Any(x => x == typeof(IMessagePackSerializationCallbackReceiver)))
            {
                // call directly
                var runtimeMethods = type.GetRuntimeMethods().Where(x => x.Name == "OnBeforeSerialize").ToArray();
                if (runtimeMethods.Length == 1)
                {
                    argValue.EmitLoad();
                    il.Emit(OpCodes.Call, runtimeMethods[0]); // don't use EmitCall helper(must use 'Call')
                }
                else
                {
                    argValue.EmitLdarg(); // force ldarg
                    il.EmitBoxOrDoNothing(type);
                    il.EmitCall(s_onBeforeSerialize);
                }
            }

            if (info.IsIntKey)
            {
                // use Array
                var maxKey = info.Members.Where(x => x.IsReadable).Select(x => x.IntKey).DefaultIfEmpty(-1).Max();
                var intKeyMap = info.Members.Where(x => x.IsReadable).ToDictionary(x => x.IntKey);

                argWriter.EmitLoad();
                var len = maxKey + 1;
                il.EmitLdc_I4(len);
                argOffset.EmitLoad();
                if (len <= MessagePackRange.MaxFixArrayCount)
                {
                    il.EmitCall(MessagePackBinaryTypeInfo.WriteFixedArrayHeaderUnsafe);
                }
                else
                {
                    il.EmitCall(MessagePackBinaryTypeInfo.WriteArrayHeader);
                }

                for (int i = 0; i <= maxKey; i++)
                {
                    if (intKeyMap.TryGetValue(i, out ObjectSerializationInfo.EmittableMember member))
                    {
                        // offset += serialzie
                        EmitSerializeValue(il, type.GetTypeInfo(), member, i, tryEmitLoadCustomFormatter, argWriter, argOffset, argValue, argResolver);
                    }
                    else
                    {
                        // Write Nil as Blanc
                        argWriter.EmitLoad();
                        argOffset.EmitLoad();
                        il.EmitCall(MessagePackBinaryTypeInfo.WriteNil);
                    }
                }
            }
            else
            {
                // use Map
                var writeCount = info.Members.Count(x => x.IsReadable);

                argWriter.EmitLoad();
                il.EmitLdc_I4(writeCount);
                argOffset.EmitLoad();
                if (writeCount <= MessagePackRange.MaxFixMapCount)
                {
                    il.EmitCall(MessagePackBinaryTypeInfo.WriteFixedMapHeaderUnsafe);
                }
                else
                {
                    il.EmitCall(MessagePackBinaryTypeInfo.WriteMapHeader);
                }

                var index = 0;
                foreach (var item in info.Members.Where(x => x.IsReadable))
                {
                    // writekey
                    argWriter.EmitLoad();
                    emitStringByteKeys();
                    il.EmitLdc_I4(index);
                    il.Emit(OpCodes.Ldelem_Ref);
                    argOffset.EmitLoad();
                    // Optimize, WriteRaw(Unity, large) or UnsafeMemory32/64.WriteRawX
                    var valueLen = MessagePackBinary.GetEncodedStringBytes(item.StringKey).Length;
                    if (valueLen <= MessagePackRange.MaxFixStringLength)
                    {
                        if (UnsafeMemory.Is64BitProcess)
                        {
                            il.EmitCall(typeof(UnsafeMemory64).GetRuntimeMethod("WriteRaw" + valueLen, new[] { s_refWriter, typeof(byte[]), s_refInt }));
                        }
                        else
                        {
                            il.EmitCall(typeof(UnsafeMemory32).GetRuntimeMethod("WriteRaw" + valueLen, new[] { s_refWriter, typeof(byte[]), s_refInt }));
                        }
                    }
                    else
                    {
                        il.EmitCall(MessagePackBinaryTypeInfo.WriteRawBytes);
                    }

                    // serialzie
                    EmitSerializeValue(il, type.GetTypeInfo(), item, index, tryEmitLoadCustomFormatter, argWriter, argOffset, argValue, argResolver);
                    index++;
                }
            }

            il.Emit(OpCodes.Ret);
        }

        static void EmitSerializeValue(ILGenerator il, TypeInfo type, ObjectSerializationInfo.EmittableMember member, int index, Func<int, ObjectSerializationInfo.EmittableMember, Action> tryEmitLoadCustomFormatter, ArgumentField argWriter, ArgumentField argOffset, ArgumentField argValue, ArgumentField argResolver)
        {
            var t = member.Type;
            var emitter = tryEmitLoadCustomFormatter(index, member);
            if (emitter != null)
            {
                emitter();

                argWriter.EmitLoad();
                argOffset.EmitLoad();

                argValue.EmitLoad();
                member.EmitLoadValue(il);
                argResolver.EmitLoad();
                il.EmitCall(typeof(IMessagePackFormatter<>).GetCachedGenericType(t).GetRuntimeMethod("Serialize", new[] { s_refWriter, s_refInt, t, typeof(IFormatterResolver) }));
            }
            else if (IsOptimizeTargetType(t))
            {
                argWriter.EmitLoad();
                argValue.EmitLoad();
                member.EmitLoadValue(il);
                argOffset.EmitLoad();
                if (t == typeof(byte[]))
                {
                    il.EmitCall(MessagePackBinaryTypeInfo.WriteBytes);
                }
                else
                {
                    il.EmitCall(MessagePackBinaryTypeInfo.WriterTypeInfo.GetDeclaredMethods("Write" + t.Name).OrderByDescending(x => x.GetParameters().Length).First());
                }
            }
            else
            {
                argResolver.EmitLoad();
                il.Emit(OpCodes.Call, s_getFormatterWithVerify.MakeGenericMethod(t));

                argWriter.EmitLoad();
                argOffset.EmitLoad();
                argValue.EmitLoad();
                member.EmitLoadValue(il);
                argResolver.EmitLoad();
                il.EmitCall(s_getSerialize(t));
            }
        }

        // T Deserialize([arg:1]ref MessagePackReader reader, [arg:2]IFormatterResolver formatterResolver);
        static void BuildDeserialize(Type type, ObjectSerializationInfo info, ILGenerator il, Func<int, ObjectSerializationInfo.EmittableMember, Action> tryEmitLoadCustomFormatter, int firstArgIndex)
        {
            var argReader = new ArgumentField(il, firstArgIndex);
            var argResolver = new ArgumentField(il, firstArgIndex + 1);

            // if(MessagePackBinary.IsNil) readSize = 1, return null;
            var falseLabel = il.DefineLabel();
            argReader.EmitLoad();
            il.EmitCall(MessagePackBinaryTypeInfo.IsNil);
            il.Emit(OpCodes.Brfalse_S, falseLabel);
            if (type.IsClass)
            {
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ret);
            }
            else
            {
                il.Emit(OpCodes.Ldstr, "typecode is null, struct not supported");
                il.Emit(OpCodes.Newobj, s_invalidOperationExceptionConstructor);
                il.Emit(OpCodes.Throw);
            }

            // var startOffset = offset;
            il.MarkLabel(falseLabel);

            // var length = ReadMapHeader
            var length = il.DeclareLocal(typeof(int)); // [loc:1]
            argReader.EmitLoad();

            if (info.IsIntKey)
            {
                il.EmitCall(MessagePackBinaryTypeInfo.ReadArrayHeader);
            }
            else
            {
                il.EmitCall(MessagePackBinaryTypeInfo.ReadMapHeader);
            }
            il.EmitStloc(length);

            // make local fields
            Label? gotoDefault = null;
            DeserializeInfo[] infoList;
            if (info.IsIntKey)
            {
                var maxKey = info.Members.Select(x => x.IntKey).DefaultIfEmpty(-1).Max();
                var len = maxKey + 1;
                var intKeyMap = info.Members.ToDictionary(x => x.IntKey);

                infoList = Enumerable.Range(0, len)
                    .Select(x =>
                    {
                        if (intKeyMap.TryGetValue(x, out ObjectSerializationInfo.EmittableMember member))
                        {
                            return new DeserializeInfo
                            {
                                MemberInfo = member,
                                LocalField = il.DeclareLocal(member.Type),
                                SwitchLabel = il.DefineLabel()
                            };
                        }
                        else
                        {
                            // return null MemberInfo, should filter null
                            if (gotoDefault == null)
                            {
                                gotoDefault = il.DefineLabel();
                            }
                            return new DeserializeInfo
                            {
                                MemberInfo = null,
                                LocalField = null,
                                SwitchLabel = gotoDefault.Value,
                            };
                        }
                    })
                    .ToArray();
            }
            else
            {
                infoList = info.Members
                    .Select(item => new DeserializeInfo
                    {
                        MemberInfo = item,
                        LocalField = il.DeclareLocal(item.Type),
                        // SwitchLabel = il.DefineLabel()
                    })
                    .ToArray();
            }

            // Read Loop(for var i = 0; i< length; i++)
            if (info.IsStringKey)
            {
                var automata = new AutomataDictionary();
                for (int i = 0; i < info.Members.Length; i++)
                {
                    automata.Add(info.Members[i].StringKey, i);
                }

                var keyArraySegment = il.DeclareLocal(typeof(ReadOnlySpan<byte>));
                var longKey = il.DeclareLocal(typeof(ulong));
                var p = il.DeclareLocal(typeof(byte*));
                var rest = il.DeclareLocal(typeof(int));

                // for (int i = 0; i < len; i++)
                il.EmitIncrementFor(length, forILocal =>
                {
                    var readNext = il.DefineLabel();
                    var loopEnd = il.DefineLabel();

                    argReader.EmitLoad();
                    il.EmitCall(MessagePackBinaryTypeInfo.ReadStringSegment);
                    il.EmitStloc(keyArraySegment);

                    il.EmitLdloca(keyArraySegment);
                    il.EmitCall(MessagePackBinaryTypeInfo.GetPointer);
                    il.EmitStloc(p);

                    // rest = arraySegment.Count
                    il.EmitLdloca(keyArraySegment);
                    il.EmitCall(typeof(ReadOnlySpan<byte>).GetRuntimeProperty(nameof(ReadOnlySpan<byte>.Length)).GetGetMethod());
                    il.EmitStloc(rest);

                    // if(rest == 0) goto End
                    il.EmitLdloc(rest);
                    il.Emit(OpCodes.Brfalse, readNext);

                    // gen automata name lookup
                    automata.EmitMatch(il, p, rest, longKey, x =>
                    {
                        var i = x.Value;
                        if (infoList[i].MemberInfo != null)
                        {
                            EmitDeserializeValue(il, infoList[i], i, tryEmitLoadCustomFormatter, argReader, argResolver);
                            il.Emit(OpCodes.Br, loopEnd);
                        }
                        else
                        {
                            il.Emit(OpCodes.Br, readNext);
                        }
                    }, () =>
                    {
                        il.Emit(OpCodes.Br, readNext);
                    });

                    il.MarkLabel(readNext);
                    argReader.EmitLoad();
                    il.EmitCall(MessagePackBinaryTypeInfo.ReadNextBlock);

                    il.MarkLabel(loopEnd);
                });
            }
            else
            {
                var key = il.DeclareLocal(typeof(int));
                var switchDefault = il.DefineLabel();

                il.EmitIncrementFor(length, forILocal =>
                {
                    var loopEnd = il.DefineLabel();

                    il.EmitLdloc(forILocal);
                    il.EmitStloc(key);

                    // switch... local = Deserialize
                    il.EmitLdloc(key);

                    il.Emit(OpCodes.Switch, infoList.Select(x => x.SwitchLabel).ToArray());

                    il.MarkLabel(switchDefault);
                    // default, only read. reader.ReadNextBlock(bytes, offset);
                    argReader.EmitLoad();
                    il.EmitCall(MessagePackBinaryTypeInfo.ReadNextBlock);
                    il.Emit(OpCodes.Br, loopEnd);

                    if (gotoDefault != null)
                    {
                        il.MarkLabel(gotoDefault.Value);
                        il.Emit(OpCodes.Br, switchDefault);
                    }

                    var i = 0;
                    foreach (var item in infoList)
                    {
                        if (item.MemberInfo != null)
                        {
                            il.MarkLabel(item.SwitchLabel);
                            EmitDeserializeValue(il, item, i++, tryEmitLoadCustomFormatter, argReader, argResolver);
                            il.Emit(OpCodes.Br, loopEnd);
                        }
                    }

                    il.MarkLabel(loopEnd);
                });
            }

            // create result object
            var structLocal = EmitNewObject(il, type, info, infoList);

            // IMessagePackSerializationCallbackReceiver.OnAfterDeserialize()
            if (type.GetTypeInfo().ImplementedInterfaces.Any(x => x == typeof(IMessagePackSerializationCallbackReceiver)))
            {
                // call directly
                var runtimeMethods = type.GetRuntimeMethods().Where(x => x.Name == "OnAfterDeserialize").ToArray();
                if (runtimeMethods.Length == 1)
                {
                    if (info.IsClass)
                    {
                        il.Emit(OpCodes.Dup);
                    }
                    else
                    {
                        il.EmitLdloca(structLocal);
                    }

                    il.Emit(OpCodes.Call, runtimeMethods[0]); // don't use EmitCall helper(must use 'Call')
                }
                else
                {
                    if (info.IsStruct)
                    {
                        il.EmitLdloc(structLocal);
                        il.Emit(OpCodes.Box, type);
                    }
                    else
                    {
                        il.Emit(OpCodes.Dup);
                    }
                    il.EmitCall(s_onAfterDeserialize);
                }
            }

            if (info.IsStruct)
            {
                il.Emit(OpCodes.Ldloc, structLocal);
            }


            il.Emit(OpCodes.Ret);
        }

        static void EmitDeserializeValue(ILGenerator il, DeserializeInfo info, int index, Func<int, ObjectSerializationInfo.EmittableMember, Action> tryEmitLoadCustomFormatter, ArgumentField argReader, ArgumentField argResolver)
        {
            var member = info.MemberInfo;
            var t = member.Type;
            var emitter = tryEmitLoadCustomFormatter(index, member);
            if (emitter != null)
            {
                emitter();
                argReader.EmitLoad();
                argResolver.EmitLoad();
                il.EmitCall(typeof(IMessagePackFormatter<>).GetCachedGenericType(t).GetRuntimeMethod("Deserialize", new[] { s_refReader, typeof(IFormatterResolver) }));
            }
            else if (IsOptimizeTargetType(t))
            {
                il.EmitLdarg(1);
                if (t == typeof(byte[]))
                {
                    il.EmitCall(MessagePackBinaryTypeInfo.ReadBytes);
                }
                else
                {
                    il.EmitCall(MessagePackBinaryTypeInfo.ReaderTypeInfo.GetDeclaredMethods("Read" + t.Name).OrderByDescending(x => x.GetParameters().Length).First());
                }
            }
            else
            {
                argResolver.EmitLoad();
                il.EmitCall(s_getFormatterWithVerify.MakeGenericMethod(t));
                argReader.EmitLoad();
                argResolver.EmitLoad();
                il.EmitCall(s_getDeserialize(t));
            }

            il.EmitStloc(info.LocalField);
        }

        static LocalBuilder EmitNewObject(ILGenerator il, Type type, ObjectSerializationInfo info, DeserializeInfo[] members)
        {
            if (info.IsClass)
            {
                foreach (var item in info.ConstructorParameters)
                {
                    var local = members.First(x => x.MemberInfo == item);
                    il.EmitLdloc(local.LocalField);
                }
                il.Emit(OpCodes.Newobj, info.BestmatchConstructor);

                foreach (var item in members.Where(x => x.MemberInfo != null && x.MemberInfo.IsWritable))
                {
                    il.Emit(OpCodes.Dup);
                    il.EmitLdloc(item.LocalField);
                    item.MemberInfo.EmitStoreValue(il);
                }

                return null;
            }
            else
            {
                var result = il.DeclareLocal(type);
                if (info.BestmatchConstructor == null)
                {
                    il.Emit(OpCodes.Ldloca, result);
                    il.Emit(OpCodes.Initobj, type);
                }
                else
                {
                    foreach (var item in info.ConstructorParameters)
                    {
                        var local = members.First(x => x.MemberInfo == item);
                        il.EmitLdloc(local.LocalField);
                    }
                    il.Emit(OpCodes.Newobj, info.BestmatchConstructor);
                    il.Emit(OpCodes.Stloc, result);
                }

                foreach (var item in members.Where(x => x.MemberInfo != null && x.MemberInfo.IsWritable))
                {
                    il.EmitLdloca(result);
                    il.EmitLdloc(item.LocalField);
                    item.MemberInfo.EmitStoreValue(il);
                }

                return result; // struct returns local result field
            }
        }

        static bool IsOptimizeTargetType(Type type)
        {
            if (type == typeof(Int16)
             || type == typeof(Int32)
             || type == typeof(Int64)
             || type == typeof(UInt16)
             || type == typeof(UInt32)
             || type == typeof(UInt64)
             || type == typeof(Single)
             || type == typeof(Double)
             || type == typeof(bool)
             || type == typeof(byte)
             || type == typeof(sbyte)
             || type == typeof(char)
             // not includes DateTime and String and Binary.
             //|| type == typeof(DateTime)
             //|| type == typeof(string)
             //|| type == typeof(byte[])
             )
            {
                return true;
            }
            return false;
        }

        // EmitInfos...

        static readonly Type s_refWriter = typeof(MessagePackWriter).MakeByRefType();
        static readonly Type s_refReader = typeof(MessagePackReader).MakeByRefType();
        static readonly Type s_refInt = typeof(int).MakeByRefType();
        static readonly MethodInfo s_getFormatterWithVerify = typeof(FormatterResolverExtensions).GetRuntimeMethods().First(x => x.Name == nameof(FormatterResolverExtensions.GetFormatterWithVerify));
        static readonly Func<Type, MethodInfo> s_getSerialize = t => typeof(IMessagePackFormatter<>).GetCachedGenericType(t).GetRuntimeMethod(nameof(IMessagePackFormatter<int>.Serialize), new[] { s_refWriter, s_refInt, t, typeof(IFormatterResolver) });
        static readonly Func<Type, MethodInfo> s_getDeserialize = t => typeof(IMessagePackFormatter<>).GetCachedGenericType(t).GetRuntimeMethod(nameof(IMessagePackFormatter<int>.Deserialize), new[] { s_refReader, typeof(IFormatterResolver) });
        // static readonly ConstructorInfo dictionaryConstructor = typeof(ByteArrayStringHashTable).GetTypeInfo().DeclaredConstructors.First(x => { var p = x.GetParameters(); return p.Length == 1 && p[0].ParameterType == typeof(int); });
        // static readonly MethodInfo dictionaryAdd = typeof(ByteArrayStringHashTable).GetRuntimeMethod("Add", new[] { typeof(string), typeof(int) });
        // static readonly MethodInfo dictionaryTryGetValue = typeof(ByteArrayStringHashTable).GetRuntimeMethod("TryGetValue", new[] { typeof(ArraySegment<byte>), refInt });
        static readonly ConstructorInfo s_invalidOperationExceptionConstructor = typeof(System.InvalidOperationException).GetTypeInfo().DeclaredConstructors.First(x => { var p = x.GetParameters(); return p.Length == 1 && p[0].ParameterType == typeof(string); });

        static readonly MethodInfo s_onBeforeSerialize = typeof(IMessagePackSerializationCallbackReceiver).GetRuntimeMethod(nameof(IMessagePackSerializationCallbackReceiver.OnBeforeSerialize), Type.EmptyTypes);
        static readonly MethodInfo s_onAfterDeserialize = typeof(IMessagePackSerializationCallbackReceiver).GetRuntimeMethod(nameof(IMessagePackSerializationCallbackReceiver.OnAfterDeserialize), Type.EmptyTypes);

        static readonly ConstructorInfo s_objectCtor = typeof(object).GetTypeInfo().DeclaredConstructors.First(x => 0u >= (uint)x.GetParameters().Length);

        internal static class MessagePackBinaryTypeInfo
        {
            public static readonly TypeInfo WriterTypeInfo = typeof(MessagePackWriterExtensions).GetTypeInfo();
            public static readonly TypeInfo ReaderTypeInfo = typeof(MessagePackReader).GetTypeInfo();

            public static readonly MethodInfo GetEncodedStringBytes = typeof(MessagePackBinary).GetRuntimeMethod(nameof(MessagePackBinary.GetEncodedStringBytes), new[] { typeof(string) });
            public static readonly MethodInfo GetPointer = typeof(MessagePackBinary).GetRuntimeMethod(nameof(MessagePackBinary.GetPointer), new[] { typeof(ReadOnlySpan<byte>) });

            public static readonly MethodInfo WriteFixedMapHeaderUnsafe = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteFixedMapHeaderUnsafe), new[] { s_refWriter, typeof(int), s_refInt });
            public static readonly MethodInfo WriteFixedArrayHeaderUnsafe = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteFixedArrayHeaderUnsafe), new[] { s_refWriter, typeof(int), s_refInt });
            public static readonly MethodInfo WriteMapHeader = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteMapHeader), new[] { s_refWriter, typeof(int), s_refInt });
            public static readonly MethodInfo WriteArrayHeader = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteArrayHeader), new[] { s_refWriter, typeof(int), s_refInt });
            public static readonly MethodInfo WritePositiveFixedIntUnsafe = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WritePositiveFixedIntUnsafe), new[] { s_refWriter, typeof(int), s_refInt });
            public static readonly MethodInfo WriteInt32 = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteInt32), new[] { s_refWriter, typeof(int), s_refInt });
            public static readonly MethodInfo WriteBytes = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteBytes), new[] { s_refWriter, typeof(byte[]), s_refInt });
            public static readonly MethodInfo WriteStringUnsafe = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteStringUnsafe), new[] { s_refWriter, typeof(string), typeof(int), s_refInt });
            public static readonly MethodInfo WriteStringBytes = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteStringBytes), new[] { s_refWriter, typeof(byte[]), s_refInt });
            public static readonly MethodInfo WriteNil = typeof(MessagePackWriterExtensions).GetRuntimeMethod(nameof(MessagePackWriterExtensions.WriteNil), new[] { s_refWriter, s_refInt });

            public static readonly MethodInfo WriteRawBytes = typeof(UnsafeMemory).GetMethod(nameof(UnsafeMemory.WriteRawBytes), new[] { s_refWriter, typeof(byte[]), s_refInt });

            public static readonly MethodInfo ReadBytes = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.ReadBytes), Type.EmptyTypes);
            public static readonly MethodInfo ReadInt32 = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.ReadInt32), Type.EmptyTypes);
            public static readonly MethodInfo ReadString = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.ReadString), Type.EmptyTypes);
            public static readonly MethodInfo ReadStringSegment = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.ReadUtf8Span), Type.EmptyTypes);
            public static readonly MethodInfo IsNil = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.IsNil), Type.EmptyTypes);
            public static readonly MethodInfo ReadNextBlock = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.ReadNextBlock), Type.EmptyTypes);
            public static readonly MethodInfo ReadArrayHeader = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.ReadArrayHeader), Type.EmptyTypes);
            public static readonly MethodInfo ReadMapHeader = typeof(MessagePackReader).GetRuntimeMethod(nameof(MessagePackReader.ReadMapHeader), Type.EmptyTypes);

            static MessagePackBinaryTypeInfo() { }
        }

        internal static class EmitInfo
        {
            public static readonly MethodInfo GetTypeFromHandle = ExpressionUtility.GetMethodInfo(() => Type.GetTypeFromHandle(default(RuntimeTypeHandle)));
            public static readonly MethodInfo TypeGetProperty = ExpressionUtility.GetMethodInfo((Type t) => t.GetProperty(default(string), default(BindingFlags)));
            public static readonly MethodInfo TypeGetField = ExpressionUtility.GetMethodInfo((Type t) => t.GetField(default(string), default(BindingFlags)));
#if DEPENDENT_ON_CUTEANT
            public static readonly MethodInfo GetCustomAttributeMessagePackFormatterAttribute = ExpressionUtility.GetMethodInfo(() => AttributeX.GetCustomAttributeX<MessagePackFormatterAttribute>(default(MemberInfo), default(bool)));
#else
            public static readonly MethodInfo GetCustomAttributeMessagePackFormatterAttribute = ExpressionUtility.GetMethodInfo(() => CustomAttributeExtensions.GetCustomAttribute<MessagePackFormatterAttribute>(default(MemberInfo), default(bool)));
#endif
            public static readonly MethodInfo ActivatorCreateInstance = ExpressionUtility.GetMethodInfo(() => Activator.CreateInstance(default(Type), default(object[]))); // 注意：这儿不能使用 Activator.CreateInstance

            internal static class MessagePackFormatterAttr
            {
                internal static readonly MethodInfo FormatterType = ExpressionUtility.GetPropertyInfo((MessagePackFormatterAttribute attr) => attr.FormatterType).GetGetMethod();
                internal static readonly MethodInfo Arguments = ExpressionUtility.GetPropertyInfo((MessagePackFormatterAttribute attr) => attr.Arguments).GetGetMethod();
            }
        }

        class DeserializeInfo
        {
            public ObjectSerializationInfo.EmittableMember MemberInfo { get; set; }
            public LocalBuilder LocalField { get; set; }
            public Label SwitchLabel { get; set; }
        }
    }

    internal delegate void AnonymousSerializeFunc<T>(byte[][] stringByteKeysField, object[] customFormatters, ref MessagePackWriter writer, ref int idx, T value, IFormatterResolver resolver);
    internal delegate T AnonymousDeserializeFunc<T>(object[] customFormatters, ref MessagePackReader reader, IFormatterResolver resolver);

    internal class AnonymousSerializableFormatter<T> : IMessagePackFormatter<T>
    {
        readonly byte[][] stringByteKeysField;
        readonly object[] serializeCustomFormatters;
        readonly object[] deserializeCustomFormatters;
        readonly AnonymousSerializeFunc<T> serialize;
        readonly AnonymousDeserializeFunc<T> deserialize;

        public AnonymousSerializableFormatter(byte[][] stringByteKeysField, object[] serializeCustomFormatters, object[] deserializeCustomFormatters, AnonymousSerializeFunc<T> serialize, AnonymousDeserializeFunc<T> deserialize)
        {
            this.stringByteKeysField = stringByteKeysField;
            this.serializeCustomFormatters = serializeCustomFormatters;
            this.deserializeCustomFormatters = deserializeCustomFormatters;
            this.serialize = serialize;
            this.deserialize = deserialize;
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, T value, IFormatterResolver formatterResolver)
        {
            if (serialize == null) ThrowHelper.ThrowInvalidOperationException_NotSupport_Serialize(this.GetType());
            serialize(stringByteKeysField, serializeCustomFormatters, ref writer, ref idx, value, formatterResolver);
        }

        public T Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (deserialize == null) ThrowHelper.ThrowInvalidOperationException_NotSupport_Deserialize(this.GetType());
            return deserialize(deserializeCustomFormatters, ref reader, formatterResolver);
        }
    }

    internal class ObjectSerializationInfo
    {
        public Type Type { get; set; }
        public bool IsIntKey { get; set; }
        public bool IsStringKey { get { return !IsIntKey; } }
        public bool IsClass { get; set; }
        public bool IsStruct { get { return !IsClass; } }
        public ConstructorInfo BestmatchConstructor { get; set; }
        public EmittableMember[] ConstructorParameters { get; set; }
        public EmittableMember[] Members { get; set; }

        ObjectSerializationInfo() { }

        public static ObjectSerializationInfo CreateOrNull(Type type, bool forceStringKey, bool contractless, bool allowPrivate)
        {
            var isClass = type.IsClass || type.IsInterface || type.IsAbstract;

#if DEPENDENT_ON_CUTEANT
            var contractAttr = type.GetCustomAttributeX<MessagePackObjectAttribute>();
            var dataContractAttr = type.GetCustomAttributeX<DataContractAttribute>();
#else
            var contractAttr = type.GetCustomAttributes<MessagePackObjectAttribute>().FirstOrDefault();
            var dataContractAttr = type.GetCustomAttribute<DataContractAttribute>();
#endif
            if (contractAttr == null && dataContractAttr == null && !forceStringKey && !contractless)
            {
                return null;
            }

            var isIntKey = true;
            var intMembers = new Dictionary<int, EmittableMember>();
            var stringMembers = new Dictionary<string, EmittableMember>();

            if (forceStringKey || contractless || (contractAttr != null && contractAttr.KeyAsPropertyName))
            {
                // All public members are serialize target except [Ignore] member.
                isIntKey = !(forceStringKey || (contractAttr != null && contractAttr.KeyAsPropertyName));

                var hiddenIntKey = 0;
                foreach (var item in type.GetRuntimeProperties())
                {
#if DEPENDENT_ON_CUTEANT
                    if (item.GetCustomAttributeX<IgnoreMemberAttribute>(true) != null) continue;
                    if (item.GetCustomAttributeX<IgnoreDataMemberAttribute>(true) != null) continue;
#else
                    if (item.GetCustomAttribute<IgnoreMemberAttribute>(true) != null) continue;
                    if (item.GetCustomAttribute<IgnoreDataMemberAttribute>(true) != null) continue;
#endif
                    if (item.IsIndexer()) continue;

                    var getMethod = item.GetGetMethod(true);
                    var setMethod = item.GetSetMethod(true);

                    var member = new EmittableMember
                    {
                        PropertyInfo = item,
                        IsReadable = (getMethod != null) && (allowPrivate || getMethod.IsPublic) && !getMethod.IsStatic,
                        IsWritable = (setMethod != null) && (allowPrivate || setMethod.IsPublic) && !setMethod.IsStatic,
                        StringKey = item.Name
                    };
                    if (!member.IsReadable && !member.IsWritable) continue;
                    member.IntKey = hiddenIntKey++;
                    if (isIntKey)
                    {
                        intMembers.Add(member.IntKey, member);
                    }
                    else
                    {
                        stringMembers.Add(member.StringKey, member);
                    }
                }
                foreach (var item in type.GetRuntimeFields())
                {
#if DEPENDENT_ON_CUTEANT
                    if (item.GetCustomAttributeX<IgnoreMemberAttribute>(true) != null) continue;
                    if (item.GetCustomAttributeX<IgnoreDataMemberAttribute>(true) != null) continue;
                    if (item.GetCustomAttributeX<System.Runtime.CompilerServices.CompilerGeneratedAttribute>(true) != null) continue;
#else
                    if (item.GetCustomAttribute<IgnoreMemberAttribute>(true) != null) continue;
                    if (item.GetCustomAttribute<IgnoreDataMemberAttribute>(true) != null) continue;
                    if (item.GetCustomAttribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>(true) != null) continue;
#endif
                    if (item.IsStatic) continue;

                    var member = new EmittableMember
                    {
                        FieldInfo = item,
                        IsReadable = allowPrivate || item.IsPublic,
                        IsWritable = allowPrivate || (item.IsPublic && !item.IsInitOnly),
                        StringKey = item.Name
                    };
                    if (!member.IsReadable && !member.IsWritable) continue;
                    member.IntKey = hiddenIntKey++;
                    if (isIntKey)
                    {
                        intMembers.Add(member.IntKey, member);
                    }
                    else
                    {
                        stringMembers.Add(member.StringKey, member);
                    }
                }
            }
            else
            {
                // Public members with KeyAttribute except [Ignore] member.
                var searchFirst = true;
                var hiddenIntKey = 0;

                foreach (var item in type.GetRuntimeProperties())
                {
#if DEPENDENT_ON_CUTEANT
                    if (item.GetCustomAttributeX<IgnoreMemberAttribute>(true) != null) continue;
                    if (item.GetCustomAttributeX<IgnoreDataMemberAttribute>(true) != null) continue;
#else
                    if (item.GetCustomAttribute<IgnoreMemberAttribute>(true) != null) continue;
                    if (item.GetCustomAttribute<IgnoreDataMemberAttribute>(true) != null) continue;
#endif
                    if (item.IsIndexer()) continue;

                    var getMethod = item.GetGetMethod(true);
                    var setMethod = item.GetSetMethod(true);

                    var member = new EmittableMember
                    {
                        PropertyInfo = item,
                        IsReadable = (getMethod != null) && (allowPrivate || getMethod.IsPublic) && !getMethod.IsStatic,
                        IsWritable = (setMethod != null) && (allowPrivate || setMethod.IsPublic) && !setMethod.IsStatic,
                    };
                    if (!member.IsReadable && !member.IsWritable) continue;

                    KeyAttribute key;
                    if (contractAttr != null)
                    {
                        // MessagePackObjectAttribute
#if DEPENDENT_ON_CUTEANT
                        key = item.GetCustomAttributeX<KeyAttribute>(true);
#else
                        key = item.GetCustomAttribute<KeyAttribute>(true);
#endif
                        if (key == null)
                        {
                            ThrowHelper.ThrowDynamicObjectResolverException_Property_Key(type, item);
                        }

                        if (key.IntKey == null && key.StringKey == null) ThrowHelper.ThrowDynamicObjectResolverException_Property_KeyAreNull(type, item);
                    }
                    else
                    {
                        // DataContractAttribute
#if DEPENDENT_ON_CUTEANT
                        var pseudokey = item.GetCustomAttributeX<DataMemberAttribute>(true);
#else
                        var pseudokey = item.GetCustomAttribute<DataMemberAttribute>(true);
#endif
                        if (pseudokey == null)
                        {
                            // This member has no DataMemberAttribute nor IgnoreMemberAttribute.
                            // But the type *did* have a DataContractAttribute on it, so no attribute implies the member should not be serialized.
                            continue;
                            //ThrowHelper.ThrowDynamicObjectResolverException_Property_DataMem(type, item);
                        }

                        // use Order first
                        if (pseudokey.Order != -1)
                        {
                            key = new KeyAttribute(pseudokey.Order);
                        }
                        else if (pseudokey.Name != null)
                        {
                            key = new KeyAttribute(pseudokey.Name);
                        }
                        else
                        {
                            key = new KeyAttribute(item.Name); // use property name
                        }
                    }

                    if (searchFirst)
                    {
                        searchFirst = false;
                        isIntKey = key.IntKey != null;
                    }
                    else
                    {
                        if ((isIntKey && key.IntKey == null) || (!isIntKey && key.StringKey == null))
                        {
                            ThrowHelper.ThrowDynamicObjectResolverException_Property_Same(type, item);
                        }
                    }

                    if (isIntKey)
                    {
                        member.IntKey = key.IntKey.Value;
                        if (intMembers.ContainsKey(member.IntKey)) ThrowHelper.ThrowDynamicObjectResolverException_Property_Duplicated(type, item);

                        intMembers.Add(member.IntKey, member);
                    }
                    else
                    {
                        member.StringKey = key.StringKey;
                        if (stringMembers.ContainsKey(member.StringKey)) ThrowHelper.ThrowDynamicObjectResolverException_Property_Duplicated(type, item);

                        member.IntKey = hiddenIntKey++;
                        stringMembers.Add(member.StringKey, member);
                    }
                }

                foreach (var item in type.GetRuntimeFields())
                {
#if DEPENDENT_ON_CUTEANT
                    if (item.GetCustomAttributeX<IgnoreMemberAttribute>(true) != null) continue;
                    if (item.GetCustomAttributeX<IgnoreDataMemberAttribute>(true) != null) continue;
                    if (item.GetCustomAttributeX<System.Runtime.CompilerServices.CompilerGeneratedAttribute>(true) != null) continue;
#else
                    if (item.GetCustomAttribute<IgnoreMemberAttribute>(true) != null) continue;
                    if (item.GetCustomAttribute<IgnoreDataMemberAttribute>(true) != null) continue;
                    if (item.GetCustomAttribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>(true) != null) continue;
#endif
                    if (item.IsStatic) continue;

                    var member = new EmittableMember
                    {
                        FieldInfo = item,
                        IsReadable = allowPrivate || item.IsPublic,
                        IsWritable = allowPrivate || (item.IsPublic && !item.IsInitOnly),
                    };
                    if (!member.IsReadable && !member.IsWritable) continue;

                    KeyAttribute key;
                    if (contractAttr != null)
                    {
                        // MessagePackObjectAttribute
#if DEPENDENT_ON_CUTEANT
                        key = item.GetCustomAttributeX<KeyAttribute>(true);
#else
                        key = item.GetCustomAttribute<KeyAttribute>(true);
#endif
                        if (key == null)
                        {
                            ThrowHelper.ThrowDynamicObjectResolverException_Field_Key(type, item);
                        }

                        if (key.IntKey == null && key.StringKey == null) ThrowHelper.ThrowDynamicObjectResolverException_Field_KeyAreNull(type, item);
                    }
                    else
                    {
                        // DataContractAttribute
#if DEPENDENT_ON_CUTEANT
                        var pseudokey = item.GetCustomAttributeX<DataMemberAttribute>(true);
#else
                        var pseudokey = item.GetCustomAttribute<DataMemberAttribute>(true);
#endif
                        if (pseudokey == null)
                        {
                            // This member has no DataMemberAttribute nor IgnoreMemberAttribute.
                            // But the type *did* have a DataContractAttribute on it, so no attribute implies the member should not be serialized.
                            continue;
                            //ThrowHelper.ThrowDynamicObjectResolverException_Field_DataMem(type, item);
                        }

                        // use Order first
                        if (pseudokey.Order != -1)
                        {
                            key = new KeyAttribute(pseudokey.Order);
                        }
                        else if (pseudokey.Name != null)
                        {
                            key = new KeyAttribute(pseudokey.Name);
                        }
                        else
                        {
                            key = new KeyAttribute(item.Name); // use property name
                        }
                    }

                    if (searchFirst)
                    {
                        searchFirst = false;
                        isIntKey = key.IntKey != null;
                    }
                    else
                    {
                        if ((isIntKey && key.IntKey == null) || (!isIntKey && key.StringKey == null))
                        {
                            ThrowHelper.ThrowDynamicObjectResolverException_Field_Same(type, item);
                        }
                    }

                    if (isIntKey)
                    {
                        member.IntKey = key.IntKey.Value;
                        if (intMembers.ContainsKey(member.IntKey)) ThrowHelper.ThrowDynamicObjectResolverException_Field_Duplicated(type, item);

                        intMembers.Add(member.IntKey, member);
                    }
                    else
                    {
                        member.StringKey = key.StringKey;
                        if (stringMembers.ContainsKey(member.StringKey)) ThrowHelper.ThrowDynamicObjectResolverException_Field_Duplicated(type, item);

                        member.IntKey = hiddenIntKey++;
                        stringMembers.Add(member.StringKey, member);
                    }
                }
            }

            // GetConstructor
            IEnumerator<ConstructorInfo> ctorEnumerator = null;
            var declaredConstructors = type.GetTypeInfo().DeclaredConstructors;
#if DEPENDENT_ON_CUTEANT
            var ctor = declaredConstructors.Where(x => x.IsPublic).SingleOrDefault(x => x.GetCustomAttributeX<SerializationConstructorAttribute>(false) != null);
#else
            var ctor = declaredConstructors.Where(x => x.IsPublic).SingleOrDefault(x => x.GetCustomAttribute<SerializationConstructorAttribute>(false) != null);
#endif
            if (ctor == null)
            {
                ctorEnumerator =
                    declaredConstructors.Where(x => x.IsPublic).OrderBy(x => x.GetParameters().Length)
                    .GetEnumerator();

                if (ctorEnumerator.MoveNext())
                {
                    ctor = ctorEnumerator.Current;
                }
            }
            // struct allows null ctor
            if (ctor == null && isClass) ThrowHelper.ThrowDynamicObjectResolverException_Ctor_None(type);

            var constructorParameters = new List<EmittableMember>();
            if (ctor != null)
            {
                var constructorLookupDictionary = stringMembers.ToLookup(x => x.Key, x => x, StringComparer.OrdinalIgnoreCase);
                do
                {
                    constructorParameters.Clear();
                    var ctorParamIndex = 0;
                    foreach (var item in ctor.GetParameters())
                    {
                        EmittableMember paramMember;
                        if (isIntKey)
                        {
                            if (intMembers.TryGetValue(ctorParamIndex, out paramMember))
                            {
                                if (paramMember.IsReadable && IsSimilarType(item.ParameterType, paramMember.Type))
                                {
                                    constructorParameters.Add(paramMember);
                                }
                                else
                                {
                                    if (ctorEnumerator != null)
                                    {
                                        ctor = null;
                                        continue;
                                    }
                                    else
                                    {
                                        ThrowHelper.ThrowDynamicObjectResolverException_Ctor_ParamType_Mismatch(type, ctorParamIndex, item);
                                    }
                                }
                            }
                            else
                            {
                                if (ctorEnumerator != null)
                                {
                                    ctor = null;
                                    continue;
                                }
                                else
                                {
                                    ThrowHelper.ThrowDynamicObjectResolverException_Ctor_Index_NotFound(type, ctorParamIndex);
                                }
                            }
                        }
                        else
                        {
                            var hasKey = constructorLookupDictionary[item.Name];
                            var len = hasKey.Count();
                            if (len != 0)
                            {
                                if (len != 1)
                                {
                                    if (ctorEnumerator != null)
                                    {
                                        ctor = null;
                                        continue;
                                    }
                                    else
                                    {
                                        ThrowHelper.ThrowDynamicObjectResolverException_Ctor_Duplicate_Matched(type, item);
                                    }
                                }

                                paramMember = hasKey.First().Value;
                                if (paramMember.IsReadable && IsSimilarType(item.ParameterType, paramMember.Type))
                                {
                                    constructorParameters.Add(paramMember);
                                }
                                else
                                {
                                    if (ctorEnumerator != null)
                                    {
                                        ctor = null;
                                        continue;
                                    }
                                    else
                                    {
                                        ThrowHelper.ThrowDynamicObjectResolverException_Ctor_NonMatched_Param(type, item);
                                    }
                                }
                            }
                            else
                            {
                                if (ctorEnumerator != null)
                                {
                                    ctor = null;
                                    continue;
                                }
                                else
                                {
                                    ThrowHelper.ThrowDynamicObjectResolverException_Ctor_Index_NotFound(type, item);
                                }
                            }
                        }
                        ctorParamIndex++;
                    }
                } while (TryGetNextConstructor(ctorEnumerator, ref ctor));

                if (ctor == null)
                {
                    ThrowHelper.ThrowDynamicObjectResolverException_Ctor_NonMatched(type);
                }
            }

            EmittableMember[] members;
            if (isIntKey)
            {
                members = intMembers.Values.OrderBy(x => x.IntKey).ToArray();
            }
            else
            {
                members = stringMembers.Values
                    .OrderBy(x =>
                    {
                        var attr = x.GetDataMemberAttribute();
                        if (attr == null) return int.MaxValue;
                        return attr.Order;
                    })
                    .ToArray();
            }

            return new ObjectSerializationInfo
            {
                Type = type,
                IsClass = isClass,
                BestmatchConstructor = ctor,
                ConstructorParameters = constructorParameters.ToArray(),
                IsIntKey = isIntKey,
                Members = members,
            };
        }

        static bool IsSimilarType(Type thisType, Type type)
        {
            // Ignore any 'ref' types
            if (thisType.IsByRef) { thisType = thisType.GetElementType(); }
            if (type.IsByRef) { type = type.GetElementType(); }

            // Handle array types
            if (thisType.IsArray && type.IsArray)
            {
                return IsSimilarType(thisType.GetElementType(), type.GetElementType());
            }

            // If the types are identical, or they're both generic parameters
            if (thisType == type ||
                thisType.IsGenericParameter ||
                type.IsGenericParameter ||
                thisType.IsAssignableFrom(type))
            {
                return true;
            }

            // Handle any generic arguments
            if (thisType.IsGenericType && type.IsGenericType)
            {
                Type[] thisArguments = thisType.GetGenericArguments();
                Type[] arguments = type.GetGenericArguments();
                if (thisArguments.Length == arguments.Length)
                {
                    for (int i = 0; i < thisArguments.Length; ++i)
                    {
                        if (!IsSimilarType(thisArguments[i], arguments[i])) { return false; }
                    }
                    return true;
                }
            }

            return false;
        }

        static bool TryGetNextConstructor(IEnumerator<ConstructorInfo> ctorEnumerator, ref ConstructorInfo ctor)
        {
            if (ctorEnumerator == null || ctor != null)
            {
                return false;
            }

            if (ctorEnumerator.MoveNext())
            {
                ctor = ctorEnumerator.Current;
                return true;
            }
            else
            {
                ctor = null;
                return false;
            }
        }

        public class EmittableMember
        {
            public bool IsProperty { get { return PropertyInfo != null; } }
            public bool IsField { get { return FieldInfo != null; } }
            public bool IsWritable { get; set; }
            public bool IsReadable { get; set; }
            public int IntKey { get; set; }
            public string StringKey { get; set; }
            public Type Type { get { return IsField ? FieldInfo.FieldType : PropertyInfo.PropertyType; } }
            public FieldInfo FieldInfo { get; set; }
            public PropertyInfo PropertyInfo { get; set; }

            public string Name
            {
                get
                {
                    return IsProperty ? PropertyInfo.Name : FieldInfo.Name;
                }
            }

            public bool IsValueType
            {
                get
                {
                    var mi = IsProperty ? (MemberInfo)PropertyInfo : FieldInfo;
                    return mi.DeclaringType.IsValueType;
                }
            }

#if DEPENDENT_ON_CUTEANT
            public MessagePackFormatterAttribute GetMessagePackFormatterAttribute()
            {
                if (IsProperty)
                {
                    return PropertyInfo.GetCustomAttributeX<MessagePackFormatterAttribute>(true);
                }
                else
                {
                    return FieldInfo.GetCustomAttributeX<MessagePackFormatterAttribute>(true);
                }
            }

            public DataMemberAttribute GetDataMemberAttribute()
            {
                if (IsProperty)
                {
                    return PropertyInfo.GetCustomAttributeX<DataMemberAttribute>(true);
                }
                else
                {
                    return FieldInfo.GetCustomAttributeX<DataMemberAttribute>(true);
                }
            }
#else
            public MessagePackFormatterAttribute GetMessagePackFormatterAttribute()
            {
                if (IsProperty)
                {
                    return (MessagePackFormatterAttribute)PropertyInfo.GetCustomAttribute<MessagePackFormatterAttribute>(true);
                }
                else
                {
                    return (MessagePackFormatterAttribute)FieldInfo.GetCustomAttribute<MessagePackFormatterAttribute>(true);
                }
            }

            public DataMemberAttribute GetDataMemberAttribute()
            {
                if (IsProperty)
                {
                    return (DataMemberAttribute)PropertyInfo.GetCustomAttribute<DataMemberAttribute>(true);
                }
                else
                {
                    return (DataMemberAttribute)FieldInfo.GetCustomAttribute<DataMemberAttribute>(true);
                }
            }
#endif

            public void EmitLoadValue(ILGenerator il)
            {
                if (IsProperty)
                {
                    il.EmitCall(PropertyInfo.GetGetMethod(true));
                }
                else
                {
                    il.Emit(OpCodes.Ldfld, FieldInfo);
                }
            }

            public void EmitStoreValue(ILGenerator il)
            {
                if (IsProperty)
                {
                    il.EmitCall(PropertyInfo.GetSetMethod(true));
                }
                else
                {
                    il.Emit(OpCodes.Stfld, FieldInfo);
                }
            }

            public object ReflectionLoadValue(object value)
            {
                if (IsProperty)
                {
                    return PropertyInfo.GetValue(value);
                }
                else
                {
                    return FieldInfo.GetValue(value);
                }
            }

            public void ReflectionStoreValue(object obj, object value)
            {
                if (IsProperty)
                {
                    PropertyInfo.SetValue(obj, value);
                }
                else
                {
                    FieldInfo.SetValue(obj, value);
                }
            }
        }
    }

    internal class MessagePackDynamicObjectResolverException : Exception
    {
        public MessagePackDynamicObjectResolverException(string message) : base(message) { }
    }
}
