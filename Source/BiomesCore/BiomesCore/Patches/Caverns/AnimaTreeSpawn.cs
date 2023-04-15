using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace BiomesCore.Patches.Caverns
{
	[HarmonyPatch(typeof(GenStep_SpecialTrees), nameof(GenStep_SpecialTrees.CanSpawnAt))]
	internal static class AnimaTreeSpawn
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var firstList = Transpilers.CellPsychologicallyOutdoors(instructions.ToList(), OpCodes.Ldarg_1);
			return Transpilers.CellUnbreachableRoofed(firstList);
		}
	}
}