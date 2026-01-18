using System;
using Verse;
using Verse.Noise;
using RimWorld.Planet;
using BiomesCore.DefModExtensions;


namespace BiomesCore.MapGeneration
{
    public class GenStep_OasisElevation : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 1449027791;
            }
        }

        float oasisBaseSize = 30;

        public override void Generate(Map map, GenStepParams parms)
        {
            Log.Message("[Biomes! Core] Generating an oasis");
            BiomesMap mapParms = new BiomesMap();

            if (map.Biome.HasModExtension<BiomesMap>())
            {
                mapParms = map.Biome.GetModExtension<BiomesMap>();
            }

        // set size and center location
            IntVec3 oasisCenter = map.Center;
            oasisCenter.x += (int)Rand.Range(0 - mapParms.oasisCtrVarPct * map.Size.x, (mapParms.oasisCtrVarPct * map.Size.x));
            oasisCenter.y += (int)Rand.Range(0 - mapParms.oasisCtrVarPct * map.Size.y, (mapParms.oasisCtrVarPct * map.Size.y));

            oasisBaseSize = mapParms.oasisSizeMapPct.RandomInRange;
            float oasisSize = oasisBaseSize  * map.Size.x;

            BiomesMapGenUtil.SetOasisBaseSize(oasisSize);

            Rot4 beachDirection = Find.World.CoastDirectionAt(map.Tile);
            if (beachDirection != null)
            {
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
            BiomesMapGenUtil.SetOasisCenter(oasisCenter);

            Log.Message("Oasis size:  " + oasisSize);
            Log.Message("Oasis center:  " + oasisCenter.x +", " + oasisCenter.y);

            MapGenFloatGrid elevation = MapGenerator.Elevation;

            foreach (IntVec3 current in map.AllCells)
            {
                float distance = BiomesMapGenUtil.DistanceBetweenPoints(oasisCenter, current);
                elevation[current] += 0.2f - oasisSize / distance * 0.1f;
            }
        }
    }
}