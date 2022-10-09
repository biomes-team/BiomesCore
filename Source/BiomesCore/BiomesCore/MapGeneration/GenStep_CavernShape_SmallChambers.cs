using System;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
	public class GenStep_CavernShape_SmallChambers : GenStep
	{
		public override int SeedPart
		{
			get
			{
				return 2104796118;
			}
		}


		public override void Generate(Map map, GenStepParams parms)
		{
            ModuleBase chambers = new Perlin(0.021, 1.7, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);

            ModuleBase columns = new Perlin(0.150, 0.5, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
			columns = new ScaleBias(0.5, 0.5, columns);

			// push columns to the centers of chambers
			ModuleBase columnsNoise = new Invert(chambers);
            columnsNoise = new ScaleBias(3f, -2f, columnsNoise);
            columnsNoise = new Min(new Const(0.0), columnsNoise);
			columns = new Add(columns, columnsNoise);

			// broken tunnel network
			ModuleBase tunnels = new Perlin(0.02, 1.7, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
			ModuleBase tunnelsNoise = new Perlin(0.005, 0.0, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.Medium);
			tunnels = new Abs(tunnels);
			tunnels = new Add(new Const(0.5), tunnels);
			tunnels = new Add(tunnels, new Clamp(0, 0.4, tunnelsNoise));

            chambers = new Min(new Add(chambers, new Const(0.85)), tunnels);

            MapGenFloatGrid elevation = MapGenerator.Elevation;

			foreach (IntVec3 cell in map.AllCells)
			{
                elevation[cell] = Math.Max(columns.GetValue(cell), chambers.GetValue(cell));
            }

		}
	}
}
