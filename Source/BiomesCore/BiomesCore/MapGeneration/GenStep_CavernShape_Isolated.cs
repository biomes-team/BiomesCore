using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
	public class GenStep_CavernShape_Isolated : GenStep
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
			ModuleBase tunnels = new Perlin(0.024, 2.2, 0.45, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
            tunnels = new ScaleBias(0.5, 0.5, tunnels);

			ModuleBase noise = new Perlin(0.02, 1.5, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.Medium);

			MapGenFloatGrid elevation = MapGenerator.Elevation;

			IntVec3 mapCenter = map.Center;
			float mapSize = map.Size.x;
			foreach (IntVec3 cell in map.AllCells)
			{
				float distanceAbsolute = (float)Math.Sqrt(Math.Pow(cell.x - mapCenter.x, 2) + Math.Pow(cell.z - mapCenter.z, 2));
				float distanceRelative = distanceAbsolute / mapSize;
				float noiseModified = noise.GetValue(cell) / distanceRelative;
				elevation[cell] = (0.44f + 1f * Math.Abs(tunnels.GetValue(cell) - 0.5f) + 0.035f * noiseModified) + distanceRelative / 2.2f;
			}

		}
	}
}
