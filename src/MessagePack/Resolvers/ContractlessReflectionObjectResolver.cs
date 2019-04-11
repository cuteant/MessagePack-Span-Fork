//using MessagePack.Formatters;
//using MessagePack.Internal;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace MessagePack.Resolvers
//{
//    public static class ContractlessReflectionObjectResolver
//    {
//        // TODO:CamelCase Option? AllowPrivate?
//        public static readonly IFormatterResolver Default = new DefaultResolver();
//        public static readonly IFormatterResolver Contractless = new ContractlessResolver();
//        public static readonly IFormatterResolver ContractlessForceStringKey = new ContractlessForceStringResolver();

//        class DefaultResolver : FormatterResolver
//        {
//            const bool ForceStringKey = false;
//            const bool Contractless = false;
//            const bool AllowPrivate = false;

//            public override IMessagePackFormatter<T> GetFormatter<T>()
//            {
//                return Cache<T>.formatter;
//            }

//            static class Cache<T>
//            {
//                public static readonly IMessagePackFormatter<T> formatter;

//                static Cache()
//                {
//                    var metaInfo = ObjectSerializationInfo.CreateOrNull(typeof(T), ForceStringKey, Contractless, AllowPrivate);
//                    if (metaInfo != null)
//                    {
//                        formatter = new ReflectionObjectFormatter<T>(metaInfo);
//                    }
//                }
//            }
//        }

//        class ContractlessResolver : FormatterResolver
//        {
//            const bool ForceStringKey = false;
//            const bool Contractless = true;
//            const bool AllowPrivate = false;

//            public override IMessagePackFormatter<T> GetFormatter<T>()
//            {
//                return Cache<T>.formatter;
//            }

//            static class Cache<T>
//            {
//                public static readonly IMessagePackFormatter<T> formatter;

//                static Cache()
//                {
//                    var metaInfo = ObjectSerializationInfo.CreateOrNull(typeof(T), ForceStringKey, Contractless, AllowPrivate);
//                    if (metaInfo != null)
//                    {
//                        formatter = new ReflectionObjectFormatter<T>(metaInfo);
//                    }
//                }
//            }
//        }

//        class ContractlessForceStringResolver : FormatterResolver
//        {
//            const bool ForceStringKey = true;
//            const bool Contractless = true;
//            const bool AllowPrivate = false;

//            public override IMessagePackFormatter<T> GetFormatter<T>()
//            {
//                return Cache<T>.formatter;
//            }

//            static class Cache<T>
//            {
//                public static readonly IMessagePackFormatter<T> formatter;

//                static Cache()
//                {
//                    var metaInfo = ObjectSerializationInfo.CreateOrNull(typeof(T), ForceStringKey, Contractless, AllowPrivate);
//                    if (metaInfo != null)
//                    {
//                        formatter = new ReflectionObjectFormatter<T>(metaInfo);
//                    }
//                }
//            }
//        }

//    }


//    public class ReflectionObjectFormatter<T> : IMessagePackFormatter<T>
//    {
//        readonly ObjectSerializationInfo metaInfo;

//        // for write
//        readonly byte[][] writeMemberNames;
//        readonly ObjectSerializationInfo.EmittableMember[] writeMembers;

//        // for read
//        readonly int[] constructorParameterIndexes;
//        readonly AutomataDictionary mapMemberDictionary;
//        readonly ObjectSerializationInfo.EmittableMember[] readMembers;


//        internal ReflectionObjectFormatter(ObjectSerializationInfo metaInfo)
//        {
//            this.metaInfo = metaInfo;

//            // for write
//            {
//                var memberNameList = new List<byte[]>(metaInfo.Members.Length);
//                var emmitableMemberList = new List<ObjectSerializationInfo.EmittableMember>(metaInfo.Members.Length);
//                foreach (var item in metaInfo.Members)
//                {
//                    if (item.IsWritable)
//                    {
//                        emmitableMemberList.Add(item);
//                        memberNameList.Add(Encoding.UTF8.GetBytes(item.Name));
//                    }
//                }
//                this.writeMemberNames = memberNameList.ToArray();
//                this.writeMembers = emmitableMemberList.ToArray();
//            }
//            // for read
//            {
//                var automata = new AutomataDictionary();
//                var emmitableMemberList = new List<ObjectSerializationInfo.EmittableMember>(metaInfo.Members.Length);
//                int index = 0;
//                foreach (var item in metaInfo.Members)
//                {
//                    if (item.IsReadable)
//                    {
//                        emmitableMemberList.Add(item);
//                        automata.Add(item.Name, index++);
//                    }
//                }
//                this.readMembers = emmitableMemberList.ToArray();
//                this.mapMemberDictionary = automata;
//            }
//        }

