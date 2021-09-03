using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
	public class GenStep_CavernShape_Tunnels : GenStep
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
			ModuleBase noise = new Perlin(0.024, 2.2, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
            noise = new ScaleBias(0.5, 0.5, noise);

            MapGenFloatGrid elevation = MapGenerator.Elevation;

			foreach (IntVec3 cell in map.AllCells)
			{
				elevation[cell] = 0.5f + 2f * Math.Abs(noise.GetValue(cell) - 0.5f);
			}

		}
	}
}
