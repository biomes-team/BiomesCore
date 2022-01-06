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
		internal static void Postfix(ref float __result, ThingDef plant, Map ___map)
		{
			if (plant.HasModExtension<Biomes_PlantControl>())
			{
				__result = ___map.Biome.CommonalityOfPlant(plant);
			}
		}
	}


	[HarmonyPatch(typeof(WildPlantSpawner), "GetBaseDesiredPlantsCountAt")]
	internal static class WildPlantSpawner_GetBaseDesiredPlantsCountAt
	{
		public static Dictionary<string, float> commonalitySum = new Dictionary<string, float>();
		public static Dictionary<IntVec3, float> commonalitySumForCell = new Dictionary<IntVec3, float>();
		internal static void Postfix(ref float __result, IntVec3 c, Map ___map)
		{
			commonalitySum.AddDistinct("Soil", 0);
			float averagecommonality = 1;
			var terrain = c.GetTerrain(___map);
			if (!commonalitySumForCell.ContainsKey(c))
			{
				foreach (ThingDef plant in ___map.Biome.AllWildPlants)
				{
					if (plant.HasModExtension<Biomes_PlantControl>())
					{
						Biomes_PlantControl plantControl = plant.GetModExtension<Biomes_PlantControl>();
						if (plantControl.terrainTags != null)
						{
							foreach (string tag in plantControl.terrainTags)
							{
								if (!commonalitySum.ContainsKey(tag))
									commonalitySum.Add(tag, 0);
								commonalitySum[tag] += ___map.Biome.CommonalityOfPlant(plant);
							}
						}
						else
						{
							commonalitySum["Soil"] += ___map.Biome.CommonalityOfPlant(plant) * 2f;
						}
					}
					else
					{
						commonalitySum["Soil"] += ___map.Biome.CommonalityOfPlant(plant) * 2f;
					}
				}
				float baseline = 200;
				if (terrain.HasModExtension<Biomes_PlantControl>())
				{
					Biomes_PlantControl plantControl2 = terrain.GetModExtension<Biomes_PlantControl>();
					if (plantControl2.terrainTags != null)
					{
						baseline = commonalitySum["Soil"];
						foreach (string tag in plantControl2.terrainTags)
						{
							if (!commonalitySum.ContainsKey(tag))
								commonalitySum.Add(tag, 0);
							averagecommonality += commonalitySum[tag] / baseline;
						}
						averagecommonality /= plantControl2.terrainTags.Count;
					}
				}
				float fertility = terrain.fertility;
                commonalitySumForCell.Add(c, (fertility + averagecommonality) / 2);
                Log.Message("cell: " + c + " fertility: " + fertility + " average commonality: " + averagecommonality + " adjusted fertility: " + commonalitySumForCell[c]);
                __result = commonalitySumForCell[c];
                return;
			}
			else
			{
				__result = commonalitySumForCell[c];
				return;
			}
		}
	}


}
