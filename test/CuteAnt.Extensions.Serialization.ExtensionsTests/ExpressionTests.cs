using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using CuteAnt.Reflection;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Xunit;

namespace CuteAnt.Extensions.Serialization.Tests
{
  public class ExpressionTests
  {
    private static readonly MessagePackMessageFormatter s_formatter = MessagePackMessageFormatter.DefaultInstance;

    [MessagePackObject]
    public struct Dummy
    {
      [Key(0)]
      public string TestField;

      [Key(1)]
      public int TestProperty { get; set; }

      public string Fact(string input) => input;
      public string Fact1<T>(T input) => input.ToString();
      public string Fact1<T1, T2>(T1 input1, T2 input2) => input1.ToString() + input2.ToString();

      public static void StaticMethod() { }

      [SerializationConstructor]
      public Dummy(string testField) : this()
      {
        TestField = testField;
      }
    }

    public class DummyException : Exception
    {
#if SERIALIZATION

      protected DummyException(
          SerializationInfo info,
          StreamingContext context) : base(info, context)
      {
      }

#endif
    }

    [Fact]
    public void CanSerializeFieldInfo()
    {
      var fieldInfo = typeof(Dummy).GetField("TestField");
      var bytes = MessagePackSerializer.Serialize(fieldInfo);
      Assert.Equal(fieldInfo, MessagePackSerializer.Deserialize<FieldInfo>(bytes));
      var copy = s_formatter.DeepCopy(fieldInfo);
      Assert.Equal(fieldInfo, copy);
    }

    [Fact]
    public void CanSerializePropertyInfo()
    {
      var propertyInfo = typeof(Dummy).GetProperty("TestProperty");
      var bytes = MessagePackSerializer.Serialize(propertyInfo);
      Assert.Equal(propertyInfo, MessagePackSerializer.Deserialize<PropertyInfo>(bytes));
      var copy = s_formatter.DeepCopy(propertyInfo);
      Assert.Equal(propertyInfo, copy);
    }

    [Fact]
    public void CanSerializeMethodInfo()
    {
      var methodInfo = TypeUtils.Method((Dummy d) => d.Fact(default));
      var copy = s_formatter.DeepCopy(methodInfo);
      Assert.Equal(methodInfo, copy);

      methodInfo = TypeUtils.Method((Dummy d) => d.Fact1<int>(default));
      copy = s_formatter.DeepCopy(methodInfo);
      Assert.Equal(methodInfo, copy);

      methodInfo = TypeUtils.Method((Dummy d) => d.Fact1<int, int>(default, default));
      copy = s_formatter.DeepCopy(methodInfo);
      Assert.Equal(methodInfo, copy);

      methodInfo = TypeUtils.Method(() => Dummy.StaticMethod());
      copy = s_formatter.DeepCopy(methodInfo);
      Assert.Equal(methodInfo, copy);

      methodInfo = TypeUtils.Method((IFoo d) => d.Fact());
      copy = s_formatter.DeepCopy(methodInfo);
      Assert.Equal(methodInfo, copy);

      methodInfo = TypeUtils.Method((IFoo d) => d.Fact(default));
      copy = s_formatter.DeepCopy(methodInfo);
      Assert.Equal(methodInfo, copy);

      methodInfo = TypeUtils.Method((IFoo d) => d.Fact1<int>(default));
      copy = s_formatter.DeepCopy(methodInfo);
      Assert.Equal(methodInfo, copy);
    }

    [Fact]
    public void CanSerializeSymbolDocumentInfo()
    {
      var info = Expression.SymbolDocument("testFile");

      var bytes = MessagePackSerializer.Serialize(info);
      var deserialized = MessagePackSerializer.Deserialize<SymbolDocumentInfo>(bytes);
      Assert.Equal(info.FileName, deserialized.FileName);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(info);
      Assert.Equal(info.FileName, deserialized.FileName);

      deserialized = (SymbolDocumentInfo)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(info);
      Assert.Equal(info.FileName, deserialized.FileName);

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(info);
      Assert.Equal(info.FileName, deserialized.FileName);

      deserialized = (SymbolDocumentInfo)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(info);
      Assert.Equal(info.FileName, deserialized.FileName);
    }

