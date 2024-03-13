using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// The call shuttle royal permit can be used in caverns.
	/// </summary>
	[HarmonyPatch(typeof(RoyalTitlePermitWorker_CallShuttle), "GetReportFromCell")]
	public class RoyalTitlePermitWorker_CallShuttle_GetReportFromCell_Patch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.GetNonCavernRoof(instructions.ToList());
		}
	}
}