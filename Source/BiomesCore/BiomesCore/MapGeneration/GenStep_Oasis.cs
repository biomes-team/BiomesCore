using System;
using Verse;
using Verse.Noise;
using RimWorld.Planet;
using BiomesCore.DefModExtensions;


namespace BiomesCore.MapGeneration
{
    public class GenStep_Oasis : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 1449027791;
            }
        }

        float oasisBaseSize = Rand.Range(30f, 30f);

        public override void Generate(Map map, GenStepParams parms)
        {
            if (!map.Biome.HasModExtension<BiomesMap>())
            {
                return;
            }
            if (!map.Biome.GetModExtension<BiomesMap>().isOasis)
            {
                return;
            }
            Log.Message("[Biomes! Core] Generating an oasis");

            MapGenFloatGrid oasisGrid = MapGenerator.FloatGridNamed("OasisGrid");
            IntVec3 oasisCenter = map.Center;
            ModuleBase moduleBase = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.Medium);

            WorldGrid grid = Find.World.grid;
            int tileID = map.Tile;


            Rot4 beachDirection = Find.World.CoastDirectionAt(map.Tile);
            if (beachDirection != null)
            {
                // If it has a beach, the oasis is smaller
                // Move the center of the oasis away from the beach
                if (beachDirection == Rot4.North)
                {
                    oasisCenter.z -= 10;
                }
                else if (beachDirection == Rot4.South)
                {
                    oasisCenter.z += 10;
                }
                else if (beachDirection == Rot4.East)
                {
                    oasisCenter.x -= 10;
                }
                else if (beachDirection == Rot4.West)
                {
                    oasisCenter.x += 10;
                }
            }


            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid fertility = MapGenerator.Fertility;
            Log.Message("Map size:" + map.Size.x);
            float oasisSize = (oasisBaseSize / 10) * (map.Size.x / 10);
            float perlin;
            Log.Message("Oasis size:  " + oasisSize);

            foreach (IntVec3 current in map.AllCells)
            {
                float distance = (float)Math.Pow((DistanceBetweenPoints(oasisCenter, current) / 6), 2) + 10;
                perlin = ((moduleBase.GetValue(current) + 1) * 7);
                //float elevationWeight = 20 * elevation[current];
                float elevationPerlin = (((moduleBase.GetValue(current) + 1) * 10) * elevation[current]);
                fertility[current] = 1 + ((oasisSize) * elevationPerlin) / distance;

                // this pushes the hills out of the lake
                elevation[current] += 0.3f - fertility[current] * 0.1f;
            }
        }


        private float DistanceBetweenPoints(IntVec3 point1, IntVec3 point2)
        {
            float dist = 0;
            double xDist = Math.Pow(point1.x - point2.x, 2);
            double zDist = Math.Pow(point1.z - point2.z, 2);
            dist = (float)Math.Sqrt(xDist + zDist);

            return dist;
        }
    }
}