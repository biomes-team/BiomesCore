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
            
            foreach (var packmate in compPackHunter.FindPackmates())
            {
                if (packmate.jobs.curJob?.def != JobDefOf.PredatorHunt && packmate.health.hediffSet.PainTotal <= 0.3)
                {
                    var job = JobMaker.MakeJob(JobDefOf.PredatorHunt, __instance.job.targetA, __instance.job.targetB);
                    packmate.jobs.StartJob(job, JobCondition.InterruptOptional);
                }
            }
        }
    }
}