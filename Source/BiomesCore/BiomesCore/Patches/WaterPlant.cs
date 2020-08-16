using RimWorld;
using Verse;
using System.Reflection.Emit;
using BiomesCore.Reflections;
using BiomesCore.Bridges;
using BiomesCore.DefModExtensions;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace BiomesCore.Patches
{

    [HarmonyPatch(typeof(PlantUtility), nameof(PlantUtility.CanEverPlantAt_NewTemp))]
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
                if ((terrain.HasTag("SaltWater") && !ext.allowInSaltWater) || (!terrain.HasTag("SaltWater") && !ext.allowInFreshWater) || (terrain.HasTag("DeepWater") && !ext.allowInDeepWater) || (!terrain.HasTag("DeepWater") && !ext.allowInShallowWater))
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
            if (terrain.HasTag("Sandy"))
            {
                if (!plantDef.HasModExtension<Biomes_SandPlant>())
                {
                    __result = false;
                    return false;
                }
                Biomes_SandPlant ext = plantDef.GetModExtension<Biomes_SandPlant>();
                if (!ext.allowOnSand)
                {
                    __result = false;
                    return false;
                }
            }
            else if (plantDef.HasModExtension<Biomes_SandPlant>())
            {
                Biomes_SandPlant ext = plantDef.GetModExtension<Biomes_SandPlant>();
                if (!ext.allowOffSand)
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(WildPlantSpawner), "GetDesiredPlantsCountAt")]
    internal static class WildPlantSpawner_GetDesiredPlantsCountAt
    {
        internal static void Postfix(ref float __result, IntVec3 c, IntVec3 forCell, float plantDensity, Map ___map)
        {
            TerrainDef terrain = ___map.terrainGrid.TerrainAt(forCell);
            if (terrain.HasTag("Water"))
            {
                Biomes_WaterPlantBiome ext = ___map.Biome.GetModExtension<Biomes_WaterPlantBiome>();
                if (ext == null)
                {
                    ext = new Biomes_WaterPlantBiome();
                }
                // This needs fertility penalty ^ 2 to be multiplied in the have the same effect as multiplying fertility by the given number
                __result = Mathf.Min(__result * ext.spawnFertilityMultiplier * ext.spawnFertilityMultiplier, 1f);
            }
        }
    }

    [HarmonyPatch(typeof(WildPlantSpawner), "GetDesiredPlantsCountIn")]
    internal static class WildPlantSpawner_GetDesiredPlantsCountIn
    {
        internal static void Postfix(ref float __result, Region reg, IntVec3 forCell, float plantDensity, Map ___map)
        {
            TerrainDef terrain = ___map.terrainGrid.TerrainAt(forCell);
            if (terrain.HasTag("Water"))
            {
                Biomes_WaterPlantBiome ext = ___map.Biome.GetModExtension<Biomes_WaterPlantBiome>();
                if (ext == null)
                {
                    ext = new Biomes_WaterPlantBiome();
                }
                // This needs fertility penalty ^ 2 to be multiplied in the have the same effect as multiplying fertility by the given number
                __result = Mathf.Min(__result * ext.spawnFertilityMultiplier * ext.spawnFertilityMultiplier, reg.CellCount);
            }
        }
    }
}
