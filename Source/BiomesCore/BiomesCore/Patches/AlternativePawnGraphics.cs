using System.Collections.Generic;
using BiomesCore.ThingComponents;
using HarmonyLib;
using Verse;

namespace BiomesCore.Patches
{
	// ToDo: https://github.com/biomes-team/BiomesCore/issues/14
	/*
	[HarmonyPatch(typeof(PawnGraphicSet), nameof(PawnGraphicSet.ResolveAllGraphics))]
	internal static class AlternativePawnGraphics
	{
		public static void Postfix(PawnGraphicSet __instance)
		{
			var pawnDef = __instance.pawn.kindDef;
			if (pawnDef == null)
			{
				return;
			}

			foreach (var comp in __instance.pawn.AllComps)
			{
				if (comp is CompDynamicPawnGraphic graphicComp && graphicComp.Active())
				{
					var data = graphicComp.Graphic();
					if (data != null)
					{
						__instance.nakedGraphic = data.Graphic;
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(PawnRenderer), nameof(PawnRenderer.BodyAngle))]
	public static class SleepingPawnAngle
	{
		public static Dictionary<Pawn, float> OverrideAngle = new Dictionary<Pawn, float>();

		internal static bool Prefix(ref float __result, PawnRenderer __instance)
		{
			if (OverrideAngle.TryGetValue(__instance.pawn, out float angle))
			{
				__result = angle;
				return false;
			}

			return true;
		}
	}
	
	[HarmonyPatch(typeof(PawnRenderer), nameof(PawnRenderer.LayingFacing))]
	public static class SleepingPawnLayFacing
	{
		public static HashSet<Pawn> OverrideDirection = new HashSet<Pawn>();

		internal static bool Prefix(ref Rot4 __result, PawnRenderer __instance)
		{
			if (!OverrideDirection.Contains(__instance.pawn))
			{
				return true;
			}

			__result = Rot4.South;
			return false;

		}
	}
		*/

}