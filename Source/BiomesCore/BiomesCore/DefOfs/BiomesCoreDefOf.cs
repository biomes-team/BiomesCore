using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCore
{
    [DefOf]
    public static class BiomesCoreDefOf
    {
        public static RoofDef BMT_RockRoofStable;
        public static IncidentDef CaveIn;
        public static GameConditionDef Earthquake;

        static BiomesCoreDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BiomesCoreDefOf));
        }
    }
}
