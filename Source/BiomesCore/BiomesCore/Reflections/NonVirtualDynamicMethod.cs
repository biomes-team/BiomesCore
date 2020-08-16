using System;
using System.Reflection;
using System.Reflection.Emit;

namespace BiomesCore.Reflections
{
    public static class MethodInfoExtensions
    {
        // Taken from http://kennethxu.blogspot.com/2009/05/strong-typed-high-performance.html -- Thank you for that how-to!
        public static DynamicMethod CreateNonVirtualDynamicMethod(this MethodInfo method)
        {
            int offset = (method.IsStatic ? 0 : 1);
            var parameters = method.GetParameters();
            int size = parameters.Length + offset;
            Type[] types = new Type[size];
            if (offset > 0) types[0] = method.DeclaringType;
            for (int i = offset; i < size; i++)
            {
                types[i] = parameters[i - offset].ParameterType;
            }

            DynamicMethod dynamicMethod = new DynamicMethod(
                "NonVirtualInvoker_" + method.Name, method.ReturnType, types, method.DeclaringType);
            ILGenerator il = dynamicMethod.GetILGenerator();
            for (int i = 0; i < types.Length; i++) il.Emit(OpCodes.Ldarg, i);
            il.EmitCall(OpCodes.Call, method, null);
            il.Emit(OpCodes.Ret);
            return dynamicMethod;
        }
    }
}
