using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BiomesCore.DefModExtensions;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{
	[HarmonyPatch]
	internal static class PreceptWorker_Animal_ThingDefs_Patch
	{
		static MethodBase TargetMethod()
		{
			return typeof(PreceptWorker_Animal).GetLambda(nameof(PreceptWorker_Animal
				.ThingDefs), parentMethodType: MethodType.Getter);
		}

		private static bool IsInsectoid(ThingDef def)
		{
			if (!def.race.Insect)
			{
				return false;
			}

			var extension = def.GetModExtension<Biomes_AnimalControl>();
			return extension != null && extension.isInsectoid;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo canBeVeneratedAnimalMethod =
				AccessTools.Method(typeof(PreceptWorker_Animal_ThingDefs_Patch), nameof(IsInsectoid));
			MethodInfo insectProperty =
				AccessTools.PropertyGetter(typeof(RaceProperties), nameof(RaceProperties.Insect));

			var instructionList = instructions.ToList();
			var indexOfGetInsect =
				instructionList.FindIndex(0, instruction => instruction.operand as MethodInfo == insectProperty);
			Log.Error($"indexOfGetInsect: {indexOfGetInsect}");

			for (var index = 0; index < instructionList.Count; ++index)
			{
				if (index != indexOfGetInsect - 1 && index != indexOfGetInsect)
				{
					yield return instructionList[index];
				}

				if (index == indexOfGetInsect - 2)
				{
					yield return new CodeInstruction(OpCodes.Call, canBeVeneratedAnimalMethod);
				}
			}
		}
	}
}