using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches.Caverns
{
	[HarmonyPatch(typeof(RoyalTitlePermitWorker_CallShuttle), "GetReportFromCell")]
	public class RoyalTitlePermitWorker_CallShuttle_GetReportFromCell
	{
		private static RoofDef GetRoofIfNotCaverns(IntVec3 c, Map map)
		{
			var roof = c.GetRoof(map);
			return roof != BiomesCoreDefOf.BMT_RockRoofStable ? roof : null;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo toPatch =
				AccessTools.Method(typeof(GridsUtility), nameof(GridsUtility.GetRoof));
			MethodInfo patched =
				AccessTools.Method(typeof(RoyalTitlePermitWorker_CallShuttle_GetReportFromCell), nameof(GetRoofIfNotCaverns));

			return TranspilerHelper.ReplaceCall(instructions.ToList(), toPatch, patched);
		}
	}
}