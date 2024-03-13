using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Tiles under cavern roof use outside beauty stats.
	/// </summary>
	[HarmonyPatch(typeof(BeautyUtility), nameof(BeautyUtility.CellBeauty))]
	public class BeautyUtility_CellBeauty_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CavernsAwarePsychologicallyOutdoors(instructions.ToList(),
				OpCodes.Ldarg_0 // Cell
				);
		}
	}
}