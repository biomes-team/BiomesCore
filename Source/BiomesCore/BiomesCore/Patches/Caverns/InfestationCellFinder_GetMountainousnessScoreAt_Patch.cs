using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Lowers infestation chance under cavern roofs.
	/// </summary>
	[HarmonyPatch(typeof(InfestationCellFinder), "GetMountainousnessScoreAt")]
	public static class InfestationCellFinder_GetMountainousnessScoreAt_Patch
	{
		private static void Postfix(IntVec3 cell, Map map, ref float __result)
		{
			if (map.roofGrid.RoofAt(cell) == BiomesCoreDefOf.BMT_RockRoofStable)
			{
				__result *= 0.25F;
			}
		}
	}
}