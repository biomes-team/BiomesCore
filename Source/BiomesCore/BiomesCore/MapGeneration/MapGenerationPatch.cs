using BiomesCore.DefModExtensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCore.MapGeneration
{
    /// <summary>
    /// Checks for map tags and runs the appropriate gensteps
    /// </summary>
    [HarmonyPatch(typeof(RimWorld.GenStep_ElevationFertility), "Generate")]
    static class ElevationFertilityPatch
    {
        static void Postfix(Map map, GenStepParams parms)
        {
            if (!map.Biome.HasModExtension<BiomesMap>())
            {
                return;
            }
            BiomesMap mapParms = map.Biome.GetModExtension<BiomesMap>();

            // generate map shapes
            if (mapParms.isIsland)
            {
                new GenStep_Island().Generate(map, parms);
            }
            else if (mapParms.isOasis)
            {
                new GenStep_Oasis().Generate(map, parms);
            }

            
            // generate elevation grid if necessary
            if(mapParms.addIslandHills)
            {
                new GenStep_IslandElevation().Generate(map, parms);
            }
        }
    }
}
