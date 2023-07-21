using System.Linq;
using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{
	/// <summary>
	/// See ModExtension BiomesMap.disallowedWeathers.
	/// </summary>
	[HarmonyPatch(typeof(WeatherDecider), "CurrentWeatherCommonality")]
	public class WeatherDecider_CurrentWeatherCommonality_Patch
	{
		internal static bool Prefix(WeatherDef weather, Map ___map, ref float __result)
		{
			var disallowedWeathers = ___map.Biome.GetModExtension<BiomesMap>()?.disallowedWeathers?.ToHashSet();
			if (disallowedWeathers != null && disallowedWeathers.Contains(weather))
			{
				Log.Error($"WeatherDecider: {weather} CANCELLED");
				return false;
			}

			return true;
		}
	}
}