using System;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
	public class GenStep_ValleyShape_Linear : GenStep
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
			IntVec3 mapCenter = map.Center;

			MapGenFloatGrid Elevation = MapGenerator.Elevation;

			float mapSize = map.Size.x;

			IntRange distRange = new IntRange(30, 60);

			distRange.min = (int)(0.18 * map.Size.x);
			distRange.max = (int)(0.30 * map.Size.x);

			int seed = (int)Rand.Value;
			float angle = Rand.RangeSeeded(0f, 360f, seed);
			Log.Message("angle = " + angle);
			float angle2 = angle;
			switch (angle)
			{
				case float temp when temp > 180:
					angle2 -= 180;
					break;
				case float temp when temp < 180:
					angle2 += 180;
					break;
				default:
					angle2 = 0;
					break;
			}
			Log.Message("angle2 = " + angle2);

			IntVec3 centerA = mapCenter + (0 * Vector3Utility.FromAngleFlat(angle)).ToIntVec3();
			IntVec3 centerB = mapCenter + ((mapSize * 0.1f) * Vector3Utility.FromAngleFlat(angle)).ToIntVec3();
			IntVec3 centerC = mapCenter + ((mapSize * 0.2f) * Vector3Utility.FromAngleFlat(angle)).ToIntVec3();
			IntVec3 centerD = mapCenter + ((mapSize * 0.3f) * Vector3Utility.FromAngleFlat(angle)).ToIntVec3();
			IntVec3 centerE = mapCenter + ((mapSize * 0.4f) * Vector3Utility.FromAngleFlat(angle)).ToIntVec3();
			IntVec3 centerF = mapCenter + ((mapSize * 0.5f) * Vector3Utility.FromAngleFlat(angle)).ToIntVec3();


			IntVec3 centerG = mapCenter + ((mapSize * 0.1f) * Vector3Utility.FromAngleFlat(angle2)).ToIntVec3();
			IntVec3 centerH = mapCenter + ((mapSize * 0.2f) * Vector3Utility.FromAngleFlat(angle2)).ToIntVec3();
			IntVec3 centerI = mapCenter + ((mapSize * 0.3f) * Vector3Utility.FromAngleFlat(angle2)).ToIntVec3();
			IntVec3 centerJ = mapCenter + ((mapSize * 0.4f) * Vector3Utility.FromAngleFlat(angle2)).ToIntVec3();
			IntVec3 centerK = mapCenter + ((mapSize * 0.5f) * Vector3Utility.FromAngleFlat(angle2)).ToIntVec3();

			ModuleBase noiseA = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
			ModuleBase noiseB = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);

			float noisinessCenter = Rand.Range(5f, 5f);
			float noisinessEdges = Rand.Range(2f, 2f);

			float size = Rand.Range(6f, 8f);       


			foreach (IntVec3 cell in map.AllCells)
			{
				Elevation[cell] = 4f * (noiseA.GetValue(cell) + 0.5f);
				float distCenter = (float)Math.Sqrt(Math.Pow(cell.x - mapCenter.x, 2) + Math.Pow(cell.z - mapCenter.z, 2));
				float distA = (float)Math.Sqrt(Math.Pow(cell.x - centerA.x, 2) + Math.Pow(cell.z - centerA.z, 2));
				float distB = (float)Math.Sqrt(Math.Pow(cell.x - centerB.x, 2) + Math.Pow(cell.z - centerB.z, 2));
				float distC = (float)Math.Sqrt(Math.Pow(cell.x - centerC.x, 2) + Math.Pow(cell.z - centerC.z, 2));
				float distD = (float)Math.Sqrt(Math.Pow(cell.x - centerD.x, 2) + Math.Pow(cell.z - centerD.z, 2));
				float distE = (float)Math.Sqrt(Math.Pow(cell.x - centerE.x, 2) + Math.Pow(cell.z - centerE.z, 2));
				float distF = (float)Math.Sqrt(Math.Pow(cell.x - centerF.x, 2) + Math.Pow(cell.z - centerF.z, 2));

				float distG = (float)Math.Sqrt(Math.Pow(cell.x - centerG.x, 2) + Math.Pow(cell.z - centerA.z, 2));
				float distH = (float)Math.Sqrt(Math.Pow(cell.x - centerH.x, 2) + Math.Pow(cell.z - centerB.z, 2));
				float distI = (float)Math.Sqrt(Math.Pow(cell.x - centerI.x, 2) + Math.Pow(cell.z - centerC.z, 2));
				float distJ = (float)Math.Sqrt(Math.Pow(cell.x - centerJ.x, 2) + Math.Pow(cell.z - centerD.z, 2));
				float distK = (float)Math.Sqrt(Math.Pow(cell.x - centerK.x, 2) + Math.Pow(cell.z - centerE.z, 2));

				float valleysection = 0;
				valleysection += Math.Max(0, 20 * (1f - (size * distCenter / mapSize)) + noisinessCenter * noiseB.GetValue(cell));
				valleysection += Math.Max(0, 20 * (1f - (size * distA / mapSize)) + noisinessCenter * noiseB.GetValue(cell));
				valleysection += Math.Max(0, 20 * (1f - (size * distB / mapSize)) + noisinessCenter * noiseB.GetValue(cell));
				valleysection += Math.Max(0, 20 * (1f - (size * distC / mapSize)) + noisinessEdges * noiseB.GetValue(cell));
				valleysection += Math.Max(0, 20 * (1f - (size * distD / mapSize)) + noisinessEdges * noiseB.GetValue(cell));
				valleysection += Math.Max(0, 20 * (1f - (size * distE / mapSize)) + noisinessEdges * noiseB.GetValue(cell));
				valleysection += Math.Max(0, 20 * (1f - (size * distF / mapSize)) + noisinessEdges * noiseB.GetValue(cell));

				valleysection += Math.Max(0, 20 * (1f - (size * distG / mapSize)) + noisinessCenter * noiseB.GetValue(cell));
				valleysection += Math.Max(0, 20 * (1f - (size * distH / mapSize)) + noisinessCenter * noiseB.GetValue(cell));
				valleysection += Math.Max(0, 20 * (1f - (size * distI / mapSize)) + noisinessEdges * noiseB.GetValue(cell));
				valleysection += Math.Max(0, 20 * (1f - (size * distJ / mapSize)) + noisinessEdges * noiseB.GetValue(cell));
				valleysection += Math.Max(0, 20 * (1f - (size * distK / mapSize)) + noisinessEdges * noiseB.GetValue(cell));

				Elevation[cell] -= valleysection;
			}
		}
	}
}
