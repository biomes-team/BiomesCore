using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
    public class GenStep_IslandShape_Cluster : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 2115796768;
            }
        }


        public override void Generate(Map map, GenStepParams parms)
        {
            IntVec3 mapCenter = map.Center;

            MapGenFloatGrid fertility = MapGenerator.Fertility;

            float mapSize = map.Size.x;

            IntRange distRange = new IntRange(10, 30);

            distRange.min = (int)(0.18 * map.Size.x);
            distRange.max = (int)(0.30 * map.Size.x);

            float angleA = Rand.Range(10f, 120f);
            float angleB = angleA + Rand.Range(60f, 120f);
            float angleC = angleB + Rand.Range(60f, 120f);
            float angleD = angleC + Rand.Range(60f, 120);
            float angleE = angleD + Rand.Range(60f, 120);
            float angleF = angleE + Rand.Range(60f, 120);

            IntVec3 centerA = mapCenter + (distRange.RandomInRange * Vector3Utility.FromAngleFlat(angleA)).ToIntVec3();
            IntVec3 centerB = mapCenter + (distRange.RandomInRange * 0.5f * Vector3Utility.FromAngleFlat(angleB)).ToIntVec3();
            IntVec3 centerC = mapCenter + (distRange.RandomInRange * 1.3f * Vector3Utility.FromAngleFlat(angleC)).ToIntVec3();
            IntVec3 centerD = mapCenter + (distRange.RandomInRange * Vector3Utility.FromAngleFlat(angleD)).ToIntVec3();
            IntVec3 centerE = mapCenter + (distRange.RandomInRange * 0.5f * Vector3Utility.FromAngleFlat(angleE)).ToIntVec3();
            IntVec3 centerF = mapCenter + (distRange.RandomInRange * 1.3f * Vector3Utility.FromAngleFlat(angleF)).ToIntVec3();

            ModuleBase noiseA = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
            ModuleBase noiseB = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);

            float noisinessA = Rand.Range(30f, 30f);
            float noisinessB = Rand.Range(30f, 30f);

            // original size = 4f
            float sizeA = Rand.Range(13f, 13f);         // smaller on average
            float sizeB = Rand.Range(13f, 13f);       // bigger on average

            int islandNumber = Rand.Range(4, 6);

            foreach (IntVec3 current in map.AllCells)
            {
                float distA = (float)Math.Sqrt(Math.Pow(current.x - centerA.x, 2) + Math.Pow(current.z - centerA.z, 2));
                float distB = (float)Math.Sqrt(Math.Pow(current.x - centerB.x, 2) + Math.Pow(current.z - centerB.z, 2));
                float distC = (float)Math.Sqrt(Math.Pow(current.x - centerC.x, 2) + Math.Pow(current.z - centerC.z, 2));
                float distD = (float)Math.Sqrt(Math.Pow(current.x - centerD.x, 2) + Math.Pow(current.z - centerD.z, 2));
                float distE = (float)Math.Sqrt(Math.Pow(current.x - centerE.x, 2) + Math.Pow(current.z - centerE.z, 2));
                float distF = (float)Math.Sqrt(Math.Pow(current.x - centerF.x, 2) + Math.Pow(current.z - centerF.z, 2));

                // island A
                float addition = Math.Max(0, 20 * (1f - (sizeA * distA / mapSize)) + noisinessA * noiseA.GetValue(current));

                // island B
                addition += Math.Max(0, 20 * (1f - (sizeB * distB / mapSize)) + noisinessA * noiseA.GetValue(current));

                // island C
                addition += Math.Max(0, 20 * (1f - (sizeA * distC / mapSize)) + noisinessB * noiseB.GetValue(current));

                // island D
                addition += Math.Max(0, 20 * (1f - (sizeB * distD / mapSize)) + noisinessB * noiseB.GetValue(current));
                switch (islandNumber)
                {
                    case int temp when temp >= 5:
                        addition += Math.Max(0, 20 * (1f - (sizeA * distE / mapSize)) + noisinessB * noiseB.GetValue(current));
                        break;
                    case 5:
                        addition += Math.Max(0, 20 * (1f - (sizeB * distF / mapSize)) + noisinessB * noiseB.GetValue(current));
                        break;
                }
                fertility[current] += addition;
            }
        }
    }
}
