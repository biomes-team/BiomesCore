using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Pawns will still get a penalty when using a work table that is outside.
	/// </summary>
	[HarmonyPatch(typeof(StatPart_WorkTableOutdoors), nameof(StatPart_WorkTableOutdoors.Applies), new Type[]
	{
		typeof(ThingDef), typeof(Map), typeof(IntVec3)
	})]
	public class StatPart_WorkTableOutdoors_Applies_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CavernsAwarePsychologicallyOutdoors(instructions.ToList(),
				OpCodes.Ldarg_2 // Cell
			);
		}
	}
}