using BiomesCore.DefModExtensions;
using System;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
    public class GenStep_IslandShape_Crescent : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 2115796768;
            }
        }

        private float islandBaseSize;
        private float islandNoise;
        private IntVec3 islandCenter;

        public override void Generate(Map map, GenStepParams parms)
        {
            BiomesMap mapParms = map.Biome.GetModExtension<BiomesMap>();
            islandNoise = Math.Min(80f, 8f * mapParms.islandNoiseRange0_10.RandomInRange);

            islandBaseSize = BiomesMapGenUtil.GetIslandBaseSize();
            islandCenter = BiomesMapGenUtil.GetIslandCenter();


            //IntVec3 mapCenter = map.Center;
            //MapGenFloatGrid fertility = MapGenerator.Fertility;

            float mapSize = map.Size.x;

            // how far the circles are from the center of the map
            IntRange cutoutVarRange = new IntRange(10, 50);

            // scale center variance to the size of the map.
            // Can't use negative numbers because the range would include 0, and the island would be a donut not a crescent
            cutoutVarRange.min = (int)(0.06 * map.Size.x);
            cutoutVarRange.max = (int)(0.12 * map.Size.x);

            int x = cutoutVarRange.RandomInRange;
            int z = cutoutVarRange.RandomInRange;

            IntVec3 mainCenter = islandCenter;

            // randomize the direction of the offset
            // this is required because the above range couldn't have negatives
            IntVec3 cutoutCenter = islandCenter;
            if (Rand.Bool)
            {
                x = 0 - x;
            }
            if (Rand.Bool)
            {
                z = 0 - z;
            }
            // actually set the center location
            cutoutCenter.x -= x;
            cutoutCenter.z -= z;

            MapGenFloatGrid islandGrid = new MapGenFloatGrid(map);
            ModuleBase noise = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
            ModuleBase cutoutNoise = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);

            float sizeAdj = Rand.Range(2.8f, 3.8f);

            float multiplier = 2.8f;

            foreach (IntVec3 current in map.AllCells)
            {
                float mainDist = (float)Math.Sqrt(Math.Pow(current.x - mainCenter.x, 2) + Math.Pow(current.z - mainCenter.z, 2));
                float cutoutDist = (float)Math.Sqrt(Math.Pow(current.x - cutoutCenter.x, 2) + Math.Pow(current.z - cutoutCenter.z, 2));

                // Main island shape
                float addition = 20 * (1f - (2.7f * mainDist / mapSize)) + 4f * noise.GetValue(current);

                // Adds the cutout
                addition -= Math.Max(0, 20 * (1f - (sizeAdj * cutoutDist / mapSize)));

                // scale the crescent so that hills show up
                addition *= multiplier;

                // spread out the edges to make beaches exist
                if (addition < 2f)
                {
                    addition = 0.2f * addition + 1.6f;
                }

                addition = Math.Max(0, addition);
                //fertility[current] += addition;
                islandGrid[current] += addition;
            }

            BiomesMapGenUtil.SetIslandFloatGrid(islandGrid);

        }
    }
}


