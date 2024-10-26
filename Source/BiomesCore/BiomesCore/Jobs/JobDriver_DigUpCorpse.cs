using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace BiomesCore
{
    public class JobDriver_DigUpCorpse : JobDriver
    {
        public override string GetReport()
        {
            return job.def.reportString.Formatted(TargetA.Label);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed) => true; // No reservations needed, and no fail point

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil toil = ToilMaker.MakeToil("DigUpCorpse");

            toil.initAction = delegate
            {
                toil.actor.pather.StopDead();
                toil.actor.jobs.curDriver.ticksLeftThisToil = job.count; // Count represents time because there wasn't any other variable I could access from the giver's side
            };
            toil.WithProgressBar(TargetIndex.B, delegate
            {
                return (float)toil.actor.jobs.curDriver.ticksLeftThisToil / Mathf.Round((float)job.count);
            });
            toil.AddFinishAction(delegate
            {
                GenSpawn.Spawn(TargetA.Pawn, toil.actor.Position, toil.actor.Map);
                TargetA.Pawn.Kill(new DamageInfo(DamageDefOf.Bite, 99999f, 999f, -1f));
                if (toil.actor.Faction == Faction.OfPlayer)
                    TargetA.Pawn?.Corpse?.SetForbidden(false);
            });
            yield return toil;
        }
    }
}
