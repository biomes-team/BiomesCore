using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace BiomesCore.Reflections
{
    public static class ReflectionExtension
    {
        // Taken from https://github.com/pardeike/Zombieland/blob/d1f558ca159cc56566e505b56532e40ec0f5ec89/Source/SafeReflections.cs, thank you @brrainz for the pointers in your discord!
        public static FieldInfo Field(this Type type, string fieldName)
        {
            var field = AccessTools.Field(type, fieldName);
            if (field == null) throw new Exception("Cannot find field '" + fieldName + "' in type " + type.FullName);
            return field;
        }

        public static Type InnerTypeStartingWith(this Type type, string prefix)
        {
            var innerType = AccessTools.FirstInner(type, subType => subType.Name.StartsWith(prefix));
            if (innerType == null) throw new Exception("Cannot find inner type starting with '" + prefix + "' in type " + type.FullName);
            return innerType;
        }

        public static MethodInfo MethodMatching(this Type type, Func<MethodInfo[], MethodInfo> predicate)
        {
            var method = predicate(type.GetMethods(AccessTools.all));
            if (method == null) throw new Exception("Cannot find method matching " + predicate + " in type " + type.FullName);
            return method;
        }

        // Taken from https://gist.github.com/Zetrith/bca7203aa4a256bd3abcf43545b52190 -- This was really helpful
        const string DisplayClassPrefix = "<>c__DisplayClass";
        const string SharedDisplayClass = "<>c";
        const string LambdaMethodInfix = "b__";
        const string LocalFunctionInfix = "g__";
        const string EnumerableStateMachineInfix = "d__";

        public static MethodInfo GetLambda(this Type parentType, string parentMethod = null, MethodType parentMethodType = MethodType.Normal, Type[] parentArgs = null, int lambdaOrdinal = 0)
        {
            var parent = GetMethod(parentType, parentMethod, parentMethodType, parentArgs);
            if (parent == null)
                throw new Exception($"Couldn't find parent method ({parentMethodType}) {parentType}::{parentMethod}");

            var parentId = GetMethodDebugId(parent);

            // Example: <>c__DisplayClass10_
            var displayClassPrefix = $"{DisplayClassPrefix}{parentId}_";

            // Example: <FillTab>b__0
            var lambdaNameShort = $"<{parent.Name}>{LambdaMethodInfix}{lambdaOrdinal}";

            // Capturing lambda
            var lambda = parentType.GetNestedTypes(AccessTools.all).
                Where(t => t.Name.StartsWith(displayClassPrefix)).
                SelectMany(AccessTools.GetDeclaredMethods).
                FirstOrDefault(m => m.Name == lambdaNameShort);

            // Example: <FillTab>b__10_0
            var lambdaNameFull = $"<{parent.Name}>{LambdaMethodInfix}{parentId}_{lambdaOrdinal}";

            // Non-capturing lambda
            if (lambda == null)
            {
                lambda = AccessTools.Method(parentType, lambdaNameFull);
            }

            // Non-capturing cached lambda
            if (lambda == null)
            {
                var sharedDisplayClass = AccessTools.Inner(parentType, SharedDisplayClass);
                if (sharedDisplayClass != null)
                {
                    lambda = AccessTools.Method(sharedDisplayClass, lambdaNameFull);
                }
            }

            if (lambda == null)
            {
                throw new Exception($"Couldn't find lambda {lambdaOrdinal} in parent method {parentType}::{parent.Name} (parent method id: {parentId})");
            }

            return lambda;
        }

        public static MethodInfo GetLocalFunc(this Type parentType, string parentMethod = null, MethodType parentMethodType = MethodType.Normal, Type[] parentArgs = null, string localFunc = null)
        {
            var parent = GetMethod(parentType, parentMethod, parentMethodType, parentArgs);
            if (parent == null)
                throw new Exception($"Couldn't find parent method ({parentMethodType}) {parentType}::{parentMethod}");

            var parentId = GetMethodDebugId(parent);

            // Example: <>c__DisplayClass10_
            var displayClassPrefix = $"{DisplayClassPrefix}{parentId}_";

            // Example: <DoWindowContents>g__Start|
            var localFuncPrefix = $"<{parentMethod}>{LocalFunctionInfix}{localFunc}|";

            // Example: <DoWindowContents>g__Start|10
            var localFuncPrefixWithId = $"<{parentMethod}>{LocalFunctionInfix}{localFunc}|{parentId}";

            var candidates = parentType.GetNestedTypes(AccessTools.all).
                Where(t => t.Name.StartsWith(displayClassPrefix)).
                SelectMany(AccessTools.GetDeclaredMethods).
                Where(m => m.Name.StartsWith(localFuncPrefix)).
                Concat(AccessTools.GetDeclaredMethods(parentType).Where(m => m.Name.StartsWith(localFuncPrefixWithId))).
                ToArray();

            if (candidates.Length == 0)
                throw new Exception($"Couldn't find local function {localFunc} in parent method {parentType}::{parent.Name} (parent method id: {parentId})");

            if (candidates.Length > 1)
                throw new Exception($"Ambiguous local function {localFunc} in parent method {parentType}::{parent.Name} (parent method id: {parentId})");

            return candidates[0];
        }

        // Based on https://github.com/dotnet/roslyn/blob/main/src/Compilers/CSharp/Portable/Symbols/Synthesized/GeneratedNameKind.cs
        // and https://github.com/dotnet/roslyn/blob/main/src/Compilers/CSharp/Portable/Symbols/Synthesized/GeneratedNames.cs
        public static int GetMethodDebugId(this MethodBase method)
        {
            string cur = null;

            try
            {
                // Try extract the debug id from the method body
                foreach (var inst in PatchProcessor.GetOriginalInstructions(method))
                {
                    // Example class names: <>c__DisplayClass10_0 or <CompGetGizmosExtra>d__7
                    if (inst.opcode == OpCodes.Newobj
                        && inst.operand is MethodBase m
                        && (cur = m.DeclaringType.Name) != null)
                    {
                        if (cur.StartsWith(DisplayClassPrefix))
                            return int.Parse(cur.Substring(DisplayClassPrefix.Length).Until('_'));
                        else if (cur.Contains(EnumerableStateMachineInfix))
                            return int.Parse(cur.After('>').Substring(EnumerableStateMachineInfix.Length));
                    }
                    // Example method names: <FillTab>b__10_0 or <DoWindowContents>g__Start|55_1
                    else if (
                        (inst.opcode == OpCodes.Ldftn || inst.opcode == OpCodes.Call)
                        && inst.operand is MethodBase f
                        && (cur = f.Name) != null
                        && cur.StartsWith("<")
                        && cur.After('>').CharacterCount('_') == 3)
                    {
                        if (cur.Contains(LambdaMethodInfix))
                            return int.Parse(cur.After('>').Substring(LambdaMethodInfix.Length).Until('_'));
                        else if (cur.Contains(LocalFunctionInfix))
                            return int.Parse(cur.After('|').Until('_'));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Extracting debug id for {method.DeclaringType}::{method.Name} failed at {cur} with: {e.Message}");
            }

            throw new Exception($"Couldn't determine debug id for parent method {method.DeclaringType}::{method.Name}");
        }

        // Copied from Harmony.PatchProcessor
        public static MethodBase GetMethod(this Type type, string methodName, MethodType methodType, Type[] args)
        {
            if (type == null) return null;

            switch (methodType)
            {
                case MethodType.Normal:
                    if (methodName == null)
                        return null;
                    return AccessTools.DeclaredMethod(type, methodName, args);

                case MethodType.Getter:
                    if (methodName == null)
                        return null;
                    return AccessTools.DeclaredProperty(type, methodName).GetGetMethod(true);

                case MethodType.Setter:
                    if (methodName == null)
                        return null;
                    return AccessTools.DeclaredProperty(type, methodName).GetSetMethod(true);

                case MethodType.Constructor:
                    return AccessTools.DeclaredConstructor(type, args);

                case MethodType.StaticConstructor:
                    return AccessTools
                        .GetDeclaredConstructors(type)
                        .FirstOrDefault(c => c.IsStatic);
            }

            return null;
        }

        public static string After(this string s, char c)
        {
            if (s.IndexOf(c) == -1)
                throw new ArgumentException($"Char {c} not found in string {s}");
            return s.Substring(s.IndexOf(c) + 1);
        }

        public static string Until(this string s, char c)
        {
            if (s.IndexOf(c) == -1)
                throw new ArgumentException($"Char {c} not found in string {s}");
            return s.Substring(0, s.IndexOf(c));
        }

        public static int CharacterCount(this string s, char c)
        {
            int num = 0;
            for (int i = 0; i < s.Length; i++)
                if (s[i] == c)
                    num++;
            return num;
        }

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

        // Helper for defining function signature needed for function replacement
        public delegate List<CodeInstruction> CodeInstructionReplacementFunction(List<CodeInstruction> opsUpToCall, CodeInstruction callInstruction);

        // Used to replace a function call in a transpiler patch
        public static IEnumerable<CodeInstruction> ReplaceFunction(this IEnumerable<CodeInstruction> instructions, CodeInstructionReplacementFunction func, string patchCallName, string originalFuncName, bool repeat = true)
        {
            //Log.Warning("Starting for " + patchCallName);
            bool found = false;
            List<CodeInstruction> runningChanges = new List<CodeInstruction>();
            foreach (CodeInstruction instruction in instructions)
            {
                // Find the section calling TryFindRandomPawnEntryCell
                //Log.Warning("Opcode: " + instruction.opcode.ToString() + ", Operand: " + instruction.operand.ToStringSafe());
                if ((repeat || !found) && (instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt) && (instruction.operand as MethodInfo)?.Name == patchCallName)
                {
                    runningChanges = func(runningChanges, instruction).ToList();
                    found = true;
                }
                else
                {
                    runningChanges.Add(instruction);
                }
            }
            if (!found)
            {
                Log.ErrorOnce(String.Format("[BiomesCore] Cannot find {0} in {1}, skipping patch", patchCallName, originalFuncName), patchCallName.GetHashCode() + originalFuncName.GetHashCode());
            }
            return runningChanges.AsEnumerable();
        }

        public static List<CodeInstruction> PreCallReplaceFunction(List<CodeInstruction> opsUpToCall, MethodInfo newMethod)
        {
            // Add call to new method
            opsUpToCall.Add(new CodeInstruction(OpCodes.Call, newMethod));
            return opsUpToCall;
        }

        public static IEnumerable<CodeInstruction> ReplaceFunction(this IEnumerable<CodeInstruction> instructions, MethodInfo newMethod, string patchCallName, string originalFuncName, bool repeat = true)
        {
            return ReplaceFunction(instructions, (opsUpToCall, _callInstruction) => PreCallReplaceFunction(opsUpToCall, newMethod), patchCallName, originalFuncName, repeat);
        }

        public static List<CodeInstruction> PreCallReplaceFunctionArgument(List<CodeInstruction> opsUpToCall, MethodInfo newMethod, IEnumerable<CodeInstruction> addedArguments, int distanceFromRight)
        {
            List<CodeInstruction> rightMostArgs = new List<CodeInstruction>();
            // Pop the last few parameters
            for (int i = 0; i < distanceFromRight; i++)
            {
                rightMostArgs.Insert(0, opsUpToCall.Pop());
            }
            // Add the new parameter arguments
            foreach (CodeInstruction arg in addedArguments)
            {
                opsUpToCall.Add(arg);
            }
            // Add the last few parameters back
            opsUpToCall.AddRange(rightMostArgs);
            // Add call to new method
            opsUpToCall.Add(new CodeInstruction(OpCodes.Call, newMethod));
            return opsUpToCall;
        }

        public static IEnumerable<CodeInstruction> ReplaceFunctionArgument(this IEnumerable<CodeInstruction> instructions, MethodInfo newMethod, CodeInstruction addedArgument, int distanceFromRight, string patchCallName, string originalFuncName, bool repeat = true)
        {
            return ReplaceFunction(
                instructions,
                (opsUpToCall, _callInstruction) => PreCallReplaceFunctionArgument(opsUpToCall, newMethod, new List<CodeInstruction>() { addedArgument }, distanceFromRight),
                patchCallName,
                originalFuncName,
                repeat);
        }

        public static IEnumerable<CodeInstruction> ReplaceFunctionArgument(this IEnumerable<CodeInstruction> instructions, MethodInfo newMethod, IEnumerable<CodeInstruction> addedArguments, int distanceFromRight, string patchCallName, string originalFuncName, bool repeat = true)
        {
            return ReplaceFunction(
                instructions,
                (opsUpToCall, _callInstruction) => PreCallReplaceFunctionArgument(opsUpToCall, newMethod, addedArguments, distanceFromRight),
                patchCallName,
                originalFuncName,
                repeat);
        }
    }
}
