using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BiomesCore.Patches
{
    [HarmonyPatch]
    static class MultiYieldPatch {
    static MethodInfo TargetMethod() {
    //foreach loop
      //foreach loop
        //if statement
          return info;
  }
  
  static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase mb) {
    // The transpiler stuff here
  }
}
    {
        MethodInfo TargetMethod()
        {
            //foreach loop
            //foreach loop
            //if statement
            return info;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase mb)
        {
            // The transpiler stuff here
        }
    }
}
