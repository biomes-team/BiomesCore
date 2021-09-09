using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
	public class GenStep_CavernShape_NarrowTunnels : GenStep
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


			IntVec3 roomLoc = map.Center;
			roomLoc.x = (int)(Rand.Range(0.3f, 1.7f) * roomLoc.x);
			roomLoc.z = (int)(Rand.Range(0.3f, 1.7f) * roomLoc.z);

			ModuleBase roomNoise = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
			float sizeAdj = Rand.Range(8f, 10f);

			foreach (IntVec3 cell in map.AllCells)
			{
				float distance = (float)Math.Sqrt(Math.Pow(cell.x - roomLoc.x, 2) + Math.Pow(cell.z - roomLoc.z, 2));

				float addition = 25 * (1f - (sizeAdj * distance / map.Size.x)) + 15f * roomNoise.GetValue(cell);

				elevation[cell] -= Math.Max(0, addition);
			}


		}
	}
}
