using System.Collections.Generic;
using Verse;
using RimWorld;
using HarmonyLib;
using BiomesCore.DefModExtensions;

namespace BiomesCore.Patches
{
	[HarmonyPatch(typeof(IncidentWorker), nameof(IncidentWorker.CanFireNow))]
	static class DisableBiomeIncidents
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

			if (parms.target is Map map && map.Biome.HasModExtension<BiomesMap>() &&
			    map.Biome.GetModExtension<BiomesMap>().isCavern &&
			    _forbiddenIncidents.Contains(__instance.def))
			{
				__result = false;
				return false;
			}

			return true;
		}
	}
}