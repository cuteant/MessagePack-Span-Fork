#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializerSession.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using CuteAnt;
using CuteAnt.Buffers;
using CuteAnt.Pool;

namespace Hyperion
{
    public class SerializerSession
    {
        public const int MinBufferSize = 9;
        private readonly Dictionary<object, int> _objects;
        public Serializer Serializer { get; private set; }
        private LinkedList<Type> _trackedTypes;
        internal byte[] _buffer;

        private int _nextObjectId;
        private /*readonly*/ ushort _nextTypeId;

        protected SerializerSession()
        {
            _objects = new Dictionary<object, int>(ReferenceEqualsComparer.Instance);
        }

        public SerializerSession(Serializer serializer)
        {
            if (serializer.Options.PreserveObjectReferences)
            {
                _objects = new Dictionary<object, int>(ReferenceEqualsComparer.Instance);
            }
            Reinitialize(serializer);
        }

        public void Reinitialize(Serializer serializer)
        {
            Serializer = serializer;
            _nextTypeId = (ushort)(serializer.Options.KnownTypes.Length);
        }

        public virtual void Clear()
        {
            _objects?.Clear();
            _trackedTypes?.Clear();

            Serializer = null;
            _buffer = null;
            _nextObjectId = 0;
        }

        public void TrackSerializedObject(object obj)
        {
            try
            {
                _objects.Add(obj, _nextObjectId++);
            }
            catch (Exception x)
            {
                ThrowHelper.ThrowException_Tracking(x);
            }
        }

        public bool TryGetObjectId(object obj, out int objectId)
        {
            return _objects.TryGetValue(obj, out objectId);
        }

        public bool ShouldWriteTypeManifest(Type type, out ushort index)
        {
            return !TryGetValue(type, out index);
        }

        public virtual byte[] GetBuffer(int length)
        {
            if (null == _buffer) { _buffer = new byte[length]; return _buffer; }
            if (length <= _buffer.Length) { return _buffer; }

            length = Math.Max(length, _buffer.Length * 2);

            _buffer = new byte[length];

            return _buffer;
        }

        public bool TryGetValue(Type key, out ushort value)
        {
            if (_trackedTypes == null || _trackedTypes.Count == 0)
            {
                value = 0;
                return false;
            }

            ushort j = _nextTypeId;
            foreach (var t in _trackedTypes)
            {
                if (key == t)
                {
                    value = j;
                    return true;
                }
                j++;
            }

            value = 0;
            return false;
        }

        public void TrackSerializedType(Type type)
        {
            if (_trackedTypes == null)
            {
                _trackedTypes = new LinkedList<Type>();
            }
            _trackedTypes.AddLast(type);
        }
    }

    public sealed class BufferManagerSerializerSession : SerializerSession
    {
        public BufferManagerSerializerSession() : base() { }

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
    public sealed class SerializerSessionPooledObjectPolicy : IPooledObjectPolicy<SerializerSession>
    {
        public SerializerSession Create() => new BufferManagerSerializerSession();

        [MethodImpl(InlineMethod.Value)]
        public SerializerSession PreGetting(SerializerSession session) => session;

        public bool Return(SerializerSession session)
        {
            if (null == session) { return false; }
            session.Clear();
            return true;
        }
    }

    /// <summary></summary>
    public sealed class SerializerSessionManager
    {
        private static SerializerSessionPooledObjectPolicy _defaultPolicy = new SerializerSessionPooledObjectPolicy();
        public static SerializerSessionPooledObjectPolicy DefaultPolicy { get => _defaultPolicy; set => _defaultPolicy = value; }

        private static ObjectPool<SerializerSession> _innerPool;
        public static ObjectPool<SerializerSession> InnerPool
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
                if (null == value) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value); }
                Interlocked.CompareExchange(ref _innerPool, value, null);
            }
        }

        public static PooledObject<SerializerSession> Create(Serializer serializer)
        {
            var session = Allocate(serializer);
            return new PooledObject<SerializerSession>(InnerPool, session);
        }

        public static SerializerSession Allocate(Serializer serializer)
        {
            var session = InnerPool.Take();
            session.Reinitialize(serializer);
            return session;
        }

        public static void Free(SerializerSession session) => InnerPool.Return(session);

        public static void Clear() => InnerPool.Clear();
    }
}