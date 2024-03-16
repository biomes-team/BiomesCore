using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace BiomesCore.Patches.ModThingComps
{
	[HarmonyPatch(typeof(PawnRenderer), nameof(PawnRenderer.BodyAngle))]
	public static class PawnRenderer_BodyAngle_Patch
	{
		public static readonly Dictionary<Pawn, float> OverrideAngle = new Dictionary<Pawn, float>();

		internal static bool Prefix(ref float __result, Pawn ___pawn)
		{
			if (OverrideAngle.TryGetValue(___pawn, out float angle))
			{
				__result = angle;
				return false;
			}

			return true;
		}
	}
}