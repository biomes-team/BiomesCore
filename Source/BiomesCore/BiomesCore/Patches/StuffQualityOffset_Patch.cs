using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{
    [HarmonyPatch]
    internal static class StuffQualityOffset_Patch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(GenRecipe), "PostProcessProduct")]
        private static IEnumerable<CodeInstruction> PostProcessProduct(IEnumerable<CodeInstruction> instructions) => Inject(instructions);

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Frame), "CompleteConstruction")]
        private static IEnumerable<CodeInstruction> CompleteConstruction(IEnumerable<CodeInstruction> instructions) => Inject(instructions);

        private static IEnumerable<CodeInstruction> Inject(IEnumerable<CodeInstruction> instructions)
        {
            var method = AccessTools.Method(typeof(QualityUtility), "GenerateQualityCreatedByPawn", new[] { typeof(Pawn), typeof(SkillDef) });
            var infix = AccessTools.Method(typeof(StuffQualityOffset_Patch), nameof(AdjustQualityIfNeeded));

            int count = 0;

            foreach (var ci in instructions)
            {
                yield return ci;

                if (ci.Calls(method))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, infix);

                    count++;
                }
            }

            if (count == 0)
                throw new Exception("StuffQualityOffset_Patch failed");
        }

        private static QualityCategory AdjustQualityIfNeeded(QualityCategory quality, Thing thing)
        {
            if (thing.Stuff?.GetModExtension<StuffQualityOffset>() is {} offset)
            {
                int qualityNum = (int) quality + offset.qualityOffset;

                var maxQuality = (int) offset.maxQuality;
                var minQuality = (int) offset.minQuality;

                if (qualityNum > maxQuality) qualityNum = maxQuality;
                if (qualityNum < minQuality) qualityNum = minQuality;

                quality = (QualityCategory) qualityNum;
            }

            return quality;
        }
    }
}
