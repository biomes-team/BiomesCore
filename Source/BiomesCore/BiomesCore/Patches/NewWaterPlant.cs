using HarmonyLib;
using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiomesCore.DefModExtensions;
[StaticConstructorOnStartup]
static class WildSpawnPreIterator
{
}
namespace BiomesCore.Patches
{

    [StaticConstructorOnStartup]
    static class CommonalityFinder
    {
        public static float landCommonality = 0;
        public static float waterCommonality = 0;
        public static float sandCommonality = 0;
        public static Dictionary<string, float> commonalitySum = new Dictionary<string, float>();

        static CommonalityFinder()
        {
            foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs.ToList())
            {
                foreach (ThingDef plant in biome.AllWildPlants)
                {
                    if (plant.HasModExtension<Biomes_WaterPlant>())
                    {
                        waterCommonality += biome.CommonalityOfPlant(plant);
                        continue;
                    }
                    if (plant.HasModExtension<Biomes_SandPlant>())
                    {
                        sandCommonality += biome.CommonalityOfPlant(plant);
                        continue;
                    }
                    landCommonality += biome.CommonalityOfPlant(plant);
                    continue;

                }
                sandCommonality /= landCommonality;
                commonalitySum.Add(biome + "_SandCommonality", sandCommonality);
                waterCommonality /= landCommonality;
                commonalitySum.Add(biome + "_WaterCommonality", waterCommonality);
                landCommonality /= landCommonality;
                commonalitySum.Add(biome + "_LandCommonality", landCommonality);
            }
        }
    }

    [HarmonyPatch(typeof(WildPlantSpawner), "GetBaseDesiredPlantsCountAt")]
    internal static class WildPlantSpawner_GetBaseDesiredPlantsCountAt
    {
        internal static void Postfix(ref float __result, IntVec3 c, Map ___map)
        {
            Dictionary<string, float> commonalitySum = CommonalityFinder.commonalitySum;
            var terrain = c.GetTerrain(___map);
            var biome = ___map.Biome;
            float fertility = terrain.fertility;
            if (terrain.HasTag("Water"))
            {
                __result = fertility * commonalitySum[biome + "_WaterCommonality"];
                return;
            }
            if (terrain.HasTag("Sandy"))
            {
                __result = fertility * commonalitySum[biome + "_SandCommonality"];
                return;
            }
            __result = fertility * commonalitySum[biome + "_LandCommonality"];
            return;

        }
    }


    //[HarmonyPatch(typeof(WildPlantSpawner), "GetCommonalityOfPlant")]
    //internal static class WildPlantSpawner_GetCommonalityOfPlant
    //{
    //    internal static bool Prefix(ref float __result, ThingDef plant, Map ___map)
    //    {
    //        float totalPlants = 0;
    //        float landPlants = 0;
    //        float waterPlants = 0;
    //        float sandPlants = 0;
    //        foreach (ThingDef plantdef in ___map.Biome.AllWildPlants)
    //        {
    //            totalPlants += ___map.Biome.CommonalityOfPlant(plant); ;
    //            if (plant.HasModExtension<Biomes_WaterPlant>())
    //            {
    //                waterPlants += ___map.Biome.CommonalityOfPlant(plant);
    //                continue;
    //            }
    //            if (plant.HasModExtension<Biomes_SandPlant>())
    //            {
    //                sandPlants += ___map.Biome.CommonalityOfPlant(plant); ;
    //                continue;
    //            }
    //            Log.Message("This is a land plant " + plant.defName);
    //            landPlants += ___map.Biome.CommonalityOfPlant(plant); ;
    //        }
    //        if (plant.HasModExtension<Biomes_WaterPlant>())
    //        {
    //            float test = (___map.Biome.CommonalityOfPlant(plant) * waterPlants) / (totalPlants / 3);
    //            Log.Message("Setting commonality of " + plant + " to " + test);
    //            __result = (___map.Biome.CommonalityOfPlant(plant) * waterPlants) / (totalPlants / 3);
    //            return false;
    //        }
    //        if (plant.HasModExtension<Biomes_SandPlant>())
    //        {
    //            __result = (___map.Biome.CommonalityOfPlant(plant) * sandPlants) / (totalPlants / 3);
    //            return false;
    //        }
    //        __result = (___map.Biome.CommonalityOfPlant(plant) * landPlants) / (totalPlants / 3);
    //        return true;
    //    }
    //}
}
