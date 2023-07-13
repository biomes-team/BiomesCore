using BiomesCore.MentalState;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{
	[HarmonyPatch(typeof(Building_Door), nameof(Building_Door.PawnCanOpen))]
	public class Building_Door_PawnCanOpen_Patch
	{
		public static void Postfix(ref bool __result, Pawn p)
		{
			__result = __result && !(p.MentalState is MentalState_Hungering);
		}
	}
}