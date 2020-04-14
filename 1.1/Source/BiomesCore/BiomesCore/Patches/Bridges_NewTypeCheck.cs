using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection.Emit;
using BiomesCore.Reflections;
using BiomesCore.Bridges;
using UnityEngine;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(GenConstruct), "CanPlaceBlueprintAt")]
    public class GenConstruct_CanPlaceBlueprintAt_Patch
    {
        public static void Postfix(ref AcceptanceReport __result, BuildableDef entDef, IntVec3 center, Map map, bool godMode = false, Thing thingToIgnore = null, Thing thing = null, ThingDef stuffDef = null)
        {
            /*TerrainDef terrainDef = entDef as TerrainDef;
            if (terrainDef == null)
            {
                TerrainAffordanceDef neededAffordance = map.terrainGrid.TerrainAt(center).terrainAffordanceNeeded;
                if (neededAffordance == TerrainAffordanceDefOf.Bridgeable || neededAffordance == TerrainAffordanceDefOf.BridgeableDeep)
                {
                    __result = new AcceptanceReport("NoFloorsOnBridges".Translate());
                }
            }*/
        }
    }

    [HarmonyPatch(typeof(Designator_RemoveBridge), "CanDesignateCell")]
    public class Designator_RemoveBridge_CanDesignateCell_Patch
    {
        public static DynamicMethod BaseCanDesignateCellInfo = AccessTools.Method(typeof(Designator_RemoveFloor), "CanDesignateCell").CreateNonVirtualDynamicMethod();

        public static bool Prefix(ref AcceptanceReport __result, Designator_RemoveBridge __instance, IntVec3 c)
        {
            if (c.InBounds(__instance.Map) && c.GetTerrain(__instance.Map).IsBiomesBridge())
            {
                __result = (AcceptanceReport)BaseCanDesignateCellInfo.Invoke(null, new object[] { __instance, c });
                return false;
            }
            return true;
        }
    }
}
