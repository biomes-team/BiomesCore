using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(WorldGenStep_Terrain))]
    [HarmonyPatch("GenerateTileFor")]
    internal static class IslandHilliness
    {
        static void Postfix(PlanetTile tile, PlanetLayer layer, ref Tile __result)
        {
            if (!__result.PrimaryBiome.HasModExtension<BiomesMap>())
            {
                return;
            }
            //if (!__result.PrimaryBiome.GetModExtension<BiomesMap>().isIsland)
            //{
            //    return;
            //}
            if (!__result.PrimaryBiome.GetModExtension<BiomesMap>().addIslandHills)
            {
                return;
            }

            switch (Rand.Range(0, 4))
            {
                case 0:
                    __result.hilliness = Hilliness.Flat;
                    break;
                case 1:
                    __result.hilliness = Hilliness.SmallHills;
                    break;
                case 2:
                    __result.hilliness = Hilliness.LargeHills;
                    break;
                case 3:
                    __result.hilliness = Hilliness.Mountainous;
                    break;
            }
        }
    }
}
