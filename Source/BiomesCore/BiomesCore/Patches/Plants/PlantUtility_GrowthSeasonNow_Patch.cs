using BiomesCore.DefModExtensions;
using BMT;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches.Plants
{
	[HarmonyPatch(typeof(PlantUtility), nameof(PlantUtility.GrowthSeasonNow), typeof(IntVec3), typeof(Map), typeof(ThingDef))]
	public static class PlantUtility_GrowthSeasonNow_Patch
	{
		// See WorkGiver_GrowerSow_JobOnCell_Patch for details.
		public static ThingDef CalculateWantedPlantDef = null;

		public static void Postfix(IntVec3 c, Map map, ThingDef plantDef, ref bool __result)
		{
			BiomesMap modExtension = map.Biome.GetModExtension<BiomesMap>();
			if (modExtension is {alwaysGrowthSeason: true})
			{
				__result = true;
			}
			else if (CalculateWantedPlantDef != null && CalculateWantedPlantDef.thingClass == typeof(BiomesPlant))
			{
				Biomes_PlantControl controlDef = CalculateWantedPlantDef.GetModExtension<Biomes_PlantControl>();
				if (controlDef != null && controlDef.optimalTemperature.Includes(c.GetTemperature(map)))
				{
					__result = true;
				}
			}
		}
	}
}