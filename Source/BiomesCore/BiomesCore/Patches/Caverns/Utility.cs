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
		/// In most cases in which vanilla checks for Room.PsychologicallyOutdoors, Caverns want the event to happen.
		/// </summary>
		/// <param name="room">Room being checked.</param>
		/// <param name="cell">Cell being checked.</param>
		/// <returns>True if this is a cavern cell or if the room is considered to be outdoors.</returns>
		public static bool PsychologicallyOutdoorsOrCavern(Room room, IntVec3 cell)
		{
			return cell.GetRoof(room.Map) == BiomesCoreDefOf.BMT_RockRoofStable || room.PsychologicallyOutdoors;
		}

		/// <summary>
		/// Check if the cell has a roof that is not cavern roof.
		/// </summary>
		/// <param name="cell">Cell to check.</param>
		/// <param name="map">Map of this cell.</param>
		/// <returns>True if the cell has any kind of roof, except for cavern stable rock roof.</returns>
		public static bool HasNonCavernRoof(IntVec3 cell, Map map)
		{
			var roof = cell.GetRoof(map);
			return roof != null && roof != BiomesCoreDefOf.BMT_RockRoofStable;
		}

		/// <summary>
		/// Translates BMT_RockRoofStable into the def for vanilla thick rock roof.
		/// </summary>
		/// <param name="c">Current cell.</param>
		/// <param name="map">Map of the cell.</param>
		/// <returns>RoofDefOf.RoofRockThick if the roof is BMT_RockRoofStable, the vanilla value otherwise.</returns>
		public static RoofDef GetRoofThickIfCavern(IntVec3 c, Map map)
		{
			var roof = c.GetRoof(map);
			return roof == BiomesCoreDefOf.BMT_RockRoofStable ? RoofDefOf.RoofRockThick : roof;
		}
	}
}