using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
    public class GenStep_IslandShape_Broken : GenStep
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

            IntRange distRange = new IntRange(3, 10);

            distRange.min = (int)(0.18 * map.Size.x);
            distRange.max = (int)(0.30 * map.Size.x);


            IntVec3 centerA = CellFinder.RandomNotEdgeCell(50, map);
            IntVec3 centerB = CellFinder.RandomNotEdgeCell(50, map);
            IntVec3 centerC = CellFinder.RandomNotEdgeCell(50, map);
            IntVec3 centerD = CellFinder.RandomNotEdgeCell(50, map);
            IntVec3 centerE = CellFinder.RandomNotEdgeCell(50, map);
            IntVec3 centerF = CellFinder.RandomNotEdgeCell(50, map);

            ModuleBase noiseA = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
            ModuleBase noiseB = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);

            float noisiness = Rand.Range(25f, 35f);

            // original size = 4f
            float sizeA = Rand.Range(7.5f, 0.5f);         // smaller on average
            float sizeB = Rand.Range(5f, 7f);       // bigger on average

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
                float addition = Math.Max(0, 20 * (1f - (sizeA * distA / mapSize)) + noisiness * noiseA.GetValue(current));

                // island B
                addition += Math.Max(0, 20 * (1f - (sizeB * distB / mapSize)) + noisiness * noiseA.GetValue(current));

                // island C
                addition += Math.Max(0, 20 * (1f - (sizeB * distC / mapSize)) + noisiness * noiseB.GetValue(current));

                // island D
                addition += Math.Max(0, 20 * (1f - (sizeA * distD / mapSize)) + noisiness * noiseB.GetValue(current));
                switch (islandNumber)
                {
                    case int temp when temp >= 5:
                        addition += Math.Max(0, 20 * (1f - (sizeA * distE / mapSize)) + noisiness * noiseB.GetValue(current));
                        break;
                    case 5:
                        addition += Math.Max(0, 20 * (1f - (sizeB * distF / mapSize)) + noisiness * noiseB.GetValue(current));
                        break;
                }
                fertility[current] += addition;
            }
        }
    }
}
