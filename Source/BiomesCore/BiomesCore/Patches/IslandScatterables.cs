using BiomesCore.DefModExtensions;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace BiomesCore.Patches
{

    [HarmonyPatch(typeof(Verse.GenStep_Scatterer), nameof(Verse.GenStep_Scatterer.Generate))]
    static class IslandScatterables
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo waterBiome = AccessTools.Field(type: typeof(GenStep_Scatterer), name: nameof(GenStep_Scatterer.allowInWaterBiome));

            CodeInstruction[] codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            foreach (CodeInstruction instruction in codeInstructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && (FieldInfo)instruction.operand == waterBiome)
                {
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_1);
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(IslandScatterables), name: nameof(IslandScatterables.AllowedInWaterBiome)));
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        static bool AllowedInWaterBiome(GenStep_Scatterer step, Map map)
        {
            if (map.Biome.HasModExtension<BiomesMap>())
            {
                if (map.Biome.GetModExtension<BiomesMap>().hasScatterables)
                {
                    return true;
                }
            }
            return step.allowInWaterBiome;
        }

    }

    //[HarmonyPatch(typeof(Verse.GenStep_Scatterer), nameof(Verse.GenStep_Scatterer.Generate))]
    //static class IslandScatterables
    //{
    //    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        FieldInfo waterBiome = AccessTools.Field(type: typeof(GenStep_Scatterer), name: nameof(GenStep_Scatterer.allowInWaterBiome));

    //        CodeInstruction[] codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
    //        foreach (CodeInstruction instruction in codeInstructions)
    //        {
    //            if (instruction.opcode == OpCodes.Ldfld && instruction.operand == waterBiome)
    //            {
    //                yield return new CodeInstruction(opcode: OpCodes.Ldarg_1);
    //                yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(IslandGeysers), name: nameof(IslandScatterables.AllowedInWaterBiome)));
    //            }
    //            else
    //            {
    //                yield return instruction;
    //            }
    //        }
    //    }

    //    static bool AllowedInWaterBiome(GenStep_Scatterer step, Map map)
    //    {
    //        if (map.Biome.HasModExtension<BiomesMap>())
    //        {
    //            if (map.Biome.GetModExtension<BiomesMap>().hasScatterables)
    //            {
    //                return true;
    //            }
    //        }
    //        return step.allowInWaterBiome;
    //    }

    //}


}
