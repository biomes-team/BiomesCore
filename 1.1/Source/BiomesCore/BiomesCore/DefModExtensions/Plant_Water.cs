using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace BiomesCore.DefModExtensions
{
    public class Biomes_WaterPlant : DefModExtension
    {
        public bool allowInFreshWater = false;
        public bool allowInSaltWater = false;
        public bool allowInShallowWater = false;
        public bool allowInDeepWater = false;
        public bool allowOnLand = false;
    }

    public class Biomes_WaterPlantBiome : DefModExtension
    {
        public float spawnFertilityMultiplier = 0.33f;
    }
}
