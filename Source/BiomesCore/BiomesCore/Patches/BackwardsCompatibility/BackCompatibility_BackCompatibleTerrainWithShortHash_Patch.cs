using BiomesCore.BackCompatibilities;
using HarmonyLib;
using Verse;

namespace BiomesCore.Patches.BackwardsCompatibility
{
	/// <summary>
	/// Inject backwards compatibility information for loading TerrainDefs through their short hash.
	/// </summary>
	[HarmonyPatch(typeof(BackCompatibility), nameof(BackCompatibility.BackCompatibleTerrainWithShortHash))]
	public class BackCompatibility_BackCompatibleTerrainWithShortHash_Patch
	{
		public static void Postfix(ushort hash, ref TerrainDef __result)
		{
			__result ??= BiomesBackCompatibilityConverter_1_4.BackCompatibleTerrainShortHash(hash);
		}
	}
}