using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace BiomesCore.DefModExtensions
{
    public class Biomes_PlantControl : DefModExtension
    {
        public bool allowInCave = false;
        public bool allowInBuilding = false;
        public bool allowUnroofed = true;
        public bool needsRest = true;
        public bool wallGrower = false;
        public List<String> terrainTags = new List<String>();
        public FloatRange lightRange = new FloatRange(0f, 1f);
        public FloatRange growingHours = new FloatRange(0.25f, 0.8f);
    }
}
