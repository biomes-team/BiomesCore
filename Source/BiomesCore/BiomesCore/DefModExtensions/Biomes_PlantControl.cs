using System.Collections.Generic;
using Verse;

namespace BiomesCore.DefModExtensions
{
	public class Biomes_PlantControl : DefModExtension
	{
		/// <summary>
		/// If set to false, the plant will grow all the time. The plant must have a thingClass of
		/// <thingClass>BMT.BiomesPlant</thingClass>.
		/// </summary>
		public bool needsRest = true;

		public List<string> terrainTags = new List<string>();

		/// <summary>
		/// Hours in which the plant will grow. The plant must have a thingClass of <thingClass>BMT.BiomesPlant</thingClass>.
		/// </summary>
		public FloatRange growingHours = new FloatRange(0.25F, 0.8F);

		/// <summary>
		/// Define a temperature range in which the plant will grow in optimal conditions. The plant must have a
		/// thingClass of <thingClass>BMT.BiomesPlant</thingClass>.
		/// </summary>
		public FloatRange optimalTemperature = new FloatRange(6.0F, 42.0F);

		// The following attributes should be cleaned up in the future; mods and code are not using them anymore.
		public bool allowInCave = false;
		public bool allowInBuilding = false;
		public bool allowUnroofed = true;
		public bool wallGrower = false;
		public FloatRange lightRange = new FloatRange(0f, 1f);
	}
}