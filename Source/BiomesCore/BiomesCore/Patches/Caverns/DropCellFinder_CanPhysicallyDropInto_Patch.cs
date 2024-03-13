using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// When in a cavern, it is possible to physically drop on tiles with a cavern roof, as long as they should be
	/// considered to be phychologically outdoors.
	/// </summary>
	[HarmonyPatch(typeof(DropCellFinder), nameof(DropCellFinder.CanPhysicallyDropInto))]
	public static class DropCellFinder_CanPhysicallyDropInto_Patch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CavernsAwarePsychologicallyOutdoors(Transpilers.GetNonCavernRoof(instructions.ToList()),
				OpCodes.Ldarg_0 // Cell.
			);
		}
	}
}