using System;
using Verse;
using Verse.Noise;
using RimWorld.Planet;
using BiomesCore.DefModExtensions;
using RimWorld;


namespace BiomesCore.MapGeneration
{
    public class GenStep_IslandTerrain : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 1449027791;
            }
        }

        private float islandBaseSize;
        private float beachSize;
        private float waterDepth;
        private TerrainDef deepTerrain;
        private TerrainDef shallowTerrain;
        private TerrainDef beachTerrain;


        public override void Generate(Map map, GenStepParams parms)
        {
            BiomesMap mapParms = map.Biome.GetModExtension<BiomesMap>();
            islandBaseSize = mapParms.islandSizeMapPct.RandomInRange * map.Size.x;

            islandBaseSize = islandBaseSize / 2;    // everything will measure distance from center
            beachSize = islandBaseSize * mapParms.islandBeachPct;

            if (mapParms.islandDeepTerrain != null)
            {
                deepTerrain = mapParms.islandDeepTerrain;
            }
            else
            {
                deepTerrain = TerrainDefOf.WaterOceanDeep;
            }
            if (mapParms.islandShallowTerrain != null)
            {
                shallowTerrain = mapParms.islandShallowTerrain;
            }
            else
            {
                shallowTerrain = TerrainDefOf.WaterOceanShallow;
            }
            if (mapParms.islandBeachTerrain != null)
            {
                beachTerrain = mapParms.islandBeachTerrain;
            }
            waterDepth = 0.1f * mapParms.islandOceanDepth0_10.RandomInRange;


            MapGenFloatGrid islandGrid = BiomesMapGenUtil.GetIslandFloatGrid();
            MapGenFloatGrid fertilityGrid = MapGenerator.Fertility;
            //float perlin;
            float distance;

            foreach (IntVec3 current in map.AllCells)
            {
                //distance = (float)BiomesMapGenUtil.DistanceBetweenPoints(oasisCenter, current);
                //oasisGrid[current] = distance + oasisNoise * noiseModule.GetValue(current);
                ////perlin = (noiseModule.GetValue(current) + 1) * 10;
                ////oasisNoise[current] = 1 + ((oasisBaseSize) * perlin) / distance;
                SetIslandTerrain(current, islandGrid[current], fertilityGrid[current], map);
            }
        }

        private void SetIslandTerrain(IntVec3 cell, float isValue, float fertValue, Map map)
        {
            // threshholds are all pretty arbitrary
            TerrainDef terrain = null;
            if (isValue > islandBaseSize && isValue <= islandBaseSize + beachSize) 
            {
                terrain = beachTerrain;
            }
            if(isValue > islandBaseSize + beachSize && isValue <= islandBaseSize + beachSize * 2)
            {
                terrain = shallowTerrain;
            }
            if (isValue > islandBaseSize + beachSize * 2)
            {
                if (fertValue < waterDepth)
                {
                    terrain = deepTerrain;
                }
                else
                {
                    terrain = shallowTerrain;
                }
            }

            if (terrain != null)
            {
                map.terrainGrid.SetTerrain(cell, terrain);
            }
        }



    }
}