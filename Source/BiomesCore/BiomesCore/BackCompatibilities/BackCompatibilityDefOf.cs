using RimWorld;
using Verse;

namespace BiomesCore.BackCompatibilities
{
	[DefOf]
	public class BackCompatibilityDefOf
	{
		public static TerrainDef BMT_Chinampa_Granite;
		public static TerrainDef BMT_Chinampa_Marble;
		public static TerrainDef BMT_Chinampa_Sandstone;
		public static TerrainDef BMT_Chinampa_Slate;
		public static TerrainDef BMT_Lava;
		public static TerrainDef BMT_Magma;
		public static TerrainDef BMT_Pebbles;
		public static TerrainDef BMT_WaterAbyssalDeep;
		public static TerrainDef BMT_WaterShallowLagoon;

		[MayRequire("BiomesTeam.BiomesIslands")]
		public static TerrainDef BMT_CoralRock_Rough;
		[MayRequire("BiomesTeam.BiomesIslands")]
		public static TerrainDef BMT_CoralRock_RoughHewn;
		[MayRequire("BiomesTeam.BiomesIslands")]
		public static TerrainDef BMT_CoralRock_Smooth;
		[MayRequire("BiomesTeam.BiomesIslands")]
		public static TerrainDef BMT_Chinampa_Coral;
		[MayRequire("BiomesTeam.BiomesIslands")]
		public static TerrainDef BMT_FlagstoneCoral;
		[MayRequire("BiomesTeam.BiomesIslands")]
		public static TerrainDef BMT_FineTileCoral;
		[MayRequire("BiomesTeam.BiomesIslands")]
		public static TerrainDef BMT_TileCoral;

		static BackCompatibilityDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(BiomesCoreDefOf));
		}
	}
}