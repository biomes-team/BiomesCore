using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Allow choosing cells with cavern roof for this check.
	/// </summary>
	[HarmonyPatch]
	public static class DropCellFinder_TryFindSafeLandingSpotCloseToColony_Patch
	{
		private static MethodBase TargetMethod()
		{
			return typeof(DropCellFinder).GetLocalFunc(nameof(DropCellFinder.TryFindSafeLandingSpotCloseToColony),
				localFunc: "SpotValidator");
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CellUnbreachableRoofed(instructions.ToList());
		}
	}
}