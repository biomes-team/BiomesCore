using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BiomesCore.DefModExtensions;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld.BaseGen;
using Verse;

namespace BiomesCore.Patches
{
	[HarmonyPatch]
	internal static class SymbolResolver_Interior_SleepingInsects_Resolve_Patch
	{
		static MethodBase TargetMethod()
		{
			return typeof(SymbolResolver_Interior_SleepingInsects).GetLambda(nameof(SymbolResolver_Interior_SleepingInsects
				.Resolve));
		}

		private static bool CanSpawnInAncientComplexes(PawnKindDef def)
		{
			if (!def.RaceProps.Insect)
			{
				return false;
			}

			var extension = def.race.GetModExtension<Biomes_AnimalControl>();
			return extension != null && extension.isInsectoid;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo canSpawnInAncientComplexes =
				AccessTools.Method(typeof(SymbolResolver_Interior_SleepingInsects_Resolve_Patch),
					nameof(CanSpawnInAncientComplexes));
			foreach (var instruction in instructions)
			{
				if (instruction.opcode != OpCodes.Callvirt)
				{
					yield return instruction;
				}

				if (instruction.opcode == OpCodes.Ldarg_1)
				{
					yield return new CodeInstruction(OpCodes.Call, canSpawnInAncientComplexes);
				}
			}
		}
	}
}