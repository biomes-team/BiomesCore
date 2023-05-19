using System.Collections.Generic;
using Verse;
using RimWorld;
using HarmonyLib;
using BiomesCore.DefModExtensions;

namespace BiomesCore.Patches
{
	[HarmonyPatch(typeof(IncidentWorker), nameof(IncidentWorker.CanFireNow))]
	internal static class DisableBiomeIncidents
	{
		private static HashSet<IncidentDef> _forbiddenCavernIncidentDefs;

		private static void Initialize()
		{
			if (_forbiddenCavernIncidentDefs != null)
			{
				return;
			}

			_forbiddenCavernIncidentDefs = new HashSet<IncidentDef>();

			foreach (var def in DefDatabase<DisableIncidentsDef>.AllDefs)
			{
				if (def.isCavern)
				{
					_forbiddenCavernIncidentDefs.AddRange(def.incidents);
				}
			}
		}

		private static List<Map> MapTargets(IncidentDef def, IIncidentTarget target)
		{
			List<Map> maps = new List<Map>();
			if (def.gameCondition != null && target.GameConditionManager != null)
			{
				var currentMaps = Find.Maps;
				for (var index = 0; index < maps.Count; ++index)
				{
					var map = currentMaps[index];
					if (map.IsPlayerHome)
					{
						maps.Add(map);
					}
				}
			}
			else if (target is Map map)
			{
				maps.Add(map);
			}

			return maps;
		}

		private static bool ShouldDisableIncident(IncidentDef def, List<Map> targetMaps)
		{
			for (var index = 0; index < targetMaps.Count; ++index)
			{
				var map = targetMaps[index];
				var extension = map.Biome.GetModExtension<BiomesMap>();
				if (extension != null && extension.isCavern && _forbiddenCavernIncidentDefs.Contains(def))
				{
					return true;
				}
			}

			return false;
		}

		private static bool Prefix(IncidentParms parms, ref IncidentWorker __instance, ref bool __result)
		{
			Initialize();

			var incidentDef = __instance.def;
			var targets = MapTargets(incidentDef, parms.target);
			if (ShouldDisableIncident(incidentDef, targets))
			{
				__result = false;
				return false;
			}

			return true;
		}
	}
}