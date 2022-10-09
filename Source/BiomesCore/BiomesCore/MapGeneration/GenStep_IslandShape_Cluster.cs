using System;
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
            float angleB = angleA + Rand.RangeSeeded(60f, 120f, (int)angleA);
            float angleC = angleB + Rand.RangeSeeded(60f, 120f, (int)angleB);
            float angleD = angleC + Rand.RangeSeeded(60f, 120, (int)angleC);
            float angleE = angleD + Rand.RangeSeeded(60f, 120, (int)angleD);
            float angleF = angleE + Rand.RangeSeeded(60f, 120, (int)angleE);

            IntVec3 centerA = mapCenter + (distRange.RandomInRange * Vector3Utility.FromAngleFlat(angleA)).ToIntVec3();
            IntVec3 centerB = mapCenter + (distRange.RandomInRange * 0.5f * Vector3Utility.FromAngleFlat(angleB)).ToIntVec3();
            IntVec3 centerC = mapCenter + (distRange.RandomInRange * 1.3f * Vector3Utility.FromAngleFlat(angleC)).ToIntVec3();
            IntVec3 centerD = mapCenter + (distRange.RandomInRange * Vector3Utility.FromAngleFlat(angleD)).ToIntVec3();
            IntVec3 centerE = mapCenter + (distRange.RandomInRange * 0.6f * Vector3Utility.FromAngleFlat(angleE)).ToIntVec3();
            IntVec3 centerF = mapCenter + (distRange.RandomInRange * 1.3f * Vector3Utility.FromAngleFlat(angleF)).ToIntVec3();

            ModuleBase noiseA = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
            ModuleBase noiseB = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);


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
                float addition = Math.Max(0, 20 * (1f - (Rand.RangeSeeded(9f, 12f, 1) * distA / mapSize)) + Rand.RangeSeeded(15f, 20f, centerA.x + centerA.y + centerA.z) * noiseA.GetValue(current));

                // island B
                addition += Math.Max(0, 20 * (1f - (Rand.RangeSeeded(6f, 9f, 2) * distB / mapSize)) + Rand.RangeSeeded(15f, 20f, 2) * noiseA.GetValue(current));

                // island C
                addition += Math.Max(0, 20 * (1f - (Rand.RangeSeeded(9f, 12f, 3) * distC / mapSize)) + Rand.RangeSeeded(15f, 20f, 3) * noiseB.GetValue(current));

                // island D
                addition += Math.Max(0, 20 * (1f - (Rand.RangeSeeded(6f, 9f, 4) * distD / mapSize)) + Rand.RangeSeeded(15f, 20f, 4) * noiseB.GetValue(current));
                switch (islandNumber)
                {
                    case int temp when temp >= 5:
                        addition += Math.Max(0, 20 * (1f - (Rand.RangeSeeded(9f, 12f, 5) * distE / mapSize)) + Rand.RangeSeeded(15f, 20f, 5) * noiseB.GetValue(current));
                        break;
                    case 5:
                        addition += Math.Max(0, 20 * (1f - (Rand.RangeSeeded(6f, 9f, 6) * distF / mapSize)) + Rand.RangeSeeded(15f, 20f, 6) * noiseB.GetValue(current));
                        break;
                }
                fertility[current] += addition;
            }
        }
    }
}
