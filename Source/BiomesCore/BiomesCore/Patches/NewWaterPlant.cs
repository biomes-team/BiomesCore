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
}
