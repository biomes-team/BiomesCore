using System;
using System.Collections.Generic;
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
		// Ensures that the new stats will always be shown at the end, before the mod information.
		private const int displayPriority = 89998;

		private class ByLabel : IComparer<Def>
		{
			public int Compare(Def x, Def y)
			{
				var result = string.Compare(x.label.ToLower(), y.label.ToLower(), StringComparison.Ordinal);
				if (result == 0)
				{
					result = x.defName.CompareTo(y.defName);
				}

				return result;
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
			// Calculate _animalsPerBiome. At this stage, _biomesPerAnimals is used to track which PawnKindDef races have
			// already been checked.
			foreach (var animalDef in DefDatabase<PawnKindDef>.AllDefsListForReading)
			{
				if (animalDef.race?.race == null || !animalDef.race.race.Animal ||
				    // Mods may have different PawnKindDefs sharing the same race ThingDef.
				    _biomesPerAnimal.ContainsKey(animalDef.race)
				   )
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

					// Keep track of already processed race ThingDefs. Commonalities are added to the dictionary later.
					if (!_biomesPerAnimal.ContainsKey(animalDef.race))
					{
						_biomesPerAnimal[animalDef.race] = new SortedDictionary<BiomeDef, float>(Comparer);
					}
				}
			}

			// Calculate the percentages relative to each biome.
			var animalsPerBiomeTemp =
				new Dictionary<BiomeDef, SortedDictionary<ThingDef, float>>();
			foreach (var entry in _animalsPerBiome)
			{
				animalsPerBiomeTemp[entry.Key] = ChanceToPercentage(entry.Value);
			}

			_animalsPerBiome = animalsPerBiomeTemp;

			// Copy the percentages relative to each biome into _biomesPerAnimal.
			foreach (var biomeEntry in _animalsPerBiome)
			{
				foreach (var animalEntry in biomeEntry.Value)
				{
					if (!_biomesPerAnimal.ContainsKey(animalEntry.Key))
					{
						Log.Error(
							$"BiomesCore.ExtraStatInfo error: cannot add [{animalEntry.Key.defName}, {biomeEntry.Key.defName}].");
					}

					_biomesPerAnimal[animalEntry.Key].Add(biomeEntry.Key, animalEntry.Value);
				}
			}
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
					"BC_SpawnPercentage".Translate(biomeEntry.Value.ToString("0.###")),
					biomeEntry.Key.description,
					displayPriority,
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

			// Some biomes such as Ocean may have no animals.
			if (!(__instance is BiomeDef biomeDef) || !_animalsPerBiome.ContainsKey(biomeDef))
			{
				yield break;
			}

			foreach (var animalEntry in _animalsPerBiome[biomeDef])
			{
				yield return new StatDrawEntry(
					StatCategories.BC_AnimalCommonality,
					animalEntry.Key.label.ToLower().CapitalizeFirst(),
					"BC_SpawnPercentage".Translate(animalEntry.Value.ToString("0.###")),
					animalEntry.Key.description,
					displayPriority,
					null,
					new List<Dialog_InfoCard.Hyperlink> {new Dialog_InfoCard.Hyperlink(animalEntry.Key)}
				);
			}
		}
	}
}