using Verse;

namespace BiomesCore.Locations
{
	/// <summary>
	/// A location attaches certain features to a specific cell on the map, without requiring to use a Thing.
	/// Locations must be stateless, and do not persist in savegames. They must be regenerated on load instead.
	/// Locations are currently only used to implement features for terrains.
	///
	/// This system is based on the active terrain library, originally found here:
	/// https://github.com/RimWorld-CCL-Reborn/ActiveTerrain
	/// The license can be found in this source folder.
	/// </summary>
	public abstract class Location
	{
		protected Map Map;

		protected IntVec3 Position;

		public Location()
		{
		}

		public abstract void Tick(int gameTick);

		public static int GetUniqueID(Map map, IntVec3 position)
		{
			// Ignores Position.y as it is not really necessary.
			return map.uniqueID << 29 | position.x << 13 | position.z;
		}

		public int GetUniqueID()
		{
			return GetUniqueID(Map, Position);
		}
	}
}