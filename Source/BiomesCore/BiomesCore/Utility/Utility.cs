using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCore
{
	public static class Utility
	{

		public static bool IsVanillaDef(this Def def)
		{
			return def.modContentPack != null && def.modContentPack.IsOfficialMod;
		}

	}

}
