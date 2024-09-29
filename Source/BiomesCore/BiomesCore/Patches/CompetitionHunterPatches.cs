using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(FoodUtility))]
    internal static class CompetitionHunterPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("IsAcceptablePreyFor")]
        public static void CompetitionHunterPatch_IsAcceptablePreyFor(Pawn predator,
            Pawn prey,
            ref bool __result)
        {
            var competitionHunter =
                predator.def.GetModExtension<CompetitionHunterModExtension>();
            if (competitionHunter == null) return;
            if (predator.def != prey.def || predator.gender != prey.gender) return;

            if ((predator.gender == Gender.Female &&
                 competitionHunter.femaleEnabled) ||
                (predator.gender == Gender.Male && competitionHunter.maleEnabled))
            {
                __result = true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("GetPreyScoreFor")]
        public static void CompetitionHunterPatch_PreyScoreFor(Pawn predator, Pawn prey,
            ref float __result)
        {
            var competitionHunter =
                predator.def.GetModExtension<CompetitionHunterModExtension>();
            if (competitionHunter == null) return;
            if (predator.def != prey.def || predator.gender != prey.gender) return;

            if ((predator.gender == Gender.Female && competitionHunter.femaleEnabled) ||
                (predator.gender == Gender.Male && competitionHunter.maleEnabled))
            {
                __result *= 1000f;
            }
        }
    }
}