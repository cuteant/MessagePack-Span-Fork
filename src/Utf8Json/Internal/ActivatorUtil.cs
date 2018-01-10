using System;
using System.Runtime.CompilerServices;
using CuteAnt.Reflection;

namespace Utf8Json
{
    internal class ActivatorUtil
    {
        [MethodImpl(InlineMethod.Value)]
        public static object CreateInstance(Type type, params object[] args)
        {
            if (null == args || args.Length == 0)
            {
                return ActivatorUtils.FastCreateInstance(type);
            }
            else
            {
                return Activator.CreateInstance(type, args);
            }
        }
    }
}
