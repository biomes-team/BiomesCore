using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches.Caverns
{
	[HarmonyPatch(typeof(GenStep_SpecialTrees), nameof(GenStep_SpecialTrees.CanSpawnAt))]
	internal static class AnimaTreeSpawn
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
			var newInstructions = Transpilers.CellPsychologicallyOutdoors(codeInstructions.ToList(), OpCodes.Ldarg_1);
			var newestInstructions = Transpilers.CellUnbreachableRoofed(newInstructions);

			/*
			var oldInstructions = Test(codeInstructions).ToList();
			var size = Math.Max(newestInstructions.Count, oldInstructions.Count);
			for (int index = 0; index < size; ++index)
			{
				string oldStr = oldInstructions.Count > index ? oldInstructions[index].ToString() : "";
				string newStr = oldInstructions.Count > index ? newestInstructions[index].ToString() : "";
				Log.Error($"{oldStr} -> {newStr}");
			}
			*/

			return newestInstructions;
		}

		private static IEnumerable<CodeInstruction> Test(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo outdoorsOriginal = AccessTools.PropertyGetter(typeof(Room), nameof(Room.PsychologicallyOutdoors));
			MethodInfo outdoorsPatched =
				AccessTools.Method(typeof(Utility), nameof(Utility.PsychologicallyOutdoorsOrCavern));

			MethodInfo roofedOriginal = AccessTools.Method(typeof(GridsUtility), nameof(GridsUtility.Roofed));
			MethodInfo roofedPatched =
				AccessTools.Method(typeof(IntVec3Extensions), nameof(IntVec3Extensions.UnbreachableRoofed));

			foreach (var line in instructions)
			{
				if (line.opcode == OpCodes.Callvirt && (MethodInfo) line.operand == outdoorsOriginal)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_1); // IntVec3 c;
					yield return new CodeInstruction(OpCodes.Call, outdoorsPatched);
				}
				else if (line.opcode == OpCodes.Call && (MethodInfo) line.operand == roofedOriginal)
				{
					yield return new CodeInstruction(OpCodes.Call, roofedPatched);
				}
				else
				{
					yield return line;
				}
			}
		}
	}
}