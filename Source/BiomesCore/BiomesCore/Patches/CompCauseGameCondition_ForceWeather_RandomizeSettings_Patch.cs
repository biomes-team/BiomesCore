using System.Linq;
using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BiomesCore.Patches
{
	/// <summary>
	/// See ModExtension BiomesMap.disallowedWeathers.
	/// </summary>
	[HarmonyPatch(typeof(CompCauseGameCondition_ForceWeather),
		nameof(CompCauseGameCondition_ForceWeather.RandomizeSettings))]
	internal static class CompCauseGameCondition_ForceWeather_RandomizeSettings_Patch
	{
		internal static void Postfix(Site site, ref WeatherDef ___weather)
		{
			var disallowedWeathers = site.Map?.Biome.GetModExtension<BiomesMap>()?.disallowedWeathers?.ToHashSet();
			if (disallowedWeathers != null && disallowedWeathers.Contains(___weather))
			{
				
				___weather = DefDatabase<WeatherDef>.AllDefsListForReading
					.Where(def => def.isBad && !disallowedWeathers.Contains(def)).RandomElement();
			}
		}
	}
}