namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
#if DEPENDENT_ON_CUTEANT
    using CuteAnt.Reflection;
#endif

    internal static class ActivatorUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object CreateInstance(Type type, params object[] args)
        {
            if (null == args || 0 >= (uint)args.Length)
            {
                return ActivatorUtils.FastCreateInstance(type);
            }
            return Activator.CreateInstance(type, args);
        }
    }
}
