using BiomesCore.DefModExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCore.MapGeneration
{

    /// <summary>
    /// This selects and runs an island generator
    /// </summary>
    public class GenStep_Island : GenStep
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
            Log.Message("[Biomes! Core] Generating an island");

            // if the island didn't specify any shapes, all shapes are valid.
            List<IslandShape> allowedShapes = map.Biome.GetModExtension<BiomesMap>().islandShapes;
            if(allowedShapes.NullOrEmpty())
            {
                allowedShapes = new List<IslandShape>();
                foreach(IslandShape s in Enum.GetValues(typeof(IslandShape)))
                {
                    allowedShapes.Add(s);
                }
            }

            //pick and run a random allowable shape
            IslandShape shape = allowedShapes.RandomElement();
            switch (shape)
            {
                case IslandShape.Smooth:
                    new GenStep_IslandShape_Smooth().Generate(map, parms);
                    break;
                case IslandShape.Rough:
                    new GenStep_IslandShape_Rough().Generate(map, parms);
                    break;
                case IslandShape.Crescent:
                    new GenStep_IslandShape_Crescent().Generate(map, parms);
                    break;
                case IslandShape.Pair:
                    new GenStep_IslandShape_Pair().Generate(map, parms);
                    break;
                case IslandShape.Cluster:
                    new GenStep_IslandShape_Cluster().Generate(map, parms);
                    break;
                case IslandShape.Broken:
                    new GenStep_IslandShape_Broken().Generate(map, parms);
                    break;

                // rough islands are probably the best default island shape
                default:
                    new GenStep_IslandShape_Rough().Generate(map, parms);
                    break;
            }

        }
    }
}
