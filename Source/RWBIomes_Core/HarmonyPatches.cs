using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Harmony;
using RimWorld.Planet;
using System.Reflection;


namespace RWBIomes_Core
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {

        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.rwb_core");

            // find the AddFoodPoisoningHediff method of the class RimWorld.FoodUtility
            MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.Planet.World), "CoastDirectionAt");

            // find the static method to call before (i.e. Prefix) the targetmethod
            HarmonyMethod prefixmethod = new HarmonyMethod(typeof(RWBIomes_Core.HarmonyPatches).GetMethod("CoastDirectionAt_Prefix"));

            // patch the targetmethod, by calling prefixmethod before it runs, with no postfixmethod (i.e. null)
            harmony.Patch(targetmethod, prefixmethod, null);

        }


        // from RF-Archipelagos
        public static bool CoastDirectionAt_Prefix(int tileID, ref Rot4 __result, ref World __instance)
        {
            var world = Traverse.Create(__instance);
            WorldGrid worldGrid = world.Field("grid").GetValue<WorldGrid>();
            if (worldGrid[tileID].biome.defName.Contains("NoBeach"))
            {
                __result = Rot4.Invalid;
                return false;
            }
            return true;
        }
    }
}
