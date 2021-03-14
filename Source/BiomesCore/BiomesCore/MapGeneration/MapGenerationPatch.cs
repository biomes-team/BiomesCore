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
    [HarmonyPatch(typeof(RimWorld.GenStep_ElevationFertility), "Generate")]
    static class ElevationFertilityPatch
    {
        static void Postfix(Map map, GenStepParams parms)
        {
            if (!map.Biome.HasModExtension<BiomesMap>())
            {
                return;
            }
            if (map.Biome.GetModExtension<BiomesMap>().isOasis)
            {
                new GenStep_Oasis().Generate(map, parms);
            }

        }
    }
}
