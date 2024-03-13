using System.Collections.Generic;
using System.Linq;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Undergrounders should always feel they are indoors in cavern biomes.
	/// </summary>
	[HarmonyPatch(typeof(ThoughtWorker_IsIndoorsForUndergrounder),
		nameof(ThoughtWorker_IsIndoorsForUndergrounder.IsAwakeAndIndoors))]
	public static class ThoughtWorker_IsIndoorsForUndergrounder_IsAwakeAndIndoors_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return TranspilerHelper.ReplaceCall(instructions.ToList(), Methods.UsesOutdoorTemperatureMethod,
				Methods.NotCavernAndUsesOutdoorTemperatureMethod);
		}
	}
}