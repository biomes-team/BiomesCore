﻿using HarmonyLib;
using Verse;

namespace BiomesCore.Locations.Patches
{
	/// <summary>
	/// Handle terrain type changes.
	/// This patch is placed in this subfolder to keep all code based on the Active Terrain mod together, under the same
	/// license.
	/// </summary>
	[HarmonyPatch(typeof(TerrainGrid), nameof(TerrainGrid.SetTerrain))]
	public static class TerrainGrid_SetTerrain_Patch
	{
		public static void Prefix(Map ___map, IntVec3 c, out TerrainDef __state)
		{
			__state = c.GetTerrain(___map);
		}

		public static void Postfix(Map ___map, IntVec3 c, TerrainDef __state)
		{
			if (MapGenerator.mapBeingGenerated != ___map)
			{
				___map.GetComponent<LocationGrid>().TerrainChanged(c, __state);
			}
		}
	}
}