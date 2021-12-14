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
		internal static void Postfix(ref float __result, IntVec3 c, Map ___map)
		{
			foreach (ThingDef plant in ___map.Biome.AllWildPlants)
			{
				if (plant.HasModExtension<Biomes_PlantControl>())
				{
					Biomes_PlantControl plantControl = plant.GetModExtension<Biomes_PlantControl>();
					foreach (String tag in plantControl.terrainTags)
					{
						if (!commonalitySum.ContainsKey(tag))
							commonalitySum.Add(tag, ___map.Biome.CommonalityOfPlant(plant));
					}
				}
                else
				{
					if (!commonalitySum.ContainsKey("Soil"))
						commonalitySum.Add("Soil", ___map.Biome.CommonalityOfPlant(plant));
				}
			}
			float averagecommonality = 1;
			var terrain = c.GetTerrain(___map);
			if (terrain.HasModExtension<Biomes_PlantControl>())
			{
				float baseline = commonalitySum["Soil"];
				foreach (string tag in commonalitySum.Keys.ToList())
				{
					commonalitySum[tag] /= baseline;
				}
				averagecommonality = 0;
				Biomes_PlantControl plantControl = terrain.GetModExtension<Biomes_PlantControl>();
				foreach (String tag in plantControl.terrainTags)
                {
					if (commonalitySum.ContainsKey(tag))
						averagecommonality += commonalitySum[tag];
                }
				averagecommonality /= plantControl.terrainTags.Count;
			}
			float fertility = terrain.fertility;
			__result = fertility * averagecommonality;
		}
	}


}
