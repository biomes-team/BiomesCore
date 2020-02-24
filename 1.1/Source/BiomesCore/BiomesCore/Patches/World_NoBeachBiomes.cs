using Verse;
using Harmony;
using RimWorld.Planet;


namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(World), nameof(World.CoastDirectionAt))]
    internal static class World_NoBeachBiomes
    {
        // from RF-Archipelagos
        internal static bool Prefix(int tileID, ref Rot4 __result, ref World __instance)
        {
            var world = Traverse.Create(__instance);
            WorldGrid worldGrid = world.Field("grid").GetValue<WorldGrid>();
            if (worldGrid[tileID].biome.defName.Contains("NoBeach"))
            {
                __result = Rot4.Invalid;
                return false;
            }
            return true;
        }
    }
}
