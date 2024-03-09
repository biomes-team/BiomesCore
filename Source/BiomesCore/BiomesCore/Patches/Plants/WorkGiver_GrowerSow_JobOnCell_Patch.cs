using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches.Plants
{
	/// <summary>
	/// The prefix of this patch makes PlantUtility.GrowthSeasonNow aware of the type of plant which is going to be
	/// planted. The postfix removes the change to restore GrowthSeasonNow to vanilla functionality.
	/// </summary>
	[HarmonyPatch(typeof(WorkGiver_GrowerSow), nameof(WorkGiver_GrowerSow.JobOnCell))]
	public static class WorkGiver_GrowerSow_JobOnCell_Patch
	{
		public static void Prefix(Pawn pawn, IntVec3 c)
		{
			PlantUtility_GrowthSeasonNow_Patch.CalculateWantedPlantDef =
				WorkGiver_Grower.CalculateWantedPlantDef(c, pawn.Map);
		}

		public static void Postfix()
		{
			PlantUtility_GrowthSeasonNow_Patch.CalculateWantedPlantDef = null;
		}
	}
}