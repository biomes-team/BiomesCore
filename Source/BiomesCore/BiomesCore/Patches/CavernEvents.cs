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



}
