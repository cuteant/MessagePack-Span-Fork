#region copyright
// -----------------------------------------------------------------------
//  <copyright file="GuidSerializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;
using CuteAnt;
using Hyperion.Extensions;

namespace Hyperion.ValueSerializers
{
    internal sealed class CombGuidSerializer : SessionIgnorantValueSerializer<CombGuid>
  {
    public const byte Manifest = 168;
    public static readonly CombGuidSerializer Instance = new CombGuidSerializer();

    public CombGuidSerializer() : base(Manifest, () => WriteValueImpl, () => ReadValueImpl)
    {
    }

    public static void WriteValueImpl(Stream stream, CombGuid g)
    {
      var bytes = g.GetByteArray();
      stream.Write(bytes);
    }

    public static CombGuid ReadValueImpl(Stream stream)
    {
      var buffer = new byte[16];
      stream.Read(buffer, 0, 16);
      return new CombGuid(buffer, CombGuidSequentialSegmentType.Guid, true);
    }
  }
}