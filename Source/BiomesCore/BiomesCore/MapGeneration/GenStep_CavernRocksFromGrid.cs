using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BiomesCore.MapGeneration
{
	/// <summary>
	/// Version of GenStep_RocksFromGrid made for caverns.
	/// </summary>
	public class GenStep_CavernRocksFromGrid : GenStep
	{
		private const float MaxMineableValue = float.MaxValue;

		private const float RoofElevationThreshold = 0.7f;

		public override int SeedPart => -1009609837;

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
					GenSpawn.Spawn(rockDef, current, map);
				}

				map.roofGrid.SetRoof(current, BiomesCoreDefOf.BMT_RockRoofStable);
			}

			GenStep_ScatterLumpsMineable genStep_ScatterLumpsMineable = new GenStep_ScatterLumpsMineable();
			genStep_ScatterLumpsMineable.maxValue = MaxMineableValue;
			float scatterLumpsCount = Find.WorldGrid[map.Tile].hilliness switch
			{
				Hilliness.Flat => 4f,
				Hilliness.SmallHills => 8f,
				Hilliness.LargeHills => 11f,
				Hilliness.Mountainous => 15f,
				Hilliness.Impassable => 16f,
				_ => 10f
			};

			genStep_ScatterLumpsMineable.countPer10kCellsRange = new FloatRange(scatterLumpsCount, scatterLumpsCount);
			genStep_ScatterLumpsMineable.Generate(map, parms);
			map.regionAndRoomUpdater.Enabled = true;
		}
	}
}