    [Fact]
    public void CanSerializeConstructorInfo()
    {
      var constructorInfo = typeof(Dummy).GetConstructor(new[] { typeof(string) });
      var bytes = MessagePackSerializer.Serialize(constructorInfo);
      Assert.Equal(constructorInfo, MessagePackSerializer.Deserialize<ConstructorInfo>(bytes));
      var copy = s_formatter.DeepCopy(constructorInfo);
      Assert.Equal(constructorInfo, copy);

      constructorInfo = typeof(Dummy).GetConstructor(Type.EmptyTypes);
      bytes = MessagePackSerializer.Serialize(constructorInfo);
      Assert.Equal(constructorInfo, MessagePackSerializer.Deserialize<ConstructorInfo>(bytes));
      copy = s_formatter.DeepCopy(constructorInfo);
      Assert.Equal(constructorInfo, copy);
    }

    [Fact]
    public void CanSerializeConstantExpression()
    {
      var expr = Expression.Constant(12);
      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<ConstantExpression>(bytes);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Value, deserialized.Value);
      Assert.Equal(expr.Type, deserialized.Type);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Value, deserialized.Value);
      Assert.Equal(expr.Type, deserialized.Type);

      deserialized = (ConstantExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Value, deserialized.Value);
      Assert.Equal(expr.Type, deserialized.Type);

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Value, deserialized.Value);
      Assert.Equal(expr.Type, deserialized.Type);

      deserialized = (ConstantExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Value, deserialized.Value);
      Assert.Equal(expr.Type, deserialized.Type);
    }

    [Fact]
    public void CanSerializeUnaryExpression()
    {
      var expr = Expression.Decrement(Expression.Constant(1));
      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<UnaryExpression>(bytes);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Method, deserialized.Method);
      Assert.Equal(expr.Operand.ConstantValue(), deserialized.Operand.ConstantValue());

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Method, deserialized.Method);
      Assert.Equal(expr.Operand.ConstantValue(), deserialized.Operand.ConstantValue());

      deserialized = (UnaryExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Method, deserialized.Method);
      Assert.Equal(expr.Operand.ConstantValue(), deserialized.Operand.ConstantValue());

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Method, deserialized.Method);
      Assert.Equal(expr.Operand.ConstantValue(), deserialized.Operand.ConstantValue());

      deserialized = (UnaryExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Method, deserialized.Method);
      Assert.Equal(expr.Operand.ConstantValue(), deserialized.Operand.ConstantValue());
    }

    [Fact]
    public void CanSerializeBinaryExpression()
    {
      var expr = Expression.Add(Expression.Constant(1), Expression.Constant(2));

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<BinaryExpression>(bytes);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Method, deserialized.Method);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Method, deserialized.Method);

      deserialized = (BinaryExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Method, deserialized.Method);

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Method, deserialized.Method);

      deserialized = (BinaryExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Method, deserialized.Method);
    }

    [Fact]
    public void CanSerializeIndexExpression()
    {
      var value = new[] { 1, 2, 3 };
      var arrayExpr = Expression.Constant(value);
      var expr = Expression.ArrayAccess(arrayExpr, Expression.Constant(1));

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<IndexExpression>(bytes);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Indexer, deserialized.Indexer);
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());
      var actual = (int[])deserialized.Object.ConstantValue();
      Assert.Equal(value[0], actual[0]);
      Assert.Equal(value[1], actual[1]);
      Assert.Equal(value[2], actual[2]);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Indexer, deserialized.Indexer);
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());
      actual = (int[])deserialized.Object.ConstantValue();
      Assert.Equal(value[0], actual[0]);
      Assert.Equal(value[1], actual[1]);
      Assert.Equal(value[2], actual[2]);

      deserialized = (IndexExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Indexer, deserialized.Indexer);
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());
      actual = (int[])deserialized.Object.ConstantValue();
      Assert.Equal(value[0], actual[0]);
      Assert.Equal(value[1], actual[1]);
      Assert.Equal(value[2], actual[2]);

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Indexer, deserialized.Indexer);
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());
      actual = (int[])deserialized.Object.ConstantValue();
      Assert.Equal(value[0], actual[0]);
      Assert.Equal(value[1], actual[1]);
      Assert.Equal(value[2], actual[2]);

      deserialized = (IndexExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Indexer, deserialized.Indexer);
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());
      actual = (int[])deserialized.Object.ConstantValue();
      Assert.Equal(value[0], actual[0]);
      Assert.Equal(value[1], actual[1]);
      Assert.Equal(value[2], actual[2]);
    }

    [Fact]
    public void CanSerializeMemberAssignment()
    {
      var property = typeof(Dummy).GetProperty("TestProperty");
      var expr = Expression.Bind(property.SetMethod, Expression.Constant(9));

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<MemberAssignment>(bytes);
      Assert.Equal(expr.BindingType, deserialized.BindingType);
      Assert.Equal(expr.Member, deserialized.Member);
      Assert.Equal(expr.Expression.ConstantValue(), deserialized.Expression.ConstantValue());

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.BindingType, deserialized.BindingType);
      Assert.Equal(expr.Member, deserialized.Member);
      Assert.Equal(expr.Expression.ConstantValue(), deserialized.Expression.ConstantValue());

      deserialized = (MemberAssignment)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.BindingType, deserialized.BindingType);
      Assert.Equal(expr.Member, deserialized.Member);
      Assert.Equal(expr.Expression.ConstantValue(), deserialized.Expression.ConstantValue());

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.BindingType, deserialized.BindingType);
      Assert.Equal(expr.Member, deserialized.Member);
      Assert.Equal(expr.Expression.ConstantValue(), deserialized.Expression.ConstantValue());

      deserialized = (MemberAssignment)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.BindingType, deserialized.BindingType);
      Assert.Equal(expr.Member, deserialized.Member);
      Assert.Equal(expr.Expression.ConstantValue(), deserialized.Expression.ConstantValue());
    }

    [Fact]
    public void CanSerializeConditionalExpression()
    {
      var expr = Expression.Condition(Expression.Constant(true), Expression.Constant(1), Expression.Constant(2));

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<ConditionalExpression>(bytes);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Test.ConstantValue(), deserialized.Test.ConstantValue());
      Assert.Equal(expr.IfTrue.ConstantValue(), deserialized.IfTrue.ConstantValue());
      Assert.Equal(expr.IfFalse.ConstantValue(), deserialized.IfFalse.ConstantValue());

      deserialized = (ConditionalExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Test.ConstantValue(), deserialized.Test.ConstantValue());
      Assert.Equal(expr.IfTrue.ConstantValue(), deserialized.IfTrue.ConstantValue());
      Assert.Equal(expr.IfFalse.ConstantValue(), deserialized.IfFalse.ConstantValue());

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Test.ConstantValue(), deserialized.Test.ConstantValue());
      Assert.Equal(expr.IfTrue.ConstantValue(), deserialized.IfTrue.ConstantValue());
      Assert.Equal(expr.IfFalse.ConstantValue(), deserialized.IfFalse.ConstantValue());

      deserialized = (ConditionalExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Test.ConstantValue(), deserialized.Test.ConstantValue());
      Assert.Equal(expr.IfTrue.ConstantValue(), deserialized.IfTrue.ConstantValue());
      Assert.Equal(expr.IfFalse.ConstantValue(), deserialized.IfFalse.ConstantValue());
    }

    [Fact]
    public void CanSerializeBlockExpression()
    {
      var expr = Expression.Block(new[] { Expression.Constant(1), Expression.Constant(2), Expression.Constant(3) });

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<BlockExpression>(bytes);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Expressions.Count, deserialized.Expressions.Count);
      Assert.Equal(expr.Expressions[0].ConstantValue(), deserialized.Expressions[0].ConstantValue());
      Assert.Equal(expr.Expressions[1].ConstantValue(), deserialized.Expressions[1].ConstantValue());
      Assert.Equal(expr.Result.ConstantValue(), deserialized.Result.ConstantValue());

      deserialized = (BlockExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Expressions.Count, deserialized.Expressions.Count);
      Assert.Equal(expr.Expressions[0].ConstantValue(), deserialized.Expressions[0].ConstantValue());
      Assert.Equal(expr.Expressions[1].ConstantValue(), deserialized.Expressions[1].ConstantValue());
      Assert.Equal(expr.Result.ConstantValue(), deserialized.Result.ConstantValue());

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Expressions.Count, deserialized.Expressions.Count);
      Assert.Equal(expr.Expressions[0].ConstantValue(), deserialized.Expressions[0].ConstantValue());
      Assert.Equal(expr.Expressions[1].ConstantValue(), deserialized.Expressions[1].ConstantValue());
      Assert.Equal(expr.Result.ConstantValue(), deserialized.Result.ConstantValue());

      deserialized = (BlockExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Expressions.Count, deserialized.Expressions.Count);
      Assert.Equal(expr.Expressions[0].ConstantValue(), deserialized.Expressions[0].ConstantValue());
      Assert.Equal(expr.Expressions[1].ConstantValue(), deserialized.Expressions[1].ConstantValue());
      Assert.Equal(expr.Result.ConstantValue(), deserialized.Result.ConstantValue());
    }

    [Fact]
    public void CanSerializeLabelTarget()
    {
      var label = Expression.Label(typeof(int), "testLabel");

      var bytes = MessagePackSerializer.Serialize(label);
      var deserialized = MessagePackSerializer.Deserialize<LabelTarget>(bytes);
      Assert.Equal(label.Name, deserialized.Name);
      Assert.Equal(label.Type, deserialized.Type);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(label);
      Assert.Equal(label.Name, deserialized.Name);
      Assert.Equal(label.Type, deserialized.Type);

      deserialized = (LabelTarget)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(label);
      Assert.Equal(label.Name, deserialized.Name);
      Assert.Equal(label.Type, deserialized.Type);

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(label);
      Assert.Equal(label.Name, deserialized.Name);
      Assert.Equal(label.Type, deserialized.Type);

      deserialized = (LabelTarget)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(label);
      Assert.Equal(label.Name, deserialized.Name);
      Assert.Equal(label.Type, deserialized.Type);
    }

    [Fact]
    public void CanSerializeLabelExpression()
    {
      var label = Expression.Label(typeof(int), "testLabel");
      var expr = Expression.Label(label, Expression.Constant(2));

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<LabelExpression>(bytes);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Target.Name, deserialized.Target.Name);
      Assert.Equal(expr.DefaultValue.ConstantValue(), deserialized.DefaultValue.ConstantValue());

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Target.Name, deserialized.Target.Name);
      Assert.Equal(expr.DefaultValue.ConstantValue(), deserialized.DefaultValue.ConstantValue());

      deserialized = (LabelExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Target.Name, deserialized.Target.Name);
      Assert.Equal(expr.DefaultValue.ConstantValue(), deserialized.DefaultValue.ConstantValue());

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Target.Name, deserialized.Target.Name);
      Assert.Equal(expr.DefaultValue.ConstantValue(), deserialized.DefaultValue.ConstantValue());

      deserialized = (LabelExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Target.Name, deserialized.Target.Name);
      Assert.Equal(expr.DefaultValue.ConstantValue(), deserialized.DefaultValue.ConstantValue());
    }

    [Fact]
    public void CanSerializeMethodCallExpression()
    {
      var methodInfo = typeof(Dummy).GetMethod("Fact");
      var expr = Expression.Call(Expression.Constant(new Dummy()), methodInfo, Expression.Constant("test string"));

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<MethodCallExpression>(bytes);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Method, deserialized.Method);
      Assert.Equal(expr.Object.ConstantValue(), deserialized.Object.ConstantValue());
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Method, deserialized.Method);
      Assert.Equal(expr.Object.ConstantValue(), deserialized.Object.ConstantValue());
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = (MethodCallExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Method, deserialized.Method);
      Assert.Equal(expr.Object.ConstantValue(), deserialized.Object.ConstantValue());
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Method, deserialized.Method);
      Assert.Equal(expr.Object.ConstantValue(), deserialized.Object.ConstantValue());
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = (MethodCallExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Method, deserialized.Method);
      Assert.Equal(expr.Object.ConstantValue(), deserialized.Object.ConstantValue());
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());
    }

    [Fact]
    public void CanSerializeDefaultExpression()
    {
      var expr = Expression.Default(typeof(int));

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<DefaultExpression>(bytes);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);

      deserialized = (DefaultExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);

      deserialized = (DefaultExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
    }

    [Fact]
    public void CanSerializeParameterExpression()
    {
      var expr = Expression.Parameter(typeof(int), "p1");

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<ParameterExpression>(bytes);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Name, deserialized.Name);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Name, deserialized.Name);

      deserialized = (ParameterExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Name, deserialized.Name);

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Name, deserialized.Name);

      deserialized = (ParameterExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Name, deserialized.Name);
    }

    [Fact]
    public void CanSerializeTargetInvocationException()
    {
      var exc = new TargetInvocationException(null);

      var bytes = MessagePackSerializer.Serialize(exc);
      var deserialized = MessagePackSerializer.Deserialize<TargetInvocationException>(bytes);
      Assert.Equal(exc.Message, deserialized.Message);
      Assert.Equal(exc.StackTrace, deserialized.StackTrace);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(exc);
      Assert.Equal(exc.Message, deserialized.Message);
      Assert.Equal(exc.StackTrace, deserialized.StackTrace);

      deserialized = (TargetInvocationException)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(exc);
      Assert.Equal(exc.Message, deserialized.Message);
      Assert.Equal(exc.StackTrace, deserialized.StackTrace);

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(exc);
      Assert.Equal(exc.Message, deserialized.Message);
      Assert.Equal(exc.StackTrace, deserialized.StackTrace);

      deserialized = (TargetInvocationException)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(exc);
      Assert.Equal(exc.Message, deserialized.Message);
      Assert.Equal(exc.StackTrace, deserialized.StackTrace);
    }

    [Fact]
    public void CanSerializeObjectDisposedException()
    {
      var exc = new ObjectDisposedException("Object is already disposed", new ArgumentException("One level deeper"));

      var bytes = MessagePackSerializer.Serialize(exc);
      var deserialized = MessagePackSerializer.Deserialize<ObjectDisposedException>(bytes);
      Assert.Equal(exc.Message, deserialized.Message);
      Assert.Equal(exc.StackTrace, deserialized.StackTrace);
      Assert.Equal(exc.InnerException.GetType(), deserialized.InnerException.GetType());
      Assert.Equal(exc.InnerException.Message, deserialized.InnerException.Message);
      Assert.Equal(exc.InnerException.StackTrace, deserialized.InnerException.StackTrace);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(exc);
      Assert.Equal(exc.Message, deserialized.Message);
      Assert.Equal(exc.StackTrace, deserialized.StackTrace);
      Assert.Equal(exc.InnerException.GetType(), deserialized.InnerException.GetType());
      Assert.Equal(exc.InnerException.Message, deserialized.InnerException.Message);
      Assert.Equal(exc.InnerException.StackTrace, deserialized.InnerException.StackTrace);

      deserialized = (ObjectDisposedException)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(exc);
      Assert.Equal(exc.Message, deserialized.Message);
      Assert.Equal(exc.StackTrace, deserialized.StackTrace);
      Assert.Equal(exc.InnerException.GetType(), deserialized.InnerException.GetType());
      Assert.Equal(exc.InnerException.Message, deserialized.InnerException.Message);
      Assert.Equal(exc.InnerException.StackTrace, deserialized.InnerException.StackTrace);

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(exc);
      Assert.Equal(exc.Message, deserialized.Message);
      Assert.Equal(exc.StackTrace, deserialized.StackTrace);
      Assert.Equal(exc.InnerException.GetType(), deserialized.InnerException.GetType());
      Assert.Equal(exc.InnerException.Message, deserialized.InnerException.Message);
      Assert.Equal(exc.InnerException.StackTrace, deserialized.InnerException.StackTrace);

      deserialized = (ObjectDisposedException)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(exc);
      Assert.Equal(exc.Message, deserialized.Message);
      Assert.Equal(exc.StackTrace, deserialized.StackTrace);
      Assert.Equal(exc.InnerException.GetType(), deserialized.InnerException.GetType());
      Assert.Equal(exc.InnerException.Message, deserialized.InnerException.Message);
      Assert.Equal(exc.InnerException.StackTrace, deserialized.InnerException.StackTrace);
    }

    [Fact]
    public void CanSerializeCatchBlock()
    {
      var expr = Expression.Catch(typeof(DummyException), Expression.Constant(2));

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<CatchBlock>(bytes);
      Assert.Equal(expr.Test, deserialized.Test);
      Assert.Equal(expr.Body.ConstantValue(), deserialized.Body.ConstantValue());

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.Test, deserialized.Test);
      Assert.Equal(expr.Body.ConstantValue(), deserialized.Body.ConstantValue());

      deserialized = (CatchBlock)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.Test, deserialized.Test);
      Assert.Equal(expr.Body.ConstantValue(), deserialized.Body.ConstantValue());

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.Test, deserialized.Test);
      Assert.Equal(expr.Body.ConstantValue(), deserialized.Body.ConstantValue());

      deserialized = (CatchBlock)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.Test, deserialized.Test);
      Assert.Equal(expr.Body.ConstantValue(), deserialized.Body.ConstantValue());
    }

    [Fact]
    public void CanSerializeGotoExpression()
    {
      var label = Expression.Label(typeof(void), "testLabel");
      var expr = Expression.Continue(label);

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<GotoExpression>(bytes);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Kind, deserialized.Kind);
      Assert.Equal(expr.Target.Name, deserialized.Target.Name);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Kind, deserialized.Kind);
      Assert.Equal(expr.Target.Name, deserialized.Target.Name);

      deserialized = (GotoExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Kind, deserialized.Kind);
      Assert.Equal(expr.Target.Name, deserialized.Target.Name);

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Kind, deserialized.Kind);
      Assert.Equal(expr.Target.Name, deserialized.Target.Name);

      deserialized = (GotoExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Kind, deserialized.Kind);
      Assert.Equal(expr.Target.Name, deserialized.Target.Name);
    }

    [Fact]
    public void CanSerializeNewExpression()
    {
      var ctor = typeof(Dummy).GetConstructor(new[] { typeof(string) });
      var expr = Expression.New(ctor, Expression.Constant("test param"));

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<NewExpression>(bytes);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Constructor, deserialized.Constructor);
      Assert.Equal(expr.Arguments.Count, deserialized.Arguments.Count);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Constructor, deserialized.Constructor);
      Assert.Equal(expr.Arguments.Count, deserialized.Arguments.Count);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = (NewExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Constructor, deserialized.Constructor);
      Assert.Equal(expr.Arguments.Count, deserialized.Arguments.Count);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Constructor, deserialized.Constructor);
      Assert.Equal(expr.Arguments.Count, deserialized.Arguments.Count);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = (NewExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Constructor, deserialized.Constructor);
      Assert.Equal(expr.Arguments.Count, deserialized.Arguments.Count);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());
    }

    [Fact]
    public void CanSerializeDebugInfoExpression()
    {
      var info = Expression.SymbolDocument("testFile");
      var expr = Expression.DebugInfo(info, 1, 2, 3, 4);

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<DebugInfoExpression>(bytes);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Document.FileName, deserialized.Document.FileName);
      Assert.Equal(expr.EndColumn, deserialized.EndColumn);
      Assert.Equal(expr.StartColumn, deserialized.StartColumn);
      Assert.Equal(expr.EndLine, deserialized.EndLine);
      Assert.Equal(expr.StartLine, deserialized.StartLine);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Document.FileName, deserialized.Document.FileName);
      Assert.Equal(expr.EndColumn, deserialized.EndColumn);
      Assert.Equal(expr.StartColumn, deserialized.StartColumn);
      Assert.Equal(expr.EndLine, deserialized.EndLine);
      Assert.Equal(expr.StartLine, deserialized.StartLine);

      deserialized = (DebugInfoExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Document.FileName, deserialized.Document.FileName);
      Assert.Equal(expr.EndColumn, deserialized.EndColumn);
      Assert.Equal(expr.StartColumn, deserialized.StartColumn);
      Assert.Equal(expr.EndLine, deserialized.EndLine);
      Assert.Equal(expr.StartLine, deserialized.StartLine);

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Document.FileName, deserialized.Document.FileName);
      Assert.Equal(expr.EndColumn, deserialized.EndColumn);
      Assert.Equal(expr.StartColumn, deserialized.StartColumn);
      Assert.Equal(expr.EndLine, deserialized.EndLine);
      Assert.Equal(expr.StartLine, deserialized.StartLine);

      deserialized = (DebugInfoExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Document.FileName, deserialized.Document.FileName);
      Assert.Equal(expr.EndColumn, deserialized.EndColumn);
      Assert.Equal(expr.StartColumn, deserialized.StartColumn);
      Assert.Equal(expr.EndLine, deserialized.EndLine);
      Assert.Equal(expr.StartLine, deserialized.StartLine);
    }

    [Fact]
    public void CanSerializeLambdaExpression()
    {
      var methodInfo = typeof(Dummy).GetMethod("Fact");
      var param = Expression.Parameter(typeof(Dummy), "dummy");
      var expr = Expression.Lambda(Expression.Call(param, methodInfo, Expression.Constant("s")), param);

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<LambdaExpression>(bytes);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Name, deserialized.Name);
      Assert.Equal(expr.TailCall, deserialized.TailCall);
      Assert.Equal(expr.ReturnType, deserialized.ReturnType);
      Assert.Equal(expr.Parameters.Count, deserialized.Parameters.Count);
      Assert.Equal(expr.Parameters[0].Name, deserialized.Parameters[0].Name);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Name, deserialized.Name);
      Assert.Equal(expr.TailCall, deserialized.TailCall);
      Assert.Equal(expr.ReturnType, deserialized.ReturnType);
      Assert.Equal(expr.Parameters.Count, deserialized.Parameters.Count);
      Assert.Equal(expr.Parameters[0].Name, deserialized.Parameters[0].Name);

      deserialized = (LambdaExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Name, deserialized.Name);
      Assert.Equal(expr.TailCall, deserialized.TailCall);
      Assert.Equal(expr.ReturnType, deserialized.ReturnType);
      Assert.Equal(expr.Parameters.Count, deserialized.Parameters.Count);
      Assert.Equal(expr.Parameters[0].Name, deserialized.Parameters[0].Name);

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Name, deserialized.Name);
      Assert.Equal(expr.TailCall, deserialized.TailCall);
      Assert.Equal(expr.ReturnType, deserialized.ReturnType);
      Assert.Equal(expr.Parameters.Count, deserialized.Parameters.Count);
      Assert.Equal(expr.Parameters[0].Name, deserialized.Parameters[0].Name);

      deserialized = (LambdaExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Name, deserialized.Name);
      Assert.Equal(expr.TailCall, deserialized.TailCall);
      Assert.Equal(expr.ReturnType, deserialized.ReturnType);
      Assert.Equal(expr.Parameters.Count, deserialized.Parameters.Count);
      Assert.Equal(expr.Parameters[0].Name, deserialized.Parameters[0].Name);
    }

    [Fact]
    public void CanSerializeLambdaExpressionContainingGenericMethod()
    {
      Expression<Func<Dummy, bool>> expr = dummy => dummy.TestField.Contains('s');

      var bytes = MessagePackSerializer.Serialize(expr, WithExpressionResolver.Instance);
      var deserialized = MessagePackSerializer.Deserialize<Expression<Func<Dummy, bool>>>(bytes, WithExpressionResolver.Instance);
      Assert.NotNull(((MethodCallExpression)deserialized.Body).Method);
      Assert.True(deserialized.Compile()(new Dummy("sausages")));
      Assert.False(deserialized.Compile()(new Dummy("field")));

      //deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      //Assert.NotNull(((MethodCallExpression)deserialized.Body).Method);
      //Assert.True(deserialized.Compile()(new Dummy("sausages")));
      //Assert.False(deserialized.Compile()(new Dummy("field")));

      //deserialized = (Expression<Func<Dummy, bool>>)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      //Assert.NotNull(((MethodCallExpression)deserialized.Body).Method);
      //Assert.True(deserialized.Compile()(new Dummy("sausages")));
      //Assert.False(deserialized.Compile()(new Dummy("field")));

      //deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      //Assert.NotNull(((MethodCallExpression)deserialized.Body).Method);
      //Assert.True(deserialized.Compile()(new Dummy("sausages")));
      //Assert.False(deserialized.Compile()(new Dummy("field")));

      //deserialized = (Expression<Func<Dummy, bool>>)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      //Assert.NotNull(((MethodCallExpression)deserialized.Body).Method);
      //Assert.True(deserialized.Compile()(new Dummy("sausages")));
      //Assert.False(deserialized.Compile()(new Dummy("field")));
    }


    [Fact]
    public void CanSerializeInvocationExpression()
    {
      var methodInfo = typeof(Dummy).GetMethod("Fact");
      var param = Expression.Parameter(typeof(Dummy), "dummy");
      var lambda = Expression.Lambda(Expression.Call(param, methodInfo, Expression.Constant("s")), param);
      var expr = Expression.Invoke(lambda, Expression.Constant(new Dummy()));

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<InvocationExpression>(bytes);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Arguments.Count, deserialized.Arguments.Count);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Arguments.Count, deserialized.Arguments.Count);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = (InvocationExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Arguments.Count, deserialized.Arguments.Count);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Arguments.Count, deserialized.Arguments.Count);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = (InvocationExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Arguments.Count, deserialized.Arguments.Count);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());
    }

    [Fact]
    public void CanSerializeElementInit()
    {
      var listAddMethod = typeof(List<int>).GetMethod("Add");
      var expr = Expression.ElementInit(listAddMethod, Expression.Constant(1));

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<ElementInit>(bytes);
      Assert.Equal(expr.AddMethod, deserialized.AddMethod);
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.AddMethod, deserialized.AddMethod);
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = (ElementInit)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.AddMethod, deserialized.AddMethod);
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.AddMethod, deserialized.AddMethod);
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());

      deserialized = (ElementInit)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.AddMethod, deserialized.AddMethod);
      Assert.Single(deserialized.Arguments);
      Assert.Equal(expr.Arguments[0].ConstantValue(), deserialized.Arguments[0].ConstantValue());
    }

    [Fact]
    public void CanSerializeLoopExpression()
    {
      var breakLabel = Expression.Label(typeof(void), "break");
      var continueLabel = Expression.Label(typeof(void), "cont");
      var expr = Expression.Loop(Expression.Constant(2), breakLabel, continueLabel);

      var bytes = MessagePackSerializer.Serialize(expr);
      var deserialized = MessagePackSerializer.Deserialize<LoopExpression>(bytes);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Body.ConstantValue(), deserialized.Body.ConstantValue());
      Assert.Equal(expr.BreakLabel.Name, deserialized.BreakLabel.Name);
      Assert.Equal(expr.ContinueLabel.Name, deserialized.ContinueLabel.Name);

      deserialized = MessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Body.ConstantValue(), deserialized.Body.ConstantValue());
      Assert.Equal(expr.BreakLabel.Name, deserialized.BreakLabel.Name);
      Assert.Equal(expr.ContinueLabel.Name, deserialized.ContinueLabel.Name);

      deserialized = (LoopExpression)MessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Body.ConstantValue(), deserialized.Body.ConstantValue());
      Assert.Equal(expr.BreakLabel.Name, deserialized.BreakLabel.Name);
      Assert.Equal(expr.ContinueLabel.Name, deserialized.ContinueLabel.Name);

      deserialized = TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopy(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Body.ConstantValue(), deserialized.Body.ConstantValue());
      Assert.Equal(expr.BreakLabel.Name, deserialized.BreakLabel.Name);
      Assert.Equal(expr.ContinueLabel.Name, deserialized.ContinueLabel.Name);

      deserialized = (LoopExpression)TypelessMessagePackMessageFormatter.DefaultInstance.DeepCopyObject(expr);
      Assert.Equal(expr.NodeType, deserialized.NodeType);
      Assert.Equal(expr.Type, deserialized.Type);
      Assert.Equal(expr.Body.ConstantValue(), deserialized.Body.ConstantValue());
      Assert.Equal(expr.BreakLabel.Name, deserialized.BreakLabel.Name);
      Assert.Equal(expr.ContinueLabel.Name, deserialized.ContinueLabel.Name);
    }
  }

  internal static class ExpressionExtensions
  {
    public static object ConstantValue(this Expression expr) => ((ConstantExpression)expr).Value;
  }

  public class WithExpressionResolver : FormatterResolver
  {
    public static readonly WithExpressionResolver Instance = new WithExpressionResolver();

    public override IMessagePackFormatter<T> GetFormatter<T>()
    {
      return (HyperionExpressionResolver.Instance.GetFormatter<T>()
           ?? StandardResolver.Instance.GetFormatter<T>());
    }
  }
}