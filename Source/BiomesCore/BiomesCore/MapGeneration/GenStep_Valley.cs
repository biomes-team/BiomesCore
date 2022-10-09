using BiomesCore.DefModExtensions;
using System;
using System.Collections.Generic;
using Verse;

namespace BiomesCore.MapGeneration
{

    /// <summary>
    /// This selects and runs a valley generator
    /// </summary>
    public class GenStep_Valley : GenStep
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
            Log.Message("[Biomes! Core] Generating a valley");

            // if the valley didn't specify any shapes, all shapes are valid.
            List<ValleyShape> allowedShapes = map.Biome.GetModExtension<BiomesMap>().valleyShapes;
            if(allowedShapes.NullOrEmpty())
            {
                allowedShapes = new List<ValleyShape>();
                foreach(ValleyShape s in Enum.GetValues(typeof(ValleyShape)))
                {
                    allowedShapes.Add(s);
                }
            }

            //pick and run a random allowable shape
            ValleyShape shape = allowedShapes.RandomElement();
            switch (shape)
            {
                case ValleyShape.Linear:
                    new GenStep_ValleyShape_Linear().Generate(map, parms);
                    break;

                default:
                    new GenStep_ValleyShape_Linear().Generate(map, parms);
                    break;
            }

        }
    }
}

