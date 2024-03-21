using RimWorld;
using Verse;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Transpiler helpers may replace vanilla code with the functions in this class.
	/// </summary>
	public static class Utility
	{
		/// <summary>
		/// Cavern aware version of the vanilla check for seeing if a room should be considered to be outside.
		/// </summary>
		/// <param name="room">Room being checked.</param>
		/// <param name="cell">Cell being checked.</param>
		/// <returns>True if the room should be considered to be outdoors.</returns>
		public static bool CavernAwarePsychologicallyOutdoors(Room room, IntVec3 cell)
		{
			if (cell.GetRoof(room.Map) == BiomesCoreDefOf.BMT_RockRoofStable)
			{
				// Vanilla PsychologicallyOutdoors uses the number of open roof cells to calculate if the room should be
				// considered to be outdoors. In a cave, we can assume that all cells will have stable rock roof.
				// So in order to see if a room should be considered to be outdoors, the same vanilla threshold is checked
				// against the total cell count of the room.
				return room.TouchesMapEdge || room.CellCount > 300;
			}

			return room.PsychologicallyOutdoors;
		}

		/// <summary>
		/// In certain cases, cavern roof needs to be ignored, such as when landing royalty shuttles or deciding if
		/// something can physically drop into the cell.
		/// </summary>
		/// <param name="cell">Cell to be checked</param>
		/// <param name="map">Map of the cell.</param>
		/// <returns>Roof of the cell. Return null if the roof is a cavern roof, or if there is no roof.</returns>
		public static RoofDef CellGetNonCavernRoof(IntVec3 cell, Map map)
		{
			RoofDef roofDef = cell.GetRoof(map);
			return roofDef != BiomesCoreDefOf.BMT_RockRoofStable ? roofDef : null;
		}

		/// <summary>
		/// Check if the cell has a roof that is not cavern roof.
		/// </summary>
		/// <param name="cell">Cell to check.</param>
		/// <param name="map">Map of this cell.</param>
		/// <returns>True if the cell has any kind of roof, except for cavern stable rock roof.</returns>
		public static bool CellHasNonCavernRoof(IntVec3 cell, Map map)
		{
			return CellGetNonCavernRoof(cell, map) != null;
		}

		public static bool RoofGridHasNonCavernRoof(RoofGrid roofGrid, IntVec3 cell)
		{
			RoofDef roofDef = roofGrid.RoofAt(cell);
			return roofDef != null && roofDef != BiomesCoreDefOf.BMT_RockRoofStable;
		}

		public static RoofDef GetRoofThickIfCavern(IntVec3 cell, Map map)
		{
			RoofDef roofDef = cell.GetRoof(map);
			return roofDef == BiomesCoreDefOf.BMT_RockRoofStable ? RoofDefOf.RoofRockThick : roofDef;
		}

		/// <summary>
		/// In certain cases, UsesOutdoorTemperature is used to determine if a pawn is not in an underground location. In
		/// the case of caves, undergrounders should be considered underground even when they are in cells that can use
		/// outdoor temperature.
		/// </summary>
		/// <param name="cell">Cell to be checked</param>
		/// <param name="map">Map of the cell.</param>
		/// <returns>True if the cell is not part of a cavern, and also uses outdoor temperature.</returns>
		public static bool NotCavernAndUsesOutdoorTemperature(IntVec3 cell, Map map)
		{
			return cell.GetRoof(map) != BiomesCoreDefOf.BMT_RockRoofStable && cell.UsesOutdoorTemperature(map);
		}
	}
}