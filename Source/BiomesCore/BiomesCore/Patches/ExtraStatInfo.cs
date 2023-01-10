using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BiomesCore.DefOfs;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{
	[HarmonyPatch]
	public static class ExtraStatInfo
	{
		private class ByLabel : IComparer<Def>
		{
			public int Compare(Def x, Def y)
			{
				return string.Compare(x.label.ToLower(), y.label.ToLower(), StringComparison.Ordinal);
			}
		}

		private static readonly ByLabel Comparer = new ByLabel();

		private static Dictionary<BiomeDef, SortedDictionary<ThingDef, float>> _animalsPerBiome =
			new Dictionary<BiomeDef, SortedDictionary<ThingDef, float>>();

		private static Dictionary<ThingDef, SortedDictionary<BiomeDef, float>> _biomesPerAnimal =
			new Dictionary<ThingDef, SortedDictionary<BiomeDef, float>>();

		private static SortedDictionary<T, float> ChanceToPercentage<T>(SortedDictionary<T, float> dict) where T : Def
		{
			var total = dict.Values.Sum();
			var newDict = new SortedDictionary<T, float>(Comparer);
			foreach (var entry in dict)
			{
				newDict.Add(entry.Key, 100.0f * entry.Value / total);
			}

			return newDict;
		}

		public static void Initialize()
		{
			foreach (var animalDef in DefDatabase<PawnKindDef>.AllDefsListForReading)
			{
				if (animalDef.race?.race == null || !animalDef.race.race.Animal)
				{
					continue;
				}

				foreach (var biomeDef in DefDatabase<BiomeDef>.AllDefsListForReading)
				{
					var commonality = Math.Max(biomeDef.CommonalityOfAnimal(animalDef),
						biomeDef.CommonalityOfPollutionAnimal(animalDef));
					if (commonality <= 0.0f)
					{
						continue;
					}

					if (!_animalsPerBiome.ContainsKey(biomeDef))
					{
						_animalsPerBiome[biomeDef] = new SortedDictionary<ThingDef, float>(Comparer);
					}

					_animalsPerBiome[biomeDef].Add(animalDef.race, commonality);

					if (!_biomesPerAnimal.ContainsKey(animalDef.race))
					{
						_biomesPerAnimal[animalDef.race] = new SortedDictionary<BiomeDef, float>(Comparer);
					}

					_biomesPerAnimal[animalDef.race].Add(biomeDef, commonality);
				}
			}

			var animalsPerBiomeTemp =
				new Dictionary<BiomeDef, SortedDictionary<ThingDef, float>>();
			foreach (var entry in _animalsPerBiome)
			{
				animalsPerBiomeTemp[entry.Key] = ChanceToPercentage(entry.Value);
			}

			_animalsPerBiome = animalsPerBiomeTemp;

			var biomesPerAnimalTemp =
				new Dictionary<ThingDef, SortedDictionary<BiomeDef, float>>();
			foreach (var entry in _biomesPerAnimal)
			{
				biomesPerAnimalTemp[entry.Key] = ChanceToPercentage(entry.Value);
			}

			_biomesPerAnimal = biomesPerAnimalTemp;
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(RaceProperties), "SpecialDisplayStats")]
		internal static IEnumerable<StatDrawEntry> Animals(IEnumerable<StatDrawEntry> __result, ThingDef parentDef)
		{
			foreach (var entry in __result)
			{
				yield return entry;
			}

			// Prevent humans and animals which cannot spawn normally such as thrumbos from showing biome info.
			if (!parentDef.race.Animal || !_biomesPerAnimal.ContainsKey(parentDef))
			{
				yield break;
			}

			foreach (var biomeEntry in _biomesPerAnimal[parentDef])
			{
				yield return new StatDrawEntry(
					StatCategories.BC_BiomeCommonality,
					biomeEntry.Key.label.ToLower().CapitalizeFirst(),
					"BC_SpawnPercentage".Translate(biomeEntry.Value.ToString("0.##")),
					biomeEntry.Key.description,
					89998,
					null,
					new List<Dialog_InfoCard.Hyperlink> {new Dialog_InfoCard.Hyperlink(biomeEntry.Key)}
				);
			}
		}


		[HarmonyPostfix]
		[HarmonyPatch(typeof(Def), "SpecialDisplayStats")]
		internal static IEnumerable<StatDrawEntry> Biomes(IEnumerable<StatDrawEntry> __result, Def __instance)
		{
			foreach (var entry in __result)
			{
				yield return entry;
			}

			if (!(__instance is BiomeDef biomeDef))
			{
				yield break;
			}

			foreach (var animalEntry in _animalsPerBiome[biomeDef])
			{
				yield return new StatDrawEntry(
					StatCategories.BC_AnimalCommonality,
					animalEntry.Key.label.ToLower().CapitalizeFirst(),
					"BC_SpawnPercentage".Translate(animalEntry.Value.ToString("0.##")),
					animalEntry.Key.description,
					89998,
					null,
					new List<Dialog_InfoCard.Hyperlink> {new Dialog_InfoCard.Hyperlink(animalEntry.Key)}
				);
			}
		}
	}
}