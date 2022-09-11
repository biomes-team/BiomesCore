using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(FoodUtility))]
    internal static class PredatorHuntingBehaviour
    {
        [HarmonyPrefix]
        [HarmonyPatch("IsAcceptablePreyFor")]
        public static bool IsAcceptablePreyFor(Pawn predator, Pawn prey, ref bool __result)
        {
            var packHunter = CompPackHunter.GetProps(predator.def);
            if (packHunter == null) return true;

            var packSize = CompPackHunter.GetPackSize(predator, packHunter.joinHuntRange);
            if (packSize <= 1) return true;

            var factor = 1 + (packSize - 1) * packHunter.packConfidenceBonusPerMember;

            __result = prey.RaceProps.canBePredatorPrey &&
                       prey.RaceProps.IsFlesh &&
                       (Find.Storyteller.difficulty.predatorsHuntHumanlikes || !prey.RaceProps.Humanlike) &&
                       prey.BodySize <= (double)predator.RaceProps.maxPreyBodySize * factor &&
                       (prey.Downed || prey.kindDef.combatPower <= 2.0 * predator.kindDef.combatPower * factor &&
                           prey.kindDef.combatPower * (double)prey.health.summaryHealth.SummaryHealthPercent *
                           prey.ageTracker.CurLifeStage.bodySizeFactor < predator.kindDef.combatPower * factor *
                           predator.health.summaryHealth.SummaryHealthPercent *
                           predator.ageTracker.CurLifeStage.bodySizeFactor) &&
                       (predator.Faction == null || prey.Faction == null || predator.HostileTo(prey)) &&
                       (predator.Faction == null || prey.HostFaction == null || predator.HostileTo(prey)) &&
                       (predator.Faction != Faction.OfPlayer || prey.Faction != Faction.OfPlayer) &&
                       (!predator.RaceProps.herdAnimal || predator.def != prey.def);

            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch("GetPreyScoreFor")]
        public static void GetPreyScoreFor(Pawn predator, Pawn prey, ref float __result)
        {
            var packDefense = CompPackDefense.GetProps(prey.def);
            if (packDefense == null) return;

            var packmate = CompPackHunter.FindAnyPackmate(prey, packDefense.joinDefenseRange);
            if (packmate == null) return;

            __result -= packDefense.predatorConfidencePenalty;
        }
    }
}