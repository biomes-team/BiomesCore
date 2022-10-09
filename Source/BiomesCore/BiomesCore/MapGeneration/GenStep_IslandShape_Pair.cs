using System;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
    public class GenStep_IslandShape_Pair : GenStep
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

            float angle = Rand.Range(0f, 360f);

            IntVec3 centerA = mapCenter + (distRange.RandomInRange * Vector3Utility.FromAngleFlat(angle)).ToIntVec3();
            IntVec3 centerB = mapCenter + (distRange.RandomInRange * Vector3Utility.FromAngleFlat(angle - 180f)).ToIntVec3();

            ModuleBase noiseA = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
            ModuleBase noiseB = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);

            float noisinessA = Rand.Range(4f, 6.5f);
            float noisinessB = Rand.Range(4f, 6.5f);

            // original size = 4f
            float sizeA = Rand.Range(4f, 5.5f);         // smaller on average
            float sizeB = Rand.Range(3.5f, 4.5f);       // bigger on average


            foreach (IntVec3 current in map.AllCells)
            {
                float distA = (float)Math.Sqrt(Math.Pow(current.x - centerA.x, 2) + Math.Pow(current.z - centerA.z, 2));
                float distB = (float)Math.Sqrt(Math.Pow(current.x - centerB.x, 2) + Math.Pow(current.z - centerB.z, 2));

                // island A
                float addition = Math.Max(0, 20 * (1f - (sizeA * distA / mapSize)) + noisinessA * noiseA.GetValue(current));

                // island B
                addition += Math.Max(0, 20 * (1f - (sizeB * distB / mapSize)) + noisinessB * noiseB.GetValue(current));

                fertility[current] += addition;
            }
        }
    }
}
