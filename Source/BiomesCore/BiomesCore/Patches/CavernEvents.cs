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
using Verse.AI;

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


    [HarmonyPatch(typeof(GenStep_AnimaTrees), "CanSpawnAt")]
    static class CavernAnimaTreePatch
    {
        static bool Prefix(IntVec3 c, Map map, int minProximityToArtificialStructures, int minProximityToCenter, int minFertileUnroofedCells, int maxFertileUnroofedCellRadius, ref bool __result)
        {
            if (!map.Biome.HasModExtension<BiomesMap>())
            {
                return true;
            }
            if (!map.Biome.GetModExtension<BiomesMap>().isCavern)
            {
                return true;
            }

            // copied from the vanilla method, but with the "isroofed" checks replaced with UsesOutdoorTemperature

            if (!c.Standable(map) || c.Fogged(map) || !c.GetRoom(map).UsesOutdoorTemperature)
                {
                __result = false;
                return false;
            }

            Plant plant = c.GetPlant(map);
            if (plant != null && plant.def.plant.growDays > 10f)
            {
                __result = false;
                return false;
            }

            List<Thing> thingList = c.GetThingList(map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i].def == ThingDefOf.Plant_TreeAnima)
                {
                    __result = false;
                    return false;
                }
            }
            if (minProximityToCenter > 0 && map.Center.InHorDistOf(c, minProximityToCenter))
            {
                __result = false;
                return false;
            }
            if (!map.reachability.CanReachFactionBase(c, map.ParentFaction))
            {
                __result = false;
                return false;
            }

            TerrainDef terrain = c.GetTerrain(map);
            if (terrain.avoidWander || terrain.fertility <= 0f)
            {
                __result = false;
                return false;
            }

            if (minProximityToArtificialStructures != 0 && GenRadial.RadialDistinctThingsAround(c, map, minProximityToArtificialStructures, useCenter: false).Any(MeditationUtility.CountsAsArtificialBuilding))
            {
                __result = false;
                return false;
            }
            int num = GenRadial.NumCellsInRadius(maxFertileUnroofedCellRadius);
            int num2 = 0;

            for (int j = 0; j < num; j++)
            {
                IntVec3 intVec = c + GenRadial.RadialPattern[j];
                if (WanderUtility.InSameRoom(intVec, c, map))
                {
                    if (intVec.InBounds(map) && intVec.UsesOutdoorTemperature(map) && intVec.GetTerrain(map).fertility > 0f)
                    {
                        num2++;
                    }
                    if (num2 >= minFertileUnroofedCells)
                    {
                        __result = true;
                        return false;
                    }
                }
            }

            __result = false;
            return false;

        }
    }

}
