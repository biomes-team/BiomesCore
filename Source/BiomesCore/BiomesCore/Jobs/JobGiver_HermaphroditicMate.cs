using RimWorld;
using Verse;
using Verse.AI;

namespace BiomesCore
{

	public class JobGiver_HermaphroditicMate : ThinkNode_JobGiver 
	{
		// Assumes that seeker is genderless and not sterile.
		private bool CanMate(Pawn seeker, Thing mateThing)
		{
			Pawn mate = mateThing as Pawn;
			if (mate == null || mate.Downed || !mate.CanCasuallyInteractNow() || mate.IsForbidden(seeker) ||
			    mate.Faction != seeker.Faction || mate.Sterile() || mate.gender != Gender.None)
			{
				return false;
			}
			
			CompEggLayer comp = mate.TryGetComp<CompEggLayer>();
			return comp == null || !comp.FullyFertilized;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			// Seeker is assumed to have a faction.
			if (pawn.gender != Gender.None || pawn.Sterile())
			{
				return null;
			}

			CompEggLayer eggLayerComp = pawn.TryGetComp<CompEggLayer>();
			if (eggLayerComp != null && eggLayerComp.FullyFertilized)
			{
				return null;
			}
			
			Pawn targetA = (Pawn) GenClosest.ClosestThingReachable(
				pawn.Position, pawn.Map, ThingRequest.ForDef(pawn.def), PathEndMode.Touch,
				TraverseParms.For(pawn), 30f, mateThing => CanMate(pawn, mateThing));
			return targetA == null ? null : JobMaker.MakeJob(JobDefOf.Mate, (LocalTargetInfo) (Thing) targetA);
		}
	}
}