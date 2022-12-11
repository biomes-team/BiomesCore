using Verse;
using HarmonyLib;
using BiomesCore.DefModExtensions;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(TerrainGrid), "SetTerrain")]
    internal class SetTerrain_UpdateCommonality
    {
        internal static void Postfix(IntVec3 c, TerrainDef newTerr, Map ___map)
        {
            var biome = ___map.LocalBiome(c);
            var biomeModExtension = biome.GetModExtension<BiomesMap>();
            if (biomeModExtension == null || !biomeModExtension.plantTaggingSystemEnabled) //If it doesn't have our ModExtension or this system isn't enabled..
                return; //Abort!
            WildPlantSpawner_GetBaseDesiredPlantsCountAt.UpdateCommonalityAt(c, ___map, biome, newTerr);
        }
    }
}