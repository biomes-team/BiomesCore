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
	/// Prevent pawns from thinking that the entire world is their bedroom when sleeping outside in a cavern.
	/// </summary>
	[HarmonyPatch(typeof(ThoughtWorker_NeedRoomSize), "CurrentStateInternal")]
	public class ThoughtWorker_NeedRoomSize_CurrentStateInternal_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo pawnPositionMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Position));

			return TranspilerHelper.ReplaceCall(instructions.ToList(), Methods.PsychologicallyOutdoorsMethod,
				Methods.PsychologicallyOutdoorsOrCavernMethod,
				new List<CodeInstruction>
				{
					new CodeInstruction(OpCodes.Ldarg_1), // Pawn
					new CodeInstruction(OpCodes.Call, pawnPositionMethod) // Pawn.Position
				});
		}
	}
}