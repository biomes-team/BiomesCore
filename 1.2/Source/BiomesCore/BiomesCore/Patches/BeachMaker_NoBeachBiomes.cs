using Verse;
using HarmonyLib;
using RimWorld.Planet;
using RimWorld;
using BiomesCore.DefModExtensions;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(BeachMaker), nameof(BeachMaker.Init))]
    internal static class BeachMaker_NoBeachBiomes
    {
        // from RF-Archipelagos
        internal static bool Prefix(Map map)
        {
            //if (map.Biome.defName.Contains("NoBeach"))
            //{
            //    return false;
            //}
            //return true;
            if (map.Biome.HasModExtension<Biomes_NoBeach>())
            {
                if (!map.Biome.GetModExtension<Biomes_NoBeach>().allowBeach)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
