using BiomesCore.DefModExtensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using System.Reflection;
using System.Reflection.Emit;
using BiomesCore.DefOfs;
using BiomesCore.MapGeneration;
using BiomesCore.Reflections;

namespace BiomesCore.Patches
{
    /// <summary>
    ///  Picks color for custom roofs, when roof overlay is toggled
    ///  Vanilla: thin roof = white, thick roof = 50% grey
    /// </summary>
    [HarmonyPatch(typeof(RoofGrid), "GetCellExtraColor")]
    static class RoofColorPatch
    {
        static void Postfix(int index, ref RoofGrid __instance, ref Color __result)
        {
            if (BiomesCoreDefOf.BMT_RockRoofStable != null && __instance.RoofAt(index) == BiomesCoreDefOf.BMT_RockRoofStable)
            {
                //__result = Color.black;

                // light grey
                //__result = new Color(0.75f, 0.75f, 0.75f, 1f);

                // dark grey
                __result = new Color(0.25f, 0.25f, 0.25f, 1f);
            }

        }
    }


    /// <summary>
    /// Allows placement of the cave roofs on map generation
    /// </summary>
    [HarmonyPatch(typeof(GenStep_RocksFromGrid), "Generate")]
    static class CaveRoofGeneration
    {
        static bool Prefix(Map map, GenStepParams parms)
        {
            
            // solid cave roof for caverns
            if (map.Biome.HasModExtension<BiomesMap>())
            {
                if (map.Biome.GetModExtension<BiomesMap>().isCavern)
                {
                    new RocksFromGrid_Cavern().Generate(map, parms);
                    return false;
                }
            }

            return true;
        }

    }


    /// <summary>
    /// cave roofs don't have to be within range of a wall
    /// </summary>
    [HarmonyPatch(typeof(RoofCollapseUtility), "WithinRangeOfRoofHolder")]
    static class RoofCollapse_Disable
    {
        static bool Prefix(IntVec3 c, Map map, ref bool __result)
        {
            if (map.roofGrid.RoofAt(c) == BiomesCoreDefOf.BMT_RockRoofStable)
            {
                __result = true;
                return false;
            }
            return true;
        }

    }

    /// <summary>
    /// Lowers infestation chance under cave roofs
    /// </summary>
    [HarmonyPatch(typeof(InfestationCellFinder), "GetMountainousnessScoreAt")]
    static class InfestationModifier
    {
        static void Postfix(IntVec3 cell, Map map, ref float __result)
        {
            if (map.roofGrid.RoofAt(cell) == BiomesCoreDefOf.BMT_RockRoofStable)
            {
                __result *= 0.25f;
            }
        }
    }

    [HarmonyPatch]
    static class Cavern_FindPlayerStartSpot
    {
        public static MethodInfo IntVec3UnbreachableRoofedInfo = AccessTools.Method(typeof(IntVec3Extensions), "UnbreachableRoofed");

        static MethodBase TargetMethod()
        {
            // Fetch the first lambda
            return typeof(GenStep_FindPlayerStartSpot).GetLambda("Generate");
        }

