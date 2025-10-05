using System.Collections.Generic;
using Verse;

namespace BiomesCore
{
    /// <summary>
    /// Adds hediffs from the list when a pawn spawns and after a save-load.
    /// Created for animals from Polluted Lands
    /// </summary>
    public class CompProperties_InitialHediff : CompProperties
	{

		public List<HediffDef> hediffDefs;

		public CompProperties_InitialHediff()
		{
			compClass = typeof(CompInitialHediff);
		}
	}

	public class CompInitialHediff : ThingComp
	{

		private CompProperties_InitialHediff Props => (CompProperties_InitialHediff)props;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			AddHediff();
		}

		public void AddHediff()
		{
			Pawn pawn = parent as Pawn;
			foreach (HediffDef hediff in Props.hediffDefs)
			{
				if (!pawn.health.hediffSet.HasHediff(hediff))
				{
					pawn.health.AddHediff(hediff);
				}
			}
		}
	}

}
