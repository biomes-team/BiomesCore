using Verse;

namespace BiomesCore.ThingComponents
{
	public class CompProperties_CompChunksOnKilled : CompProperties
	{
		public IntRange chunkCountRange = IntRange.one;

		public CompProperties_CompChunksOnKilled() => compClass = typeof(CompChunksOnKilled);
	}
	
	public class CompChunksOnKilled : ThingComp
	{
		public CompProperties_CompChunksOnKilled Props => (CompProperties_CompChunksOnKilled) props;

		public CompChunksOnKilled()
		{
		}
	}
}