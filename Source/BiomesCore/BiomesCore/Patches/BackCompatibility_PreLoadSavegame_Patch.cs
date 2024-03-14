using System.Collections.Generic;
using BiomesCore.BackCompatibilities;
using HarmonyLib;
using Verse;

namespace BiomesCore.Patches
{
	/// <summary>
	/// Inject BackCompatibilityConverter instances into vanilla code.
	/// </summary>
	[HarmonyPatch(typeof(Verse.BackCompatibility), nameof(Verse.BackCompatibility.PreLoadSavegame))]
	public static class BackCompatibility_PreLoadSavegame_Patch
	{
		public static void Prefix(List<BackCompatibilityConverter> ___conversionChain)
		{
			___conversionChain.Add(new BiomesBackCompatibilityConverter_1_4());
		}
	}
}