using System.Collections.Generic;
using Verse;
using RimWorld;
using Verse.AI;

namespace BiomesCore
{
    public class JobDriver_BloodDrinking : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetA, job);
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch)
                .FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            yield return new Toil
            {
                initAction = delegate
                {
                    var nutrientToFill = pawn.needs.food.MaxLevel - pawn.needs.food.CurLevel;
                    pawn.needs.food.CurLevel = pawn.needs.food.MaxLevel;
                    var victim = TargetA.Pawn;
                    HealthUtility.AdjustSeverity(victim, HediffDefOf.BloodLoss, nutrientToFill * 0.15f);
                }
            };
        }
    }
}
