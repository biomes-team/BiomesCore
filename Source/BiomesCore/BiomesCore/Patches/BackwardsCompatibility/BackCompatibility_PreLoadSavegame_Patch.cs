using System.Collections.Generic;
using BiomesCore.BackCompatibilities;
using HarmonyLib;
using Verse;

namespace BiomesCore.Patches.BackwardsCompatibility
{
	/// <summary>
	/// Inject BackCompatibilityConverter instances into vanilla code.
	/// </summary>
	[HarmonyPatch(typeof(BackCompatibility), nameof(BackCompatibility.PreLoadSavegame))]
	public static class BackCompatibility_PreLoadSavegame_Patch
	{
		public static void Prefix(List<BackCompatibilityConverter> ___conversionChain)
		{
			___conversionChain.Add(new BiomesBackCompatibilityConverter_1_4());
		}
	}
}