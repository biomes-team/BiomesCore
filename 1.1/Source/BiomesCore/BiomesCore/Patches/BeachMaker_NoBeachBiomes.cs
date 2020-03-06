using Verse;
using HarmonyLib;
using RimWorld.Planet;
using RimWorld;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(BeachMaker), nameof(BeachMaker.Init))]
    internal static class BeachMaker_NoBeachBiomes
    {
        // from RF-Archipelagos
        internal static bool Prefix(Map map)
        {
            if (map.Biome.defName.Contains("NoBeach"))
            {
                return false;
            }
            return true;
        }
    }
}
