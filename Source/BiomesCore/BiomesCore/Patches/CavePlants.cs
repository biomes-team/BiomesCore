using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{
    // This patch stops normal plants from spawning under natural roofs, and the new cave plants from spawning without an appropriate roof.
    [HarmonyPatch(typeof(PlantUtility), nameof(PlantUtility.CanEverPlantAt_NewTemp))]
    internal static class PlantUtility_CanEverPlantAt_CavePlants
    {
        internal static bool Prefix(ref bool __result, ThingDef plantDef, IntVec3 c, Map map)
        {
            RoofDef roof = map.roofGrid.RoofAt(c);
            TerrainDef terrain = map.terrainGrid.TerrainAt(c);
            string biome = map.Biome.defName;
            if (map.Biome.defName != "BMT_SurfaceCavern") // quick hack for making sure we're not blocking crops under mountains. Needs to be replaced later.
                return true;
            if (roof != null)
            {
                if (roof.isNatural)
                {
                    if (!plantDef.HasModExtension<Biomes_PlantControl>() && !plantDef.plant.cavePlant)
                    {
                        __result = false;
                        return false;
                    }
                    if (roof.defName == "BiomesCaverns_RockRoofStable")
                    {
                        if (!plantDef.HasModExtension<Biomes_PlantControl>())
                        {
                            __result = false;
                            return false;
                        }
                        Biomes_PlantControl ext = plantDef.GetModExtension<Biomes_PlantControl>();
                        if (!ext.cavePlant)
                        {
                            __result = false;
                            return false;
                        }
                        return true;

                    }
                }
            }
            if (plantDef.HasModExtension<Biomes_PlantControl>())
            {
                Biomes_PlantControl ext = plantDef.GetModExtension<Biomes_PlantControl>();
                if (ext.cavePlant)
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
    }
}
