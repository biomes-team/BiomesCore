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
	[HarmonyPatch]
	public class FindPlayerStartSpot
	{
		static MethodBase TargetMethod()
		{
			return typeof(GenStep_FindPlayerStartSpot).GetLambda(nameof(GenStep_FindPlayerStartSpot.Generate));
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CellUnbreachableRoofed(instructions.ToList());
		}
	}
}