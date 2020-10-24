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
    public class Biomes_SandPlant  : DefModExtension
    {
        public bool allowOnSand = false;
        public bool allowOffSand = false;
    }

    public class Biomes_BiomePlantControl : DefModExtension
    {
        public float biomeTotalCommonality = 0f;
        public float biomeWaterCommonality = 0f;
        public float biomeSandCommonality = 0f;
        public float biomeLandCommonality = 0f;
    }
}
