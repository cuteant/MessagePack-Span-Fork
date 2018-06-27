#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ValueSerializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Hyperion.Compilation;
using Hyperion.Internal;

namespace Hyperion.ValueSerializers
{
  public abstract class ValueSerializer
  {
    public abstract void WriteManifest([NotNull] Stream stream, [NotNull] SerializerSession session);
    public abstract void WriteValue([NotNull] Stream stream, object value, [NotNull] SerializerSession session);
    public abstract object ReadValue([NotNull] Stream stream, [NotNull] DeserializerSession session);
    public abstract Type GetElementType();

    public virtual void EmitWriteValue(ICompiler<ObjectWriter> c, int stream, int fieldValue, int session)
    {
      var converted = c.Convert<object>(fieldValue);
#if NET40
      var method = typeof(ValueSerializer).GetMethod(nameof(WriteValue));
#else
      var method = typeof(ValueSerializer).GetTypeInfo().GetMethod(nameof(WriteValue));
#endif

      //write it to the value serializer
      var vs = c.Constant(this);
      c.EmitCall(method, vs, stream, converted, session);
    }

    public virtual int EmitReadValue([NotNull] ICompiler<ObjectReader> c, int stream, int session,
        [NotNull] FieldInfo field)
    {
#if NET40
      var method = typeof(ValueSerializer).GetMethod(nameof(ReadValue));
#else
      var method = typeof(ValueSerializer).GetTypeInfo().GetMethod(nameof(ReadValue));
#endif
      var ss = c.Constant(this);
      var read = c.Call(method, ss, stream, session);
      read = c.Convert(read, field.FieldType);
      return read;
    }

    /// <summary>
    /// Marks a given <see cref="ValueSerializer"/> as one requiring a preallocated byte buffer to perform its operations. The byte[] value will be accessible in <see cref="ValueSerializer.EmitWriteValue"/> and <see cref="ValueSerializer.EmitReadValue"/> in the <see cref="ICompiler{TDel}"/> with <see cref="ICompiler{TDel}.GetVariable{T}"/> under following name <see cref="DefaultCodeGenerator.PreallocatedByteBuffer"/>.
    /// </summary>
    public virtual int PreallocatedByteBufferSize => 0;

    protected static MethodInfo GetStatic([NotNull] LambdaExpression expression, [NotNull] Type expectedReturnType)
    {
      var unaryExpression = (UnaryExpression)expression.Body;
      #region ## 苦竹 修改 ##
      // http://stackoverflow.com/questions/8225302/get-the-name-of-a-method-using-an-expression
      // http://stackoverflow.com/questions/12166785/lambda-expressions-t-functin-tout-and-methodinfo
      var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;
      var constantExpression = methodCallExpression.Object as ConstantExpression;

      MethodInfo method;
      if (constantExpression != null)
      {
        method = constantExpression.Value as MethodInfo;
      }
      else
      {
        constantExpression = methodCallExpression.Arguments
            .Single(a => a.Type == typeof(MethodInfo) && a.NodeType == ExpressionType.Constant) as ConstantExpression;
        method = constantExpression.Value as MethodInfo;
      }
      //var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;
      //var methodCallObject = (ConstantExpression)methodCallExpression.Object;
      //var method = (MethodInfo)methodCallObject.Value;
      #endregion

      if (method.IsStatic == false)
      {
        ThrowHelper.ThrowException_Method(method);
      }

      if (method.ReturnType != expectedReturnType)
      {
        ThrowHelper.ThrowException_Method(method, expectedReturnType);
      }

      return method;
    }
  }
}