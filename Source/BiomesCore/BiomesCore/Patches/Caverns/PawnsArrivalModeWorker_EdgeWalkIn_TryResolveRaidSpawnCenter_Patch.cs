using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Pawns can arrive walking in cells having cavern roof.
	/// </summary>
	[HarmonyPatch]
	public static class PawnsArrivalModeWorker_EdgeWalkIn_TryResolveRaidSpawnCenter_Patch
	{
		[HarmonyTargetMethods]
		private static IEnumerable<MethodInfo> TargetMethods()
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