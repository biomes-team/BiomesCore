using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace BiomesCore
{
	public class JobDriver_EatCustomThing : JobDriver
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed) =>
			pawn.Reserve(TargetA, job, errorOnFailed: errorOnFailed);

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

			CompCustomThingEater customThingEater = pawn.GetComp<CompCustomThingEater>();
			if (customThingEater == null)
			{
				yield break;
			}

			Dictionary<ThingDef, float> thingsToNutrition = customThingEater.Props.thingsToNutrition;
			yield return Toils_General.Do(() =>
			{
				if (TargetA.Thing is Filth filth && thingsToNutrition.TryGetValue(filth.def, out var nutrition))
				{
					pawn.needs.food.CurLevel += nutrition;
					filth.Destroy();
				}
				ReadyForNextToil();
			});
		}
	}
}