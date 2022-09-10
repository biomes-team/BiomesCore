using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(JobDriver_PredatorHunt))]
    internal class JobDriver_PredatorHunt_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("MakeNewToils")]
        public static void MakeNewToils_Prefix(JobDriver_PredatorHunt __instance)
        {
            if (Scribe.mode == LoadSaveMode.PostLoadInit) return;

            var compPackHunter = __instance.pawn.TryGetComp<CompPackHunter>();
            if (compPackHunter == null) return;
            
            if (__instance.pawn.Dead || !__instance.pawn.Spawned) return;
            
            foreach (var packmate in CompPackHunter.FindPackmates(__instance.pawn, compPackHunter.Props.joinHuntRange))
            {
                if (packmate.jobs.curJob?.def != JobDefOf.PredatorHunt && packmate.health.hediffSet.PainTotal <= compPackHunter.Props.joinMaxPain)
                {
                    if (packmate.ageTracker.Adult)
                    {
                        if (packmate.needs.food.CurCategory != HungerCategory.Fed)
                        {
                            var job = JobMaker.MakeJob(JobDefOf.PredatorHunt, __instance.job.targetA, __instance.job.targetB);
                            packmate.jobs.StartJob(job, JobCondition.InterruptOptional);
                        }
                        else
                        {
                            var job = JobMaker.MakeJob(JobDefOf.AttackMelee, __instance.job.targetA);
                            packmate.jobs.StartJob(job, JobCondition.InterruptOptional);
                        }
                    }
                    else
                    {
                        var job = JobMaker.MakeJob(JobDefOf.Follow, __instance.pawn);
                        job.locomotionUrgency = LocomotionUrgency.Sprint;
                        job.checkOverrideOnExpire = true;
                        job.expiryInterval = 5000;
                        packmate.jobs.StartJob(job, JobCondition.InterruptOptional);
                    }
                }
            }
        }
    }
}