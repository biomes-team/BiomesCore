using System.Collections.Generic;
using System.Linq;
using BiomesCore.Patches;
using RimWorld;
using Verse;

namespace BiomesCore.StockGenerators
{
	/// <summary>
	/// Generates items with a higher quality than normal.
	/// Any maxTechLevelGenerate set in the XML may be ignored if the trader comes from a faction, as it will use the
	/// faction's technology level instead.
	/// Final quality follows a gaussian curve centered on the minimum. Small values are way more common than larger ones.
	/// </summary>
	public class HighQualityItems : StockGenerator
	{
		/// <summary>
		/// List of thingDefs to generate.
		/// </summary>
		private List<ThingDef> thingDefs = new List<ThingDef>();

		/// <summary>
		/// Minimum quality to generate, and also the most common value.
		/// </summary>
		private QualityCategory minQuality = QualityCategory.Awful;

		/// <summary>
		/// Maximum quality to generate, and also the most infrequent value.
		/// </summary>
		private QualityCategory maxQuality = QualityCategory.Awful;

		public override IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			foreach (var err in base.ConfigErrors(parentDef))
			{
				yield return err;
			}

			if (thingDefs.Count == 0)
			{
				yield return "HighQualityItem: thingDefs must not be empty.";
			}

			foreach (var thingDef in thingDefs)
			{
				if (!Enumerable.Any(thingDef.comps, comp => comp.compClass == typeof(CompQuality)))
				{
					yield return $"HighQualityItem: {thingDef} does not have quality.";
				}
			}

			if (minQuality > maxQuality)
			{
				yield return "HighQualityItem: minQuality must be smaller than maxQuality.";
			}
		}

		public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
		{
			maxTechLevelGenerate = faction != null ? faction.def.techLevel : TechLevel.Archotech;

			// Force the stock generation system to create items with the specified quality.
			ChangeStockQuality.SetQuality(minQuality, minQuality, maxQuality);
			foreach (var thingDef in thingDefs)
			{
				if (maxTechLevelGenerate < thingDef.techLevel)
				{
					continue;
				}

				var count = RandomCountOf(thingDef);
				for (var index = 0; index < count; ++index)
				{
					foreach (var thing in StockGeneratorUtility.TryMakeForStock(thingDef, 1, faction))
					{
						yield return thing;
					}
				}
			}

			// Return the stock generation system to its default behaviour.
			ChangeStockQuality.ResetQuality();
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDefs.Contains(thingDef);
		}
	}
}