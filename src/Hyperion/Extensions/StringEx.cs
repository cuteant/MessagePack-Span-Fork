#region copyright
// -----------------------------------------------------------------------
//  <copyright file="StringEx.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

#if !NET40
using System.Runtime.CompilerServices;
#endif

namespace Hyperion.Extensions
{
  internal static class StringEx
  {
#if !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    internal static byte[] ToUtf8Bytes(this string str)
    {
      return NoAllocBitConverter.Utf8.GetBytes(str);
    }

#if !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    internal static string FromUtf8Bytes(byte[] bytes, int offset, int count)
    {
      return NoAllocBitConverter.Utf8.GetString(bytes, offset, count);
    }
  }
}