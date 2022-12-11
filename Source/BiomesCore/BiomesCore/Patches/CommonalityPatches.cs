using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using BiomesCore.DefModExtensions;
namespace BiomesCore.Patches
{
	[HarmonyPatch(typeof(WildPlantSpawner), "GetCommonalityOfPlant")]
	internal static class WildPlantSpawner_GetCommonalityOfPlant
	{
		internal static void Postfix(ref float __result, ThingDef plant, Map ___map)
		{
			if (plant.HasModExtension<Biomes_PlantControl>())
				__result = ___map.Biome.CommonalityOfPlant(plant);
		}
	}

	[HarmonyPatch(typeof(WildPlantSpawner), "GetBaseDesiredPlantsCountAt")]
	internal static class WildPlantSpawner_GetBaseDesiredPlantsCountAt
	{
		// TODO whoever coded this apparently assumed there is only ever one map in the game?!
		public static Dictionary<string, float> commonalitySum = new Dictionary<string, float>();
		public static Dictionary<IntVec3, float> commonalitySumForCell = new Dictionary<IntVec3, float>();
		
		internal static void Postfix(ref float __result, IntVec3 c, Map ___map)
		{
			var biome = ___map.LocalBiome(c);
			var biomeModExtension = biome.GetModExtension<BiomesMap>();
			if (biomeModExtension == null || !biomeModExtension.plantTaggingSystemEnabled) //If it doesn't have our ModExtension or this system isn't enabled..
				return; //Abort!
			if (!commonalitySumForCell.ContainsKey(c))
				UpdateCommonalityAt(c, ___map, biome, baseDesiredPlantCountAt: __result);
			__result = commonalitySumForCell[c];
		}

		public static void UpdateCommonalityAt(IntVec3 c, Map map, BiomeDef biome, TerrainDef terrain = null, float? baseDesiredPlantCountAt = null)
		{
			if (commonalitySumForCell.ContainsKey(c)) //If this cell is already cached..
				commonalitySumForCell.Remove(c); //Yeet, if this called it's on purpose.
			if (terrain == null) //Was not passed in..
				terrain = c.GetTerrain(map); //Get it.
			commonalitySum.AddDistinct("Soil", 0);
			float averagecommonality = 1;
			foreach (ThingDef plant in biome.AllWildPlants)
			{
				Biomes_PlantControl plantControl = plant.GetModExtension<Biomes_PlantControl>();
				if (plantControl != null)
				{
					if (plantControl.terrainTags != null)
					{
						foreach (string tag in plantControl.terrainTags)
						{
							if (!commonalitySum.ContainsKey(tag))
								commonalitySum.Add(tag, 0);
							commonalitySum[tag] += biome.CommonalityOfPlant(plant);
						}
					}
					else
						commonalitySum["Soil"] += biome.CommonalityOfPlant(plant) * 2f;
				}
				else
					commonalitySum["Soil"] += biome.CommonalityOfPlant(plant) * 2f;
			}
			float baseline = 200;
			var terrainPlantControl = terrain.GetModExtension<Biomes_PlantControl>();
			if (terrainPlantControl != null)
			{
				if (terrainPlantControl.terrainTags != null)
				{
					baseline = commonalitySum["Soil"];
					foreach (string tag in terrainPlantControl.terrainTags)
					{
						if (!commonalitySum.ContainsKey(tag))
							commonalitySum.Add(tag, 0);
						averagecommonality += commonalitySum[tag] / baseline;
					}
					averagecommonality /= terrainPlantControl.terrainTags.Count;
				}
			}
			commonalitySumForCell.Add(c, ((baseDesiredPlantCountAt ?? WildPlantSpanwer_GetBaseDesiredPlantCountAt_Copy(c, map)) + averagecommonality) / 2);
			//if (Prefs.DevMode)
			//	BiomesCore.Log("cell: " + c + " fertility: " + terrain.fertility + " average commonality: " + averagecommonality + " adjusted fertility: " + commonalitySumForCell[c]);
		}

		//Copied from WildPlantSpawner.GetBaseDesiredPlantCountAt and inlined its call to WildPlantSpawner.GoodRoofForCavePlant, made it static and added map arg.
		public static float WildPlantSpanwer_GetBaseDesiredPlantCountAt_Copy(IntVec3 c, Map map)
        {
			float num = c.GetTerrain(map).fertility;
			if (c.GetRoof(map)?.isNatural ?? false)
			{
				num *= 0.5f;
			}
			return num;
		}
	}
}
