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
        public List<string> graphicPaths;
        public List<string> sowingGraphicPaths;
        public List<string> leaflessGraphicPaths;
        public List<string> immatureGraphicPaths;

        public Graphic GraphicForBiome(BiomeDef biome)
        {
            if (graphicPaths == null) 
                return null;
            if (graphics == null)
                graphics = new List<Graphic>(graphicPaths.Count);
            var biomeIndex = biomes.IndexOf(biome);
            if (biomeIndex == -1)
                return null;
            if (graphics[biomeIndex] == null)
                return graphics[biomeIndex] = GraphicDatabase.Get<Graphic_Random>(graphicPaths[biomeIndex]);
            return graphics[biomeIndex];
        }

        public Graphic LeaflessGraphicPerBiome(BiomeDef biome)
        {
            if (leaflessGraphicPaths == null)
                return null;
            if (leaflessGraphics == null)
                leaflessGraphics = new List<Graphic>(leaflessGraphicPaths.Count);
            var biomeIndex = biomes.IndexOf(biome);
            if (biomeIndex == -1)
                return null;
            if (leaflessGraphics[biomeIndex] == null)
                return leaflessGraphics[biomeIndex] = GraphicDatabase.Get<Graphic_Random>(leaflessGraphicPaths[biomeIndex]);
            return leaflessGraphics[biomeIndex];
        }

        public Graphic SowingGraphicPerBiome(BiomeDef biome)
        {
            if (sowingGraphicPaths == null)
                return null;
            if (sowingGraphics == null)
                sowingGraphics = new List<Graphic>(sowingGraphicPaths.Count);
            var biomeIndex = biomes.IndexOf(biome);
            if (biomeIndex == -1)
                return null;
            if (sowingGraphics[biomeIndex] == null)
                return sowingGraphics[biomeIndex] = GraphicDatabase.Get<Graphic_Random>(sowingGraphicPaths[biomeIndex]);
            return sowingGraphics[biomeIndex];
        }

        public Graphic ImmatureGraphicPerBiome(BiomeDef biome)
        {
            if (immatureGraphicPaths == null)
                return null;
            if (immatureGraphics == null)
                immatureGraphics = new List<Graphic>(immatureGraphicPaths.Count);
            var biomeIndex = biomes.IndexOf(biome);
            if (biomeIndex == -1)
                return null;
            if (immatureGraphics[biomeIndex] == null)
                return immatureGraphics[biomeIndex] = GraphicDatabase.Get<Graphic_Random>(immatureGraphicPaths[biomeIndex]);
            return immatureGraphics[biomeIndex];
        }
    }
}