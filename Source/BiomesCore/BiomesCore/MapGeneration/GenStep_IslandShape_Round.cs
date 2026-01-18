using BiomesCore.DefModExtensions;
using System;
using System.Collections.Generic;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
    public class GenStep_IslandShape_Round : GenStep
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
            islandBaseSize = mapParms.islandSizeMapPct.RandomInRange * map.Size.x;
            islandNoise = Math.Min(80f, 8f * mapParms.islandNoiseRange0_10.RandomInRange);

            islandCenter = map.Center;
            islandCenter.x += (int)Rand.Range(0 - mapParms.islandCtrVarPct * map.Size.x, (mapParms.islandCtrVarPct * map.Size.x));
            islandCenter.y += (int)Rand.Range(0 - mapParms.islandCtrVarPct * map.Size.y, (mapParms.islandCtrVarPct * map.Size.y));

            IntVec3 mapCenter = map.Center;
            MapGenFloatGrid islandGrid = new MapGenFloatGrid(map);
            MapGenFloatGrid fertility = MapGenerator.Fertility;

            ModuleBase noiseModule = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);

            float distance;

            foreach (IntVec3 current in map.AllCells)
            {
                distance = (float)BiomesMapGenUtil.DistanceBetweenPoints(islandCenter, current);
                islandGrid[current] = distance + islandNoise * noiseModule.GetValue(current);
                //perlin = (noiseModule.GetValue(current) + 1) * 10;
                //oasisNoise[current] = 1 + ((oasisBaseSize) * perlin) / distance;
                //SetOasisTerrain(current, oasisGrid[current], map);
            }

            BiomesMapGenUtil.SetIslandFloatGrid(islandGrid);
            BiomesMapGenUtil.SetIslandCenter(islandCenter);
            BiomesMapGenUtil.SetIslandBaseSize(islandBaseSize);

        }
    }
}

