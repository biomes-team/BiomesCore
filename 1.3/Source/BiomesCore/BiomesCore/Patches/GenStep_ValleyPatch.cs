using BiomesCore.DefModExtensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCore.GenSteps
{
    [HarmonyPatch(typeof(RimWorld.GenStep_ElevationFertility))]
    [HarmonyPatch(nameof(RimWorld.GenStep_ElevationFertility.Generate))]
    internal static class ValleyPatch
    {
        static void Postfix(Map map, GenStepParams parms)
        {
            if (!map.Biome.HasModExtension<BiomesMap>())
            {
                return;
            }
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            BiomesMap valleyMap = map.Biome.GetModExtension<BiomesMap>();
            IntVec3 center = map.Center;
            float size = (map.Size.x * map.Size.y) / 2f;
            Random random = new Random();
            int valleyDirectionDecider = random.Next(1, 2);
            float distance = 0;
            float distance2 = 0;
            float distance3 = 0;

            int valleyType = Rand.Range(0, 2);
            foreach (IntVec3 current in map.AllCells)
            {
                //distance = (float)Math.Sqrt(Math.Pow(current.x - center.x, 2) + Math.Pow(current.z - center.z, 2))
                distance = (float)Math.Sqrt(Math.Pow(current.x - center.x, 2) + Math.Pow(current.z - center.z, 2));

                switch (valleyType)
                {
                    case 0:
                        distance2 = (float)Math.Sqrt(Math.Pow(current.z - center.z, 2));
                        break;
                    case 1:
                        distance2 = (float)Math.Sqrt(Math.Pow(current.x - center.x, 2));
                        break;
                    case 2:
                        distance2 = (float)Math.Sqrt(Math.Pow(current.x - center.x, 2));
                        break;
                }
                distance3 = distance + (distance2 * 1.3f);
                //distance3 = Rand.Range(distance, distance2);
                elevation[current] *= (Math.Min(Rand.Range(valleyMap.minHillEdgeMultiplier, valleyMap.maxHillEdgeMultiplier), Rand.Range(valleyMap.minHillEncroachment, valleyMap.maxHillEncroachment) * 2) * distance3 / size);
            }
        }
    }
}
