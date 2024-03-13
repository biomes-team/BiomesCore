using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Cells with cavern roof are considered valid entry points for pawns.
	/// </summary>
	[HarmonyPatch]
	public class RCellFinder_TryFindRandomPawnEntryCell_Patch
	{
		[HarmonyTargetMethods]
		private static IEnumerable<MethodInfo> TargetMethods()
		{
			foreach (var nestedType in typeof(RCellFinder).GetNestedTypes(AccessTools.all))
			{
				foreach (var method in nestedType.GetMethods(AccessTools.all))
				{
					if (method.Name.Contains(nameof(RCellFinder.TryFindRandomPawnEntryCell)))
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