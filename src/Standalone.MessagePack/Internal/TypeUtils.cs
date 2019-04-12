namespace MessagePack.Internal
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>A collection of utility functions for dealing with Type information.</summary>
    internal static class TypeUtils
    {
        private static readonly CachedReadConcurrentDictionary<string, Type> _resolveTypeCache =
            new CachedReadConcurrentDictionary<string, Type>(DictionaryCacheConstants.SIZE_MEDIUM, StringComparer.Ordinal)
            {
                { "null", (Type)null }
            };
        private static readonly CachedReadConcurrentDictionary<string, Assembly> _assemblyCache =
            new CachedReadConcurrentDictionary<string, Assembly>(StringComparer.Ordinal);

        public static Type ResolveType(string qualifiedTypeName)
        {
            if (!TryResolveType(qualifiedTypeName, out var result))
            {
                ThrowTypeAccessException(qualifiedTypeName);
            }
            return result;
        }

        /// <summary>Gets <see cref="Type"/> by full name (with falling back to the first part only).</summary>
        /// <param name="qualifiedTypeName">The type name.</param>
        /// <param name="type"></param>
        /// <returns>The <see cref="Type"/> if valid.</returns>
        public static bool TryResolveType(string qualifiedTypeName, out Type type)
        {
            if (string.IsNullOrWhiteSpace(qualifiedTypeName))
            {
                type = null; return false;
            }

            if (_resolveTypeCache.TryGetValue(qualifiedTypeName, out type)) { return true; }

            type = InternalResolveType(qualifiedTypeName);
            return type != null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Type InternalResolveType(string qualifiedTypeName)
        {
            TryPerformUncachedTypeResolution(qualifiedTypeName, out var type);
            return type;
        }

        private static bool TryPerformUncachedTypeResolution(string qualifiedTypeName, out Type type)
        {
            string assemblyName = ParseAssemblyName(qualifiedTypeName);

            Assembly[] allAssemblies;
            if (assemblyName != null)
            {
                Assembly assembly = null;

                //assembly = Assembly.Load(assemblyName);
                try
                {
                    // look, I don't like using obsolete methods as much as you do but this is the only way
                    // Assembly.Load won't check the GAC for a partial name
#pragma warning disable 618, 612
                    assembly = Assembly.LoadWithPartialName(assemblyName);
#pragma warning restore 618, 612
                }
                catch { }

                if (assembly == null)
                {
                    // will find assemblies loaded with Assembly.LoadFile outside of the main directory
                    Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly asm in loadedAssemblies)
                    {
                        // check for both full name or partial name match
                        if (string.Equals(asm.FullName, assemblyName, StringComparison.Ordinal) ||
                            string.Equals(asm.GetName().Name, assemblyName, StringComparison.Ordinal))
                        {
                            assembly = asm;
                            break;
                        }
                    }
                }

                if (assembly != null)
                {
                    type = assembly.GetType(qualifiedTypeName, false);
                    if (type != null) { return true; }
                }

                allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            }
            else
            {
                allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                if (TryResolveFromAllAssemblies(qualifiedTypeName, out type, allAssemblies)) { return true; }
            }

            type = Type.GetType(qualifiedTypeName, throwOnError: false)
                ?? Type.GetType(qualifiedTypeName, ResolveAssembly, ResolveType, false);

            return type != null;

            Assembly ResolveAssembly(AssemblyName asmName)
            {
                var fullAssemblyName = asmName.FullName;
                if (_assemblyCache.TryGetValue(fullAssemblyName, out var result)) return result;

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var name = assembly.GetName();
                    _assemblyCache[name.FullName] = assembly;
                    _assemblyCache[name.Name] = assembly;
                }
                if (_assemblyCache.TryGetValue(fullAssemblyName, out result)) return result;

                result = Assembly.Load(asmName);
                var resultName = result.GetName();
                _assemblyCache[resultName.Name] = result;
                _assemblyCache[resultName.FullName] = result;
                return result;
            }

            Type ResolveType(Assembly asm, string name, bool ignoreCase)
            {
                //if (TryResolveFromAllAssemblies(name, out var result, allAssemblies)) { return result; }
                return asm?.GetType(name, throwOnError: false, ignoreCase: ignoreCase) ?? Type.GetType(name, throwOnError: false, ignoreCase: ignoreCase);
            }
        }

        private static string Combine(string typeName, string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName)) { return typeName; }

            return $"{typeName}, {assemblyName}";
        }

        private static bool TryResolveFromAllAssemblies(string fullName, out Type type, Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                type = assembly.GetType(fullName, false);
                if (type != null) { return true; }
            }

            type = null;
            return false;
        }

        internal static string ParseAssemblyName(string fullyQualifiedTypeName)
        {
            int? assemblyDelimiterIndex = GetAssemblyDelimiterIndex(fullyQualifiedTypeName);

            string assemblyName;

            if (assemblyDelimiterIndex != null)
            {
                assemblyName = fullyQualifiedTypeName.Trim(assemblyDelimiterIndex.GetValueOrDefault() + 1, fullyQualifiedTypeName.Length - assemblyDelimiterIndex.GetValueOrDefault() - 1);
            }
            else
            {
                assemblyName = null;
            }

            return assemblyName;
        }

        private static string Trim(this string s, int start, int length)
        {
            // References: https://referencesource.microsoft.com/#mscorlib/system/string.cs,2691
            // https://referencesource.microsoft.com/#mscorlib/system/string.cs,1226
            if (s == null) { throw new ArgumentNullException(); }
            if (start < 0) { throw new ArgumentOutOfRangeException(nameof(start)); }
            if (length < 0) { throw new ArgumentOutOfRangeException(nameof(length)); }
            int end = start + length - 1;
            if (end >= s.Length) { throw new ArgumentOutOfRangeException(nameof(length)); }
            for (; start < end; start++)
            {
                if (!char.IsWhiteSpace(s[start])) { break; }
            }
            for (; end >= start; end--)
            {
                if (!char.IsWhiteSpace(s[end])) { break; }
            }
            return s.Substring(start, end - start + 1);
        }

        private static int? GetAssemblyDelimiterIndex(string fullyQualifiedTypeName)
        {
            // we need to get the first comma following all surrounded in brackets because of generic types
            // e.g. System.Collections.Generic.Dictionary`2[[System.String, mscorlib,Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
            int scope = 0;
            for (int i = 0; i < fullyQualifiedTypeName.Length; i++)
            {
                char current = fullyQualifiedTypeName[i];
                switch (current)
                {
                    case '[':
                        scope++;
                        break;
                    case ']':
                        scope--;
                        break;
                    case ',':
                        if (scope == 0)
                        {
                            return i;
                        }
                        break;
                }
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowTypeAccessException(string qualifiedTypeName)
        {
            throw GetTypeAccessException();

            TypeAccessException GetTypeAccessException()
            {
                return new TypeAccessException($"Unable to find a type named {qualifiedTypeName}");
            }
        }

        /// <summary>Returns the <see cref="MethodInfo"/> for the simple method call in the provided <paramref name="expression"/>.</summary>
        /// <typeparam name="T">The containing type of the method.</typeparam>
        /// <typeparam name="TResult">The return type of the method.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>The <see cref="MethodInfo"/> for the simple method call in the provided <paramref name="expression"/>.</returns>
        public static MethodInfo Method<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            var methodCall = expression.Body as MethodCallExpression;
            if (null == methodCall) { ThrowArgumentException_Expr(); }
            return methodCall.Method;
        }

        /// <summary>Returns the <see cref="MethodInfo"/> for the simple method call in the provided <paramref name="expression"/>.</summary>
        /// <typeparam name="T">The containing type of the method.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>The <see cref="MethodInfo"/> for the simple method call in the provided <paramref name="expression"/>.</returns>
        public static MethodInfo Method<T>(Expression<Func<T>> expression)
        {
            var methodCall = expression.Body as MethodCallExpression;
            if (null == methodCall) { ThrowArgumentException_Expr(); }
            return methodCall.Method;
        }

        /// <summary>Returns the <see cref="MethodInfo"/> for the simple method call in the provided <paramref name="expression"/>.</summary>
        /// <typeparam name="T">The containing type of the method.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>The <see cref="MethodInfo"/> for the simple method call in the provided <paramref name="expression"/>.</returns>
        public static MethodInfo Method<T>(Expression<Action<T>> expression)
        {
            var methodCall = expression.Body as MethodCallExpression;
            if (null == methodCall) { ThrowArgumentException_Expr(); }
            return methodCall.Method;
        }

        /// <summary>Returns the <see cref="MethodInfo"/> for the simple method call in the provided <paramref name="expression"/>.</summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The <see cref="MethodInfo"/> for the simple method call in the provided <paramref name="expression"/>.</returns>
        public static MethodInfo Method(Expression<Action> expression)
        {
            var methodCall = expression.Body as MethodCallExpression;
            if (null == methodCall) { ThrowArgumentException_Expr(); }
            return methodCall.Method;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentException_Expr()
        {
            throw GetArgumentException();

            ArgumentException GetArgumentException()
            {
                return new ArgumentException("Expression type unsupported.");
            }
        }
    }
}
