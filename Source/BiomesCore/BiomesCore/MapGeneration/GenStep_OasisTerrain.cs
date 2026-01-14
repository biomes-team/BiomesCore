using System;
using Verse;
using Verse.Noise;
using RimWorld.Planet;
using BiomesCore.DefModExtensions;
using RimWorld;


namespace BiomesCore.MapGeneration
{
    public class GenStep_OasisTerrain : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 1449027791;
            }
        }

        private TerrainDef outerTerrain;
        private TerrainDef shoreTerrain;
        private TerrainDef shallowTerrain;
        private TerrainDef deepTerarin;


        public override void Generate(Map map, GenStepParams parms)
        {
            Log.Message("[Biomes! Core] Generating an oasis - terrain");
            BiomesMap mapParms = new BiomesMap();

            if (map.Biome.HasModExtension<BiomesMap>())
            {
                mapParms = map.Biome.GetModExtension<BiomesMap>();
            }

            outerTerrain = mapParms.oasisOuterTerrain;
            shoreTerrain = mapParms.oasisShoreTerrain;
            shallowTerrain = mapParms.oasisShallowTerrain;
            deepTerarin = mapParms.oasisDeepTerrain;


            IntVec3 oasisCenter = BiomesMapGenUtil.GetOasisCenter();
            float oasisBaseSize = BiomesMapGenUtil.GetOasisBaseSize();

            ModuleBase moduleBase = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.Medium);

            MapGenFloatGrid oasisNoise = new MapGenFloatGrid(map);

            MapGenFloatGrid fertility = MapGenerator.Fertility;
            //float oasisSize = (oasisBaseSize / 10) * (map.Size.x / 10);
            float oasisSize = oasisBaseSize;

            float perlin;

            foreach (IntVec3 current in map.AllCells)
            {
                float distance = (float)BiomesMapGenUtil.DistanceBetweenPoints(oasisCenter, current);

                perlin = (moduleBase.GetValue(current) + 1) * 10;
                oasisNoise[current] = 1 + ((oasisSize) * perlin) / distance;
                SetOasisTerrain(current, oasisNoise[current], map);
            }
        }

        private void SetOasisTerrain(IntVec3 cell, float value, Map map)
        {
            // threshholds are all pretty arbitrary
            TerrainDef terrain = TerrainDefOf.Sand;
            if(value > 60)
            {
                terrain = deepTerarin;
            }
            else if (value > 35)
            {
                terrain = shallowTerrain;
            }
            else if (value > 26)
            {
                terrain = shoreTerrain;
            }
            else if (value > 20)
            {
                terrain = outerTerrain;
            }

            if (value > 20)
            {
                map.terrainGrid.SetTerrain(cell, terrain);
            }
        }

    }
}