        // Changes
        // IntVec3.Roofed -> IntVec3Extended.Roofed
        [HarmonyPriority(Priority.First)]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.ReplaceFunction(
                IntVec3UnbreachableRoofedInfo,
                "Roofed",
                "GenStep_FindPlayerStartSpot.Generate");
        }
    }

    [HarmonyPatch]
    static class Cavern_DropCellFinder_RandomDropSpot
    {
        public static MethodInfo IntVec3UnbreachableRoofedInfo = AccessTools.Method(typeof(IntVec3Extensions), "UnbreachableRoofed");

        static MethodBase TargetMethod()
        {
            // Fetch the first lambda in Generate
            return typeof(DropCellFinder).GetLambda("RandomDropSpot");
        }

        // Changes
        // IntVec3.Roofed -> IntVec3Extended.Roofed
        [HarmonyPriority(Priority.First)]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.ReplaceFunction(
                IntVec3UnbreachableRoofedInfo,
                "Roofed",
                "DropCellFinder.RandomDropSpot");
        }
    }

    [HarmonyPatch]
    static class Cavern_DropCellFinder_TradeDropSpot
    {
        public static MethodInfo RoofGridUnbreachableRoofedInfo = AccessTools.Method(typeof(RoofGridExtensions), "UnbreachableRoofed", new Type[] { typeof(RoofGrid), typeof(IntVec3) });

        static MethodBase TargetMethod()
        {
            // Fetch the third lambda
            return typeof(DropCellFinder).GetLambda("TradeDropSpot", lambdaOrdinal: 2);
        }

        // Changes
        // RoofGrid.Roofed -> RoofGridExtended.Roofed
        [HarmonyPriority(Priority.First)]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.ReplaceFunction(
                RoofGridUnbreachableRoofedInfo,
                "Roofed",
                "DropCellFinder.TradeDropSpot");
        }
    }

    [HarmonyPatch]
    static class Cavern_DropCellFinder_TryFindSafeLandingSpotCloseToColony
    {
        public static MethodInfo IntVec3UnbreachableRoofedInfo = AccessTools.Method(typeof(IntVec3Extensions), "UnbreachableRoofed");

        static MethodBase TargetMethod()
        {
            // Fetch the third lambda
            return typeof(DropCellFinder).GetLocalFunc("TryFindSafeLandingSpotCloseToColony", localFunc: "SpotValidator");
        }

        // Changes
        // IntVec3.Roofed -> IntVec3Extended.Roofed
        [HarmonyPriority(Priority.First)]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.ReplaceFunction(
                IntVec3UnbreachableRoofedInfo,
                "Roofed",
                "DropCellFinder.TryFindSafeLandingSpotCloseToColony");
        }
    }

    [HarmonyPatch(typeof(DropCellFinder), "FindRaidDropCenterDistant")]
    static class Cavern_DropCellFinder_FindRaidDropCenterDistant
    {
        public static MethodInfo IntVec3UnbreachableRoofedInfo = AccessTools.Method(typeof(IntVec3Extensions), "UnbreachableRoofed");

        // Changes
        // IntVec3.Roofed -> IntVec3Extended.Roofed
        [HarmonyPriority(Priority.First)]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.ReplaceFunction(
                IntVec3UnbreachableRoofedInfo,
                "Roofed",
                "DropCellFinder.FindRaidDropCenterDistant");
        }
    }


    [HarmonyPatch(typeof(DropCellFinder), "CanPhysicallyDropInto")]
    static class CanPhysicallyDropIntoCavernRoofs
    {
        public static MethodInfo RoofDefUnbreachableRoofedInfo = AccessTools.Method(typeof(RoofDefExtensions), "UnbreachableRoofed");

        // Changes
        // IntVec3.Roofed -> IntVec3Extended.Roofed
        // in lambda function passed to TryFindCentralCell
        [HarmonyPriority(Priority.First)]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool repeat = true;
            OpCode codeToMatch = OpCodes.Ldfld;
            string fieldToMatch = "isThickRoof";
            string parentMethodName = "CanPhysicallyDropInto";
            bool found = false;
            List<CodeInstruction> runningChanges = new List<CodeInstruction>();
            foreach (CodeInstruction instruction in instructions)
            {
                // Find the section calling fieldToMatch and change to RoofDefUnbreachableRoofedInfo call
                if ((repeat || !found) && instruction.opcode == codeToMatch && (instruction.operand as FieldInfo)?.Name == fieldToMatch)
                {
                    runningChanges.Add(new CodeInstruction(OpCodes.Call, RoofDefUnbreachableRoofedInfo));
                    // runningChanges.Add(instruction);
                    found = true;
                }
                else
                {
                    runningChanges.Add(instruction);
                }
            }
            if (!found)
            {
                Log.ErrorOnce(String.Format("[BiomesCaverns] Cannot find {0} in {1}, skipping patch", fieldToMatch, parentMethodName), parentMethodName.GetHashCode() + fieldToMatch.GetHashCode());
            }
            return runningChanges.AsEnumerable();
        }
    }
}
