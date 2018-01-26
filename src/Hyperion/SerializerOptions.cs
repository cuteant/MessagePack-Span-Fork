#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializerOptions.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CuteAnt;
using Hyperion.SerializerFactories;
using Hyperion.Surrogates;

namespace Hyperion
{
    public class SerializerOptions
    {
        internal static readonly Surrogate[] EmptySurrogates = new Surrogate[0];
        public static readonly Surrogate[] DefaultSurrogates = new Surrogate[]
        {
            //Surrogate.Create<DateTimeOffset, DateTimeOffsetSurrogate>(DateTimeOffsetSurrogate.ToSurrogate, DateTimeOffsetSurrogate.FromSurrogate),
            // 内部序列化 TimeSpan 字节数要小些
            //Surrogate.Create<TimeSpan, TimeSpanSurrogate>(TimeSpanSurrogate.ToSurrogate, TimeSpanSurrogate.FromSurrogate),
            Surrogate.Create<Uri, UriSurrogate>(UriSurrogate.ToSurrogate, UriSurrogate.FromSurrogate),
            Surrogate.Create<IPAddress, IPAddressSurrogate>(IPAddressSurrogate.ToSurrogate, IPAddressSurrogate.FromSurrogate),
            Surrogate.Create<IPEndPoint, IPEndPointSurrogate>(IPEndPointSurrogate.ToSurrogate, IPEndPointSurrogate.FromSurrogate),
        };

        private static readonly ValueSerializerFactory[] DefaultValueSerializerFactories =
        {
            new ConsistentArraySerializerFactory(),
            new MethodInfoSerializerFactory(),
            new PropertyInfoSerializerFactory(),
            new ConstructorInfoSerializerFactory(),
            new FieldInfoSerializerFactory(),
            new DelegateSerializerFactory(),
            new ToSurrogateSerializerFactory(),
            new FromSurrogateSerializerFactory(),
            new FSharpMapSerializerFactory(),
            new FSharpListSerializerFactory(), 
            //order is important, try dictionaries before enumerables as dicts are also enumerable
            new ExceptionSerializerFactory(),
            new ImmutableCollectionsSerializerFactory(),
            new ExpandoObjectSerializerFactory(),
            new DefaultDictionarySerializerFactory(),
            new DictionarySerializerFactory(),
            new ArraySerializerFactory(),
#if SERIALIZATION
            new ISerializableSerializerFactory(), //TODO: this will mess up the indexes in the serializer payload
#endif
            new EnumerableSerializerFactory(),
        };

        internal readonly bool IgnoreISerializable;
        internal readonly bool PreserveObjectReferences;
        internal readonly Surrogate[] Surrogates;
        internal readonly ValueSerializerFactory[] ValueSerializerFactories;
        internal readonly bool VersionTolerance;
        internal readonly Type[] KnownTypes;
        internal readonly Dictionary<Type, ushort> KnownTypesDict = new Dictionary<Type, ushort>();

        private readonly Surrogate[] _surrogates;
        private readonly ValueSerializerFactory[] _serializerFactories;

        public SerializerOptions(bool versionTolerance = false, bool preserveObjectReferences = false,
          IEnumerable<Surrogate> surrogates = null, IEnumerable<ValueSerializerFactory> serializerFactories = null,
          IEnumerable<Type> knownTypes = null, bool ignoreISerializable = false)
        {
            VersionTolerance = versionTolerance;
            _surrogates = surrogates?.ToArray() ?? EmptySurrogates;
            if (surrogates != null && surrogates.Any())
            {
                Surrogates = surrogates.Concat(DefaultSurrogates).ToArray();
            }
            else
            {
                Surrogates = DefaultSurrogates;
            }

            _serializerFactories = serializerFactories?.ToArray() ?? EmptyArray<ValueSerializerFactory>.Instance;
            //use the default factories + any user defined
            ValueSerializerFactories = serializerFactories == null
                ? DefaultValueSerializerFactories
                : serializerFactories.Concat(DefaultValueSerializerFactories).ToArray();

            KnownTypes = knownTypes?.ToArray() ?? new Type[] { };
            for (var i = 0; i < KnownTypes.Length; i++)
            {
                KnownTypesDict.Add(KnownTypes[i], (ushort)i);
            }

            PreserveObjectReferences = preserveObjectReferences;
            IgnoreISerializable = ignoreISerializable;
        }

        public SerializerOptions Clone(bool? versionTolerance = null)
        {
            return new SerializerOptions(
                versionTolerance: versionTolerance.HasValue ? versionTolerance.Value : this.VersionTolerance,
                preserveObjectReferences: this.PreserveObjectReferences,
                surrogates: this._surrogates,
                serializerFactories: this._serializerFactories,
                knownTypes: this.KnownTypes,
                ignoreISerializable: this.IgnoreISerializable);
        }
    }
}