using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
	public class GenStep_CavernShape_FleshChambers : GenStep
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
			//ModuleBase tunnels = new Perlin(0.020, 0.2, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
			ModuleBase tunnels = new Perlin(0.030, 0.5, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
			tunnels = new ScaleBias(0.5, 0.5, tunnels);

			//ModuleBase chambers = new Perlin(0.025, 0.5, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.Medium);
			ModuleBase chambers = new Perlin(0.020, 0.5, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.Medium);

			MapGenFloatGrid elevation = MapGenerator.Elevation;

			foreach (IntVec3 cell in map.AllCells)
			{
				//elevation[cell] = 0.53f + 2f * Math.Abs(tunnels.GetValue(cell) - 0.5f);
				//elevation[cell] = 1.1f + 2f * chambers.GetValue(cell);

				elevation[cell] = 0.53f + 2f * Math.Abs(tunnels.GetValue(cell) - 0.5f);
				elevation[cell] = Math.Min(elevation[cell], 1.3f + 2f * chambers.GetValue(cell));

            }

		}
	}
}
