using System;
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
            var compBottomFeeder = pawn.GetComp<CompBottomFeeder>();
            if (compBottomFeeder == null) yield break;

            Toil toil;

            if (compBottomFeeder.Props.eatWhileMoving)
            {
                toil = Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            }
            else
            {
                yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);

                toil = new Toil
                {
                    defaultCompleteMode = ToilCompleteMode.Delay,
                    socialMode = RandomSocialMode.Off,
                    defaultDuration = 1000
                };

                toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);

                var compGlower = pawn.GetComp<CompDefaultOffGlower>();

                if (compGlower != null && compBottomFeeder.Props.shouldGlow)
                {
                    toil.AddPreInitAction(delegate
                    {
                        EnableGlow(pawn, compGlower);
                    });

                    toil.AddFinishAction(delegate
                    {
                        DisableGlow(pawn, compGlower);
                    });
                }
            }

            toil.WithEffect(() => compBottomFeeder.Props.effecterDef, () => pawn);

            toil.AddPreTickAction(() =>
            {
                pawn.needs.food.CurLevel += compBottomFeeder.Props.foodGainPerTick;
            });

            toil.endConditions.Add(() => pawn.needs.food.CurLevel >= pawn.needs.food.MaxLevel ? JobCondition.Succeeded : JobCondition.Ongoing);

            yield return toil;
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
