using RimWorld;
using Verse;

namespace BiomesCore
{
	[DefOf]
	public static class BiomesCore_PawnKindDefOf
	{
		[MayRequire("BiomesTeam.BiomesIslands")]
		public static PawnKindDef BiomesIslands_BlueSeaSnail;

		static BiomesCore_PawnKindDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(BiomesCore_BiomeDefOf));
		}
	}
}
