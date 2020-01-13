using BiomesCore.DefModExtensions;
using Harmony;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(GenStep_ScatterLumpsMineable), "ScatterAt")]
    internal static class GenStep_ScatterLumpsMineable_OrePreferredBiome
    {
        internal static bool Prefix(GenStep_ScatterLumpsMineable __instance, IntVec3 c, Map map, List<IntVec3> ___recentLumpCells)
        {
            ThingDef chosen = null;
            if (__instance.forcedDefToScatter != null)
            {
                chosen = __instance.forcedDefToScatter;
            }
            else
            {
                chosen = DefDatabase<ThingDef>.AllDefs.RandomElementByWeightWithFallback(delegate (ThingDef d)
                {
                    if (d.building == null)
                    {
                        return 0f;
                    }
                    if (d.building.mineableThing != null && d.building.mineableThing.BaseMarketValue > __instance.maxValue)
                    {
                        return 0f;
                    }
                    var preferred = d.GetModExtension<DefModExtension_OrePreferredBiome>();
                    if (preferred != null && !preferred.allowedBiomes.Contains(map.Biome))
                    {
                        return d.building.mineableScatterCommonality * 0.15f;
                    }
                    return d.building.mineableScatterCommonality;
                }, null);
            }
            if (chosen != null)
            {
                int numCells = (__instance.forcedLumpSize <= 0) ? chosen.building.mineableScatterLumpSizeRange.RandomInRange : __instance.forcedLumpSize;
                ___recentLumpCells.Clear();
                foreach (IntVec3 intVec in GridShapeMaker.IrregularLump(c, map, numCells))
                {
                    GenSpawn.Spawn(chosen, intVec, map, WipeMode.Vanish);
                    ___recentLumpCells.Add(intVec);
                }
            }
            return false;
        }
    }
}