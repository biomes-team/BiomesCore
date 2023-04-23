using Verse;

namespace BiomesCore.ThingComponents
{
	public class CompProperties_CompTerrainRecolor : CompProperties
	{
		public CompProperties_CompTerrainRecolor() => compClass = typeof(CompTerrainRecolor);
	}

	public class CompTerrainRecolor : ThingComp
	{
		private CompProperties_CompTerrainRecolor Props => (CompProperties_CompTerrainRecolor) props;

		public CompTerrainRecolor()
		{
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			var colorable = parent.GetComp<CompColorable>();
			colorable?.SetColor(parent.Position.GetTerrain(parent.Map).color);
		}
	}
}