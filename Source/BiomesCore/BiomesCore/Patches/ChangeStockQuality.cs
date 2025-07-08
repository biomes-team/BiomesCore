using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BiomesCore.Patches
{
	/// <summary>
	/// Temporarily changes the quality levels of items generated for trader stock.
	/// </summary>
	[HarmonyPatch(typeof(CompQuality), nameof(CompQuality.PostPostGeneratedForTrader))]
	public class ChangeStockQuality
	{
		internal class QualityRange
		{
			public QualityCategory minQuality;
			public QualityCategory centerQuality;
			public QualityCategory maxQuality;

			public QualityRange(QualityCategory min, QualityCategory center, QualityCategory max)
			{
				minQuality = min;
				centerQuality = center;
				maxQuality = max;
			}
		}

		private static QualityRange _qualityRange;

		public static void SetQuality(QualityCategory min, QualityCategory center, QualityCategory max)
		{
			_qualityRange = new QualityRange(min, center, max);
		}

		public static void ResetQuality()
		{
			_qualityRange = null;
		}

		internal static bool Prefix(CompQuality __instance, TraderKindDef trader, PlanetTile forTile, Faction forFaction)
		{
			if (_qualityRange == null)
			{
				return true;
			}

			var fromGaussian = Rand.Gaussian((float) _qualityRange.centerQuality + 0.5f, 2.5f);
			QualityCategory quality;
			if (fromGaussian < (float) _qualityRange.minQuality)
			{
				quality = _qualityRange.minQuality;
			}
			else if (fromGaussian > (float) _qualityRange.maxQuality)
			{
				quality = _qualityRange.maxQuality;
			}
			else
			{
				quality = (QualityCategory) (int) fromGaussian;
			}

			__instance.SetQuality(quality, ArtGenerationContext.Outsider);
			return false;
		}
	}
}