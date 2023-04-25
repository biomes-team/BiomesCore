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

		private static HashSet<TerrainDef> TerrainToAvoid;

		private static bool CellUnbreachableRoofedAndValid(IntVec3 c, Map map)
		{
			if (TerrainToAvoid == null)
			{
				TerrainToAvoid = new HashSet<TerrainDef>();
				foreach (var def in DefDatabase<AvoidTerrainOnGameStartDef>.AllDefs)
				{
					TerrainToAvoid.AddRange(def.terrains);
				}
			}

			if (c.UnbreachableRoofed(map))
			{
				return false;
			}

			foreach (var cell in GenRadial.RadialCellsAround(c, 10, true))
			{
				if (TerrainToAvoid.Contains(cell.GetTerrain(map)))
				{
					return false;
				}
			}

			return true;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo newMethod =
				AccessTools.Method(typeof(FindPlayerStartSpot), nameof(CellUnbreachableRoofedAndValid));

			return TranspilerHelper.ReplaceCall(instructions.ToList(), Methods.CellRoofedOriginal, newMethod);
		}
	}
}