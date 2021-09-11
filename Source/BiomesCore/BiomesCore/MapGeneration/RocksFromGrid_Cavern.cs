using BiomesCore.DefOfs;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCore.MapGeneration
{
    public class RocksFromGrid_Cavern : GenStep
    {
        private float maxMineableValue = 3.40282347E+38f;

        private const int MinRoofedCellsPerGroup = 20;

        public override int SeedPart
        {
            get
            {
                return 1182952823;
            }
        }

        public override void Generate(Map map, GenStepParams parms)
        {
            Log.Message("Generating cavern roofs");

            map.regionAndRoomUpdater.Enabled = false;
            float num = 0.7f;

            MapGenFloatGrid elevation = MapGenerator.Elevation;

            foreach (IntVec3 current in map.AllCells)
            {
                float num2 = elevation[current];
                if (num2 > num)
                {
                    ThingDef def = GenStep_RocksFromGrid.RockDefAt(current);
                    GenSpawn.Spawn(def, current, map, WipeMode.Vanish);
                }
                map.roofGrid.SetRoof(current, BiomesCoreDefOf.BMT_RockRoofStable);

            }
            //BoolGrid visited = new BoolGrid(map);
            //List<IntVec3> toRemove = new List<IntVec3>();
            //foreach (IntVec3 current2 in map.AllCells)
            //{
            //    if (!visited[current2])
            //    {
            //        toRemove.Clear();
            //        map.floodFiller.FloodFill(current2, (IntVec3 x) => true, delegate (IntVec3 x)

            //        {
            //            visited[x] = true;
            //                toRemove.Add(x);
            //            }, 2147483647, false, null);
            //            if (toRemove.Count < 20)
            //            {
            //                for (int j = 0; j < toRemove.Count; j++)
            //                {
            //                    map.roofGrid.SetRoof(toRemove[j], null);
            //                }
            //            }
            //    }
            //}
            GenStep_ScatterLumpsMineable genStep_ScatterLumpsMineable = new GenStep_ScatterLumpsMineable();
            genStep_ScatterLumpsMineable.maxValue = this.maxMineableValue;
            float num3 = 10f;
            switch (Find.WorldGrid[map.Tile].hilliness)
            {
                case Hilliness.Flat:
                    num3 = 4f;
                    break;
                case Hilliness.SmallHills:
                    num3 = 8f;
                    break;
                case Hilliness.LargeHills:
                    num3 = 11f;
                    break;
                case Hilliness.Mountainous:
                    num3 = 15f;
                    break;
                case Hilliness.Impassable:
                    num3 = 16f;
                    break;
            }
            genStep_ScatterLumpsMineable.countPer10kCellsRange = new FloatRange(num3, num3);
            genStep_ScatterLumpsMineable.Generate(map, parms);
            map.regionAndRoomUpdater.Enabled = true;
        }
    }
}
