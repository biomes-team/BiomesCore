using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{
	/// <summary>
	/// See ModExtension BiomesMap.disallowedWeathers.
	/// </summary>
	[HarmonyPatch(typeof(WeatherDecider), nameof(WeatherDecider.WeatherCommonalities), MethodType.Getter)]
	public static class WeatherDecider_WeatherCommonalities_Patch
	{
		public static IEnumerable<WeatherCommonalityRecord> Postfix(IEnumerable<WeatherCommonalityRecord> values,
			Map ___map)
		{
			HashSet<WeatherDef> disallowedWeathers =
				___map.Biome.GetModExtension<BiomesMap>()?.disallowedWeathers?.ToHashSet();

			foreach (WeatherCommonalityRecord record in values)
			{
				if (disallowedWeathers == null || !disallowedWeathers.Contains(record.weather))
				{
					yield return record;
				}
			}
		}
	}
}