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
            if (map.Biome.GetModExtension<BiomesMap>().isIsland)
            {
                new GenStep_Island().Generate(map, parms);
            }
            else if (map.Biome.GetModExtension<BiomesMap>().isOasis)
            {
                new GenStep_Oasis().Generate(map, parms);
            }

        }
    }
}
