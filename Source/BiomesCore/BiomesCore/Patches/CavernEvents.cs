using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using RimWorld;
using HarmonyLib;
using BiomesCore.DefModExtensions;

namespace BiomesCore.Patches
{
	[HarmonyPatch(typeof(IncidentWorker), nameof(IncidentWorker.CanFireNow))]
	static class CavernGameConditions
	{
		private static HashSet<IncidentDef> _forbiddenIncidents;

		private static void Initialize()
		{
			if (_forbiddenIncidents != null)
			{
				return;
			}

			_forbiddenIncidents = new HashSet<IncidentDef>();

			foreach (var def in DefDatabase<DisableIncidentsDef>.AllDefs)
			{
				if (def.isCavern)
				{
					_forbiddenIncidents.AddRange(def.incidents);
				}
			}
		}

		static bool Prefix(IncidentParms parms, ref IncidentWorker __instance, ref bool __result)
		{
			Initialize();

			BiomeDef biome = Find.WorldGrid[parms.target.Tile].biome;
			if (biome.HasModExtension<BiomesMap>() && biome.GetModExtension<BiomesMap>().isCavern &&
			    _forbiddenIncidents.Contains(__instance.def))
			{
				__result = false;
				return false;
			}

			return true;
		}
	}

	[HarmonyPatch(typeof(GenStep_SpecialTrees), nameof(GenStep_SpecialTrees.CanSpawnAt))]
	internal static class CavernAnimaTreePatch
	{
		private static bool PsychologicallyOutdoorsOrCavern(Room room, IntVec3 cell)
		{
			return cell.GetRoof(room.Map) == BiomesCoreDefOf.BMT_RockRoofStable || room.PsychologicallyOutdoors;
		}

		[HarmonyPriority(Priority.First)]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo outdoorsOriginal = AccessTools.PropertyGetter(typeof(Room), nameof(Room.PsychologicallyOutdoors));
			MethodInfo outdoorsPatched =
				AccessTools.Method(typeof(CavernAnimaTreePatch), nameof(PsychologicallyOutdoorsOrCavern));

			MethodInfo roofedOriginal = AccessTools.Method(typeof(GridsUtility), nameof(GridsUtility.Roofed));
			MethodInfo roofedPatched =
				AccessTools.Method(typeof(IntVec3Extensions), nameof(IntVec3Extensions.UnbreachableRoofed));

			foreach (var line in instructions)
			{
				if (line.opcode == OpCodes.Callvirt && (MethodInfo) line.operand == outdoorsOriginal)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_1); // IntVec3 c;
					yield return new CodeInstruction(OpCodes.Call, outdoorsPatched);
				}
				else if (line.opcode == OpCodes.Call && (MethodInfo) line.operand == roofedOriginal)
				{
					yield return new CodeInstruction(OpCodes.Call, roofedPatched);
				}
				else
				{
					yield return line;
				}
			}
		}
	}
}