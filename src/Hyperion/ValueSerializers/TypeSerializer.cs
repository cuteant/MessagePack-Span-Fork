#region copyright
// -----------------------------------------------------------------------
//  <copyright file="TypeSerializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;
using CuteAnt;
using CuteAnt.Reflection;
using Hyperion.Extensions;
using AntTypeSerializer = CuteAnt.Reflection.TypeSerializer;

namespace Hyperion.ValueSerializers
{
    internal sealed class TypeSerializer : ValueSerializer
    {
        public const byte Manifest = 16;
        public static readonly TypeSerializer Instance = new TypeSerializer();

        public override void WriteManifest(Stream stream, SerializerSession session)
        {
            ushort typeIdentifier;
            if (session.ShouldWriteTypeManifest(TypeEx.RuntimeType, out typeIdentifier))
            {
                stream.WriteByte(Manifest);
            }
            else
            {
                stream.Write(new[] { ObjectSerializer.ManifestIndex });
                UInt16Serializer.WriteValueImpl(stream, typeIdentifier, session);
            }
        }

        public override void WriteValue(Stream stream, object value, SerializerSession session)
        {
            if (value == null)
            {
                stream.WriteLengthEncodedByteArray(EmptyArray<byte>.Instance, session);
            }
            else
            {
                var type = (Type)value;
                if (session.Serializer.Options.PreserveObjectReferences && session.TryGetObjectId(type, out int existingId))
                {
                    ObjectReferenceSerializer.Instance.WriteManifest(stream, session);
                    ObjectReferenceSerializer.Instance.WriteValue(stream, existingId, session);
                }
                else
                {
                    if (session.Serializer.Options.PreserveObjectReferences)
                    {
                        session.TrackSerializedObject(type);
                    }
                    //type was not written before, add it to the tacked object list
                    var typeKey = AntTypeSerializer.GetTypeKeyFromType(type);
                    stream.WriteLengthEncodedByteArray(typeKey.TypeName, session);
                    Int32Serializer.WriteValueImpl(stream, typeKey.HashCode, session);
                }
            }
        }

        public override object ReadValue(Stream stream, DeserializerSession session)
        {
            var typeName = stream.ReadLengthEncodedByteArray(session);
            if (typeName.Length == 0) { return null; }
            var hashCode = stream.ReadInt32(session);
            var type = AntTypeSerializer.GetTypeFromTypeKey(new TypeKey(hashCode, typeName));

            //add the deserialized type to lookup
            if (session.Serializer.Options.PreserveObjectReferences)
            {
                session.TrackDeserializedObject(type);
            }
            return type;
        }

        public override Type GetElementType()
        {
            return typeof(Type);
        }
    }
}