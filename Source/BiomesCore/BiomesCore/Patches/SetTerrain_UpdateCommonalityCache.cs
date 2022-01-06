using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(TerrainGrid), "SetTerrain")]
    internal class SetTerrain_UpdateCommonality
    {
        internal static void Postfix(IntVec3 c, TerrainDef newTerr, Map ___map)
        {
            WildPlantSpawner_GetBaseDesiredPlantsCountAt.UpdateCommonalityAt(c, ___map, newTerr);
        }
    }
}