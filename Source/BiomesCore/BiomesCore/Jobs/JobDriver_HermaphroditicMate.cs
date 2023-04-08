using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace BiomesCore
{
	public class JobDriver_HermaphroditicMate : JobDriver
	{
		private const int MateDuration = 500;

		public const int TicksBetweenHeartMotes = 100;

		private Pawn Mate => (Pawn) job.GetTarget(TargetIndex.A).Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOnDowned(TargetIndex.A);
			this.FailOnNotCasualInterruptible(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			Toil toil = Toils_General.WaitWith(TargetIndex.A, 500);
			toil.tickAction = delegate
			{
				if (pawn.IsHashIntervalTick(100))
				{
					FleckMaker.ThrowMetaIcon(pawn.Position, pawn.Map, FleckDefOf.Heart);
				}

				if (Mate.IsHashIntervalTick(100))
				{
					FleckMaker.ThrowMetaIcon(Mate.Position, pawn.Map, FleckDefOf.Heart);
				}
			};
			yield return toil;
			yield return Toils_General.Do(delegate
			{
				PawnUtility.Mated(pawn, Mate);
				PawnUtility.Mated(Mate, pawn);
			});
		}
	}
}