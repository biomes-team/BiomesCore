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
        public bool allowInFreshWater = false;
        public bool allowInSaltWater = false;
        public bool allowInShallowWater = false;
        public bool allowInDeepWater = false;
        public bool allowOnLand = true;
        public bool allowOnSand = true;
        public bool allowOffSand = true;
        public bool cavePlant = false;
        public bool needsRest = false;
        public FloatRange growingHours = new FloatRange(0.25f, 0.8f);
    }
}
