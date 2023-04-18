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
	// 
	/// <summary>
	/// Used by caravans (both from the colony and friendly traders) when they want to leave.
	/// </summary>
	[HarmonyPatch]
	public class RCellFinder_TryFindRandomSpotJustOutsideColony
	{
		static MethodBase TargetMethod()
		{
			return typeof(RCellFinder).GetLambda(nameof(RCellFinder.TryFindRandomSpotJustOutsideColony), parentArgs: new[]
			{
				typeof(IntVec3), typeof(Map), typeof(Pawn), typeof(IntVec3).MakeByRefType(), typeof(Predicate<IntVec3>)
			}, lambdaOrdinal: 0);
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CellPsychologicallyOutdoors(instructions.ToList(), OpCodes.Ldarg_1);
		}
	}
}