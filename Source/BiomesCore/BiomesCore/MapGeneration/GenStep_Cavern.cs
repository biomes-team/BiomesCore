﻿using BiomesCore.DefModExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            // if the valley didn't specify any shapes, all shapes are valid.
            List<CavernShape> allowedShapes = map.Biome.GetModExtension<BiomesMap>().cavernShapes;
            if(allowedShapes.NullOrEmpty())
            {
                allowedShapes = new List<CavernShape>();
                foreach(CavernShape s in Enum.GetValues(typeof(CavernShape)))
                {
                    allowedShapes.Add(s);
                }
            }

            //pick and run a random allowable shape
            CavernShape shape = allowedShapes.RandomElement();
            switch (shape)
            {
                case CavernShape.Tunnels:
                    new GenStep_CavernShape_Tunnels().Generate(map, parms);
                    break;

                default:
                    new GenStep_CavernShape_Tunnels().Generate(map, parms);
                    break;
            }

        }
    }
}

