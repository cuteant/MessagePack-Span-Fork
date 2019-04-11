using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using GrEmit;

namespace MessagePack.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var idx = 1;
            var writer = new MessagePackWriterShim();

            var serialize = TestSuccess();
            serialize(ref writer, ref idx, "ssssssss");

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        delegate void RawFormatterSerialize(ref MessagePackWriterShim writer, ref int idx, object value);

        ref struct MessagePackWriterShim
        {
            public void WriteNil(ref int idx)
            {
                Console.WriteLine("idx: " + idx++);
                Console.WriteLine("null");
            }

            public void WriterString(string str, ref int idx)
            {
                Console.WriteLine("idx: " + idx++);
                Console.WriteLine(str);
            }
        }

        private static RawFormatterSerialize TestSuccess()
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(),
                typeof(void),
                new[] { typeof(MessagePackWriterShim).MakeByRefType(), typeof(int).MakeByRefType(), typeof(object) }, typeof(Program).Module, true);

            using (var il = new GroboIL(method))
            {
                if (typeof(object).IsClass)
                {
                    var elseBody = il.DefineLabel("");
                    il.Ldarg(2);
                    il.Brtrue(elseBody);

                    il.Ldarg(0);
                    il.Ldarg(1);

                    il.Call(typeof(MessagePackWriterShim).GetMethod(nameof(MessagePackWriterShim.WriteNil)));
                    il.Ret();

                    il.MarkLabel(elseBody);
                }

                il.Ldarg(0);
                il.Ldarg(2);
                il.Castclass(typeof(string));
                il.Ldarg(1);
                il.Call(typeof(MessagePackWriterShim).GetMethod(nameof(MessagePackWriterShim.WriterString)));

                il.Ret();
                Console.WriteLine(il.GetILCode());
                return (RawFormatterSerialize)method.CreateDelegate(typeof(RawFormatterSerialize));
            }
        }
        private static readonly MethodInfo getTypeMethod = ((MethodCallExpression)((Expression<Func<object, Type>>)(obj => obj.GetType())).Body).Method;
    }
}
