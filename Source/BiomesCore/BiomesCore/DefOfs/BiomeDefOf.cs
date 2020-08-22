using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiomesCore
{
	[DefOf]
	public static class BiomesCore_BiomeDefOf
	{
		[MayRequire("BiomesTeam.BiomesIslands")]
		public static BiomeDef BiomesIslands_Atoll;
		[MayRequire("BiomesTeam.BiomesIslands")]
		public static BiomeDef BiomesIslands_TropicalIsland;
		[MayRequire("BiomesTeam.ExtraIslands")]
		public static BiomeDef BiomesIslands_TemperateIsland;
		[MayRequire("BiomesTeam.BiomesIslands")]
		public static BiomeDef BiomesIslands_BorealIsland;
		[MayRequire("BiomesTeam.BiomesIslands")]
		public static BiomeDef BiomesIslands_TundraIsland;
		[MayRequire("BiomesTeam.BiomesIslands")]
		public static BiomeDef BiomesIslands_DesertIsland;
		[MayRequire("BiomesTeam.BiomesTeam.Oasis")]
		public static BiomeDef BiomesIslands_Oasis;
		[MayRequire("BiomesTeam.MinorValleys")]
		public static BiomeDef BiomesValleys_TemperateForestValley;
		[MayRequire("BiomesTeam.MinorValleys")]
		public static BiomeDef BiomesValleys_TemperateForestCanyon;
		[MayRequire("BiomesTeam.MinorValleys")]
		public static BiomeDef BiomesValleys_TemperateSwampValley;

		static BiomesCore_BiomeDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(BiomesCore_BiomeDefOf));
		}
	}
}
