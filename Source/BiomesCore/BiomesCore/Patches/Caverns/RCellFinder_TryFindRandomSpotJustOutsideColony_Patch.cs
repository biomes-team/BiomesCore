using System;
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
	/// Caravans (both from the colony and friendly traders) can gather at points considered to be outdoors in caverns.
	/// </summary>
	[HarmonyPatch]
	public static class RCellFinder_TryFindRandomSpotJustOutsideColony_Patch
	{
		private static MethodBase TargetMethod()
		{
			return typeof(RCellFinder).GetLambda(nameof(RCellFinder.TryFindRandomSpotJustOutsideColony), parentArgs: new[]
			{
				typeof(IntVec3), typeof(Map), typeof(Pawn), typeof(IntVec3).MakeByRefType(), typeof(Predicate<IntVec3>)
			}, lambdaOrdinal: 0);
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CavernsAwarePsychologicallyOutdoors(instructions.ToList(),
				OpCodes.Ldarg_1 // Cell
			);
		}
	}
}