using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace BiomesCore.Patches.Caverns
{
	[HarmonyPatch(typeof(IncidentWorker_GauranlenPodSpawn), "CanSpawnPodAt")]
	internal static class GauranlenPodSpawn
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var newInstructions = Transpilers.CellPsychologicallyOutdoors(instructions.ToList(), OpCodes.Ldarg_0);
			var secondList = Transpilers.CellUnbreachableRoofed(newInstructions);

			return secondList;
		}
	}
}