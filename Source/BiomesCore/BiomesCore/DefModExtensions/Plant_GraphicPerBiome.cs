using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace BiomesCore.DefModExtensions
{
    public class Plant_GraphicPerBiome : DefModExtension
    {
        public List<BiomeDef> biomes;
        public Graphic[] graphics;
        public Graphic[] sowingGraphics;
        public Graphic[] leaflessGraphics;
        public Graphic[] immatureGraphics;
        public List<string> graphicPaths;
        public List<string> sowingGraphicPaths;
        public List<string> leaflessGraphicPaths;
        public List<string> immatureGraphicPaths;
        bool initialized = false;

        public void Initialize()
        {
            var count = biomes.Count;
            graphics = new Graphic[count];
            leaflessGraphics = new Graphic[count];
            sowingGraphics = new Graphic[count];
            immatureGraphics = new Graphic[count];
            for (int i = 0; i < count; i++)
            {
                if (graphicPaths[i] != null)
                    graphics[i] = GraphicDatabase.Get(typeof(Graphic_Random), graphicPaths[i], ShaderDatabase.CutoutPlant, Vector2.one, Color.white, Color.white);
                if (leaflessGraphicPaths != null && leaflessGraphicPaths[i] != null)
                    leaflessGraphics[i] = GraphicDatabase.Get(typeof(Graphic_Random), leaflessGraphicPaths[i], ShaderDatabase.CutoutPlant, Vector2.one, Color.white, Color.white); ;
                if (sowingGraphicPaths != null && sowingGraphicPaths[i] != null)
                    sowingGraphics[i] = GraphicDatabase.Get(typeof(Graphic_Random), sowingGraphicPaths[i], ShaderDatabase.CutoutPlant, Vector2.one, Color.white, Color.white);
                if (immatureGraphicPaths != null && immatureGraphicPaths[i] != null)
                    immatureGraphics[i] = GraphicDatabase.Get(typeof(Graphic_Random), immatureGraphicPaths[i], ShaderDatabase.CutoutPlant, Vector2.one, Color.white, Color.white);
            }
            initialized = true;
        }

        public Graphic GraphicForBiome(BiomeDef biome)
        {
            if (!initialized)
                Initialize();
            var biomeIndex = biomes.IndexOf(biome);
            if (biomeIndex == -1 || graphics[biomeIndex] == null)
                return null;
            return graphics[biomeIndex];
        }

        public Graphic LeaflessGraphicPerBiome(BiomeDef biome)
        {
            if (!initialized)
                Initialize();
            var biomeIndex = biomes.IndexOf(biome);
            if (biomeIndex == -1 || leaflessGraphics[biomeIndex] == null)
                return null;
            return leaflessGraphics[biomeIndex];
        }

        public Graphic SowingGraphicPerBiome(BiomeDef biome)
        {
            if (!initialized)
                Initialize();
            var biomeIndex = biomes.IndexOf(biome);
            if (biomeIndex == -1 || sowingGraphics[biomeIndex] == null)
                return null;
            return sowingGraphics[biomeIndex];
        }

        public Graphic ImmatureGraphicPerBiome(BiomeDef biome)
        {
            if (!initialized)
                Initialize();
            var biomeIndex = biomes.IndexOf(biome);
            if (biomeIndex == -1 || immatureGraphics[biomeIndex] == null)
                return null;
            return immatureGraphics[biomeIndex];
        }
    }
}