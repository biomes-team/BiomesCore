using BiomesCore.DefModExtensions;
using HarmonyLib;
using System.Linq;
using Verse;
using RimWorld;
using Verse.AI;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(JobGiver_GetFood), "TryGiveJob")]
    public static class JobGiver_GetFood_TryGiveJob_Patch
    {
        public static bool Prefix(ref Job __result, Pawn pawn, HungerCategory ___minCategory, float ___maxLevelPercentage)
        {
            Need_Food food = pawn.needs.food;
            if (food == null || (int)food.CurCategory < (int)___minCategory || food.CurLevelPercentage > ___maxLevelPercentage)
            {
                return true;
            }
            var extension = pawn.def.GetModExtension<Biomes_AnimalControl>();
            if (extension != null && extension.isBloodDrinkingAnimal)
            {
                var nearestPawn = pawn.Map.mapPawns.AllPawnsSpawned
                    .Where(x => x.def != pawn.def && x.Position.DistanceTo(pawn.Position) <= 50
                    && x.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss) != null
                    && pawn.CanReserveAndReach(x, PathEndMode.Touch, Danger.Deadly))
                    .OrderBy(x => x.Position.DistanceTo(pawn.Position)).FirstOrDefault();
                if (nearestPawn != null)
                {
                    __result = JobMaker.MakeJob(BiomesCoreDefOf.BC_BloodDrinking, nearestPawn);
                    return false;
                }
            }
            var comp = pawn.GetComp<CompBottomFeeder>();
            if (comp != null)
            {
                var nearestCell = pawn.Map.AllCells.Where(x => x.GetTerrain(pawn.Map).HasTag(comp.Props.feedingTerrainTag)
                && pawn.CanReserveAndReach(x, PathEndMode.OnCell, Danger.Deadly)).OrderBy(x => x.DistanceTo(pawn.Position)).FirstOrDefault();
                if (nearestCell.IsValid)
                {
                    __result = JobMaker.MakeJob(BiomesCoreDefOf.BC_BottomFeeder, nearestCell);
                    Log.Message("Gave job: " + nearestCell);
                    return false;
                }
            }
            return true;
        }
    }
}
