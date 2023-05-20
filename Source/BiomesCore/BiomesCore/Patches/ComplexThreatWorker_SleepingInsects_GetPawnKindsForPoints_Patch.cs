using System.Collections.Generic;
using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{
	[HarmonyPatch(typeof(ComplexThreatWorker_SleepingInsects), "GetPawnKindsForPoints")]
	internal static class ComplexThreatWorker_SleepingInsects_GetPawnKindsForPoints_Patch
	{
		static IEnumerable<PawnKindDef> Postfix(IEnumerable<PawnKindDef> values)
		{
			foreach (var value in values)
			{
				var extension = value.race.GetModExtension<Biomes_AnimalControl>();

				if (extension == null || extension.isInsectoid)
				{
					yield return value;
				}
			}
		}
	}
}