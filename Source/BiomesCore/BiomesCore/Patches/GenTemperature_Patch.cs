using System;
using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(GenTemperature))]
    internal static class GenTemperature_Patch
    {
        [ThreadStatic]
        private static float[] _cache;
        
        [HarmonyPostfix]
        [HarmonyPatch("SeasonalShiftAmplitudeAt")]
        private static void SeasonalShiftAmplitudeAt(PlanetTile tile, ref float __result)
        {
            if (_cache == null)
            {
                _cache = new float[DefDatabase<BiomeDef>.DefCount];

                for (var i = 0; i < _cache.Length; i++)
                {
                    var ext = DefDatabase<BiomeDef>.AllDefsListForReading[i].GetModExtension<BiomesMap>();
                    _cache[i] = ext?.seasonalTemperatureShift ?? 1f;
                }
            }

            __result *= _cache[Find.WorldGrid[tile].PrimaryBiome.index];
        }
    }
}
