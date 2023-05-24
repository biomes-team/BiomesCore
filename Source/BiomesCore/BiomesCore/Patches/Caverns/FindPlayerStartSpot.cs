using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches.Caverns
{
	[HarmonyPatch]
	internal static class FindPlayerStartSpot
	{
		static MethodBase TargetMethod()
		{
			return typeof(GenStep_FindPlayerStartSpot).GetLambda(nameof(GenStep_FindPlayerStartSpot.Generate));
		}

		private static HashSet<TerrainDef> _terrainToAvoid;

		/// <summary>
		/// The lambda being transpiled is negated, which is why this function returns true for invalid spots.
		/// </summary>
		/// <param name="c">Cell being evaluated.</param>
		/// <param name="map">Map being evaluated.</param>
		/// <returns>True if this is not a good starting point.</returns>
		private static bool InvalidStartSpot(IntVec3 c, Map map)
		{
			if (_terrainToAvoid == null)
			{
				_terrainToAvoid = new HashSet<TerrainDef>();
				foreach (var def in DefDatabase<AvoidTerrainOnGameStartDef>.AllDefsListForReading)
				{
					_terrainToAvoid.AddRange(def.terrains);
				}
			}

			var result = c.Roofed(map) && c.GetRoof(map) != BiomesCoreDefOf.BMT_RockRoofStable;

			if (!result)
			{
				foreach (var loopCell in GenRadial.RadialCellsAround(c, 10, true))
				{
					if (loopCell.InBounds(map) && _terrainToAvoid.Contains(loopCell.GetTerrain(map)))
					{
						result = true;
						break;
					}
				}
			}

			return result;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo newMethod =
				AccessTools.Method(typeof(FindPlayerStartSpot), nameof(InvalidStartSpot));

			return TranspilerHelper.ReplaceCall(instructions.ToList(), Methods.CellRoofedMethod, newMethod);
		}
	}
}