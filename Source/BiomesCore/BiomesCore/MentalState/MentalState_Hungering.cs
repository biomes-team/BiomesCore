using RimWorld;
using Verse;

namespace BiomesCore.MentalState
{
	public class MentalState_Hungering : Verse.AI.MentalState
	{

		public override void PostStart(string reason)
		{
			base.PostStart(reason);
			var food = pawn.needs.food;
			if (food != null)
			{
				food.CurLevelPercentage = 0.1F;
				
				LessonAutoActivator.TeachOpportunity(BiomesCoreDefOf.BMT_HungeringAnimalsConcept, OpportunityType.Critical);

			}
			
			
		}

		public override void PostEnd()
		{
			base.PostEnd();
			var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(BiomesCoreDefOf.BMT_HungeringHediff);
			if (hediff != null)
			{
				pawn.health.RemoveHediff(hediff);
			}
		}

		public override RandomSocialMode SocialModeMax() => RandomSocialMode.Off;

		public override void MentalStateTick()
		{
			base.MentalStateTick();
			if (!pawn.IsHashIntervalTick(33))
			{
				return;
			}

			// Taming a hungering creature or getting its food bar filled removes this mental state.
			if (pawn.Faction == Faction.OfPlayer || pawn.needs.food?.CurLevelPercentage >= 1.0F)
			{
				RecoverFromState();
			}
		}
	}
}