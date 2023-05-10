using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BiomesCore.DefModExtensions;
using BiomesCore.Reflections;
using BMT;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(PlantUtility), "GrowthSeasonNow")]
    internal static class PlantUtility_GrowthSeasonNow
    {
        internal static bool Prefix(IntVec3 c, Map map, ref bool __result)
        {
            var modExtension = map.Biome.GetModExtension<BiomesMap>();
            if (modExtension is { alwaysGrowthSeason: true })
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
    
    [HarmonyPatch(typeof(Zone_Growing), "GrowingQuadrumsDescription")]
    internal static class Zone_Growing_GrowingQuadrumsDescription
    {
        internal static bool Prefix(int tile, ref string __result)
        {
            var modExtension = Find.WorldGrid[tile].biome.GetModExtension<BiomesMap>();
            if (modExtension is { alwaysGrowthSeason: true })
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
                : cellTemp > max ? Mathf.InverseLerp(max + span / 2f, max, cellTemp) : 1f;
                
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
                        list.Add(new StatDrawEntry(StatCategoryDefOf.Basics, "MinGrowthTemperature".Translate(), minValue.ToStringTemperature(), "Stat_Thing_Plant_MinGrowthTemperature_Desc".Translate(), 4152));
                        list.Add(new StatDrawEntry(StatCategoryDefOf.Basics, "MaxGrowthTemperature".Translate(), maxValue.ToStringTemperature(), "Stat_Thing_Plant_MaxGrowthTemperature_Desc".Translate(), 4153));
                    }

                    __result = list;
                }
            }
        }
    }

	internal static class GrowthSeasonNowHelper
	{
		public static readonly MethodInfo GrowthSeasonMethod =
			AccessTools.Method(typeof(PlantUtility), nameof(PlantUtility.GrowthSeasonNow));

		public static readonly MethodInfo GrowthSeasonPlaceholderMethod =
			AccessTools.Method(typeof(GrowthSeasonNowHelper), nameof(GrowthSeasonPlaceholder));

		public static readonly MethodInfo IsPlantGrowthSeasonMethod =
			AccessTools.Method(typeof(GrowthSeasonNowHelper), nameof(IsPlantGrowthSeason));

		// Used to remove existing PlantUtility.GrowthSeasonNow checks when it is not feasible to convert them to
		// IsPlantGrowthSeason checks.
		public static bool GrowthSeasonPlaceholder(IntVec3 c, Map map, bool forSowing = false)
		{
			return true;
		}

		public static bool IsPlantGrowthSeason(IntVec3 cell, Map map, ThingDef plantDef)
		{
			if (plantDef.thingClass == typeof(BiomesPlant))
			{
				var controlDef = plantDef.GetModExtension<Biomes_PlantControl>();
				if (controlDef != null && controlDef.optimalTemperature.Includes(cell.GetTemperature(map)))
				{
					return true;
				}
			}

			return PlantUtility.GrowthSeasonNow(cell, map);
		}
	}

	[HarmonyPatch(typeof(WorkGiver_GrowerSow), nameof(WorkGiver_GrowerSow.JobOnCell))]
	internal static class WorkGiver_GrowerSow_JobOnCell
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
			ILGenerator generator)
		{
			// Replace both PlantUtility.GrowthSeasonNow checks with the placeholder. The first one is made before the
			// ThingDef of the plant is known, so it cannot be replaced with IsPlantGrowthSeason. The second one is redundant
			// so it is not worth patching.
			var instructionList = TranspilerHelper.ReplaceCall(instructions.ToList(),
				GrowthSeasonNowHelper.GrowthSeasonMethod, GrowthSeasonNowHelper.GrowthSeasonPlaceholderMethod);
			var instructionIndex = 0;

			// Now we need to insert a new IsGrowthSeason check after the code already knows which def will be planted.
			// This is done after storing the thingList to avoid overwriting labels.
			var wantedPlantField = AccessTools.Field(type: typeof(WorkGiver_Grower), name: "wantedPlantDef");
			bool insertedNewIsGrowthSeason = false;
			Label isGrowthSeasonLabel = generator.DefineLabel();
			while (!insertedNewIsGrowthSeason)
			{
				var instruction = instructionList[instructionIndex];
				yield return instruction;

				// List<Thing> thingList has been stored as a local variable.
				if (instruction.opcode == OpCodes.Stloc_1)
				{
					insertedNewIsGrowthSeason = true;
					// Load the current cell.
					yield return new CodeInstruction(OpCodes.Ldarg_2);
					// Load the current map.
					yield return new CodeInstruction(OpCodes.Ldloc_0);
					// ThingDef of the plant intended to be sown.
					yield return new CodeInstruction(OpCodes.Ldsfld, wantedPlantField); 
					// Perform our plant growth season check.
					yield return new CodeInstruction(OpCodes.Call, GrowthSeasonNowHelper.IsPlantGrowthSeasonMethod);
					// Carry on with the execution if the check succeeds.
					yield return new CodeInstruction(OpCodes.Brtrue_S, isGrowthSeasonLabel);
					// Return null to signal to the caller that the plant cannot be sown.
					yield return new CodeInstruction(OpCodes.Ldnull);
					yield return new CodeInstruction(OpCodes.Ret);
				}

				++instructionIndex;
			}
			// Add the new label to the next instruction to carry on with the regular execution.
			instructionList[instructionIndex].labels.Add(isGrowthSeasonLabel);

			// Return the remaining instructions.
			while (instructionIndex < instructionList.Count)
			{
				yield return instructionList[instructionIndex];
				++instructionIndex;
			}
		}
	}
}
