using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{

	[HarmonyPatch(typeof(FoodUtility), "IsAcceptablePreyFor")]
	public static class Patch_FoodUtility_IsAcceptablePreyFor
	{

		[HarmonyPrefix]
		public static bool Prefix(ref bool __result, ref Pawn predator, ref Pawn prey)
		{
			Biomes_AnimalControl animalControl = predator.def?.GetModExtension<Biomes_AnimalControl>();
			if (animalControl != null)
			{
				if (animalControl.canHuntOnlyOnDefs.NullOrEmpty())
				{
					return true;
				}
				if (animalControl.canHuntOnlyOnDefs.Contains(prey.def))
				{
					return true;
				}
				__result = false;
				return false;
			}
			return true;
		}

	}

}
