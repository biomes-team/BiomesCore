using BiomesCore.ThingComponents;
using HarmonyLib;
using Verse;

namespace BiomesCore.Patches
{
	[HarmonyPatch(typeof(PawnGraphicSet), nameof(PawnGraphicSet.ResolveAllGraphics))]
	internal static class AlternativePawnGraphics
	{
		public static void Postfix(PawnGraphicSet __instance)
		{
			PawnKindDef pawnDef = __instance.pawn.kindDef;
			if (pawnDef == null)
			{
				return;
			}

			GraphicData alternativeGraphic = null;
			var compSleepGraphic = __instance.pawn.GetComp<CompSleepGraphic>();
			if (compSleepGraphic != null && compSleepGraphic.Active())
			{
				alternativeGraphic = compSleepGraphic.Graphic();
			}

			if (alternativeGraphic != null)
			{
				__instance.ClearCache();
				__instance.nakedGraphic = alternativeGraphic.Graphic;
			}
		}
	}

	[HarmonyPatch(typeof(PawnRenderer), nameof(PawnRenderer.BodyAngle))]
	internal static class SleepingPawnAngle
	{
		internal static bool Prefix(ref float __result, PawnRenderer __instance)
		{
			var compSleepGraphic = __instance.pawn.GetComp<CompSleepGraphic>();
			if (compSleepGraphic != null && compSleepGraphic.Active())
			{
				__result = 0.0F;
				return false;
			}

			return true;
		}
	}
}