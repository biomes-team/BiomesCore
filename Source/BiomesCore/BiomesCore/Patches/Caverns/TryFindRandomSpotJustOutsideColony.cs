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
	public class TryFindRandomSpotJustOutsideColony
	{
		static MethodBase TargetMethod()
		{
			MethodBase methodToPatch = null;
			foreach (var currentLambdaType in typeof(RCellFinder).GetNestedTypes(AccessTools.all))
			{
				foreach (var currentLambdaMethod in currentLambdaType.GetMethods(AccessTools.allDeclared))
				{
					if (currentLambdaMethod.Name.Contains(nameof(RCellFinder.TryFindRandomSpotJustOutsideColony)))
					{
						var parameters = currentLambdaMethod.GetParameters();
						if (parameters.Length == 1 && parameters[0].ParameterType == typeof(IntVec3))
						{
							methodToPatch = currentLambdaMethod;
							break;
						}
					}
				}

				if (methodToPatch != null)
				{
					break;
				}
			}

			return methodToPatch;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CellPsychologicallyOutdoors(instructions.ToList(), OpCodes.Ldarg_1);
		}
	}
}