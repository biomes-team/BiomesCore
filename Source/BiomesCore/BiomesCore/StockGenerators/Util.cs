using Verse;

namespace BiomesCore.StockGenerators
{
	public static class Util
	{
		public static readonly SimpleCurve SelectionChanceFromWildnessCurve = new SimpleCurve
		{
			new CurvePoint(0.0f, 100f),
			new CurvePoint(0.25f, 60f),
			new CurvePoint(0.5f, 30f),
			new CurvePoint(0.75f, 12f),
			new CurvePoint(1f, 2f)
		};

		public static bool AcceptableTemperature(PawnKindDef def, int forTile)
		{
			var tempTile = forTile;
			if (tempTile == -1 && Find.AnyPlayerHomeMap != null)
			{
				tempTile = Find.AnyPlayerHomeMap.Tile;
			}

			return tempTile == -1 || Find.World.tileTemperatures.SeasonAndOutdoorTemperatureAcceptableFor(tempTile, def.race);
		}

		public static bool ExoticAnimalPart(ThingDef def)
		{
			return def.thingSetMakerTags != null && def.thingSetMakerTags.Contains("AnimalPart") && def.tradeTags != null &&
			       def.tradeTags.Contains("ExoticMisc");
		}
	}
}