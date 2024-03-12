using RimWorld;
using RimWorld.Planet;

namespace BiomesCore.WorldMap
{
	/// <summary>
	/// These utility functions replicate some parts of the vanilla BiomeWorker scoring system. They can be used to
	/// determine where vanilla would place its biomes. That information can be used in the Biomes! BiomeWorker
	/// implementations to place biomes on top of vanilla biomes, to choose the appropriate biome for islands and so on.
	/// </summary>
	public static class BiomeWorkerUtil
	{
		/// <summary>
		/// Returns true if an ocean tile can be permanently covered by ice.
		/// </summary>
		/// <param name="tile">Tile to check. Assumed to be covered by water.</param>
		/// <returns>True if this ocean tile could be a sea ice biome.</returns>
		public static bool IsPermanentIce(Tile tile)
		{
			// See BiomeWorker_IceSheet and BiomeWorker_SeaIce for details.
			return BiomeWorker_IceSheet.PermaIceScore(tile) >= 22.0F;
		}

		/// <summary>
		/// Determines where to spawn boreal islands. Uses the same scoring as RimWorld.Planet.BiomeWorker_BorealForest.
		/// </summary>
		/// <param name="tile">Tile to check.</param>
		/// <returns>Boreal island / archipelago score.</returns>
		public static float BorealScore(Tile tile)
		{
			return tile.temperature < -10.0F || tile.rainfall < 600.0F ? 0.0F : 15F;
		}

		/// <summary>
		/// Determines where to spawn desert islands. Uses the same scoring as RimWorld.Planet.BiomeWorker_Desert.
		/// </summary>
		/// <param name="tile">Tile to check.</param>
		/// <returns>Desert island / archipelago score.</returns>
		public static float DesertScore(Tile tile)
		{
			return tile.rainfall >= 600.0F ? 0.0F : tile.temperature + 0.0001F;
		}

		/// <summary>
		/// Determines where to spawn temperate islands. Uses the same scoring as RimWorld.Planet.BiomeWorker_Temperate.
		/// </summary>
		/// <param name="tile">Tile to check.</param>
		/// <returns>Temperate island / archipelago score.</returns>
		public static float TemperateScore(Tile tile)
		{
			return tile.temperature < -10.0F || tile.rainfall < 600.0F
				? 0.0F
				: (15.0F + (tile.temperature - 7.0F) + (tile.rainfall - 600.0F) / 180.0F);
		}


		/// <summary>
		/// Determines where to spawn tropical islands. Uses the same scoring as RimWorld.Planet.BiomeWorker_Tropical.
		/// </summary>
		/// <param name="tile">Tile to check.</param>
		/// <returns>Tropical island / archipelago score.</returns>
		public static float TropicalScore(Tile tile)
		{
			return tile.temperature < 15.0F || tile.rainfall < 2000.0F
				? 0.0F
				: (28.0F + (tile.temperature - 20.0F) * 1.5F + (tile.rainfall - 600.0F) / 165.0F);
		}

		/// <summary>
		/// Determines where to spawn tundra islands. Uses the same scoring as RimWorld.Planet.BiomeWorker_Tundra.
		/// </summary>
		/// <param name="tile">Tile to check.</param>
		/// <returns>Tundra island / archipelago score.</returns>
		public static float TundraScore(Tile tile)
		{
			return -tile.temperature;
		}
	}
}