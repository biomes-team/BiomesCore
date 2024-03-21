using System.Collections.Generic;
using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches.Incidents
{
	/// <summary>
	/// Implementation of DisableIncidentsExtension.
	/// </summary>
	[HarmonyPatch(typeof(IncidentWorker), nameof(IncidentWorker.CanFireNow))]
	public static class IncidentWorker_CanFireNow_Patch
	{
		private static HashSet<BiomeDef> BiomeTargets(IIncidentTarget target)
		{
			var biomeTargets = new HashSet<BiomeDef>();

			if (target is Map map)
			{
				biomeTargets.Add(map.Biome);
			}
			else
			{
				var currentMaps = Find.Maps;
				for (int index = 0; index < currentMaps.Count; ++index)
				{
					var currentMap = currentMaps[index];
					if (currentMap.IsPlayerHome)
					{
						biomeTargets.Add(currentMap.Biome);
					}
				}
			}

			return biomeTargets;
		}

		private static bool ShouldDisableIncident(IncidentDef def, HashSet<BiomeDef> targetBiomes)
		{
			bool shouldDisable = false;
			foreach (BiomeDef biomeDef in targetBiomes)
			{
				var extension = biomeDef.GetModExtension<DisableIncidentsExtension>();
				if (extension != null && extension.disabledIncidents.Contains(def))
				{
					shouldDisable = true;
					break;
				}
			}

			return shouldDisable;
		}

		private static bool Prefix(IncidentParms parms, ref IncidentWorker __instance, ref bool __result)
		{
			HashSet<BiomeDef> targetBiomes = BiomeTargets(parms.target);
			if (ShouldDisableIncident(__instance.def, targetBiomes))
			{
				__result = false;
				return false;
			}

			return true;
		}
	}
}