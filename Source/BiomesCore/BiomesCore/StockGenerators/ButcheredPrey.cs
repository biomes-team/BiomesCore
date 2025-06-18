using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
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
		/// Butchery rules are not straightforward. These products are cached only once.
		/// </summary>
		private static HashSet<ThingDef> _huntingProducts;

		/// <summary>
		/// Favors animals common in the current biome, with low wildness.
		/// </summary>
		/// <param name="kind">Animal being checked.</param>
		/// <param name="biome">Current biome.</param>
		/// <returns>Random weight for the current animal.</returns>
		private static float SelectionChance(PawnKindDef kind, BiomeDef biome)
		{
			//float fromWildness = Util.SelectionChanceFromWildnessCurve.Evaluate(kind.RaceProps.wildness);
            float fromWildness = Util.SelectionChanceFromWildnessCurve.Evaluate(kind.race.GetStatValueAbstract(StatDefOf.Wildness));
            return 100.0f * biome.CommonalityOfAnimal(kind) + fromWildness;
		}

		public override IEnumerable<Thing> GenerateThings(PlanetTile forTile, Faction faction = null)
		{
			// Get current biome.
			if (forTile == -1)
			{
				yield break;
			}

			BiomeDef biome = Find.World.grid.Surface[forTile].PrimaryBiome;
			List<PawnKindDef> biomeAnimalPawnKindDefs = biome.AllWildAnimals
				.Where(def => def.RaceProps.Animal && wildnessRange.Includes(def.race.GetStatValueAbstract(StatDefOf.Wildness)) &&
				              (!checkTemperature || Util.AcceptableTemperature(def, forTile))).ToList();

			int kindCount = Math.Min(kindCountRange.RandomInRange, biomeAnimalPawnKindDefs.Count);
			if (kindCount <= 0)
			{
				yield break;
			}

			var pawnKindDefs = new HashSet<PawnKindDef>();
			while (pawnKindDefs.Count < kindCount)
			{
				biomeAnimalPawnKindDefs.TryRandomElementByWeight(def => SelectionChance(def, biome), out var pawnKindDef);
				pawnKindDefs.Add(pawnKindDef);
			}

			foreach (var pawnKindDef in pawnKindDefs)
			{
				// Simulate butchering n animals
				int animalCount = RandomCountOf(pawnKindDef.race);
				ThingDef meatDef = pawnKindDef.RaceProps.meatDef;
				if (meatDef != null && meatDef.tradeability.TraderCanSell())
				{
					var meatAmount = AnimalProductionUtility.AdultMeatAmount(pawnKindDef.race) * animalCount *
					                 butcheringEfficiency;
					foreach (var thing in StockGeneratorUtility.TryMakeForStock(meatDef, (int) meatAmount, faction))
					{
						yield return thing;
					}
				}

				ThingDef leatherDef = pawnKindDef.RaceProps.leatherDef;
				if (leatherDef != null && leatherDef.tradeability.TraderCanSell())
				{
					float leatherAmount = AnimalProductionUtility.AdultLeatherAmount(pawnKindDef.race) * animalCount *
					                      butcheringEfficiency;
					foreach (Thing thing in StockGeneratorUtility.TryMakeForStock(leatherDef, (int) leatherAmount, faction))
					{
						yield return thing;
					}
				}

				if (pawnKindDef.race.butcherProducts != null)
				{
					foreach (ThingDefCountClass butcherProduct in pawnKindDef.race.butcherProducts)
					{
						if (!butcherProduct.thingDef.tradeability.TraderCanSell())
						{
							continue;
						}

						float count = butcherProduct.count * butcheringEfficiency;
						foreach (var thing in StockGeneratorUtility.TryMakeForStock(butcherProduct.thingDef, (int) count, faction))
						{
							yield return thing;
						}
					}
				}

				// Assume adult individuals.
				PawnKindLifeStage lifeStage = pawnKindDef.lifeStages.Count == 0 ? null : pawnKindDef.lifeStages.Last();
				if (lifeStage?.butcherBodyPart != null &&
				    lifeStage.butcherBodyPart.thing.tradeability.TraderCanSell())
				{
					// Random gender.
					var gender = pawnKindDef.fixedGender ?? (pawnKindDef.RaceProps.hasGenders
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

		private static void PopulateHuntingProductsCache()
		{
			if (_huntingProducts != null)
			{
				return;
			}

			_huntingProducts = new HashSet<ThingDef>();

			foreach (PawnKindDef pawnKindDef in DefDatabase<PawnKindDef>.AllDefsListForReading)
			{
				if (!pawnKindDef.RaceProps.Animal)
				{
					continue;
				}

				ThingDef meatThingDef = pawnKindDef.RaceProps.meatDef;
				if (meatThingDef != null && meatThingDef.tradeability.TraderCanSell())
				{
					_huntingProducts.Add(meatThingDef);
				}

				ThingDef leatherThingDef = pawnKindDef.RaceProps.leatherDef;
				if (leatherThingDef != null && leatherThingDef.tradeability.TraderCanSell())
				{
					_huntingProducts.Add(leatherThingDef);
				}

				if (pawnKindDef.race.butcherProducts != null)
				{
					foreach (ThingDefCountClass butcherProduct in pawnKindDef.race.butcherProducts)
					{
						if (butcherProduct.thingDef.tradeability.TraderCanSell())
						{
							_huntingProducts.Add(butcherProduct.thingDef);
						}
					}
				}

				// Assume adult individuals.
				PawnKindLifeStage lifeStage = pawnKindDef.lifeStages.Count == 0 ? null : pawnKindDef.lifeStages.Last();
				if (lifeStage?.butcherBodyPart != null &&
				    lifeStage.butcherBodyPart.thing.tradeability.TraderCanSell())
				{
					_huntingProducts.Add(lifeStage.butcherBodyPart.thing);
				}
			}
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			PopulateHuntingProductsCache();
			return thingDef.tradeability != Tradeability.None &&
			       (Util.ExoticAnimalPart(thingDef) || _huntingProducts.Contains(thingDef));
		}
	}
}