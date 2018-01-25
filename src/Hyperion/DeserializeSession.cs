#region copyright
// -----------------------------------------------------------------------
//  <copyright file="DeserializeSession.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using CuteAnt;
using CuteAnt.Buffers;
using CuteAnt.Pool;
using Hyperion.Internal;
using IntToObjectLookup = System.Collections.Generic.List<object>;
using IntToTypeLookup = System.Collections.Generic.List<System.Type>;
using TypeToVersionInfoLookup = System.Collections.Generic.Dictionary<System.Type, Hyperion.TypeVersionInfo>;

namespace Hyperion
{
    public class TypeVersionInfo
    {

    }
    public class DeserializerSession
    {
        public const int MinBufferSize = 9;
        internal byte[] _buffer;
        private IntToTypeLookup _identifierToType;
        private readonly IntToObjectLookup _objectById;
        private readonly TypeToVersionInfoLookup _versionInfoByType;
        public Serializer Serializer { get; private set; }
        private /*readonly*/ int _offset;

        protected DeserializerSession()
        {
            _objectById = new IntToObjectLookup(capacity: 1);
            _versionInfoByType = new TypeToVersionInfoLookup();
        }

        public DeserializerSession([NotNull] Serializer serializer)
        {
            if (serializer.Options.PreserveObjectReferences)
            {
                _objectById = new IntToObjectLookup(capacity: 1);
            }
            if (serializer.Options.VersionTolerance)
            {
                _versionInfoByType = new TypeToVersionInfoLookup();
            }
            Reinitialize(serializer);
        }

        public void Reinitialize(Serializer serializer)
        {
            Serializer = serializer;
            _offset = serializer.Options.KnownTypes.Length;
        }

        public virtual void Clear()
        {
            _identifierToType = null;
            _objectById?.Clear();
            _versionInfoByType?.Clear();

            Serializer = null;
            _buffer = null;
        }

        public virtual byte[] GetBuffer(int length)
        {
            if (null == _buffer) { _buffer = new byte[length]; return _buffer; }
            if (length <= _buffer.Length) { return _buffer; }

            length = Math.Max(length, _buffer.Length * 2);

            _buffer = new byte[length];

            return _buffer;
        }

        public void TrackDeserializedObject([NotNull]object obj)
        {
            _objectById.Add(obj);
        }

        public object GetDeserializedObject(int id)
        {
            return _objectById[id];
        }

        public void TrackDeserializedType([NotNull]Type type)
        {
            if (_identifierToType == null)
            {
                _identifierToType = new IntToTypeLookup(capacity: 1);
            }
            _identifierToType.Add(type);
        }

        public Type GetTypeFromTypeId(int typeId)
        {

            if (typeId < _offset)
            {
                return Serializer.Options.KnownTypes[typeId];
            }
            if (_identifierToType == null)
                throw new ArgumentException(nameof(typeId));

            return _identifierToType[typeId - _offset];
        }

        public void TrackDeserializedTypeWithVersion([NotNull]Type type, [NotNull] TypeVersionInfo versionInfo)
        {
            TrackDeserializedType(type);
            _versionInfoByType.Add(type, versionInfo);
        }

        public TypeVersionInfo GetVersionInfo([NotNull]Type type)
        {
            return _versionInfoByType[type];
        }
    }

    public sealed class BufferManagerDeserializerSession : DeserializerSession
    {
        public BufferManagerDeserializerSession() : base() { }

        public override void Clear()
        {
            if (_buffer != null) { BufferManager.Shared.Return(_buffer); }
            base.Clear();
        }

        public override byte[] GetBuffer(int length)
        {
            if (null == _buffer) { _buffer = BufferManager.Shared.Rent(length); return _buffer; }
            if (length <= _buffer.Length) { return _buffer; }

            length = Math.Max(length, _buffer.Length * 2);

            BufferManager.Shared.Return(_buffer);
            _buffer = BufferManager.Shared.Rent(length);

            return _buffer;
        }
    }

    /// <summary></summary>
    public sealed class DeserializerSessionPooledObjectPolicy : IPooledObjectPolicy<DeserializerSession>
    {
        public DeserializerSession Create() => new BufferManagerDeserializerSession();

        [MethodImpl(InlineMethod.Value)]
        public DeserializerSession PreGetting(DeserializerSession session) => session;

        public bool Return(DeserializerSession session)
        {
            if (null == session) { return false; }
            session.Clear();
            return true;
        }
    }

    /// <summary></summary>
    public sealed class DeserializerSessionManager
    {
        private static DeserializerSessionPooledObjectPolicy _defaultPolicy = new DeserializerSessionPooledObjectPolicy();
        public static DeserializerSessionPooledObjectPolicy DefaultPolicy { get => _defaultPolicy; set => _defaultPolicy = value; }

        private static ObjectPool<DeserializerSession> _innerPool;
        public static ObjectPool<DeserializerSession> InnerPool
        {
            [MethodImpl(InlineMethod.Value)]
            get
            {
                var pool = Volatile.Read(ref _innerPool);
                if (pool == null)
                {
                    pool = SynchronizedObjectPoolProvider.Default.Create(DefaultPolicy);
                    var current = Interlocked.CompareExchange(ref _innerPool, pool, null);
                    if (current != null) { return current; }
                }
                return pool;
            }
            set
            {
                if (null == value) { throw new ArgumentNullException(nameof(value)); }
                Interlocked.CompareExchange(ref _innerPool, value, null);
            }
        }

        public static PooledObject<DeserializerSession> Create(Serializer serializer)
        {
            var sb = Allocate(serializer);
            return new PooledObject<DeserializerSession>(InnerPool, sb);
        }

        public static DeserializerSession Allocate(Serializer serializer)
        {
            var session = InnerPool.Take();
            session.Reinitialize(serializer);
            return session;
        }

        public static void Free(DeserializerSession session) => InnerPool.Return(session);

        public static void Clear() => InnerPool.Clear();
    }
}