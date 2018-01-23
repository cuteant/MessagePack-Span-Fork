using System;
using System.Linq;
using System.Reflection;
using CuteAnt.Reflection;

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

        public MethodInfoFormatter(bool throwOnError)
        {
            _throwOnError = throwOnError;
        }

        public TMethod Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            var startOffset = offset;
            var name = MessagePackBinary.ReadString(bytes, offset, out readSize);
            offset += readSize;
            var hashCode = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
            offset += readSize;
            var typeName = MessagePackBinary.ReadBytes(bytes, offset, out readSize);
            offset += readSize;
            var declaringType = TypeSerializer.GetTypeFromTypeKey(new TypeKey(hashCode, typeName), _throwOnError);
            var argumentCount = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
            offset += readSize;
            var parameterTypes = Type.EmptyTypes;
            if (argumentCount > 0)
            {
                parameterTypes = new Type[argumentCount];
                for (var idx = 0; idx < argumentCount; idx++)
                {
                    hashCode = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                    offset += readSize;
                    typeName = MessagePackBinary.ReadBytes(bytes, offset, out readSize);
                    offset += readSize;
                    parameterTypes[idx] = TypeSerializer.GetTypeFromTypeKey(new TypeKey(hashCode, typeName), _throwOnError);
                }
            }
            var method = GetMethodExt(declaringType, name,
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                parameterTypes);
            if (method.IsGenericMethodDefinition)
            {
                argumentCount = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                offset += readSize;
                var genericTypeArguments = new Type[argumentCount];
                for (var idx = 0; idx < argumentCount; idx++)
                {
                    hashCode = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                    offset += readSize;
                    typeName = MessagePackBinary.ReadBytes(bytes, offset, out readSize);
                    offset += readSize;
                    genericTypeArguments[idx] = TypeSerializer.GetTypeFromTypeKey(new TypeKey(hashCode, typeName), _throwOnError);
                }
                method = method.MakeGenericMethod(genericTypeArguments);
            }
            readSize = offset - startOffset;
            return (TMethod)method;
        }

        public int Serialize(ref byte[] bytes, int offset, TMethod value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var declaringType = value.DeclaringType;
            var startOffset = offset;
            offset += MessagePackBinary.WriteString(ref bytes, offset, value.Name);
            var typeKey = TypeSerializer.GetTypeKeyFromType(declaringType);
            offset += MessagePackBinary.WriteInt32(ref bytes, offset, typeKey.HashCode);
            var typeName = typeKey.TypeName;
            offset += MessagePackBinary.WriteBytes(ref bytes, offset, typeName, 0, typeName.Length);
            var arguments = value.GetParameters().Select(p => p.ParameterType).ToArray();
            offset += MessagePackBinary.WriteInt32(ref bytes, offset, arguments.Length);
            for (int idx = 0; idx < arguments.Length; idx++)
            {
                typeKey = TypeSerializer.GetTypeKeyFromType(arguments[idx]);
                offset += MessagePackBinary.WriteInt32(ref bytes, offset, typeKey.HashCode);
                typeName = typeKey.TypeName;
                offset += MessagePackBinary.WriteBytes(ref bytes, offset, typeName, 0, typeName.Length);
            }
            if (value.IsGenericMethod)
            {
                var genericTypeArguments = value.GetGenericArguments();
                offset += MessagePackBinary.WriteInt32(ref bytes, offset, genericTypeArguments.Length);
                for (int idx = 0; idx < genericTypeArguments.Length; idx++)
                {
                    typeKey = TypeSerializer.GetTypeKeyFromType(genericTypeArguments[idx]);
                    offset += MessagePackBinary.WriteInt32(ref bytes, offset, typeKey.HashCode);
                    typeName = typeKey.TypeName;
                    offset += MessagePackBinary.WriteBytes(ref bytes, offset, typeName, 0, typeName.Length);
                }
            }
            return offset - startOffset;
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
            if (matchingMethod == null && thisType.GetTypeInfo().IsInterface)
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
#if NET40
            foreach (MethodInfo methodInfo in type.GetMethods(bindingFlags).Where(_ => string.Equals(_.Name, name, StringComparison.Ordinal)))
#else
            foreach (MethodInfo methodInfo in type.GetTypeInfo().GetMember(name, MemberTypes.Method, bindingFlags))
#endif
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
                            throw new AmbiguousMatchException("More than one matching method found!");
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
            if (thisType.GetTypeInfo().IsGenericType && type.GetTypeInfo().IsGenericType)
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
