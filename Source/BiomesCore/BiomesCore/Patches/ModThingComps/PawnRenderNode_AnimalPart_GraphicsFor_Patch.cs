using BiomesCore.ThingComponents;
using HarmonyLib;
using Verse;

namespace BiomesCore.Patches.ModThingComps
{
	/// <summary>
	/// Apply any required graphic changes.
	/// </summary>
	[HarmonyPatch(typeof(PawnRenderNode_AnimalPart), nameof(PawnRenderNode_AnimalPart.GraphicFor))]
	public static class PawnRenderNode_AnimalPart_GraphicsFor_Patch
	{
		public static void Postfix(Pawn pawn, ref Graphic __result)
		{
			if (pawn == null || __result == null)
			{
				return;
			}

			foreach (var comp in pawn.AllComps)
			{
				if (comp is CompDynamicAnimalGraphic graphicComp && graphicComp.Active())
				{
					var data = graphicComp.Graphic(__result);
					if (data != null)
					{
						__result = data;
					}
				}
			}
		}
	}
}