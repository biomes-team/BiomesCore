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
                    if (plant.HasModExtension<Biomes_PlantControl>())
                    {
                        Biomes_PlantControl ext = plant.GetModExtension<Biomes_PlantControl>();
                        if (ext.allowInFreshWater || ext.allowInSaltWater)
                        {
                            waterCommonality += biome.CommonalityOfPlant(plant);
                            continue;
                        }
                        if (!ext.allowOffSand)
                        {
                            sandCommonality += biome.CommonalityOfPlant(plant);
                            continue;
                        }
                    }
                    landCommonality += biome.CommonalityOfPlant(plant);
                    continue;
                }
                sandCommonality /= landCommonality;
                if (!commonalitySum.ContainsKey(biome + "_SandCommonality"))
                    commonalitySum.Add(biome + "_SandCommonality", sandCommonality);
                waterCommonality /= landCommonality;
                if (!commonalitySum.ContainsKey(biome + "_WaterCommonality"))
                    commonalitySum.Add(biome + "_WaterCommonality", waterCommonality);
                landCommonality /= landCommonality;
                if (!commonalitySum.ContainsKey(biome + "_LandCommonality"))
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
