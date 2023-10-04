using Verse;

namespace BiomesCore
{
	public static class HashExtension
	{

		public static int HashOffsetTicks(this IntVec3 cell)
		{
			return Find.TickManager.TicksGame + cell.GetHashCode();
		}

		public static bool IsHashIntervalTick(this IntVec3 cell, int interval)
		{
			return HashOffsetTicks(cell) % interval == 0;
		}

		public static bool IsHashIntervalTick(this TerrainComp terrainComp, int interval)
		{
			return terrainComp.parent.Position.IsHashIntervalTick(interval);
		}
		
		public static bool IsHashIntervalTick(this ThingComp thingComp, int interval)
		{
			return thingComp.parent.IsHashIntervalTick(interval);
		}

	}
}