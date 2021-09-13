using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
	public class GenStep_CavernShape_Chambers : GenStep
	{
		public override int SeedPart
		{
			get
			{
				return 2115796768;
			}
		}


		public override void Generate(Map map, GenStepParams parms)
		{
			ModuleBase tunnels = new Perlin(0.024, 2.2, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
            tunnels = new ScaleBias(0.5, 0.5, tunnels);

			ModuleBase noise = new Perlin(0.02, 1.5, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.Medium);

			MapGenFloatGrid elevation = MapGenerator.Elevation;

			foreach (IntVec3 cell in map.AllCells)
			{
                elevation[cell] = 0.53f + 2f * Math.Abs(tunnels.GetValue(cell) - 0.5f) + 0.10f * noise.GetValue(cell);
			}

		}
	}
}
