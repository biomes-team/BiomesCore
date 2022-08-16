using RimWorld;
using System.Collections.Generic;
using Verse.AI;

namespace BiomesCore
{
    public class JobDriver_BottomFeeder : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            Toil wait = new Toil();
            wait.defaultCompleteMode = ToilCompleteMode.Delay;
            wait.defaultDuration = 1000;
            wait.socialMode = RandomSocialMode.Off;
            wait.FailOnCannotTouch<Toil>(TargetIndex.A, PathEndMode.Touch);
            yield return wait.WithProgressBarToilDelay(TargetIndex.A, true);
            yield return Toils_General.Do(() => ++this.pawn.needs.food.CurLevel);
        }
    }
}
