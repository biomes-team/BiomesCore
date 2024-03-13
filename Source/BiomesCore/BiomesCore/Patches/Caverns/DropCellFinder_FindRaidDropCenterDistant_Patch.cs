using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// When in a cavern, always allow roofed tiles as a potential raid drop point.
	/// </summary>
	[HarmonyPatch(typeof(DropCellFinder), nameof(DropCellFinder.FindRaidDropCenterDistant))]
	public static class DropCellFinder_FindRaidDropCenterDistant_Patch
	{
		public static void Prefix(Map map, ref bool allowRoofed)
		{
			if (!allowRoofed)
			{
				BiomesMap extension = map.Biome.GetModExtension<BiomesMap>();
				allowRoofed = extension != null && extension.isCavern;
			}
		}
	}
}