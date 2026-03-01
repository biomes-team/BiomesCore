using BiomesCore.DefModExtensions;
using BiomesCore.Defs;
using HarmonyLib;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace BiomesCore.Patches
{

	[HarmonyPatch(typeof(GeneUIUtility), "DrawGeneBasics")]
	public static class Patch_DrawGeneBasics
    {

        public static bool enableCustomBackground = true;

		[HarmonyPrefix]
		public static bool Prefix(GeneDef gene, Rect geneRect, GeneType geneType, bool doBackground, bool clickable, bool overridden)
        {
            if (enableCustomBackground && gene is BMT_GeneDef newGeneDef)
            {
                try
                {
                    DrawGeneBasics(newGeneDef, geneRect, geneType, doBackground, clickable, overridden);
                }
                catch (Exception arg)
                {
                    Log.Error("Failed render custom background for gene: " + gene.defName + ". Resetting BMT backgrounds to vanilla. Reason: " + arg);
                    enableCustomBackground = false;
				}
                return false;
            }
            return true;
        }

        public static void DrawGeneBasics(BMT_GeneDef gene, Rect geneRect, GeneType geneType, bool doBackground, bool clickable, bool overridden)
        {
            GUI.BeginGroup(geneRect);
            Rect rect = geneRect.AtZero();
            if (doBackground)
            {
                Widgets.DrawHighlight(rect);
                GUI.color = new(1f, 1f, 1f, 0.05f);
                Widgets.DrawBox(rect);
                GUI.color = Color.white;
            }
            float num = rect.width - Text.LineHeight;
            Rect rect2 = new(geneRect.width / 2f - num / 2f, 0f, num, num);
            Color iconColor = gene.IconColor;
            if (overridden)
            {
                iconColor.a = 0.75f;
                GUI.color = ColoredText.SubtleGrayColor;
            }
            CachedTexture cachedTexture = gene.BackgroundTexture(gene, geneType);
            GUI.DrawTexture(rect2, cachedTexture.Texture);
            Widgets.DefIcon(rect2, gene, null, 0.9f, null, drawPlaceholder: false, iconColor);
            Text.Font = GameFont.Tiny;
            float num2 = Text.CalcHeight(gene.LabelCap, rect.width);
            Rect rect3 = new(0f, rect.yMax - num2, rect.width, num2);
            GUI.DrawTexture(new(rect3.x, rect3.yMax - num2, rect3.width, num2), TexUI.GrayTextBG);
            Text.Anchor = TextAnchor.LowerCenter;
            if (overridden)
            {
                GUI.color = ColoredText.SubtleGrayColor;
            }
            if (doBackground && num2 < (Text.LineHeight - 2f) * 2f)
            {
                rect3.y -= 3f;
            }
            Widgets.Label(rect3, gene.LabelCap);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
            if (clickable)
            {
                if (Widgets.ButtonInvisible(rect))
                {
                    Find.WindowStack.Add(new Dialog_InfoCard(gene));
                }
                if (Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                }
            }
            GUI.EndGroup();
        }

    }

}
