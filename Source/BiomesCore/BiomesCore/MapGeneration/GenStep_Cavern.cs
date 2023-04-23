using BiomesCore.DefModExtensions;
using System;
using System.Collections.Generic;
using Verse;

namespace BiomesCore.MapGeneration
{

    /// <summary>
    /// This selects and runs a valley generator
    /// </summary>
    public class GenStep_Cavern : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 2113696768;
            }
        }

        public override void Generate(Map map, GenStepParams parms)
        {
            Log.Message("[Biomes! Core] Generating a cavern");
            
            List<CavernShape> allowedShapes = map.Biome.GetModExtension<BiomesMap>().cavernShapes;
            if (allowedShapes.NullOrEmpty()) return;

            //pick and run a random allowable shape
            CavernShape shape = allowedShapes.RandomElement();
            switch (shape)
            {
                case CavernShape.Vanilla:
                    break;
                case CavernShape.TunnelNetwork:
                    new GenStep_CavernShape_TunnelNetwork().Generate(map, parms);
                    break;
                case CavernShape.Smooth:
                    new GenStep_CavernShape_Smoth().Generate(map, parms);
                    break;
                case CavernShape.Tubes:
                    new GenStep_CavernShape_Tubes().Generate(map, parms);
                    break;
                case CavernShape.LargeChambers:
                    new GenStep_CavernShape_LargeChambers().Generate(map, parms);
                    break;
                case CavernShape.SmallChambers:
                    new GenStep_CavernShape_SmallChambers().Generate(map, parms);
                    break;

                // vanilla map gen
                default:    
                    //new GenStep_CavernShape_TunnelNetwork().Generate(map, parms);
                    break;
            }

        }
    }
}

