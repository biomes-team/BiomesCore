using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Allow special trees to spawn under cavern roof.
	/// </summary>
	[HarmonyPatch(typeof(GenStep_SpecialTrees), nameof(GenStep_SpecialTrees.CanSpawnAt))]
	internal static class GenStep_SpecialTrees_CanSpawnAt_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var firstList = Transpilers.CavernsAwarePsychologicallyOutdoors(instructions.ToList(),
				OpCodes.Ldarg_1 // Cell
				);
			return Transpilers.CellUnbreachableRoofed(firstList);
		}
	}
}