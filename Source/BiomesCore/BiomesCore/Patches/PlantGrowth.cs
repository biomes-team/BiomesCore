using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(PlantUtility), "GrowthSeasonNow")]
    internal static class PlantUtility_GrowthSeasonNow
    {
        internal static bool Prefix(IntVec3 c, Map map, ref bool __result)
        {
            var modExtension = map.Biome.GetModExtension<BiomesMap>();
            if (modExtension is { alwaysGrowthSeason: true })
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
    
    [HarmonyPatch(typeof(Zone_Growing), "GrowingQuadrumsDescription")]
    internal static class Zone_Growing_GrowingQuadrumsDescription
    {
        internal static bool Prefix(int tile, ref string __result)
        {
            var modExtension = Find.WorldGrid[tile].biome.GetModExtension<BiomesMap>();
            if (modExtension is { alwaysGrowthSeason: true })
            {
                __result = "GrowYearRound".Translate();
                return false;
            }

            return true;
        }
    }
    
    [HarmonyPatch(typeof(Plant), "GrowthRateFactor_Temperature", MethodType.Getter)]
    internal static class Plant_GrowthRateFactor_Temperature
    {
        internal static bool Prefix(Plant __instance, ref float __result)
        {
            var modExtension = __instance.def.GetModExtension<Biomes_PlantControl>();
            if (modExtension == null) return true;
            
            if (!GenTemperature.TryGetTemperatureForCell(__instance.Position, __instance.Map, out var cellTemp))
            {
                __result = 1f;
                return false;
            }

            var min = modExtension.optimalTemperature.min;
            var max = modExtension.optimalTemperature.max;
            var span = modExtension.optimalTemperature.Span;

            __result = cellTemp < min 
                ? Mathf.InverseLerp(min - span / 6f, min, cellTemp) 
                : cellTemp > max ? Mathf.InverseLerp(max + span / 2f, max, cellTemp) : 1f;
                
            return false;
        }
    }
}
