﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ICompiler.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Reflection;

namespace Hyperion.Compilation
{
    public interface ICompiler<out TDel>
    {
        int NewObject(Type type);
        int Parameter<T>(string name);
        int Variable<T>(string name);
        int Variable(string name,Type type);
        int GetVariable<T>(string name);
        int Constant(object value);
        int CastOrUnbox(int value, Type type);
        void EmitCall(MethodInfo method, int target, params int[] arguments);
        void EmitStaticCall(MethodInfo method, params int[] arguments);
        int Call(MethodInfo method, int target, params int[] arguments);
        int StaticCall(MethodInfo method, params int[] arguments);
        int ReadField(FieldInfo field, int target);
        int WriteField(FieldInfo field, int typedTarget, int target, int value);
        TDel Compile();
        int Convert<T>(int value);
        int WriteVar(int variable, int value);
        void Emit(int value);
        int Convert(int value, Type type);
    }
}