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
	}
}