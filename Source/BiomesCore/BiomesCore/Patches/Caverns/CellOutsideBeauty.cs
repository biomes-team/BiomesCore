using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace BiomesCore.Patches.Caverns
{
	[HarmonyPatch(typeof(BeautyUtility), nameof(BeautyUtility.CellBeauty))]
	public class CellOutsideBeauty
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CellPsychologicallyOutdoors(instructions.ToList(), OpCodes.Ldarg_0);
		}
	}
}