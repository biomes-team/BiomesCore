using Verse;
using HarmonyLib;
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
            if (map.Biome.HasModExtension<BiomesMap>())
            {
                if (!map.Biome.GetModExtension<BiomesMap>().allowBeach)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
