using System.Collections.Generic;
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
		private static bool PsychologicallyOutdoorsOrCavern(Room room, IntVec3 cell)
		{
			return cell.GetRoof(room.Map) == BiomesCoreDefOf.BMT_RockRoofStable || room.PsychologicallyOutdoors;
		}

		[HarmonyPriority(Priority.First)]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo outdoorsOriginal = AccessTools.PropertyGetter(typeof(Room), nameof(Room.PsychologicallyOutdoors));
			MethodInfo outdoorsPatched =
				AccessTools.Method(typeof(AnimaTreeSpawn), nameof(PsychologicallyOutdoorsOrCavern));

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