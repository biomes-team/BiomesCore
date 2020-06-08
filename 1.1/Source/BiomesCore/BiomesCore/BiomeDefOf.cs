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

		public static BiomeDef BiomesValleys_TemperateForestValley;

		static BiomesCore_BiomeDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(BiomesCore_BiomeDefOf));
		}
	}
}
