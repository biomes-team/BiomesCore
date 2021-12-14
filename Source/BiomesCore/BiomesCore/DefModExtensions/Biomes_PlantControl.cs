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
        public bool allowInWater = false;
        public bool allowInFresh = false;
        public bool allowInSalty = false;
        public bool allowInShallow = false;
        public bool allowInDeep = false;
        public bool allowInChestDeep = false;
        public bool allowInBoggy = false;
        public bool allowOnLand = false;
        public bool allowOnDry = false;
        public bool allowInSandy = false;
        public bool allowInCave = false;
        public bool allowInBuilding = false;
        public bool allowOutside = true;
        public bool needsRest = true;
        public List<String> terrainTags = new List<String>();
        public FloatRange lightRange = new FloatRange(0f, 1f);
        public FloatRange growingHours = new FloatRange(0.25f, 0.8f);
    }
}
