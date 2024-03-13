using System.Collections.Generic;
using System.Linq;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Undergrounders should never feel they are outdoors in cavern biomes.
	/// </summary>
	[HarmonyPatch(typeof(ThoughtWorker_IsOutdoorsForUndergrounder), "CurrentStateInternal")]
	public static class ThoughtWorker_IsOutdoorsForUndergrounder_CurrentStateInternal_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return TranspilerHelper.ReplaceCall(instructions.ToList(), Methods.UsesOutdoorTemperatureMethod,
				Methods.NotCavernAndUsesOutdoorTemperatureMethod);
		}
	}
}