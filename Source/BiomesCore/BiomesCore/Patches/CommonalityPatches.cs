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

    //[StaticConstructorOnStartup]
    //static class CommonalityFinder
    //{
    //    public static float landCommonality = 0;
    //    public static float waterCommonality = 0;
    //    public static float sandCommonality = 0;
    //    //public static Dictionary<string, float> commonalitySum = new Dictionary<string, float>();

    //    static CommonalityFinder()
    //    {
    //        foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs.ToList())
    //        {
    //            foreach (ThingDef plant in biome.AllWildPlants)
    //            {
    //                if (plant.HasModExtension<Biomes_PlantControl>())
    //                {
    //                    Biomes_PlantControl ext = plant.GetModExtension<Biomes_PlantControl>();
    //                    if (ext.allowInFreshWater || ext.allowInSaltWater)
    //                    {
    //                        waterCommonality += biome.CommonalityOfPlant(plant);
    //                        continue;
    //                    }
    //                    if (!ext.allowOffSand)
    //                    {
    //                        sandCommonality += biome.CommonalityOfPlant(plant);
    //                        continue;
    //                    }
    //                }
    //                landCommonality += biome.CommonalityOfPlant(plant);
    //                continue;
    //            }
    //            sandCommonality /= landCommonality;
    //            if (!commonalitySum.ContainsKey(biome + "_SandCommonality"))
    //                commonalitySum.Add(biome + "_SandCommonality", sandCommonality);
    //            waterCommonality /= landCommonality;
    //            if (!commonalitySum.ContainsKey(biome + "_WaterCommonality"))
    //                commonalitySum.Add(biome + "_WaterCommonality", waterCommonality);
    //            landCommonality /= landCommonality;
    //            if (!commonalitySum.ContainsKey(biome + "_LandCommonality"))
    //                commonalitySum.Add(biome + "_LandCommonality", landCommonality);
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(WildPlantSpawner), "GetBaseDesiredPlantsCountAt")]
    //internal static class WildPlantSpawner_GetBaseDesiredPlantsCountAt
    //{
    //    internal static void Postfix(ref float __result, IntVec3 c, Map ___map)
    //    {
    //        Dictionary<string, float> commonalitySum = CommonalityFinder.commonalitySum;
    //        var terrain = c.GetTerrain(___map);
    //        var biome = ___map.Biome;
    //        float fertility = terrain.fertility;
    //        if (terrain.HasTag("Water"))
    //        {
    //            __result = fertility * commonalitySum[biome + "_WaterCommonality"];
    //            return;
    //        }
    //        if (terrain.HasTag("Sandy"))
    //        {
    //            __result = fertility * commonalitySum[biome + "_SandCommonality"];
    //            return;
    //        }
    //        __result = fertility * commonalitySum[biome + "_LandCommonality"];
    //        return;

    //    }
    //}



    [HarmonyPatch(typeof(WildPlantSpawner), "GetBaseDesiredPlantsCountAt")]
    internal static class WildPlantSpawner_GetBaseDesiredPlantsCountAt
    {
        public static Dictionary<string, float> commonalitySum = new Dictionary<string, float>();
        internal static void Postfix(ref float __result, IntVec3 c, Map ___map)
        {
            var biome = ___map.Biome;
            var terrain = c.GetTerrain(___map);
            if (!commonalitySum.ContainsKey("WaterCommonality") || !commonalitySum.ContainsKey("SandCommonality") || !commonalitySum.ContainsKey("LandCommonality"))
            {
                float landCommonality = 0;
                float waterCommonality = 0;
                float sandCommonality = 0;
                float wetlandCommonality = 0;
                float caveCommonality = 0;
                foreach (ThingDef plant in biome.AllWildPlants)
                {
                    if (plant.HasModExtension<Biomes_PlantControl>())
                    {
                        Biomes_PlantControl ext = plant.GetModExtension<Biomes_PlantControl>();
                        if (ext.allowInWater)
                        {
                            waterCommonality += biome.CommonalityOfPlant(plant);
                        }
                        if (!ext.allowInSandy)
                        {
                            sandCommonality += biome.CommonalityOfPlant(plant);
                        }
                        if (ext.allowInBoggy)
                        {
                            wetlandCommonality += biome.CommonalityOfPlant(plant);
                        }
                        if (ext.allowInCave)
                        {
                            caveCommonality += biome.CommonalityOfPlant(plant);
                        }
                        if (ext.allowOnLand)
                        {
                            landCommonality += biome.CommonalityOfPlant(plant);
                        }
                        continue;
                    }
                    else
                    {
                        landCommonality += biome.CommonalityOfPlant(plant);
                        continue;
                    }
                }
                // divides each commonality sum by the land commonality sum. The land commonality sum must eb divided last, no matter what.
                sandCommonality /= landCommonality;
                if (!commonalitySum.ContainsKey("SandCommonality"))
                    commonalitySum.Add("SandCommonality", sandCommonality);

                waterCommonality /= landCommonality;
                if (!commonalitySum.ContainsKey("WaterCommonality"))
                    commonalitySum.Add("WaterCommonality", waterCommonality);

                wetlandCommonality /= landCommonality;
                if (!commonalitySum.ContainsKey("CaveCommonality"))
                    commonalitySum.Add("WetlandCommonality", wetlandCommonality);

                caveCommonality /= landCommonality;
                if (!commonalitySum.ContainsKey("CaveCommonality"))
                    commonalitySum.Add("CaveCommonality", caveCommonality);

                landCommonality /= landCommonality;
                if (!commonalitySum.ContainsKey("LandCommonality"))
                    commonalitySum.Add("LandCommonality", landCommonality);

                float fertility = terrain.fertility;
                RoofDef roof = ___map.roofGrid.RoofAt(c);

                if (terrain.HasTag("Water"))
                    __result = fertility * commonalitySum["WaterCommonality"];

                if (terrain.HasTag("Sandy"))
                    __result = fertility * commonalitySum["SandCommonality"];

                if (terrain.HasTag("Boggy"))
                    __result = fertility * commonalitySum["WetlandCommonality"];

                if (roof != null)
                    if (roof.isNatural)
                        __result = fertility * commonalitySum["CaveCommonality"];

                if (terrain.HasTag("Soil"))
                    __result = fertility * commonalitySum["LandCommonality"];

                return;
            }
            else
            {
                float fertility = terrain.fertility;
                RoofDef roof = ___map.roofGrid.RoofAt(c);

                if (terrain.HasTag("Water"))
                    __result = fertility * commonalitySum["WaterCommonality"];

                if (terrain.HasTag("Sandy"))
                    __result = fertility * commonalitySum["SandCommonality"];

                if (terrain.HasTag("Boggy"))
                    __result = fertility * commonalitySum["WetlandCommonality"];

                if (roof != null)
                    if (roof.isNatural)
                        __result = fertility * commonalitySum["CaveCommonality"];

                if (terrain.HasTag("Soil"))
                    __result = fertility * commonalitySum["LandCommonality"];
                return;
            }

        }
    }


}
