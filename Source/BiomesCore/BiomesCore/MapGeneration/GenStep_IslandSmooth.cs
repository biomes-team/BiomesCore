using BiomesCore.DefModExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
    /// <summary>
    /// The original island generator. Suitable for atolls.
    /// </summary>
    public class GenStep_IslandSmooth : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 2115796768;
            }
        }

        // allowed sizes for the little circles that get piled to make the landmass
        public IntRange SizeRange = new IntRange(3, 16);

        // used for the total size of the main ellipse. These numbers work well for the default map size, but are adjusted to actual map size before use
        private IntRange totalRange = new IntRange(100, 150);
        public int totalDist;


        public override void Generate(Map map, GenStepParams parms)
        {
            MapGenFloatGrid fertility = MapGenerator.Fertility;

            // make ellipse centered on the map's center. Smaller maps get smaller ellipses
            IntRange NoiseRange = new IntRange(-50, 50);
            NoiseRange.min = (int)(0 - 0.15 * map.Size.x);
            NoiseRange.max = (int)(0.15 * map.Size.x);

            IntVec3 mapCenter = map.Center;
            int x = NoiseRange.RandomInRange;
            int z = NoiseRange.RandomInRange;

            IntVec3 focus1 = mapCenter;
            focus1.x += x;
            focus1.z += z;

            IntVec3 focus2 = mapCenter;
            focus2.x -= x;
            focus2.z -= z;

            List<IntVec3> shape = MakeEllipse(focus1, focus2, map);

            // generate random circles and cut them out of the ellipse, so that the final shape is squigglier
            // some circles won't overlap with the main shape, so maps can end up either really squiggly or almost perfectly elliptical. Variety!
            List<IntVec3> originalShape = shape;
            IntRange cutoutSizes = new IntRange(Math.Min(totalRange.min / 8, 56), Math.Min(totalRange.max / 3, 56));

            int numCutouts = 6;
            for (int i = 0; i < numCutouts; i++)
            {
                IntVec3 tempCenter = map.AllCells.RandomElement();

                if (!originalShape.Contains(tempCenter) && shape.Count >= 0.75 * originalShape.Count)
                {
                    shape = shape.Except(GenRadial.RadialCellsAround(tempCenter, cutoutSizes.RandomInRange, true)).ToList();
                }
            }

            Makelandmass(shape, ref fertility, map);
        }




        private List<IntVec3> MakeEllipse(IntVec3 focus1, IntVec3 focus2, Map map)
        {
            SizeRange.max = 5 + map.Size.x / 20;

            totalRange.max = (int)(Math.Min(0.6 * map.Size.x, map.Size.x - (3 * SizeRange.max)));
            totalRange.min = Math.Max((int)(0.45 * map.Size.x), SizeRange.min + (int)DistanceBetweenPoints(focus1, focus2));

            totalDist = totalRange.RandomInRange;

            List<IntVec3> output = new List<IntVec3>();

            foreach (IntVec3 current in map.AllCells)
            {
                if (DistanceBetweenPoints(focus1, current) + DistanceBetweenPoints(focus2, current) < totalDist)
                {
                    output.Add(current);
                }
            }
            return output;
        }


        private float DistanceBetweenPoints(IntVec3 point1, IntVec3 point2)
        {
            float dist = 0;
            double xDist = Math.Pow(point1.x - point2.x, 2);
            double zDist = Math.Pow(point1.z - point2.z, 2);
            dist = (float)Math.Sqrt(xDist + zDist);

            return dist;
        }


        /// <summary>
        /// Makes the landmass shape on the map's fertility grid. 
        /// Generates randomly-sized circles on each tile in the main shape and increments fertility in each circle.
        /// The circles "pile" up, resulting in a bumpy, irregular landmass with the same overall shape as the initial landCells list
        /// </summary>
        private void Makelandmass(List<IntVec3> landCells, ref MapGenFloatGrid fertility, Map map)
        {
            // smaller shapes get higher increments. For atolls, this lets smaller "hills" get tall enough to actually spawn water at the peak
            float fertIncrement = 0.06f;

            if (totalDist < 90)
            {
                fertIncrement = 0.09f;
            }
            if (totalDist < 70)
            {
                fertIncrement = 0.13f;
            }
            if (totalDist < 55)
            {
                fertIncrement = 0.2f;
            }
            if (totalDist < 30)
            {
                fertIncrement = 0.3f;
            }

            foreach (IntVec3 a in landCells)
            {
                if (Rand.Bool)
                {
                    IntVec3 ne = a;
                    List<IntVec3> island = new List<IntVec3>();

                    // Small chance of bigger circles. Helps the overall shape to look more natural.
                    // For atolls, this helps to flatten/ spread out the sand and shallow water around the island. 
                    if (Rand.Chance(0.3f))
                    {
                        if (Rand.Chance(0.2f))
                        {
                            island = GenRadial.RadialCellsAround(ne, Math.Min(56, 3.5f * SizeRange.RandomInRange), true).ToList();

                        }
                        else
                        {
                            island = GenRadial.RadialCellsAround(ne, Math.Min(56, 2.0f * SizeRange.RandomInRange), true).ToList();
                        }
                    }
                    else
                    {
                        island = GenRadial.RadialCellsAround(ne, Math.Min(56, SizeRange.RandomInRange), true).ToList();
                    }

                    foreach (IntVec3 groundTile in island)
                    {
                        if (groundTile.InBounds(map))
                        {
                            fertility[groundTile] += fertIncrement;
                        }
                    }
                }
            }
        }

    }
}
