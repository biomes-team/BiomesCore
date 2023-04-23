using System.Collections.Generic;
using Verse;

namespace BiomesCore.DefModExtensions
{
    public class Biomes_PlantControl : DefModExtension
    {
        public bool allowInCave = false;
        public bool allowInBuilding = false;
        public bool allowUnroofed = true;
        public bool needsRest = true;
        public bool wallGrower = false;
        public List<string> terrainTags = new List<string>();
        public FloatRange lightRange = new FloatRange(0f, 1f);
        public FloatRange growingHours = new FloatRange(0.25f, 0.8f);
        public FloatRange optimalTemperature = new FloatRange(6f, 42f);
    }
}
