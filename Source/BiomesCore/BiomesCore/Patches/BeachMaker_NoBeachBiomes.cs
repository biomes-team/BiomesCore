using Verse;
using HarmonyLib;
using RimWorld;
using BiomesCore.DefModExtensions;

namespace BiomesCore.Patches
{

    // from RF-Archipelagos
    // Now works through mutators?
    //[HarmonyPatch(typeof(BeachMaker), nameof(BeachMaker.Init))]
    //internal static class BeachMaker_NoBeachBiomes
    //{
    //    internal static bool Prefix(Map map)
    //    {
    //        if (map.Biome.HasModExtension<BiomesMap>())
    //        {
    //            if (!map.Biome.GetModExtension<BiomesMap>().allowBeach)
    //            {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }
    //}
}
