using RimWorld;
using Verse;
using BiomesCore.DefModExtensions;
using System.Collections.Generic;
using HarmonyLib;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(PlantUtility), nameof(PlantUtility.CanEverPlantAt), typeof(ThingDef), typeof(IntVec3), typeof(Map), typeof(bool))]
	internal static class PlantUtility_CanEverPlantAt
	{
		internal static bool Prefix(ref bool __result, ThingDef plantDef, IntVec3 c, Map map)
		{
			if (!c.InBounds(map))
			{
				return true;
			}

			TerrainDef terrain = map.terrainGrid.TerrainAt(c);
			
			if (!plantDef.HasModExtension<Biomes_PlantControl>())//this section governs plants that should not use the BMT plant spawning system.
			{
				/*
				if (map.Biome.HasModExtension<BiomesMap>())
				{
					BiomesMap biome = map.Biome.GetModExtension<BiomesMap>();
					if (biome.isCavern)
					{
						__result = false;
						return false;
					}
				}
				*/
				if (terrain.HasTag("Water"))
				{
					__result = false;
					return false;
				}

			}

			List<Thing> list = map.thingGrid.ThingsListAt(c);
			foreach (Thing thing in list) //governs plant that grow on buildings, such as planters or hydroponics systems. These should bypass our other checks.
			{
				if (thing?.def.building != null)
				{
					if (plantDef.plant.sowTags.Contains(thing.def.building.sowTag))
					{
						__result = plantDef.plant.sowTags.Contains(thing.def.building.sowTag);
						return plantDef.plant.sowTags.Contains(thing.def.building.sowTag);
					}
				}
			}
			
			if (terrain.HasModExtension<Biomes_PlantControl>() && plantDef.HasModExtension<Biomes_PlantControl>()) //this section governs plants that should.
			{
				if (map.Biome.HasModExtension<BiomesMap>())
				{
					BiomesMap biome = map.Biome.GetModExtension<BiomesMap>();
					if (biome.isCavern)
					{
						if (!map.Biome.AllWildPlants.Contains(plantDef))
						{
							__result = false;
							return false;
						}
					}
				}
				
				Biomes_PlantControl plantExt = plantDef.GetModExtension<Biomes_PlantControl>();
				Biomes_PlantControl terrainExt = terrain.GetModExtension<Biomes_PlantControl>();
				
				if (plantExt.wallGrower)
                {
					
                }
				
				if (map.roofGrid.RoofAt(c) != null) //checks for cave cells.
				{
					if (!map.roofGrid.RoofAt(c).isNatural && !plantExt.allowInBuilding)
					{
						__result = false;
						return false;
					}
					if (map.roofGrid.RoofAt(c).isNatural && !plantExt.allowInCave)
					{
						__result = false;
						return false;
					}
				}
                else if (!plantExt.allowUnroofed) //code to prevent cave plants from spawning outside
                {
					__result = false;
					return false;
				}

				// terrain tags
				if (!plantExt.terrainTags.NullOrEmpty())
				{
					if (terrainExt.terrainTags.NullOrEmpty())
                    {
						__result = false;
						return false;
                    }
					
					foreach (string tag in terrainExt.terrainTags)
					{
						if (!plantExt.terrainTags.Contains(tag))
						{
							__result = false;
							return false;
						}
					}
				}
				else // prevent water spawns if no terrain tags
				{
					if (terrain.HasTag("Water") || terrain.IsWater)
					{
						__result = false;
						return false;
					}
				}
			}
			else if (!terrain.HasModExtension<Biomes_PlantControl>() && terrain.HasTag("Water"))
			{
				__result = false;
				return false;
			}
			return true;
		}
	}

	//[HarmonyPatch(typeof(WildPlantSpawner), "GetDesiredPlantsCountAt")]
	//internal static class WildPlantSpawner_GetDesiredPlantsCountAt
	//{
	//    internal static void Postfix(ref float __result, IntVec3 c, IntVec3 forCell, float plantDensity, Map ___map)
	//    {
	//        TerrainDef terrain = ___map.terrainGrid.TerrainAt(forCell);
	//        if (terrain.HasTag("Water"))
	//        {
	//            Biomes_WaterPlantBiome ext = ___map.Biome.GetModExtension<Biomes_WaterPlantBiome>();
	//            if (ext == null)
	//            {
	//                ext = new Biomes_WaterPlantBiome();
	//            }
	//            // This needs fertility penalty ^ 2 to be multiplied in the have the same effect as multiplying fertility by the given number
	//            __result = Mathf.Min(__result * ext.spawnFertilityMultiplier * ext.spawnFertilityMultiplier, 1f);
	//        }
	//    }
	//}

	//[HarmonyPatch(typeof(WildPlantSpawner), "GetDesiredPlantsCountIn")]
	//internal static class WildPlantSpawner_GetDesiredPlantsCountIn
	//{
	//    internal static void Postfix(ref float __result, Region reg, IntVec3 forCell, float plantDensity, Map ___map)
	//    {
	//        TerrainDef terrain = ___map.terrainGrid.TerrainAt(forCell);
	//        if (terrain.HasTag("Water"))
	//        {
	//            Biomes_WaterPlantBiome ext = ___map.Biome.GetModExtension<Biomes_WaterPlantBiome>();
	//            if (ext == null)
	//            {
	//                ext = new Biomes_WaterPlantBiome();
	//            }
	//            // This needs fertility penalty ^ 2 to be multiplied in the have the same effect as multiplying fertility by the given number
	//            __result = Mathf.Min(__result * ext.spawnFertilityMultiplier * ext.spawnFertilityMultiplier, reg.CellCount);
	//        }
	//    }
	//}

	/// <summary>
	/// Prevents ambrosia sprouts from spawning at water
	/// </summary>
	[HarmonyPatch(typeof(IncidentWorker_AmbrosiaSprout), "CanSpawnAt")]
	internal static class AmbrosiaSprout_CanSpawnAt
	{
		internal static bool Prefix(IntVec3 c, Map map, ref bool __result)
		{
			if (!PlantUtility.CanEverPlantAt(ThingDefOf.Plant_Ambrosia, c, map))
			{
				__result = false;
				return false;
			}
			return true;
		}
	}

	/// <summary>
	/// Prevent normal growing zones from being placed in water
	/// </summary>
	[HarmonyPatch(typeof(Designator_ZoneAdd_Growing), "CanDesignateCell")]
	internal static class DesignatorZoneGrowing_CanDesignateCell
	{
		static bool Prefix(IntVec3 c, ref AcceptanceReport __result)
		{
			if (Find.CurrentMap.terrainGrid.TerrainAt(c).IsWater)
			{
				__result = false;
				return false;
			}
			return true;
		}
	}

}
