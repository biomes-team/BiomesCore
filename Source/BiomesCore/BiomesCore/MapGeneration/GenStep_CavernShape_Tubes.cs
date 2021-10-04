using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
	public class GenStep_CavernShape_Tubes : GenStep
	{
		public override int SeedPart
		{
			get
			{
				return 2115123768;
			}
		}


		public override void Generate(Map map, GenStepParams parms)
		{
			// set to an arbitrarily high value
			ModuleBase tubes = new Const(10);

			int tubeCount = Rand.Range(4, 10);
			float angle = Rand.Range(0, 180);
			IntVec3 center = map.Center;

			for(int i = 0; i < tubeCount; i++)
            {
                ModuleBase newTube = new AxisAsValueX();

                newTube = new Abs(newTube);

				float slope = 0.05f;
                float width = Rand.Range(-10, 5);
                width = Math.Min(width, Rand.Range(-10, 5));
                newTube = new ScaleBias(slope, 0 - slope * width, newTube);

                ModuleBase noiseX = new Multiply(new Const(15.0), new Perlin(0.014, 2.0, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High));

                // displace and rotate need to be in this order
                newTube = new Displace(newTube, noiseX, new Const(0.0), new Const(0.0));
                newTube = new Rotate(0.0, Rand.Range(angle, angle + 45f), 0.0, newTube);

				IntVec3 newCenter = new IntVec3((int)Rand.Range(center.x * 0.2f, center.x * 1.8f), 0, (int)Rand.Range(center.z * 0.2f, center.z * 1.8f));
				newTube = new Translate(0f - newCenter.x, 0.0, 0f - newCenter.z, newTube);

				// noise
				newTube = new Add(newTube, new Clamp(0.0, 0.5, new Perlin(0.005, 0.0, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.Medium)));

				tubes = new Min(tubes, newTube);
            }
			
			MapGenFloatGrid elevation = MapGenerator.Elevation;

			foreach (IntVec3 cell in map.AllCells)
			{
                elevation[cell] = tubes.GetValue(cell);
			}

		}
	}
}
