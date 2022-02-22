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
            if (graphics == null)
                return null;
            var biomeIndex = biomes.IndexOf(biome);
            if (biomeIndex == -1)
                return null;
            return graphics[biomeIndex];
        }

        public Graphic LeaflessGraphicPerBiome(BiomeDef biome)
        {
            if (leaflessGraphics == null)
                return null;
            var biomeIndex = biomes.IndexOf(biome);
            if (biomeIndex == -1)
                return null;
            return leaflessGraphics[biomeIndex];
        }

        public Graphic SowingGraphicPerBiome(BiomeDef biome)
        {
            if (sowingGraphics == null)
                return null;
            var biomeIndex = biomes.IndexOf(biome);
            if (biomeIndex == -1)
                return null;
            return sowingGraphics[biomeIndex];
        }

        public Graphic ImmatureGraphicPerBiome(BiomeDef biome)
        {
            if (immatureGraphics == null)
                return null;
            var biomeIndex = biomes.IndexOf(biome);
            if (biomeIndex == -1)
                return null;
            return immatureGraphics[biomeIndex];
        }
    }
}