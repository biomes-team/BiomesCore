using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{


    // This patch stops normal plants from spawning under natural roofs, and the new cave plants from spawning without an appropriate roof.
    [HarmonyPatch(typeof(PlantUtility), nameof(PlantUtility.CanEverPlantAt), new[] { typeof(ThingDef), typeof(IntVec3), typeof(Map), typeof(bool) })]
    internal static class PlantUtility_CanEverPlantAt_CavePlants
    {
        internal static bool Prefix(ThingDef plantDef, IntVec3 c, Map map, ref bool __result)
        {
            // is it a Biomes! Caverns map
            if (!map.Biome.HasModExtension<BiomesMap>())
            {
                return true;
            }
            if (!map.Biome.GetModExtension<BiomesMap>().isCavern)
            {
                return true;
            }

            // if roofed, only Biomes! caveplants are valid
            RoofDef roof = map.roofGrid.RoofAt(c);
            if (roof?.isNatural == true)                       // ?. returns null if roof is null
            {
                if (!plantDef.HasModExtension<Biomes_PlantControl>()/* && !plantDef.plant.cavePlant*/)
                {
                    __result = false;
                    return false;
                }
                //if (roof.defName == "BMT_RockRoofStable")
                //{
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
                //}
            }
            
            // if no roof, don't spawn Biomes! cave plants
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
