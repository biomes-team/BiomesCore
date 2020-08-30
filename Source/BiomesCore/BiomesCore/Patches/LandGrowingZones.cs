using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{
    /// <summary>
    /// Prevent normal growing zones from being placed in water
    /// </summary>
    [HarmonyPatch(typeof(RimWorld.Designator_ZoneAdd_Growing), "CanDesignateCell")]
    internal static class LandGrowingZones
    {
        static bool Prefix(IntVec3 c, ref AcceptanceReport __result)
        {
            if (Find.CurrentMap.terrainGrid.TerrainAt(c).IsWater)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
