using System;
using System.Text;
using CuteAnt.Pool;

namespace ServiceStack.Text
{
    /// <summary>
    /// Reusable StringBuilder ThreadStatic Cache
    /// </summary>
    public static class StringBuilderCache
    {
        public static StringBuilder Allocate() => StringBuilderManager.Allocate();

        public static void Free(StringBuilder sb) => StringBuilderManager.Free(sb);

        public static string ReturnAndFree(StringBuilder sb) => StringBuilderManager.ReturnAndFree(sb);
    }

    /// <summary>
    /// Alternative Reusable StringBuilder ThreadStatic Cache
    /// </summary>
    public static class StringBuilderCacheAlt
    {
        public static StringBuilder Allocate() => StringBuilderManager.Allocate();

        public static void Free(StringBuilder sb) => StringBuilderManager.Free(sb);

        public static string ReturnAndFree(StringBuilder sb) => StringBuilderManager.ReturnAndFree(sb);
    }
}