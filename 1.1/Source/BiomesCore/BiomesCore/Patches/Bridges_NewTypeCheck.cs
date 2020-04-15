using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection.Emit;
using BiomesCore.Reflections;
using BiomesCore.Bridges;
using UnityEngine;

namespace BiomesCore.Patches
{
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
