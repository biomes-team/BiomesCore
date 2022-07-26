using RimWorld;
using System.Collections.Generic;
using HarmonyLib;
using System.Linq;
using Verse;
using BiomesCore.DefModExtensions;
using System.Reflection.Emit;
using System.Reflection;
using System;
using Verse.AI;

namespace BiomesCore.Patches
{
    [HarmonyPatch]
    public static class PawnsArrivalModeWorker_EdgeWalkIn_Patch
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (var nestedType in typeof(PawnsArrivalModeWorker_EdgeWalkIn).GetNestedTypes(AccessTools.all))
            {
                foreach (var method in nestedType.GetMethods(AccessTools.all))
                {
                    if (method.Name.Contains("<TryResolveRaidSpawnCenter>"))
                    {
                        yield return method;
                    }
                }
            }
            foreach (var nestedType in typeof(RCellFinder).GetNestedTypes(AccessTools.all))
            {
                foreach (var method in nestedType.GetMethods(AccessTools.all))
                {
                    if (method.Name.Contains("<TryFindRandomPawnEntryCell>"))
                    {
                        yield return method;
                    }
                }
            }
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codes = codeInstructions.ToList();
            var roofedInfo = AccessTools.Method(typeof(RoofGrid), nameof(RoofGrid.Roofed), new Type[] { typeof(IntVec3)});
            var allowIfCavernInfo = AccessTools.Method(typeof(PawnsArrivalModeWorker_EdgeWalkIn_Patch), nameof(AllowIfCavern));
            foreach (var code in codes)
            {
                if (code.Calls(roofedInfo))
                {
                    yield return new CodeInstruction(OpCodes.Call, allowIfCavernInfo);
                }
                else
                {
                    yield return code;
                }
            }
        }

        public static bool AllowIfCavern(RoofGrid roofgrid, IntVec3 cell)
        {
            var map = Traverse.Create(roofgrid).Field("map").GetValue() as Map;
            if (map.Biome.HasModExtension<BiomesMap>())
            {
                BiomesMap biome = map.Biome.GetModExtension<BiomesMap>();
                if (biome.isCavern)
                {
                    return false;
                }
            }
            return roofgrid.Roofed(cell);
        }
    }


}
