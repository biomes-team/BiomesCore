using System.Collections.Generic;
using System.Linq;
using BiomesCore.DefModExtensions;
using BMT;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesCore.Patches
{
	[HarmonyPatch(typeof(PlantUtility), nameof(PlantUtility.GrowthSeasonNow))]
	internal static class PlantUtility_GrowthSeasonNow_Patch
	{
		// See WorkGiver_GrowerSow_JobOnCell_Patch for details.
		public static ThingDef CalculateWantedPlantDef = null;

		internal static void Postfix(IntVec3 c, Map map, ref bool __result)
		{
			var modExtension = map.Biome.GetModExtension<BiomesMap>();
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

	[HarmonyPatch(typeof(Zone_Growing), "GrowingQuadrumsDescription")]
	internal static class Zone_Growing_GrowingQuadrumsDescription
	{
		internal static bool Prefix(int tile, ref string __result)
		{
			var modExtension = Find.WorldGrid[tile].biome.GetModExtension<BiomesMap>();
			if (modExtension is {alwaysGrowthSeason: true})
			{
				__result = "GrowYearRound".Translate();
				return false;
			}

			return true;
		}
	}

	[HarmonyPatch(typeof(Plant), "GrowthRateFactor_Temperature", MethodType.Getter)]
	internal static class Plant_GrowthRateFactor_Temperature
	{
		internal static bool Prefix(Plant __instance, ref float __result)
		{
			var modExtension = __instance.def.GetModExtension<Biomes_PlantControl>();
			if (modExtension == null) return true;

			if (!GenTemperature.TryGetTemperatureForCell(__instance.Position, __instance.Map, out var cellTemp))
			{
				__result = 1f;
				return false;
			}

			var min = modExtension.optimalTemperature.min;
			var max = modExtension.optimalTemperature.max;
			var span = modExtension.optimalTemperature.Span;

			__result = cellTemp < min
				? Mathf.InverseLerp(min - span / 6f, min, cellTemp)
				: cellTemp > max
					? Mathf.InverseLerp(max + span / 2f, max, cellTemp)
					: 1f;

			return false;
		}
	}

	[HarmonyPatch(typeof(ThingDef), "SpecialDisplayStats")]
	internal static class ThingDef_SpecialDisplayStats
	{
		internal static void Postfix(ThingDef __instance, ref IEnumerable<StatDrawEntry> __result)
		{
			if (__instance.plant != null)
			{
				var modExtension = __instance.GetModExtension<Biomes_PlantControl>();
				if (modExtension != null)
				{
					var list = __result.ToList();

					var minLabel = "MinGrowthTemperature".Translate().CapitalizeFirst();
					var maxLabel = "MaxGrowthTemperature".Translate().CapitalizeFirst();

					var minValue = modExtension.optimalTemperature.min - 6f;
					var maxValue = modExtension.optimalTemperature.max + 16f;

					if (list.RemoveAll(e => e.LabelCap == minLabel || e.LabelCap == maxLabel) > 0)
					{
						list.Add(new StatDrawEntry(StatCategoryDefOf.Basics, "MinGrowthTemperature".Translate(),
							minValue.ToStringTemperature(), "Stat_Thing_Plant_MinGrowthTemperature_Desc".Translate(), 4152));
						list.Add(new StatDrawEntry(StatCategoryDefOf.Basics, "MaxGrowthTemperature".Translate(),
							maxValue.ToStringTemperature(), "Stat_Thing_Plant_MaxGrowthTemperature_Desc".Translate(), 4153));
					}

					__result = list;
				}
			}
		}
	}

	/// <summary>
	/// The prefix of this patch makes PlantUtility.GrowthSeasonNow aware of the type of plant which is going to be
	/// planted. The postfix removes the change to restore GrowthSeasonNow to vanilla functionality.
	/// </summary>
	[HarmonyPatch(typeof(WorkGiver_GrowerSow), nameof(WorkGiver_GrowerSow.JobOnCell))]
	internal static class WorkGiver_GrowerSow_JobOnCell_Patch
	{
		private static void Prefix(Pawn pawn, IntVec3 c)
		{
			Map map = pawn.Map;
			PlantUtility_GrowthSeasonNow_Patch.CalculateWantedPlantDef = WorkGiver_Grower.CalculateWantedPlantDef(c, map);
		}

		private static void Postfix()
		{
			PlantUtility_GrowthSeasonNow_Patch.CalculateWantedPlantDef = null;
		}
	}
}