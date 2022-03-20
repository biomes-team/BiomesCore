using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
