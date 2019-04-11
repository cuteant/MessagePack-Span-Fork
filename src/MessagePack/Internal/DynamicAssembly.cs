﻿using System;
using System.Reflection;
using System.Reflection.Emit;

namespace MessagePack.Internal
{
    internal class DynamicAssembly
    {
#if NETFRAMEWORK
        readonly string moduleName;
#endif
        readonly AssemblyBuilder assemblyBuilder;
        readonly ModuleBuilder moduleBuilder;

        // don't expose ModuleBuilder
        // public ModuleBuilder ModuleBuilder { get { return moduleBuilder; } }

        readonly object gate = new object();

        public DynamicAssembly(string moduleName)
        {
#if NETFRAMEWORK

            var assemblyName = moduleName;
            moduleName = assemblyName + ".dll";
            AssemblyName an = new AssemblyName();
            an.Name = assemblyName;
            this.assemblyBuilder = System.AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            this.moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
#else
            this.assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(moduleName), AssemblyBuilderAccess.Run);
            //this.assemblyBuilder = System.AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(moduleName), AssemblyBuilderAccess.Run);

            this.moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
#endif
        }

        // requires lock on mono environment. see: https://github.com/neuecc/MessagePack-CSharp/issues/161

        public TypeBuilder DefineType(string name, TypeAttributes attr)
        {
            lock (gate)
            {
                return moduleBuilder.DefineType(name, attr);
            }
        }

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent)
        {
            lock (gate)
            {
                return moduleBuilder.DefineType(name, attr, parent);
            }
        }

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, Type[] interfaces)
        {
            lock (gate)
            {
                return moduleBuilder.DefineType(name, attr, parent, interfaces);
            }
        }

#if NETFRAMEWORK

        public AssemblyBuilder Save()
        {
            assemblyBuilder.Save(moduleName + ".dll");
            return assemblyBuilder;
        }

#endif
    }
}
