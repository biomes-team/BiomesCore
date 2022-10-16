using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BiomesCore.StockGenerators
{
	/// <summary>
	/// Generates meat and leather of prey that hunters may have taken in the current biome.
	/// countRange/totalPriceRange will be used to calculate the number of prey to "butcher".
	/// </summary>
	public class ButcheredPrey : StockGenerator
	{
		/// <summary>
		/// Total number of animal types to choose.
		/// </summary>
		private IntRange kindCountRange = new IntRange(1, 1);

		/// <summary>
		/// Choose only animals with a wildness value between this interval.
		/// </summary>
		private FloatRange wildnessRange = new FloatRange(0.3f, 0.80f);

		/// <summary>
		/// Consider only animals which could survive the current temperatures.
		/// </summary>
		private bool checkTemperature = true;

		/// <summary>
		/// Multiplier to apply to all butchered products.
		/// </summary>
		private float butcheringEfficiency = 0.75f;

		/// <summary>
		/// Obtaining all special butchered products is costly. This cache stores them so they are only calculated once.
		/// </summary>
		private static HashSet<ThingDef> _butcheredProducts;

		/// <summary>
		/// Favors animals common in the current biome, with low wildness.
		/// </summary>
		/// <param name="kind">Animal being checked.</param>
		/// <param name="biome">Current biome.</param>
		/// <returns>Random weight for the current animal.</returns>
		private static float SelectionChance(PawnKindDef kind, BiomeDef biome)
		{
			var fromWildness = Util.SelectionChanceFromWildnessCurve.Evaluate(kind.RaceProps.wildness);
			return 100.0f * biome.CommonalityOfAnimal(kind) + fromWildness;
		}

		public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
		{
			// Get current biome.
			if (forTile == -1)
			{
				yield break;
			}

			var biome = Find.World.grid.tiles[forTile].biome;
			var biomeAnimals = biome.AllWildAnimals
				.Where(def => def.RaceProps.Animal && wildnessRange.Includes(def.RaceProps.wildness) &&
				              (!checkTemperature || Util.AcceptableTemperature(def, forTile))).ToList();

			var kindCount = Math.Min(kindCountRange.RandomInRange, biomeAnimals.Count);
			if (kindCount <= 0)
			{
				yield break;
			}

			var chosenKinds = new HashSet<PawnKindDef>();
			while (chosenKinds.Count < kindCount)
			{
				biomeAnimals.TryRandomElementByWeight(def => SelectionChance(def, biome), out var chosenKind);
				chosenKinds.Add(chosenKind);
			}

			foreach (var chosenKind in chosenKinds)
			{
				// Simulate butchering n animals
				var animalCount = RandomCountOf(chosenKind.race);
				var meatDef = chosenKind.RaceProps.meatDef;
				if (meatDef != null && meatDef.tradeability.TraderCanSell())
				{
					var meatAmount = AnimalProductionUtility.AdultMeatAmount(chosenKind.race) * animalCount *
					                 butcheringEfficiency;
					foreach (var thing in StockGeneratorUtility.TryMakeForStock(meatDef, (int) meatAmount, faction))
					{
						yield return thing;
					}
				}

				var leatherDef = chosenKind.RaceProps.leatherDef;
				if (leatherDef != null && leatherDef.tradeability.TraderCanSell())
				{
					var leatherAmount = AnimalProductionUtility.AdultLeatherAmount(chosenKind.race) * animalCount *
					                    butcheringEfficiency;
					foreach (var thing in StockGeneratorUtility.TryMakeForStock(leatherDef, (int) leatherAmount, faction))
					{
						yield return thing;
					}
				}

				if (chosenKind.race.butcherProducts != null)
				{
					foreach (var butcherProduct in chosenKind.race.butcherProducts)
					{
						if (!butcherProduct.thingDef.tradeability.TraderCanSell()) continue;
						var count = butcherProduct.count * butcheringEfficiency;
						foreach (var thing in StockGeneratorUtility.TryMakeForStock(butcherProduct.thingDef, (int) count, faction))
						{
							yield return thing;
						}
					}
				}

				// Assume adult individuals.
				var lifeStage = chosenKind.lifeStages.Count == 0 ? null : chosenKind.lifeStages.Last();
				if (lifeStage?.butcherBodyPart != null &&
				    lifeStage.butcherBodyPart.thing.tradeability.TraderCanSell())
				{
					// Random gender.
					var gender = chosenKind.fixedGender ?? (chosenKind.RaceProps.hasGenders
						? Rand.Value >= 0.5 ? Gender.Female : Gender.Male
						: Gender.None);
					if (gender == Gender.None ||
					    gender == Gender.Male && lifeStage.butcherBodyPart.allowMale ||
					    gender == Gender.Female && lifeStage.butcherBodyPart.allowFemale)
					{
						foreach (var thing in StockGeneratorUtility.TryMakeForStock(lifeStage.butcherBodyPart.thing, 1, faction))
						{
							yield return thing;
						}
					}
				}
			}
		}

		private static void PopulateButcheredProductsCache()
		{
			if (_butcheredProducts != null)
			{
				return;
			}

			_butcheredProducts = new HashSet<ThingDef>();

			var animals = DefDatabase<PawnKindDef>.AllDefs
				.Where(def => def.RaceProps.Animal && def.race.butcherProducts != null)
				.ToList();
			foreach (var animal in animals)
			{
				foreach (var butcherProduct in animal.race.butcherProducts)
				{
					if (butcherProduct.thingDef.tradeability.TraderCanSell())
					{
						_butcheredProducts.Add(butcherProduct.thingDef);
					}
				}
			}
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			PopulateButcheredProductsCache();
			return thingDef.tradeability != Tradeability.None &&
			       (thingDef.IsLeather || thingDef.IsMeat || Util.ExoticAnimalPart(thingDef) ||
			        _butcheredProducts.Contains(thingDef));
		}
	}
}