using System.Collections.Generic;
using System.Reflection;
using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Prevent the siege raid strategy and the mechanoid siege raid strategy from taking place in cave maps.
	/// </summary>
	[HarmonyPatch]
	public static class RaidStrategyWorker_Siege_CanUseWith_Patch
	{
		[HarmonyTargetMethods]
		public static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(RaidStrategyWorker_Siege), "CanUseWith");
			yield return AccessTools.Method(typeof(RaidStrategyWorker_SiegeMechanoid), "CanUseWith");
		}

		public static void Postfix(ref bool __result, IncidentParms parms)
		{
			if (parms.target is Map map)
			{
				BiomesMap biome = map.Biome.GetModExtension<BiomesMap>();
				if (biome != null && biome.isCavern)
				{
					__result = false;
				}
			}
		}
	}
}