using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Gauranlen pods can spawn on cells with cavern roof. They can also spawn on cavern rooms that are considered to be
	/// outdoors.
	/// </summary>
	[HarmonyPatch(typeof(IncidentWorker_GauranlenPodSpawn), "CanSpawnPodAt")]
	internal static class IncidentWorker_GauranlenPodSpawn_CanSpawnPodAt_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var newInstructions = Transpilers.CavernsAwarePsychologicallyOutdoors(instructions.ToList(), OpCodes.Ldarg_0);
			var secondList = Transpilers.CellHasNonCavernRoof(newInstructions);

			return secondList;
		}
	}
}