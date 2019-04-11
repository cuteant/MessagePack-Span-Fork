
using System;

namespace MessagePack.Formatters
{

    public sealed class TupleFormatter<T1> : IMessagePackFormatter<Tuple<T1>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, Tuple<T1> value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(1, ref idx);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(ref writer, ref idx, value.Item1, formatterResolver);
        }

        public Tuple<T1> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var count = reader.ReadArrayHeader();
            if (count != 1) ThrowHelper.ThrowInvalidOperationException_Tuple_Count();

            var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref reader, formatterResolver);
            return new Tuple<T1>(item1);
        }
    }


    public sealed class TupleFormatter<T1, T2> : IMessagePackFormatter<Tuple<T1, T2>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, Tuple<T1, T2> value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(2, ref idx);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(ref writer, ref idx, value.Item1, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(ref writer, ref idx, value.Item2, formatterResolver);
        }

        public Tuple<T1, T2> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var count = reader.ReadArrayHeader();
            if (count != 2) ThrowHelper.ThrowInvalidOperationException_Tuple_Count();

            var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref reader, formatterResolver);
            var item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(ref reader, formatterResolver);
            return new Tuple<T1, T2>(item1, item2);
        }
    }


    public sealed class TupleFormatter<T1, T2, T3> : IMessagePackFormatter<Tuple<T1, T2, T3>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, Tuple<T1, T2, T3> value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(3, ref idx);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(ref writer, ref idx, value.Item1, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(ref writer, ref idx, value.Item2, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(ref writer, ref idx, value.Item3, formatterResolver);
        }

        public Tuple<T1, T2, T3> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var count = reader.ReadArrayHeader();
            if (count != 3) ThrowHelper.ThrowInvalidOperationException_Tuple_Count();

            var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref reader, formatterResolver);
            var item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(ref reader, formatterResolver);
            var item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(ref reader, formatterResolver);
            return new Tuple<T1, T2, T3>(item1, item2, item3);
        }
    }


    public sealed class TupleFormatter<T1, T2, T3, T4> : IMessagePackFormatter<Tuple<T1, T2, T3, T4>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, Tuple<T1, T2, T3, T4> value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(4, ref idx);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(ref writer, ref idx, value.Item1, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(ref writer, ref idx, value.Item2, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(ref writer, ref idx, value.Item3, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(ref writer, ref idx, value.Item4, formatterResolver);
        }

        public Tuple<T1, T2, T3, T4> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var count = reader.ReadArrayHeader();
            if (count != 4) ThrowHelper.ThrowInvalidOperationException_Tuple_Count();

            var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref reader, formatterResolver);
            var item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(ref reader, formatterResolver);
            var item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(ref reader, formatterResolver);
            var item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(ref reader, formatterResolver);
            return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }
    }


    public sealed class TupleFormatter<T1, T2, T3, T4, T5> : IMessagePackFormatter<Tuple<T1, T2, T3, T4, T5>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, Tuple<T1, T2, T3, T4, T5> value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(5, ref idx);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(ref writer, ref idx, value.Item1, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(ref writer, ref idx, value.Item2, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(ref writer, ref idx, value.Item3, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(ref writer, ref idx, value.Item4, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T5>().Serialize(ref writer, ref idx, value.Item5, formatterResolver);
        }

        public Tuple<T1, T2, T3, T4, T5> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var count = reader.ReadArrayHeader();
            if (count != 5) ThrowHelper.ThrowInvalidOperationException_Tuple_Count();

            var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref reader, formatterResolver);
            var item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(ref reader, formatterResolver);
            var item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(ref reader, formatterResolver);
            var item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(ref reader, formatterResolver);
            var item5 = formatterResolver.GetFormatterWithVerify<T5>().Deserialize(ref reader, formatterResolver);
            return new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        }
    }


    public sealed class TupleFormatter<T1, T2, T3, T4, T5, T6> : IMessagePackFormatter<Tuple<T1, T2, T3, T4, T5, T6>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, Tuple<T1, T2, T3, T4, T5, T6> value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(6, ref idx);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(ref writer, ref idx, value.Item1, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(ref writer, ref idx, value.Item2, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(ref writer, ref idx, value.Item3, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(ref writer, ref idx, value.Item4, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T5>().Serialize(ref writer, ref idx, value.Item5, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T6>().Serialize(ref writer, ref idx, value.Item6, formatterResolver);
        }

        public Tuple<T1, T2, T3, T4, T5, T6> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var count = reader.ReadArrayHeader();
            if (count != 6) ThrowHelper.ThrowInvalidOperationException_Tuple_Count();

            var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref reader, formatterResolver);
            var item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(ref reader, formatterResolver);
            var item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(ref reader, formatterResolver);
            var item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(ref reader, formatterResolver);
            var item5 = formatterResolver.GetFormatterWithVerify<T5>().Deserialize(ref reader, formatterResolver);
            var item6 = formatterResolver.GetFormatterWithVerify<T6>().Deserialize(ref reader, formatterResolver);
            return new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
        }
    }


    public sealed class TupleFormatter<T1, T2, T3, T4, T5, T6, T7> : IMessagePackFormatter<Tuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, Tuple<T1, T2, T3, T4, T5, T6, T7> value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(7, ref idx);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(ref writer, ref idx, value.Item1, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(ref writer, ref idx, value.Item2, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(ref writer, ref idx, value.Item3, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(ref writer, ref idx, value.Item4, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T5>().Serialize(ref writer, ref idx, value.Item5, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T6>().Serialize(ref writer, ref idx, value.Item6, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T7>().Serialize(ref writer, ref idx, value.Item7, formatterResolver);
        }

        public Tuple<T1, T2, T3, T4, T5, T6, T7> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var count = reader.ReadArrayHeader();
            if (count != 7) ThrowHelper.ThrowInvalidOperationException_Tuple_Count();

            var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref reader, formatterResolver);
            var item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(ref reader, formatterResolver);
            var item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(ref reader, formatterResolver);
            var item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(ref reader, formatterResolver);
            var item5 = formatterResolver.GetFormatterWithVerify<T5>().Deserialize(ref reader, formatterResolver);
            var item6 = formatterResolver.GetFormatterWithVerify<T6>().Deserialize(ref reader, formatterResolver);
            var item7 = formatterResolver.GetFormatterWithVerify<T7>().Deserialize(ref reader, formatterResolver);
            return new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
        }
    }


    public sealed class TupleFormatter<T1, T2, T3, T4, T5, T6, T7, TRest> : IMessagePackFormatter<Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(8, ref idx);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(ref writer, ref idx, value.Item1, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(ref writer, ref idx, value.Item2, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(ref writer, ref idx, value.Item3, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(ref writer, ref idx, value.Item4, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T5>().Serialize(ref writer, ref idx, value.Item5, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T6>().Serialize(ref writer, ref idx, value.Item6, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T7>().Serialize(ref writer, ref idx, value.Item7, formatterResolver);
            formatterResolver.GetFormatterWithVerify<TRest>().Serialize(ref writer, ref idx, value.Rest, formatterResolver);
        }

        public Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var count = reader.ReadArrayHeader();
            if (count != 8) ThrowHelper.ThrowInvalidOperationException_Tuple_Count();

            var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref reader, formatterResolver);
            var item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(ref reader, formatterResolver);
            var item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(ref reader, formatterResolver);
            var item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(ref reader, formatterResolver);
            var item5 = formatterResolver.GetFormatterWithVerify<T5>().Deserialize(ref reader, formatterResolver);
            var item6 = formatterResolver.GetFormatterWithVerify<T6>().Deserialize(ref reader, formatterResolver);
            var item7 = formatterResolver.GetFormatterWithVerify<T7>().Deserialize(ref reader, formatterResolver);
            var item8 = formatterResolver.GetFormatterWithVerify<TRest>().Deserialize(ref reader, formatterResolver);
            return new Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, item8);
        }
    }

}
