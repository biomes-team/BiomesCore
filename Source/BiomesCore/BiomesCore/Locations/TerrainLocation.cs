using Verse;

namespace BiomesCore.Locations
{
	/// <summary>
	/// Used for terrains that can trigger features in their location.
	/// </summary>
	public abstract class TerrainLocation : Location
	{
		public void Initialize(TerrainLocationDefExtension extension, Map map, IntVec3 position)
		{
			Map = map;
			Position = position;
			OnInitialize(extension);
		}

		protected abstract void OnInitialize(TerrainLocationDefExtension extension);

		/// <summary>
		/// Called when the terrain is set on a cell. This can happen during the game, after map generation, or after load.
		/// </summary>
		/// <param name="grid">Reference to the location grid.</param>
		public abstract void TerrainRegistered(LocationGrid grid);

		/// <summary>
		/// Called when the terrain is removed from a cell as its terrain type changes.
		/// </summary>
		/// <param name="grid">Reference to the location grid.</param>
		public abstract void TerrainRemoved(LocationGrid grid);
	}
}