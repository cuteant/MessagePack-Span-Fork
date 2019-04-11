using System;
using System.Linq;
using System.Reflection;

namespace MessagePack.Formatters
{
    public sealed class MethodInfoFormatter : MethodInfoFormatter<MethodInfo>
    {
        public static readonly IMessagePackFormatter<MethodInfo> Instance = new MethodInfoFormatter();

        public MethodInfoFormatter() : base() { }

        public MethodInfoFormatter(bool throwOnError) : base(throwOnError) { }
    }

    public class MethodInfoFormatter<TMethod> : IMessagePackFormatter<TMethod>
        where TMethod : MethodInfo
    {
        private readonly bool _throwOnError;

        public MethodInfoFormatter() : this(true) { }

        public MethodInfoFormatter(bool throwOnError) => _throwOnError = throwOnError;

        public TMethod Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var name = reader.ReadString();
            var declaringType = reader.ReadNamedType(_throwOnError);
            var argumentCount = reader.ReadArrayHeader();
            var parameterTypes = Type.EmptyTypes;
            if (argumentCount > 0)
            {
                parameterTypes = new Type[argumentCount];
                for (var idx = 0; idx < argumentCount; idx++)
                {
                    parameterTypes[idx] = reader.ReadNamedType(_throwOnError);
                }
            }
            var method = GetMethodExt(declaringType, name,
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                parameterTypes);
            if (method.IsGenericMethodDefinition)
            {
                argumentCount = reader.ReadArrayHeader();
                var genericTypeArguments = new Type[argumentCount];
                for (var idx = 0; idx < argumentCount; idx++)
                {
                    genericTypeArguments[idx] = reader.ReadNamedType(_throwOnError);
                }
                method = method.MakeGenericMethod(genericTypeArguments);
            }
            return (TMethod)method;
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, TMethod value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteString(value.Name, ref idx);

            writer.WriteNamedType(value.DeclaringType, ref idx);

            var arguments = value.GetParameters().Select(p => p.ParameterType).ToArray();
            writer.WriteArrayHeader(arguments.Length, ref idx);
            for (int i = 0; i < arguments.Length; i++)
            {
                writer.WriteNamedType(arguments[i], ref idx);
            }
            if (value.IsGenericMethod)
            {
                var genericTypeArguments = value.GetGenericArguments();
                writer.WriteArrayHeader(genericTypeArguments.Length, ref idx);
                for (int i = 0; i < genericTypeArguments.Length; i++)
                {
                    writer.WriteNamedType(genericTypeArguments[i], ref idx);
                }
            }
        }

        /// <summary>Search for a method by name, parameter types, and binding flags.  
        /// Unlike GetMethod(), does 'loose' matching on generic
        /// parameter types, and searches base interfaces.</summary>
        /// <exception cref="AmbiguousMatchException"/>
        public static MethodInfo GetMethodExt(Type thisType,
                                              string name,
                                              BindingFlags bindingFlags,
                                              params Type[] parameterTypes)
        {
            MethodInfo matchingMethod = null;

            // Check all methods with the specified name, including in base classes
            GetMethodExt(ref matchingMethod, thisType, name, bindingFlags, parameterTypes);

            // If we're searching an interface, we have to manually search base interfaces
            if (matchingMethod == null && thisType.IsInterface)
            {
                foreach (Type interfaceType in thisType.GetInterfaces())
                {
                    GetMethodExt(ref matchingMethod,
                        interfaceType,
                        name,
                        bindingFlags,
                        parameterTypes);
                }
            }

            return matchingMethod;
        }

        private static void GetMethodExt(ref MethodInfo matchingMethod,
                                         Type type,
                                         string name,
                                         BindingFlags bindingFlags,
                                         params Type[] parameterTypes)
        {
            // Check all methods with the specified name, including in base classes
            foreach (MethodInfo methodInfo in type.GetMember(name, MemberTypes.Method, bindingFlags))
            {
                // Check that the parameter counts and types match, 
                // with 'loose' matching on generic parameters
                ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                if (parameterInfos.Length == parameterTypes.Length)
                {
                    int i = 0;
                    for (; i < parameterInfos.Length; ++i)
                    {
                        if (!IsSimilarType(parameterInfos[i].ParameterType, parameterTypes[i]))
                        {
                            break;
                        }
                    }
                    if (i == parameterInfos.Length)
                    {
                        if (matchingMethod == null)
                        {
                            matchingMethod = methodInfo;
                        }
                        else
                        {
                            ThrowHelper.ThrowAmbiguousMatchException();
                        }
                    }
                }
            }
        }

        /// <summary>Special type used to match any generic parameter type in GetMethodExt().</summary>
        public class T { }

        /// <summary>Determines if the two types are either identical, or are both generic 
        /// parameters or generic types with generic parameters in the same
        ///  locations (generic parameters match any other generic paramter,
        /// and concrete types).</summary>
        private static bool IsSimilarType(Type thisType, Type type)
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
            // or the special 'T' type, treat as a match
            if (thisType == type || thisType.IsGenericParameter || thisType == typeof(T) || type.IsGenericParameter || type == typeof(T))
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
    }
}
