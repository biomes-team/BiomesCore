using Verse;

namespace BiomesCore.DefModExtensions
{
	public class DropChunksWhenDestroyed : DefModExtension
	{
		public IntRange chunkCountRange = new IntRange(1, 1);
	}
}