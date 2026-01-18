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
        private float shallowSize;
        private float shoreSize;
        private float surroundingSize;
        private float oasisBaseSize;
        private float oasisNoise;

        public override void Generate(Map map, GenStepParams parms)
        {
            BiomesMap mapParms = new BiomesMap();

            if (map.Biome.HasModExtension<BiomesMap>())
            {
                mapParms = map.Biome.GetModExtension<BiomesMap>();
            }

            if (mapParms.oasisOuterTerrain != null)
            {
                outerTerrain = mapParms.oasisOuterTerrain;
            }
            if (mapParms.oasisShoreTerrain != null)
            {
                shoreTerrain = mapParms.oasisShoreTerrain;
            }
            if (mapParms.oasisShallowTerrain!= null)
            {
                shallowTerrain = mapParms.oasisShallowTerrain;
            }
            if (mapParms.oasisDeepTerrain != null)
            {
                deepTerarin = mapParms.oasisDeepTerrain;
            }

            IntVec3 oasisCenter = BiomesMapGenUtil.GetOasisCenter();
            oasisBaseSize = BiomesMapGenUtil.GetOasisBaseSize() / 2;    // everything else will measure distance from center

            shallowSize = oasisBaseSize * mapParms.oasisShallowPct;
            shoreSize = oasisBaseSize * mapParms.oasisShorePct;
            surroundingSize = oasisBaseSize * mapParms.oasisSurroundingPct;
            oasisNoise = Math.Min(80f, 8f * mapParms.oasisNoiseRange0_10.RandomInRange);

            ModuleBase noiseModule = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.Medium);
            
            MapGenFloatGrid oasisGrid = new MapGenFloatGrid(map);

            //float perlin;
            float distance;

            foreach (IntVec3 current in map.AllCells)
            {
                distance = (float)BiomesMapGenUtil.DistanceBetweenPoints(oasisCenter, current);
                oasisGrid[current] = distance + oasisNoise * noiseModule.GetValue(current);
                //perlin = (noiseModule.GetValue(current) + 1) * 10;
                //oasisNoise[current] = 1 + ((oasisBaseSize) * perlin) / distance;
                SetOasisTerrain(current, oasisGrid[current], map);
            }
        }

        private void SetOasisTerrain(IntVec3 cell, float value, Map map)
        {
            // threshholds are all pretty arbitrary
            TerrainDef terrain = TerrainDefOf.Sand;
            if (value < oasisBaseSize - shallowSize)
            {
                terrain = deepTerarin;
            }
            else if (value < oasisBaseSize)
            {
                terrain = shallowTerrain;
            }
            else if (value < oasisBaseSize + shoreSize)
            {
                terrain = shoreTerrain;
            }
            else if (value < oasisBaseSize + shoreSize + surroundingSize)
            {
                terrain = outerTerrain;
            }
           

            if (value < oasisBaseSize + shoreSize + surroundingSize)
            {
                { 
                    if (terrain != null)
                    {
                        map.terrainGrid.SetTerrain(cell, terrain);
                    }
                }
            }
        }

    }
}