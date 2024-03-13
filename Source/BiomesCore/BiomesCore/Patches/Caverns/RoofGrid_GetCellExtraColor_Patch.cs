using HarmonyLib;
using UnityEngine;
using Verse;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	///  Picks color for cave roofs when roof overlay is toggled.
	/// </summary>
	[HarmonyPatch(typeof(RoofGrid), nameof(RoofGrid.GetCellExtraColor))]
	public static class RoofGrid_GetCellExtraColor_Patch
	{
		// Dark grey.
		private static Color RockRoofStableColor = new Color(0.25F, 0.25F, 0.25F, 1.0F);

		private static void Postfix(int index, ref RoofGrid __instance, ref Color __result)
		{
			if (__instance.RoofAt(index) == BiomesCoreDefOf.BMT_RockRoofStable)
			{
				__result = RockRoofStableColor;
			}
		}
	}
}