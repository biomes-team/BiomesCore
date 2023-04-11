using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BiomesCore.MapGeneration
{
    public class RocksFromGrid_Cavern : GenStep
    {
        private const float MaxMineableValue = float.MaxValue;

        private const float RoofElevationThreshold = 0.7f;

        public override int SeedPart => 1182952823;

        public override void Generate(Map map, GenStepParams parms)
        {
            map.regionAndRoomUpdater.Enabled = false;

            MapGenFloatGrid elevation = MapGenerator.Elevation;

            foreach (IntVec3 current in map.AllCells)
            {
                float num2 = elevation[current];
                if (num2 > RoofElevationThreshold)
                {
                    ThingDef rockDef = GenStep_RocksFromGrid.RockDefAt(current);
                    GenSpawn.Spawn(rockDef, current, map, WipeMode.Vanish);
                }
                map.roofGrid.SetRoof(current, BiomesCoreDefOf.BMT_RockRoofStable);
            }

            GenStep_ScatterLumpsMineable genStep_ScatterLumpsMineable = new GenStep_ScatterLumpsMineable();
            genStep_ScatterLumpsMineable.maxValue = MaxMineableValue;
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
