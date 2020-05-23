using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection.Emit;
using BiomesCore.Reflections;
using BiomesCore.Bridges;
using BiomesCore.DefModExtensions;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(PlantUtility), "CanEverPlantAt")]
    internal static class PlantUtility_CanEverPlantAt
    {
        internal static bool Prefix(ref bool __result, ThingDef plantDef, IntVec3 c, Map map)
        {
            TerrainDef terrain = map.terrainGrid.TerrainAt(c);
            if (terrain.HasTag("Water"))
            {
                if (!plantDef.HasModExtension<Biomes_WaterPlant>())
                {
                    __result = false;
                    return false;
                }
                Biomes_WaterPlant ext = plantDef.GetModExtension<Biomes_WaterPlant>();
                if ((terrain.HasTag("Salty") && !ext.allowInSaltWater) || (!terrain.HasTag("Salty") && !ext.allowInFreshWater) || (terrain.HasTag("Deep") && !ext.allowInDeepWater) || (!terrain.HasTag("Deep") && !ext.allowInShallowWater))
                {
                    __result = false;
                    return false;
                }
            }
            else if (plantDef.HasModExtension<Biomes_WaterPlant>() )
            {
                Biomes_WaterPlant ext = plantDef.GetModExtension<Biomes_WaterPlant>();
                if (!ext.allowOnLand)
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
    }
}
