using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;
using HarmonyLib;
using BiomesCore.DefModExtensions;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(IncidentWorker_Aurora), "CanFireNowSub")]
    static class CavernAuroraPatch
    {
        static bool Prefix(IncidentParms parms, ref bool __result)
        {
            List<Map> maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                if (maps[i].IsPlayerHome && !maps[i].Biome.HasModExtension<BiomesMap>())
                {
                    return true;
                }
                if (!maps[i].Biome.GetModExtension<BiomesMap>().isCavern)
                {
                    return true;
                }
            }

            __result = false;
            return false;
        }
    }


    [HarmonyPatch(typeof(IncidentWorker_MakeGameCondition), "CanFireNowSub")]
    static class CavernEclipseAndFalloutPatch
    {
        static bool Prefix(IncidentParms parms, ref IncidentWorker __instance, ref bool __result)
        {

            if (__instance.def == IncidentDefOf.ToxicFallout)
            {
                BiomeDef biome = Find.WorldGrid[parms.target.Tile].biome;
                if (!biome.HasModExtension<BiomesMap>())
                {
                    return true;
                }
                if (!biome.GetModExtension<BiomesMap>().isCavern)
                {
                    return true;
                }
                __result = false;
                return false;
            }


            if (__instance.def == IncidentDefOf.Eclipse)
            {
                List<Map> maps = Find.Maps;
                for (int i = 0; i < maps.Count; i++)
                {
                    if (maps[i].IsPlayerHome && !maps[i].Biome.HasModExtension<BiomesMap>())
                    {
                        return true;
                    }
                    if (!maps[i].Biome.GetModExtension<BiomesMap>().isCavern)
                    {
                        return true;
                    }
                }

                __result = false;
                return false;
            }


            return true;

        }
    }

    [HarmonyPatch(typeof(IncidentWorker_Flashstorm), "CanFireNowSub")]
    static class CavernFlashstormPatch
    {
        static bool Prefix(IncidentParms parms, ref IncidentWorker __instance, ref bool __result)
        {
            if (__instance.def == IncidentDefOf.Eclipse || __instance.def == IncidentDefOf.ToxicFallout)
            {
                BiomeDef biome = Find.WorldGrid[parms.target.Tile].biome;
                if (!biome.HasModExtension<BiomesMap>())
                {
                    return true;
                }
                if (!biome.GetModExtension<BiomesMap>().isCavern)
                {
                    return true;
                }
                __result = false;
                return false;
            }
            return true;

        }
    }
}
