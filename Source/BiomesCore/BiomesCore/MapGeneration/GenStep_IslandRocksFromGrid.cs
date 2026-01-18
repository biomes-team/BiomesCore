using BiomesCore.DefModExtensions;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace BiomesCore.MapGeneration
{
    // from GenStep_RocksFromGrid
    // this makes hills actually spawn on islands
    public class GenStep_IslandRocksFromGrid : GenStep
    {
        private class RoofThreshold
        {
            public RoofDef roofDef;

            public float minGridVal;
        }

        private float maxMineableValue = 3.40282347E+38f;

        private const int MinRoofedCellsPerGroup = 20;

        public override int SeedPart => -386892601;

        public override void Generate(Map map, GenStepParams parms)
        {

            //if (!map.Biome.HasModExtension<BiomesMap>())
            //{
            //    return;
            //}
            //if (!map.Biome.GetModExtension<BiomesMap>().isIsland)
            //{
            //    return;
            //}
            //if (!map.Biome.GetModExtension<BiomesMap>().addIslandHills)
            //{
            //    return;
            //}
            Log.Message("[Biomes Core] Generating island hills...");
            map.regionAndRoomUpdater.Enabled = false;
            float roofThreshhold = 0.7f;
            List<GenStep_IslandRocksFromGrid.RoofThreshold> list = new List<GenStep_IslandRocksFromGrid.RoofThreshold>();
            list.Add(new GenStep_IslandRocksFromGrid.RoofThreshold
            {
                roofDef = RoofDefOf.RoofRockThick,
                minGridVal = roofThreshhold * 1.14f
            });
            list.Add(new GenStep_IslandRocksFromGrid.RoofThreshold
            {
                roofDef = RoofDefOf.RoofRockThin,
                minGridVal = roofThreshhold * 1.04f
            });

            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid caves = MapGenerator.Caves;

            foreach (IntVec3 current in map.AllCells)
            {
                float curElev = elevation[current];
                if (curElev > roofThreshhold)
                {
                    if (caves[current] <= 0f)
                    {
                        ThingDef def = GenStep_RocksFromGrid.RockDefAt(current);
                        GenSpawn.Spawn(def, current, map, WipeMode.Vanish);
                    }
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (curElev > list[i].minGridVal)
                        {
                            map.roofGrid.SetRoof(current, list[i].roofDef);
                            break;
                        }
                    }
                }
            }

            BoolGrid visited = new BoolGrid(map);
            List<IntVec3> toRemove = new List<IntVec3>();
            foreach (IntVec3 current2 in map.AllCells)
            {
                if (!visited[current2])
                {
                    if (this.IsNaturalRoofAt(current2, map))
                    {
                        toRemove.Clear();
                        map.floodFiller.FloodFill(current2, (IntVec3 x) => this.IsNaturalRoofAt(x, map), delegate (IntVec3 x)
                        {
                            visited[x] = true;
                            toRemove.Add(x);
                        }, 2147483647, false, null);
                        if (toRemove.Count < MinRoofedCellsPerGroup)
                        {
                            for (int j = 0; j < toRemove.Count; j++)
                            {
                                map.roofGrid.SetRoof(toRemove[j], null);
                            }
                        }
                    }
                }
            }

            GenStep_ScatterLumpsMineable genStep_ScatterLumpsMineable = new GenStep_ScatterLumpsMineable();
            genStep_ScatterLumpsMineable.maxValue = this.maxMineableValue;

            float oreTuning = 10f;
            switch (Find.WorldGrid[map.Tile].hilliness)
            {
                case Hilliness.Flat:
                    oreTuning = 4f;
                    break;
                case Hilliness.SmallHills:
                    oreTuning = 8f;
                    break;
                case Hilliness.LargeHills:
                    oreTuning = 11f;
                    break;
                case Hilliness.Mountainous:
                    oreTuning = 15f;
                    break;
                case Hilliness.Impassable:
                    oreTuning = 16f;
                    break;
            }

            // This scales the amount of available ore for islands
            oreTuning *= 0.8f;

            genStep_ScatterLumpsMineable.countPer10kCellsRange = new FloatRange(oreTuning, oreTuning);
            genStep_ScatterLumpsMineable.Generate(map, parms);

            map.regionAndRoomUpdater.Enabled = true;
        }


        private bool IsNaturalRoofAt(IntVec3 c, Map map)
        {
            return c.Roofed(map) && c.GetRoof(map).isNatural;
        }
    }
}

