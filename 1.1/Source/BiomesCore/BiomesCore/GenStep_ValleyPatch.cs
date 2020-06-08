using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesIslands.GenSteps
{
    [HarmonyPatch(typeof(RimWorld.GenStep_ElevationFertility))]
    [HarmonyPatch(nameof(RimWorld.GenStep_ElevationFertility.Generate))]
    internal static class ValleyPatch
    {
        static void Postfix(Map map, GenStepParams parms)
        {
            if (!map.Biome.HasModExtension<ValleyMap>())
            {
                return;
            }
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            ValleyMap valleyMap = map.Biome.GetModExtension<ValleyMap>();
            IntVec3 center = map.Center;
            int size = map.Size.x / 2;
            foreach (IntVec3 current in map.AllCells)
            {
                float distance = (float)Math.Sqrt(Math.Pow(current.x - center.x, 2) + Math.Pow(current.z - center.z, 2));
                elevation[current] *= Math.Min(Rand.Range(valleyMap.minHillEdgeMultiplier, valleyMap.maxHillEdgeMultiplier), Rand.Range(valleyMap.minHillEncroachment, valleyMap.maxHillEncroachment) * distance / size);
            }
        }
    }
}
