using BiomesCore.DefModExtensions;
using HarmonyLib;
using Verse;

namespace BiomesCore.Patches
{
	[HarmonyPatch(typeof(PollutionUtility), "StimulatedByPollution")]
	internal static class PollutionUtility_StimulatedByPollution_Patch
	{
		private static void Postfix(ref bool __result, Pawn pawn)
		{
			if (__result)
			{
				var extension = pawn.def.GetModExtension<Biomes_AnimalControl>();
				__result = extension == null || extension.isInsectoid;
			}
		}
	}
}