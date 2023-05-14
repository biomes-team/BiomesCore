using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{
	[HarmonyPatch(typeof(JobGiver_GetFood), nameof(JobGiver_GetFood.GetPriority))]
	internal static class JobGiver_GetFood_GetPriority_Patch
	{
		private static void Postfix(Pawn pawn, ref float __result)
		{
			if (__result > 0.0F)
			{
				return;
			}
			
			var extension = pawn.def.GetModExtension<Biomes_AnimalControl>();
			Need_Food food = pawn.needs.food;
			if (extension != null && food != null && extension.eatWhenFed)
			{
				__result = 6.5F;
			}
		}
	}
}