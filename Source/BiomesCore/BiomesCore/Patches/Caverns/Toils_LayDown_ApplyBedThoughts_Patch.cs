using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Apply slept outside thought when a pawn sleeps outside in a cavern.
	/// </summary>
	[HarmonyPatch(typeof(Toils_LayDown), "ApplyBedThoughts")]
	public static class Toils_LayDown_ApplyBedThoughts_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo pawnPositionMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Position));

			return TranspilerHelper.ReplaceCall(instructions.ToList(), Methods.PsychologicallyOutdoorsMethod,
				Methods.CavernAwarePsychologicallyOutdoorsMethod,
				new List<CodeInstruction>
				{
					new CodeInstruction(OpCodes.Ldarg_0), // Actor
					new CodeInstruction(OpCodes.Call, pawnPositionMethod) // Actor.Position
				});
		}
	}
}