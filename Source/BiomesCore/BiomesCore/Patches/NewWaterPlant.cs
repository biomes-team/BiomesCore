using HarmonyLib;
using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiomesCore.DefModExtensions;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(WildPlantSpawner), "GetCommonalityOfPlant")]
    internal static class WildPlantSpawner_GetCommonalityOfPlant
    {
        internal static bool Prefix(ref float __result, ThingDef plant, Map ___map)
        {
            float totalPlants = 0;
            float landPlants = 0;
            float waterPlants = 0;
            float sandPlants = 0;
            foreach (ThingDef plantdef in ___map.Biome.AllWildPlants)
            {
                totalPlants += ___map.Biome.CommonalityOfPlant(plant); ;
                if (plant.HasModExtension<Biomes_WaterPlant>())
                {
                    waterPlants += ___map.Biome.CommonalityOfPlant(plant);
                    continue;
                }
                if (plant.HasModExtension<Biomes_SandPlant>())
                {
                    sandPlants += ___map.Biome.CommonalityOfPlant(plant); ;
                    continue;
                }
                landPlants += ___map.Biome.CommonalityOfPlant(plant); ;
            }
            if (plant.HasModExtension<Biomes_WaterPlant>())
            {
                __result = (___map.Biome.CommonalityOfPlant(plant) * waterPlants) / (totalPlants / 3);
                return false;
            }
            if (plant.HasModExtension<Biomes_SandPlant>())
            {
                __result = (___map.Biome.CommonalityOfPlant(plant) * sandPlants) / (totalPlants / 3);
                return false;
            }
            __result = (___map.Biome.CommonalityOfPlant(plant) * landPlants) / (totalPlants / 3);
            return true;
        }
    }
}
