using Verse;
using HarmonyLib;
using RimWorld;
using BiomesCore.DefModExtensions;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(RimWorld.TileMutatorWorker_Coast), "GeneratePostTerrain")]
    static class BeachPatch
    {
        static bool Prefix(Map map)
        {

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
