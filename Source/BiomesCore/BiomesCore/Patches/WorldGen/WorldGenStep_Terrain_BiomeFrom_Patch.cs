using BiomesCore.Planet;
using HarmonyLib;
using RimWorld.Planet;

namespace BiomesCore.Patches.WorldGen
{
	/// <summary>
	/// Pre-generate data for each tile of the world map.
	/// Biomes are generated at the end of this method so our calculations must be done before that.
	/// </summary>
	[HarmonyPatch(typeof(WorldGenStep_Terrain), "BiomeFrom")]
	public class WorldGenStep_Terrain_BiomeFrom_Patch
	{
		public static void Prefix(Tile ws, PlanetTile tile, PlanetLayer layer)
		{
			WorldGenInfoHandler.GenerateTileFor(tile, layer);
		}
	}
}