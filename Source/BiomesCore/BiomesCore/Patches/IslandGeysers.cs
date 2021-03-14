using HarmonyLib;
using RimWorld;
using Verse;
using BiomesCore.DefModExtensions;

namespace BiomesCore.Patches
{

    //[HarmonyPatch(typeof(GenStep_ScatterThings), "Generate", null)]

    [HarmonyPatch(typeof(GenStep_ScatterThings), nameof(GenStep_ScatterThings.Generate))]
    internal static class IslandGeysers
    {
        internal static bool Prefix(Map map, GenStepParams parms, ref GenStep_ScatterThings __instance)
        {
            if (__instance.thingDef == ThingDefOf.SteamGeyser)
            {
                if (map.Biome.HasModExtension<BiomesMap>())
                {
                    if (map.Biome.GetModExtension<BiomesMap>().hasScatterables && __instance.allowInWaterBiome == false)
                    {
                        bool seaSpawns = __instance.allowInWaterBiome;
                        FloatRange range = __instance.countPer10kCellsRange;
                        __instance.allowInWaterBiome = true;
                        __instance.countPer10kCellsRange.min /= 2;
                        __instance.countPer10kCellsRange.max /= 2;
                        __instance.Generate(map, parms);
                        __instance.allowInWaterBiome = seaSpawns;
                        __instance.countPer10kCellsRange = range;
                        return false;
                    }
                }
               
            }

            return true;
        }
    }

}

