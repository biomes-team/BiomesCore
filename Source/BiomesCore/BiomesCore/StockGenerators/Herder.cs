using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BiomesCore.StockGenerators
{
	internal class AnimalProductSet
	{
		public readonly HashSet<ThingDef> Products = new HashSet<ThingDef>();
		public ThingDef EggFertilizedDef;
	}

	/// <summary>
	/// Generates randomly chosen animals, along with leather and animal products related to them.
	/// Will always try to generate at least one male and one female animal of each kind.
	/// </summary>
	public class Herder : StockGenerator
	{
		/// <summary>
		/// Total number of animals types to be chosen. countRange/totalPriceRange animals of each type will be in stock.
		/// </summary>
		private IntRange kindCountRange = new IntRange(1, 1);

		/// <summary>
		/// Chosen animal types must have one of these tradeTags. Will also purchase animals with the same tags.
		/// </summary>
		private List<string> tradeTags = new List<string>();

		/// <summary>
		/// The wildness of the chosen animal types must be in this range.
		/// </summary>
		private FloatRange wildnessRange = new FloatRange(0.0f, 0.70f);

		/// <summary>
		/// If enabled, the chosen animals must be able to survive the current temperatures.
		/// </summary>
		private bool checkTemperature = true;

		/// <summary>
		/// Generates this amount of leather of each chosen animal.
		/// </summary>
		private FloatRange leatherPriceRange = FloatRange.Zero;

		/// <summary>
		/// Generates this amount of each animal product produced by each chosen animal.
		/// Wool is considered an animal product for this value.
		/// </summary>
		private FloatRange animalProductPriceRange = FloatRange.Zero;

		/// <summary>
		/// Generates this amount of fertilized eggs for any chosen animal that lays eggs.
		/// </summary>
		private IntRange fertilizedEggCountRange = new IntRange(0, 0);

		public override IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			foreach (var err in base.ConfigErrors(parentDef))
			{
				yield return err;
			}

			if (tradeTags.Count == 0)
			{
				yield return "StockGenerator_Herder: tradeTags must not be empty.";
			}
		}

		private bool AcceptablePawnKindDef(PawnKindDef def, int forTile, Faction faction = null)
		{
			return def.RaceProps.Animal && def.race.tradeTags != null &&
			       tradeTags.Any(tag => def.race.tradeTags.Contains(tag)) && wildnessRange.Includes(def.race.GetStatValueAbstract(StatDefOf.Wildness));
		}


		private static float SelectionChance(PawnKindDef kind) =>
			Util.SelectionChanceFromWildnessCurve.Evaluate(kind.race.GetStatValueAbstract(StatDefOf.Wildness));

		private static AnimalProductSet AnimalProducts(PawnKindDef pawnKind)
		{
			var set = new AnimalProductSet();

			foreach (var comp in pawnKind.race.comps)
			{
				switch (comp)
				{
					case CompProperties_Shearable shearableComp:
						set.Products.Add(shearableComp.woolDef);
						break;
					case CompProperties_Milkable milkableComp:
						set.Products.Add(milkableComp.milkDef);
						break;
					case CompProperties_EggLayer eggComp:
						set.Products.Add(eggComp.eggUnfertilizedDef);
						set.EggFertilizedDef = eggComp.eggFertilizedDef;
						break;
				}
			}

			set.Products.RemoveWhere(def =>
				def == null || !def.tradeability.TraderCanSell() ||
				def.tradeTags != null && def.tradeTags.Contains("ExoticMisc"));

			return set;
		}

		public override IEnumerable<Thing> GenerateThings(PlanetTile forTile, Faction faction = null)
		{
			var acceptableKinds =
				DefDatabase<PawnKindDef>.AllDefsListForReading.Where(kind =>
						AcceptablePawnKindDef(kind, forTile, faction) &&
						(!checkTemperature || Util.AcceptableTemperature(kind, forTile)))
					.ToList();

			var kindCount = Math.Min(kindCountRange.RandomInRange, acceptableKinds.Count);
			if (kindCount <= 0)
			{
				yield break;
			}

			var chosenKinds = new HashSet<PawnKindDef>();
			while (chosenKinds.Count < kindCount)
			{
				acceptableKinds.TryRandomElementByWeight(SelectionChance, out var chosenKind);
				chosenKinds.Add(chosenKind);
			}

			foreach (var chosenKind in chosenKinds)
			{
				// Animal generation.
				var animalCount = RandomCountOf(chosenKind.race);
				if (chosenKind.race.race.hasGenders && animalCount >= 2)
				{
					yield return PawnGenerator.GeneratePawn(new PawnGenerationRequest(chosenKind, tile: forTile,
						fixedGender: Gender.Female));
					yield return PawnGenerator.GeneratePawn(new PawnGenerationRequest(chosenKind, tile: forTile,
						fixedGender: Gender.Male));
					animalCount -= 2;
				}

				while (animalCount > 0)
				{
					yield return PawnGenerator.GeneratePawn(new PawnGenerationRequest(chosenKind, tile: forTile));
					--animalCount;
				}

				// Leather generation.
				var leatherDef = chosenKind.RaceProps.leatherDef;

				if (leatherPriceRange != FloatRange.Zero && leatherDef != null && leatherDef.tradeability.TraderCanSell())
				{
					var productCount = Convert.ToInt32(leatherPriceRange.RandomInRange / leatherDef.BaseMarketValue);
					foreach (var thing in StockGeneratorUtility.TryMakeForStock(leatherDef, productCount, faction))
					{
						yield return thing;
					}
				}

				// Animal product generation.
				var set = AnimalProducts(chosenKind);
				if (animalProductPriceRange != FloatRange.Zero)
				{
					foreach (var animalProduct in set.Products)
					{
						if (!animalProduct.tradeability.TraderCanSell())
						{
							continue;
						}

						var count = Convert.ToInt32(animalProductPriceRange.RandomInRange / animalProduct.BaseMarketValue);
						foreach (var thing in StockGeneratorUtility.TryMakeForStock(animalProduct, count, faction))
						{
							yield return thing;
						}
					}
				}

				if (set.EggFertilizedDef != null && set.EggFertilizedDef.tradeability.TraderCanSell())
				{
					var count = fertilizedEggCountRange.RandomInRange;
					foreach (var thing in StockGeneratorUtility.TryMakeForStock(set.EggFertilizedDef, count, faction))
					{
						yield return thing;
					}
				}
			}
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef.tradeability != Tradeability.None && (
				thingDef.category == ThingCategory.Pawn && thingDef.race.Animal &&
				tradeTags.Any(tag => thingDef.tradeTags != null && thingDef.tradeTags.Contains(tag)) ||
				thingDef.IsLeather || (thingDef.IsAnimalProduct && thingDef.defName != "VRESaurids_HumanEgg") || thingDef.IsWool
			);
		}
	}
}