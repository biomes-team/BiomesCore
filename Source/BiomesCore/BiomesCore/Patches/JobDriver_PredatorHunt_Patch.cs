using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(JobDriver_PredatorHunt))]
    internal class JobDriver_PredatorHunt_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("MakeNewToils")]
        public static void MakeNewToils_Prefix(JobDriver_PredatorHunt __instance, ref IEnumerable<Toil> __result)
        {
            if (Scribe.mode == LoadSaveMode.PostLoadInit) return;
            if (__instance.pawn.Dead || !__instance.pawn.Spawned) return;
            
            var compLurePrey = CompLurePrey.GetProps(__instance.pawn.def);
            if (compLurePrey != null)
            {
                IEnumerable<Toil> WithLurePrey(IEnumerator<Toil> original)
                {
                    yield return StartLurePrey(__instance, compLurePrey);
                    while (original.MoveNext()) yield return original.Current;
                }

                __result = WithLurePrey(__result.GetEnumerator());
                return;
            }

            var compPackHunter = CompPackHunter.GetProps(__instance.pawn.def);
            if (compPackHunter != null)
            {
                IEnumerable<Toil> WithPackHunt(IEnumerator<Toil> original)
                {
                    yield return StartPackHunt(__instance, compPackHunter);
                    while (original.MoveNext()) yield return original.Current;
                }

                __result = WithPackHunt(__result.GetEnumerator());
                return;
            }
        }

        private static Toil StartLurePrey(JobDriver_PredatorHunt jobDriver, CompProperties_LurePrey compLurePrey)
        {
            var curJob = jobDriver.pawn.jobs.curJob;
            var prey = curJob.GetTarget(TargetIndex.A).Thing as Pawn;
            
            var toil = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = compLurePrey.preyApproachTimeout
            };

            toil.initAction = () =>
            {
                if (prey == null || !prey.Spawned || prey.InMentalState || prey.Downed || 
                    !prey.CanReach(toil.actor, PathEndMode.Touch, Danger.Some))
                {
                    jobDriver.ReadyForNextToil();
                    return;
                }
                
                toil.actor.pather.StopDead();
                
                var job = JobMaker.MakeJob(JobDefOf.Goto, toil.actor);
                job.locomotionUrgency = LocomotionUrgency.Walk;
                job.checkOverrideOnExpire = true;
                job.expiryInterval = compLurePrey.preyApproachTimeout;
                prey.jobs.StartJob(job, JobCondition.InterruptOptional);
            };

            toil.tickAction = () =>
            {
                if (prey == null || prey.Position.InHorDistOf(toil.actor.Position, compLurePrey.attackOnDistance))
                {
                    jobDriver.ReadyForNextToil();
                }
            };
            
            return toil;
        }

        private static Toil StartPackHunt(JobDriver_PredatorHunt jobDriver, CompProperties_PackHunter compPackHunter)
        {
            return Toils_General.Do(() =>
            {
                foreach (var packmate in CompPackHunter.FindPackmates(jobDriver.pawn, compPackHunter.joinHuntRange))
                {
                    if (packmate.jobs.curJob?.def != JobDefOf.PredatorHunt &&
                        packmate.health.hediffSet.PainTotal <= compPackHunter.joinMaxPain)
                    {
                        if (packmate.ageTracker.Adult)
                        {
                            if (packmate.needs.food.CurCategory != HungerCategory.Fed)
                            {
                                var job = JobMaker.MakeJob(JobDefOf.PredatorHunt, jobDriver.job.targetA, jobDriver.job.targetB);
                                packmate.jobs.StartJob(job, JobCondition.InterruptOptional);
                            }
                            else
                            {
                                var job = JobMaker.MakeJob(JobDefOf.AttackMelee, jobDriver.job.targetA);
                                packmate.jobs.StartJob(job, JobCondition.InterruptOptional);
                            }
                        }
                        else
                        {
                            var job = JobMaker.MakeJob(JobDefOf.Follow, jobDriver.pawn);
                            job.locomotionUrgency = LocomotionUrgency.Sprint;
                            job.checkOverrideOnExpire = true;
                            job.expiryInterval = 5000;
                            packmate.jobs.StartJob(job, JobCondition.InterruptOptional);
                        }
                    }
                }
            });
        }
    }
}