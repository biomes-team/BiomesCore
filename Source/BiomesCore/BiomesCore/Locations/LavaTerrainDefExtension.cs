using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BiomesCore.Locations
{
	/// <summary>
	/// Magma terrain pushes heat, glows, and has a chance to set nearby things on fire.
	/// </summary>
	public class LavaTerrainDefExtension : TerrainLocationDefExtension
	{
		public float heatPerSecond = 0.0F;
		public float heatPushMinTemperature = -99999f;
		public float heatPushMaxTemperature = 99999f;

		public float overlightRadius = -1.0F;
		public float glowRadius = 14.0F;
		public ColorInt glowColor = new ColorInt(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);

		/// <summary>
		/// LavaTerrainLocation requires a dummy CompProperties_Glower to implement its features. To avoid creating multiple
		/// identical instances of this class, it is allocated here in the DefExtension instead.
		/// </summary>
		public CompProperties_Glower CompPropertiesGlower = null;

		public LavaTerrainDefExtension()
		{
		}

		public override System.Type TerrainLocationType()
		{
			return typeof(LavaTerrainLocation);
		}

		public override TickerType TickerType()
		{
			return Verse.TickerType.Rare;
		}

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string error in base.ConfigErrors())
			{
				yield return error;
			}

			if (heatPerSecond == 0.0F)
			{
				yield return FormatConfigError($"{nameof(heatPerSecond)} value of {heatPerSecond} must not be zero.");
			}

			if (heatPushMaxTemperature < heatPushMinTemperature)
			{
				yield return FormatConfigError(
					$"{nameof(heatPushMaxTemperature)} value of {heatPushMaxTemperature} must be smaller than the {nameof(heatPushMinTemperature)} value of {heatPushMinTemperature}.");
			}

			if (overlightRadius <= 0.0F)
			{
				yield return FormatConfigError(
					$"{nameof(overlightRadius)} value of {overlightRadius} must be larger than zero.");
			}

			if (glowRadius <= 0.0F)
			{
				yield return FormatConfigError($"{nameof(glowRadius)} value of {glowRadius} must be larger than zero.");
			}

			CompPropertiesGlower = new CompProperties_Glower()
			{
				glowColor = glowColor,
				glowRadius = glowRadius,
				overlightRadius = overlightRadius
			};
		}
	}
}