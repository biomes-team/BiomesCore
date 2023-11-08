using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace BiomesCore
{
    public class JobDriver_BottomFeeder : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            var compBottomFeeder = pawn.GetComp<CompBottomFeeder>();
            CompGlower compGlower = pawn.GetComp<CompDefaultOffGlower>();
            bool pawnShouldGlow = compGlower != null && compBottomFeeder.Props.shouldGlow == true;
            if (compBottomFeeder == null) yield break;
            Toil wait = new Toil();
            wait.defaultCompleteMode = ToilCompleteMode.Delay;
            wait.WithEffect(() => compBottomFeeder.Props.effecterDef, () => TargetLocA);
            wait.defaultDuration = 1000;
            wait.socialMode = RandomSocialMode.Off;
            wait.FailOnCannotTouch<Toil>(TargetIndex.A, PathEndMode.Touch);
            wait.AddPreTickAction(delegate
            {
                if (pawnShouldGlow) { EnableGlow(pawn, compGlower); }
            });
            wait.AddFinishAction(delegate
            {
                if (pawnShouldGlow) { DisableGlow(pawn, compGlower); };
            });
            yield return wait.WithProgressBarToilDelay(TargetIndex.A, true);
            yield return Toils_General.Do(() => ++this.pawn.needs.food.CurLevel);
        }

        private void EnableGlow(Pawn pawn, CompGlower compGlower)
        {
            if (compGlower != null)
            {
                pawn.Map.glowGrid.RegisterGlower(compGlower);
            }
        }

        private void DisableGlow(Pawn pawn, CompGlower compGlower)
        {
            pawn.Map.glowGrid.DeRegisterGlower(compGlower);
            compGlower.UpdateLit(pawn.Map);
        }
    }
}
