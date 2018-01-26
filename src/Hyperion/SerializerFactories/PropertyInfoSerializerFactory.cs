#region copyright
// -----------------------------------------------------------------------
//  <copyright file="PropertyInfoSerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Reflection;
using CuteAnt.Collections;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    internal sealed class PropertyInfoSerializerFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type)
        {
            return type.GetTypeInfo().IsSubclassOf(typeof(PropertyInfo));
        }

        public override bool CanDeserialize(Serializer serializer, Type type)
        {
            return CanSerialize(serializer, type);
        }

        public override ValueSerializer BuildSerializer(Serializer serializer, Type type,
            CachedReadConcurrentDictionary<Type, ValueSerializer> typeMapping)
        {
            var os = new ObjectSerializer(type);
            typeMapping.TryAdd(type, os);
            ObjectReader reader = (stream, session) =>
            {
                var name = stream.ReadString(session);
                var owner = stream.ReadObject(session) as Type;

#if NET40
                var property = owner.GetProperty(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#else
                var property = owner.GetTypeInfo().GetProperty(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#endif
                return property;
            };
            ObjectWriter writer = (stream, obj, session) =>
            {
                var property = (PropertyInfo)obj;
                var name = property.Name;
                var owner = property.DeclaringType;
                StringSerializer.WriteValueImpl(stream, name, session);
                stream.WriteObjectWithManifest(owner, session);
            };
            os.Initialize(reader, writer);

            return os;
        }
    }
}