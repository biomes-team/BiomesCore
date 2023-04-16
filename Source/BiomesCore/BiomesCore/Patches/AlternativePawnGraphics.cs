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