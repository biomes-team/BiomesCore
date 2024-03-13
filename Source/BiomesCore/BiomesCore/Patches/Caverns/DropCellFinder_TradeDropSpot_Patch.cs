using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Allow dropping trade goods on cells with cavern roof.
	/// </summary>
	[HarmonyPatch]
	public static class DropCellFinder_TradeDropSpot_Patch
	{
		private static MethodBase TargetMethod()
		{
			// Fetch the third lambda.
			return typeof(DropCellFinder).GetLambda(nameof(DropCellFinder.TradeDropSpot), lambdaOrdinal: 2);
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.RoofGridUnbreachableRoofed(instructions.ToList());
		}
	}
}