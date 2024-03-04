using System.Collections.Generic;
using Verse;

namespace BiomesCore.Locations
{
	/// <summary>
	/// When a TerrainDef with an associated TerrainLocationDefExtension is placed on a map, a TerrainLocation instance of
	/// the type specified by this DefExtension will be created on the cell of the terrain. The Location instance will be
	/// removed if the terrain is removed. This can be used to associate features to a terrain.
	/// </summary>
	public abstract class TerrainLocationDefExtension : DefModExtension
	{
		/// <summary>
		/// TerrainLocation type to use for terrains with this DefExtension.
		/// </summary>
		/// <returns>Type associated to this DefExtension.</returns>
		public abstract System.Type TerrainLocationType();

		/// <summary>
		/// Chosen tick interval for the terrain location instance. Can be set to Never for locations which should not tick.
		/// </summary>
		/// <returns>Chosen tick type.</returns>
		public abstract TickerType TickerType();

		/// <summary>
		/// Utility function to format config errors in def extensions.
		/// </summary>
		/// <param name="error">Error message.</param>
		/// <returns>Properly formatted error message.</returns>
		protected string FormatConfigError(string error)
		{
			return $"Error in {GetType().Name}: {error}";
		}

		/// <summary>
		/// Configuration errors of the DefExtension. Child instances should override it to validate their own fields.
		/// </summary>
		/// <returns>List of configuration errors.</returns>
		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string error in base.ConfigErrors())
			{
				yield return error;
			}

			if (!TerrainLocationType().IsSubclassOf(typeof(TerrainLocation)))
			{
				yield return FormatConfigError(
					$"{nameof(TerrainLocationType)} must return a type which inherits from {nameof(TerrainLocation)}");
			}
		}
	}
}