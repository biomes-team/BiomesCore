using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;

namespace BiomesCore.Compatability
{
    [StaticConstructorOnStartup]
    public static class FertileFieldsCompatibilityPatch
    {
        static FertileFieldsCompatibilityPatch()
        {
            TerrainAffordanceDef fertileGravelAffordance = DefDatabase<TerrainAffordanceDef>.GetNamed("Gravel", false);
            if (fertileGravelAffordance != null)
            {
                TerrainDef pebbles = TerrainDef.Named("BiomesCore_Pebbles");
                pebbles.affordances.Add(fertileGravelAffordance);
            }
        }
    }
}
