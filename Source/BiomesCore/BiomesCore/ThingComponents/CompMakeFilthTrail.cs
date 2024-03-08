using RimWorld;
using Verse;

namespace BiomesCore
{
	public class CompMakeFilthTrail : ThingComp
	{
		public ThingDef filthDef;

		private IntVec3 lastPos = IntVec3.Invalid;

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			filthDef = ((CompProperties_MakeFilthTrail) props).filthDef;
		}

		public override void CompTickRare()
		{
			if (parent.Spawned && parent.Position != lastPos)
			{
				lastPos = parent.Position;
				FilthMaker.TryMakeFilth(lastPos, parent.Map, filthDef);
			}
		}
	}

	public class CompProperties_MakeFilthTrail : CompProperties
	{
		public ThingDef filthDef;

		public CompProperties_MakeFilthTrail() => compClass = typeof(CompMakeFilthTrail);
	}
}