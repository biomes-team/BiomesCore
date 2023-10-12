using Verse;

namespace BiomesCore.ActiveTerrain
{
	/// <summary>
	/// Calculate hash tick offsets and intervals, for both cells and terrainComps.
	/// </summary>
	public static class ActiveTerrainHashExtension
	{
		public static int FastHashCode(this IntVec3 cell)
		{
			// cell.GetHashCode() includes cell.y in the hash, which is not really necessary.
			return Gen.HashCombineInt(Gen.HashCombineInt(0, cell.x), cell.z);
		}

		private static int HashOffsetTicks(this IntVec3 cell)
		{
			return Find.TickManager.TicksGame + cell.FastHashCode();
		}

		private static bool IsHashIntervalTick(this IntVec3 cell, int interval)
		{
			return HashOffsetTicks(cell) % interval == 0;
		}

		public static bool IsHashIntervalTick(this TerrainComp terrainComp, int interval)
		{
			return terrainComp.parent.Position.IsHashIntervalTick(interval);
		}
	}
}