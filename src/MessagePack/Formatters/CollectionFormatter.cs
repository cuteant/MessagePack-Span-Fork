using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MessagePack.Formatters
{
    public sealed class ArrayFormatter<T> : IMessagePackFormatter<T[]>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, T[] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var formatter = formatterResolver.GetFormatterWithVerify<T>();

            writer.WriteArrayHeader(value.Length, ref idx);

            for (int i = 0; i < value.Length; i++)
            {
                formatter.Serialize(ref writer, ref idx, value[i], formatterResolver);
            }
        }

        public T[] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var formatter = formatterResolver.GetFormatterWithVerify<T>();

            var len = reader.ReadArrayHeader();
            var array = new T[len];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = formatter.Deserialize(ref reader, formatterResolver);
            }
            return array;
        }
    }

    public sealed class ByteArraySegmentFormatter : IMessagePackFormatter<ArraySegment<byte>>
    {
        public static readonly ByteArraySegmentFormatter Instance = new ByteArraySegmentFormatter();

        ByteArraySegmentFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, ArraySegment<byte> value, IFormatterResolver formatterResolver)
        {
            if (value.Array != null)
            {
                writer.WriteBytes(value.Array, value.Offset, value.Count, ref idx);
                return;
            }

            writer.WriteNil(ref idx);
        }

        public ArraySegment<byte> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return default; }

            // use ReadBytesSegment? But currently straem api uses memory pool so can't save arraysegment...
            var binary = reader.ReadBytes();
            return new ArraySegment<byte>(binary, 0, binary.Length);
        }
    }

    public sealed class ArraySegmentFormatter<T> : IMessagePackFormatter<ArraySegment<T>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, ArraySegment<T> value, IFormatterResolver formatterResolver)
        {
            if (value.Array == null) { writer.WriteNil(ref idx); return; }

            var formatter = formatterResolver.GetFormatterWithVerify<T>();

            writer.WriteArrayHeader(value.Count, ref idx);

            var array = value.Array;
            for (int i = 0; i < value.Count; i++)
            {
                var item = array[value.Offset + i];
                formatter.Serialize(ref writer, ref idx, item, formatterResolver);
            }
        }

        public ArraySegment<T> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return default; }

            var array = formatterResolver.GetFormatterWithVerify<T[]>().Deserialize(ref reader, formatterResolver);
            return new ArraySegment<T>(array, 0, array.Length);
        }
    }

    // List<T> is popular format, should avoid abstraction.
    public sealed class ListFormatter<T> : IMessagePackFormatter<List<T>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, List<T> value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var formatter = formatterResolver.GetFormatterWithVerify<T>();

            var c = value.Count;
            writer.WriteArrayHeader(c, ref idx);

            for (int i = 0; i < c; i++)
            {
                formatter.Serialize(ref writer, ref idx, value[i], formatterResolver);
            }
        }

        public List<T> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var formatter = formatterResolver.GetFormatterWithVerify<T>();

            var len = reader.ReadArrayHeader();
            var list = new List<T>(len);
            for (int i = 0; i < len; i++)
            {
                list.Add(formatter.Deserialize(ref reader, formatterResolver));
            }
            return list;
        }
    }

    public abstract class CollectionFormatterBase<TElement, TIntermediate, TEnumerator, TCollection> : IMessagePackFormatter<TCollection>
        where TCollection : IEnumerable<TElement>
        where TEnumerator : IEnumerator<TElement>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, TCollection value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            // Optimize iteration(array is fastest)
            if (value is TElement[] array)
            {
                var formatter = formatterResolver.GetFormatterWithVerify<TElement>();

                writer.WriteArrayHeader(array.Length, ref idx);

                foreach (var item in array)
                {
                    formatter.Serialize(ref writer, ref idx, item, formatterResolver);
                }
            }
            else
            {
                var formatter = formatterResolver.GetFormatterWithVerify<TElement>();

                // knows count or not.
                var seqCount = GetCount(value);
                writer.WriteArrayHeader(seqCount, ref idx);

                // Unity's foreach struct enumerator causes boxing so iterate manually.
                var e = GetSourceEnumerator(value);
                try
                {
                    while (e.MoveNext())
                    {
                        formatter.Serialize(ref writer, ref idx, e.Current, formatterResolver);
                    }
                }
                finally
                {
                    e.Dispose();
                }
            }
        }

        public TCollection Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return default; }

            var formatter = formatterResolver.GetFormatterWithVerify<TElement>();

            var len = reader.ReadArrayHeader();

            var list = Create(len);
            for (int i = 0; i < len; i++)
            {
                Add(list, i, formatter.Deserialize(ref reader, formatterResolver));
            }

            return Complete(list);
        }

        // abstraction for serialize
        protected virtual int GetCount(TCollection sequence)
        {
            if (sequence is ICollection<TElement> collection)
            {
                return collection.Count;
            }
            else
            {
                if (sequence is IReadOnlyCollection<TElement> c2)
                {
                    return c2.Count;
                }
            }

            return sequence.Count();
        }

        // Some collections can use struct iterator, this is optimization path
        protected abstract TEnumerator GetSourceEnumerator(TCollection source);

        // abstraction for deserialize
        protected abstract TIntermediate Create(int count);
        protected abstract void Add(TIntermediate collection, int index, TElement value);
        protected abstract TCollection Complete(TIntermediate intermediateCollection);
    }

    public abstract class CollectionFormatterBase<TElement, TIntermediate, TCollection> : CollectionFormatterBase<TElement, TIntermediate, IEnumerator<TElement>, TCollection>
        where TCollection : IEnumerable<TElement>
    {
        protected override IEnumerator<TElement> GetSourceEnumerator(TCollection source) => source.GetEnumerator();
    }

    public abstract class CollectionFormatterBase<TElement, TCollection> : CollectionFormatterBase<TElement, TCollection, TCollection>
        where TCollection : IEnumerable<TElement>
    {
        protected sealed override TCollection Complete(TCollection intermediateCollection) => intermediateCollection;
    }

    public sealed class GenericCollectionFormatter<TElement, TCollection> : CollectionFormatterBase<TElement, TCollection>
         where TCollection : ICollection<TElement>, new()
    {
        protected override TCollection Create(int count) => new TCollection();

        protected override void Add(TCollection collection, int index, TElement value) => collection.Add(value);
    }

    public sealed class LinkedListFormatter<T> : CollectionFormatterBase<T, LinkedList<T>, LinkedList<T>.Enumerator, LinkedList<T>>
    {
        protected override void Add(LinkedList<T> collection, int index, T value) => collection.AddLast(value);

        protected override LinkedList<T> Complete(LinkedList<T> intermediateCollection) => intermediateCollection;

        protected override LinkedList<T> Create(int count) => new LinkedList<T>();

        protected override LinkedList<T>.Enumerator GetSourceEnumerator(LinkedList<T> source) => source.GetEnumerator();
    }

    public sealed class QeueueFormatter<T> : CollectionFormatterBase<T, Queue<T>, Queue<T>.Enumerator, Queue<T>>
    {
        protected override int GetCount(Queue<T> sequence) => sequence.Count;

        protected override void Add(Queue<T> collection, int index, T value) => collection.Enqueue(value);

        protected override Queue<T> Create(int count) => new Queue<T>(count);

        protected override Queue<T>.Enumerator GetSourceEnumerator(Queue<T> source) => source.GetEnumerator();

        protected override Queue<T> Complete(Queue<T> intermediateCollection) => intermediateCollection;
    }

    // should deserialize reverse order.
    public sealed class StackFormatter<T> : CollectionFormatterBase<T, T[], Stack<T>.Enumerator, Stack<T>>
    {
        protected override int GetCount(Stack<T> sequence) => sequence.Count;

        protected override void Add(T[] collection, int index, T value)
        {
            // add reverse
            collection[collection.Length - 1 - index] = value;
        }

        protected override T[] Create(int count) => new T[count];

        protected override Stack<T>.Enumerator GetSourceEnumerator(Stack<T> source) => source.GetEnumerator();

        protected override Stack<T> Complete(T[] intermediateCollection) => new Stack<T>(intermediateCollection);
    }

    public sealed class HashSetFormatter<T> : CollectionFormatterBase<T, HashSet<T>, HashSet<T>.Enumerator, HashSet<T>>
    {
        protected override int GetCount(HashSet<T> sequence) => sequence.Count;

        protected override void Add(HashSet<T> collection, int index, T value) => collection.Add(value);

        protected override HashSet<T> Complete(HashSet<T> intermediateCollection) => intermediateCollection;

        protected override HashSet<T> Create(int count) => new HashSet<T>();

        protected override HashSet<T>.Enumerator GetSourceEnumerator(HashSet<T> source) => source.GetEnumerator();
    }

    public sealed class ReadOnlyCollectionFormatter<T> : CollectionFormatterBase<T, T[], ReadOnlyCollection<T>>
    {
        protected override void Add(T[] collection, int index, T value) => collection[index] = value;

        protected override ReadOnlyCollection<T> Complete(T[] intermediateCollection) => new ReadOnlyCollection<T>(intermediateCollection);

        protected override T[] Create(int count) => new T[count];
    }

    public sealed class InterfaceListFormatter<T> : CollectionFormatterBase<T, T[], IList<T>>
    {
        protected override void Add(T[] collection, int index, T value) => collection[index] = value;

        protected override T[] Create(int count) => new T[count];

        protected override IList<T> Complete(T[] intermediateCollection) => intermediateCollection;
    }

    public sealed class InterfaceCollectionFormatter<T> : CollectionFormatterBase<T, T[], ICollection<T>>
    {
        protected override void Add(T[] collection, int index, T value) => collection[index] = value;

        protected override T[] Create(int count) => new T[count];

        protected override ICollection<T> Complete(T[] intermediateCollection) => intermediateCollection;
    }

    public sealed class InterfaceEnumerableFormatter<T> : CollectionFormatterBase<T, T[], IEnumerable<T>>
    {
        protected override void Add(T[] collection, int index, T value) => collection[index] = value;

        protected override T[] Create(int count) => new T[count];

        protected override IEnumerable<T> Complete(T[] intermediateCollection) => intermediateCollection;
    }

    // [Key, [Array]]
    public sealed class InterfaceGroupingFormatter<TKey, TElement> : IMessagePackFormatter<IGrouping<TKey, TElement>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, IGrouping<TKey, TElement> value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(2, ref idx);
            formatterResolver.GetFormatterWithVerify<TKey>().Serialize(ref writer, ref idx, value.Key, formatterResolver);
            formatterResolver.GetFormatterWithVerify<IEnumerable<TElement>>().Serialize(ref writer, ref idx, value, formatterResolver);
        }

        public IGrouping<TKey, TElement> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var count = reader.ReadArrayHeader();
            if (count != 2) ThrowHelper.ThrowInvalidOperationException_Grouping_Format();

            var key = formatterResolver.GetFormatterWithVerify<TKey>().Deserialize(ref reader, formatterResolver);
            var value = formatterResolver.GetFormatterWithVerify<IEnumerable<TElement>>().Deserialize(ref reader, formatterResolver);

            return new Grouping<TKey, TElement>(key, value);
        }
    }

    public sealed class InterfaceLookupFormatter<TKey, TElement> : CollectionFormatterBase<IGrouping<TKey, TElement>, Dictionary<TKey, IGrouping<TKey, TElement>>, ILookup<TKey, TElement>>
    {
        protected override void Add(Dictionary<TKey, IGrouping<TKey, TElement>> collection, int index, IGrouping<TKey, TElement> value)
        {
            collection.Add(value.Key, value);
        }

        protected override ILookup<TKey, TElement> Complete(Dictionary<TKey, IGrouping<TKey, TElement>> intermediateCollection)
        {
            return new Lookup<TKey, TElement>(intermediateCollection);
        }

        protected override Dictionary<TKey, IGrouping<TKey, TElement>> Create(int count)
        {
            return new Dictionary<TKey, IGrouping<TKey, TElement>>(count);
        }
    }

    class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        readonly TKey key;
        readonly IEnumerable<TElement> elements;

        public Grouping(TKey key, IEnumerable<TElement> elements)
        {
            this.key = key;
            this.elements = elements;
        }

        public TKey Key => key;

        public IEnumerator<TElement> GetEnumerator() => elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => elements.GetEnumerator();
    }

    class Lookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        readonly Dictionary<TKey, IGrouping<TKey, TElement>> groupings;

        public Lookup(Dictionary<TKey, IGrouping<TKey, TElement>> groupings) => this.groupings = groupings;

        public IEnumerable<TElement> this[TKey key] => groupings[key];

        public int Count => groupings.Count;

        public bool Contains(TKey key) => groupings.ContainsKey(key);

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator() => groupings.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => groupings.Values.GetEnumerator();
    }

    // NonGenerics

    public sealed class NonGenericListFormatter<T> : IMessagePackFormatter<T>
        where T : class, IList, new()
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, T value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var formatter = formatterResolver.GetFormatterWithVerify<object>();

            writer.WriteArrayHeader(value.Count, ref idx);
            foreach (var item in value)
            {
                formatter.Serialize(ref writer, ref idx, item, formatterResolver);
            }
        }

        public T Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return default; }

            var formatter = formatterResolver.GetFormatterWithVerify<object>();

            var count = reader.ReadArrayHeader();

            var list = new T();
            for (int i = 0; i < count; i++)
            {
                list.Add(formatter.Deserialize(ref reader, formatterResolver));
            }

            return list;
        }
    }

    public sealed class NonGenericInterfaceListFormatter : IMessagePackFormatter<IList>
    {
        public static readonly IMessagePackFormatter<IList> Instance = new NonGenericInterfaceListFormatter();

        NonGenericInterfaceListFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, IList value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var formatter = formatterResolver.GetFormatterWithVerify<object>();

            writer.WriteArrayHeader(value.Count, ref idx);
            foreach (var item in value)
            {
                formatter.Serialize(ref writer, ref idx, item, formatterResolver);
            }
        }

        public IList Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return default; }

            var formatter = formatterResolver.GetFormatterWithVerify<object>();

            var count = reader.ReadArrayHeader();

            var list = new object[count];
            for (int i = 0; i < count; i++)
            {
                list[i] = formatter.Deserialize(ref reader, formatterResolver);
            }
            return list;
        }
    }

    public sealed class NonGenericDictionaryFormatter<T> : IMessagePackFormatter<T>
        where T : class, IDictionary, new()
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, T value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var formatter = formatterResolver.GetFormatterWithVerify<object>();

            writer.WriteMapHeader(value.Count, ref idx);
            foreach (DictionaryEntry item in value)
            {
                formatter.Serialize(ref writer, ref idx, item.Key, formatterResolver);
                formatter.Serialize(ref writer, ref idx, item.Value, formatterResolver);
            }
        }

        public T Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var formatter = formatterResolver.GetFormatterWithVerify<object>();

            var count = reader.ReadMapHeader();

            var dict = new T();
            for (int i = 0; i < count; i++)
            {
                var key = formatter.Deserialize(ref reader, formatterResolver);
                var value = formatter.Deserialize(ref reader, formatterResolver);
                dict.Add(key, value);
            }
            return dict;
        }
    }

    public sealed class NonGenericInterfaceDictionaryFormatter : IMessagePackFormatter<IDictionary>
    {
        public static readonly IMessagePackFormatter<IDictionary> Instance = new NonGenericInterfaceDictionaryFormatter();

        NonGenericInterfaceDictionaryFormatter() { }

        public void Serialize(ref MessagePackWriter writer, ref int idx, IDictionary value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var formatter = formatterResolver.GetFormatterWithVerify<object>();

            writer.WriteMapHeader(value.Count, ref idx);
            foreach (DictionaryEntry item in value)
            {
                formatter.Serialize(ref writer, ref idx, item.Key, formatterResolver);
                formatter.Serialize(ref writer, ref idx, item.Value, formatterResolver);
            }
        }

        public IDictionary Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var formatter = formatterResolver.GetFormatterWithVerify<object>();

            var count = reader.ReadMapHeader();

            var dict = new Dictionary<object, object>(count);
            for (int i = 0; i < count; i++)
            {
                var key = formatter.Deserialize(ref reader, formatterResolver);
                var value = formatter.Deserialize(ref reader, formatterResolver);
                dict.Add(key, value);
            }
            return dict;
        }
    }

    public sealed class ObservableCollectionFormatter<T> : CollectionFormatterBase<T, ObservableCollection<T>>
    {
        protected override void Add(ObservableCollection<T> collection, int index, T value) => collection.Add(value);

        protected override ObservableCollection<T> Create(int count) => new ObservableCollection<T>();
    }

    public sealed class ReadOnlyObservableCollectionFormatter<T> : CollectionFormatterBase<T, ObservableCollection<T>, ReadOnlyObservableCollection<T>>
    {
        protected override void Add(ObservableCollection<T> collection, int index, T value) => collection.Add(value);

        protected override ObservableCollection<T> Create(int count) => new ObservableCollection<T>();

        protected override ReadOnlyObservableCollection<T> Complete(ObservableCollection<T> intermediateCollection)
        {
            return new ReadOnlyObservableCollection<T>(intermediateCollection);
        }
    }

    public sealed class InterfaceReadOnlyListFormatter<T> : CollectionFormatterBase<T, T[], IReadOnlyList<T>>
    {
        protected override void Add(T[] collection, int index, T value) => collection[index] = value;

        protected override T[] Create(int count) => new T[count];

        protected override IReadOnlyList<T> Complete(T[] intermediateCollection) => intermediateCollection;
    }

    public sealed class InterfaceReadOnlyCollectionFormatter<T> : CollectionFormatterBase<T, T[], IReadOnlyCollection<T>>
    {
        protected override void Add(T[] collection, int index, T value) => collection[index] = value;

        protected override T[] Create(int count) => new T[count];

        protected override IReadOnlyCollection<T> Complete(T[] intermediateCollection) => intermediateCollection;
    }

    public sealed class InterfaceSetFormatter<T> : CollectionFormatterBase<T, HashSet<T>, ISet<T>>
    {
        protected override void Add(HashSet<T> collection, int index, T value) => collection.Add(value);

        protected override ISet<T> Complete(HashSet<T> intermediateCollection) => intermediateCollection;

        protected override HashSet<T> Create(int count) => new HashSet<T>();
    }

    public sealed class ConcurrentBagFormatter<T> : CollectionFormatterBase<T, System.Collections.Concurrent.ConcurrentBag<T>>
    {
        protected override int GetCount(ConcurrentBag<T> sequence) => sequence.Count;

        protected override void Add(ConcurrentBag<T> collection, int index, T value) => collection.Add(value);

        protected override ConcurrentBag<T> Create(int count) => new ConcurrentBag<T>();
    }

    public sealed class ConcurrentQueueFormatter<T> : CollectionFormatterBase<T, System.Collections.Concurrent.ConcurrentQueue<T>>
    {
        protected override int GetCount(ConcurrentQueue<T> sequence) => sequence.Count;

        protected override void Add(ConcurrentQueue<T> collection, int index, T value) => collection.Enqueue(value);

        protected override ConcurrentQueue<T> Create(int count) => new ConcurrentQueue<T>();
    }

    public sealed class ConcurrentStackFormatter<T> : CollectionFormatterBase<T, T[], ConcurrentStack<T>>
    {
        protected override int GetCount(ConcurrentStack<T> sequence) => sequence.Count;

        protected override void Add(T[] collection, int index, T value)
        {
            // add reverse
            collection[collection.Length - 1 - index] = value;
        }

        protected override T[] Create(int count) => new T[count];

        protected override ConcurrentStack<T> Complete(T[] intermediateCollection) => new ConcurrentStack<T>(intermediateCollection);
    }
}
