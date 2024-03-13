using BiomesCore.DefModExtensions;
using BiomesCore.MapGeneration;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Replaces the vanilla GenStep_RocksFromGrid implementation with a caverns specific one when needed.
	/// </summary>
	[HarmonyPatch(typeof(GenStep_RocksFromGrid), nameof(GenStep_RocksFromGrid.Generate))]
	public static class GenStep_RocksFromGrid_Generate_Patch
	{
		private static bool Prefix(Map map, GenStepParams parms)
		{
			var modExtension = map.Biome.GetModExtension<BiomesMap>();

			if (modExtension?.isCavern == true && !modExtension.cavernShapes.NullOrEmpty())
			{
				new GenStep_CavernRocksFromGrid().Generate(map, parms);
				return false;
			}

			return true;
		}
	}
}