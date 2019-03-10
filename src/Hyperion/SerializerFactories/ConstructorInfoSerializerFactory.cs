#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ConstructorInfoSerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Linq;
using System.Reflection;
using CuteAnt.Collections;
using Hyperion.Extensions;
using Hyperion.ValueSerializers;

namespace Hyperion.SerializerFactories
{
    internal sealed class ConstructorInfoSerializerFactory : ValueSerializerFactory
    {
        public override bool CanSerialize(Serializer serializer, Type type)
        {
            return type.IsSubclassOf(typeof(ConstructorInfo));
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
                var owner = stream.ReadObject(session) as Type;
                var arguments = stream.ReadObject(session) as Type[];

                var ctor = owner.GetConstructor(arguments);
                return ctor;
            };
            ObjectWriter writer = (stream, obj, session) =>
            {
                var ctor = (ConstructorInfo)obj;
                var owner = ctor.DeclaringType;
                var arguments = ctor.GetParameters().Select(p => p.ParameterType).ToArray();
                stream.WriteObjectWithManifest(owner, session);
                stream.WriteObjectWithManifest(arguments, session);
            };
            os.Initialize(reader, writer);

            return os;
        }
    }
}