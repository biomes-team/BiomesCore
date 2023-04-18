using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches.Caverns
{
	[HarmonyPatch]
	public class PawnsArrivalEdgeWalkIn
	{
		[HarmonyTargetMethods]
		static IEnumerable<MethodInfo> TargetMethods()
		{
			foreach (var nestedType in typeof(PawnsArrivalModeWorker_EdgeWalkIn).GetNestedTypes(AccessTools.all))
			{
				foreach (var method in nestedType.GetMethods(AccessTools.all))
				{
					if (method.Name.Contains(nameof(PawnsArrivalModeWorker_EdgeWalkIn.TryResolveRaidSpawnCenter)))
					{
						yield return method;
					}
				}
			}
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.RoofGridUnbreachableRoofed(instructions.ToList());
		}
	}
}