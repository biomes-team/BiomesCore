using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace BiomesCore.Patches.ModThingComps
{
	[HarmonyPatch(typeof(PawnRenderer), nameof(PawnRenderer.LayingFacing))]
	public static class PawnRenderer_LayingFacing_Patch
	{
		public static readonly HashSet<Pawn> OverrideDirection = new HashSet<Pawn>();

		internal static bool Prefix(ref Rot4 __result, Pawn ___pawn)
		{
			if (!OverrideDirection.Contains(___pawn))
			{
				return true;
			}

			__result = Rot4.South;
			return false;
		}
	}
}