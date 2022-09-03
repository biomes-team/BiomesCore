using RimWorld;
using System.Collections.Generic;
using Verse.AI;

namespace BiomesCore
{
    public class JobDriver_EatCustomThing : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            
            var wait = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 500,
                socialMode = RandomSocialMode.Off
            };
            
            wait.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            
            yield return wait.WithProgressBarToilDelay(TargetIndex.A, true);
            
            yield return Toils_General.Do(() =>
            {
                var customThingEater = pawn.GetComp<CompCustomThingEater>();
                if (customThingEater != null)
                {
                    var thing = TargetA.Thing;
                    if (thing == null) return;

                    if (customThingEater.Props.thingsToNutrition.TryGetValue(thing.def, out var nutrition))
                    {
                        pawn.needs.food.CurLevel += nutrition;
                        if (thing.stackCount > 1) thing.SplitOff(1);
                        else thing.Destroy();
                    }
                }
            });
        }
    }
}
