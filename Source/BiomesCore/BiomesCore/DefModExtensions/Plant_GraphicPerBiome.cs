using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace BiomesCore.DefModExtensions
{
    public class Plant_GraphicPerBiome : DefModExtension
    {
        public List<BiomeDef> biomes;
        public List<Graphic> graphics;
        public List<Graphic> sowingGraphics;
        public List<Graphic> leaflessGraphics;
        public List<Graphic> immatureGraphics;
        
        public Graphic GraphicForBiome(BiomeDef biome)
        {
            return graphics[biomes.IndexOf(biome)];
        }

        public Graphic LeaflessGraphicPerBiome(BiomeDef biome)
        {
            return leaflessGraphics[biomes.IndexOf(biome)];
        }

        public Graphic SowingGraphicPerBiome(BiomeDef biome)
        {
            return sowingGraphics[biomes.IndexOf(biome)];
        }

        public Graphic ImmatureGraphicPerBiome(BiomeDef biome)
        {
            return immatureGraphics[biomes.IndexOf(biome)];
        }
    }
}