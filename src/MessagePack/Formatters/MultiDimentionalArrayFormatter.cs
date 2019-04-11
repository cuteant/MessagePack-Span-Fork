namespace MessagePack.Formatters
{
    // multi dimentional array serialize to [i, j, [seq]]

    public sealed class TwoDimentionalArrayFormatter<T> : IMessagePackFormatter<T[,]>
    {
        const int ArrayLength = 3;

        public void Serialize(ref MessagePackWriter writer, ref int idx, T[,] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var i = value.GetLength(0);
            var j = value.GetLength(1);

            var formatter = formatterResolver.GetFormatterWithVerify<T>();

            writer.WriteArrayHeader(ArrayLength, ref idx);
            writer.WriteInt32(i, ref idx);
            writer.WriteInt32(j, ref idx);

            writer.WriteArrayHeader(value.Length, ref idx);
            foreach (var item in value)
            {
                formatter.Serialize(ref writer, ref idx, item, formatterResolver);
            }
        }

        public T[,] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var formatter = formatterResolver.GetFormatterWithVerify<T>();

            var len = reader.ReadArrayHeader();
            if (len != ArrayLength) ThrowHelper.ThrowInvalidOperationException_T_Format1();

            var iLength = reader.ReadInt32();
            var jLength = reader.ReadInt32();
            var maxLen = reader.ReadArrayHeader();

            var array = new T[iLength, jLength];

            var i = 0;
            var j = -1;
            for (int loop = 0; loop < maxLen; loop++)
            {
                if (j < jLength - 1)
                {
                    j++;
                }
                else
                {
                    j = 0;
                    i++;
                }

                array[i, j] = formatter.Deserialize(ref reader, formatterResolver);
            }

            return array;
        }
    }

    public sealed class ThreeDimentionalArrayFormatter<T> : IMessagePackFormatter<T[,,]>
    {
        const int ArrayLength = 4;

        public void Serialize(ref MessagePackWriter writer, ref int idx, T[,,] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var i = value.GetLength(0);
            var j = value.GetLength(1);
            var k = value.GetLength(2);

            var formatter = formatterResolver.GetFormatterWithVerify<T>();

            writer.WriteArrayHeader(ArrayLength, ref idx);
            writer.WriteInt32(i, ref idx);
            writer.WriteInt32(j, ref idx);
            writer.WriteInt32(k, ref idx);

            writer.WriteArrayHeader(value.Length, ref idx);
            foreach (var item in value)
            {
                formatter.Serialize(ref writer, ref idx, item, formatterResolver);
            }
        }

        public T[,,] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var formatter = formatterResolver.GetFormatterWithVerify<T>();

            var len = reader.ReadArrayHeader();
            if (len != ArrayLength) ThrowHelper.ThrowInvalidOperationException_T_Format2();

            var iLength = reader.ReadInt32();
            var jLength = reader.ReadInt32();
            var kLength = reader.ReadInt32();
            var maxLen = reader.ReadArrayHeader();

            var array = new T[iLength, jLength, kLength];

            var i = 0;
            var j = 0;
            var k = -1;
            for (int loop = 0; loop < maxLen; loop++)
            {
                if (k < kLength - 1)
                {
                    k++;
                }
                else if (j < jLength - 1)
                {
                    k = 0;
                    j++;
                }
                else
                {
                    k = 0;
                    j = 0;
                    i++;
                }

                array[i, j, k] = formatter.Deserialize(ref reader, formatterResolver);
            }

            return array;
        }
    }

    public sealed class FourDimentionalArrayFormatter<T> : IMessagePackFormatter<T[,,,]>
    {
        const int ArrayLength = 5;

        public void Serialize(ref MessagePackWriter writer, ref int idx, T[,,,] value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var i = value.GetLength(0);
            var j = value.GetLength(1);
            var k = value.GetLength(2);
            var l = value.GetLength(3);

            var formatter = formatterResolver.GetFormatterWithVerify<T>();

            writer.WriteArrayHeader(ArrayLength, ref idx);
            writer.WriteInt32(i, ref idx);
            writer.WriteInt32(j, ref idx);
            writer.WriteInt32(k, ref idx);
            writer.WriteInt32(l, ref idx);

            writer.WriteArrayHeader(value.Length, ref idx);
            foreach (var item in value)
            {
                formatter.Serialize(ref writer, ref idx, item, formatterResolver);
            }
        }

        public T[,,,] Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var formatter = formatterResolver.GetFormatterWithVerify<T>();

            var len = reader.ReadArrayHeader();
            if (len != ArrayLength) ThrowHelper.ThrowInvalidOperationException_T_Format3();

            var iLength = reader.ReadInt32();
            var jLength = reader.ReadInt32();
            var kLength = reader.ReadInt32();
            var lLength = reader.ReadInt32();
            var maxLen = reader.ReadArrayHeader();

            var array = new T[iLength, jLength, kLength, lLength];

            var i = 0;
            var j = 0;
            var k = 0;
            var l = -1;
            for (int loop = 0; loop < maxLen; loop++)
            {
                if (l < lLength - 1)
                {
                    l++;
                }
                else if (k < kLength - 1)
                {
                    l = 0;
                    k++;
                }
                else if (j < jLength - 1)
                {
                    l = 0;
                    k = 0;
                    j++;
                }
                else
                {
                    l = 0;
                    k = 0;
                    j = 0;
                    i++;
                }

                array[i, j, k, l] = formatter.Deserialize(ref reader, formatterResolver);
            }

            return array;
        }
    }
}