//        public void Serialize(ref MessagePackWriter writer, ref int idx, T value, IFormatterResolver formatterResolver)
//        {
//            // reduce generic method size, avoid write code in <T> type.
//            if (metaInfo.IsIntKey)
//            {
//                ReflectionObjectFormatterHelper.WriteArraySerialize(metaInfo, writeMembers, ref writer, ref idx, value, formatterResolver);
//            }
//            else
//            {
//                ReflectionObjectFormatterHelper.WriteMapSerialize(metaInfo, writeMembers, writeMemberNames, ref writer, ref idx, value, formatterResolver);
//            }
//        }

//        public T Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
//        {
//            return (T)ReflectionObjectFormatterHelper.Deserialize(metaInfo, readMembers, constructorParameterIndexes, mapMemberDictionary, ref reader, formatterResolver);
//        }
//    }

//    internal static class ReflectionObjectFormatterHelper
//    {
//        internal static void WriteArraySerialize(ObjectSerializationInfo metaInfo, ObjectSerializationInfo.EmittableMember[] writeMembers, ref MessagePackWriter writer, ref int idx, object value, IFormatterResolver formatterResolver)
//        {
//            writer.WriteArrayHeader(writeMembers.Length, ref idx);
//            foreach (var item in metaInfo.Members)
//            {
//                if (item == null)
//                {
//                    writer.WriteNil(ref idx);
//                }
//                else
//                {
//                    var memberValue = item.ReflectionLoadValue(value);
//                    MessagePackSerializer.NonGeneric.Serialize(item.Type, ref writer, ref idx, memberValue, formatterResolver);
//                }
//            }
//        }

//        internal static void WriteMapSerialize(ObjectSerializationInfo metaInfo, ObjectSerializationInfo.EmittableMember[] writeMembers, byte[][] memberNames, ref MessagePackWriter writer, ref int idx, object value, IFormatterResolver formatterResolver)
//        {
//            writer.WriteMapHeader(writeMembers.Length, ref idx);

//            for (int i = 0; i < writeMembers.Length; i++)
//            {
//                writer.WriteStringBytes(memberNames[i], ref idx);
//                var memberValue = writeMembers[i].ReflectionLoadValue(value);
//                MessagePackSerializer.NonGeneric.Serialize(writeMembers[i].Type, ref writer, ref idx, memberValue, formatterResolver);
//            }
//        }

//        internal static object Deserialize(ObjectSerializationInfo metaInfo, ObjectSerializationInfo.EmittableMember[] readMembers, int[] constructorParameterIndexes, AutomataDictionary mapMemberDictionary, ref MessagePackReader reader, IFormatterResolver formatterResolver)
//        {
//            object[] parameters = null;

//            var headerType = reader.GetMessagePackType();
//            if (headerType == MessagePackType.Nil)
//            {
//                reader.AdvanceCurrentSpan(1);
//                return null;
//            }
//            else if (headerType == MessagePackType.Array)
//            {
//                var arraySize = reader.ReadArrayHeader();

//                // ReadValues
//                parameters = new object[arraySize];
//                for (int i = 0; i < arraySize; i++)
//                {
//                    var info = readMembers[i];
//                    if (info != null)
//                    {
//                        parameters[i] = MessagePackSerializer.NonGeneric.Deserialize(info.Type, ref reader, formatterResolver);
//                    }
//                    else
//                    {
//                        reader.ReadNextBlock();
//                    }
//                }
//            }
//            else if (headerType == MessagePackType.Map)
//            {
//                var mapSize = reader.ReadMapHeader();

//                // ReadValues
//                parameters = new object[mapSize];
//                for (int i = 0; i < mapSize; i++)
//                {
//                    var rawPropName = reader.ReadStringSegment();

//                    if (mapMemberDictionary.TryGetValue(rawPropName, out int index))
//                    {
//                        var info = readMembers[index];
//                        parameters[index] = MessagePackSerializer.NonGeneric.Deserialize(info.Type, ref reader, formatterResolver);
//                    }
//                    else
//                    {
//                        reader.ReadNextBlock();
//                    }
//                }
//            }
//            else
//            {
//                ThrowHelper.ThrowInvalidOperationException_MessagePackType(reader.Peek());
//            }

//            // CreateObject
//            object result = null;
//            if (constructorParameterIndexes.Length == 0)
//            {
//                result = Activator.CreateInstance(metaInfo.Type);
//            }
//            else
//            {
//                var args = new object[constructorParameterIndexes.Length];
//                for (int i = 0; i < constructorParameterIndexes.Length; i++)
//                {
//                    args[i] = parameters[constructorParameterIndexes[i]];
//                }

//                result = Activator.CreateInstance(metaInfo.Type, args);
//            }

//            // SetMembers
//            for (int i = 0; i < readMembers.Length; i++)
//            {
//                var info = readMembers[i];
//                if (info != null)
//                {
//                    info.ReflectionStoreValue(result, parameters[i]);
//                }
//            }

//            return result;
//        }
//    }
//}
