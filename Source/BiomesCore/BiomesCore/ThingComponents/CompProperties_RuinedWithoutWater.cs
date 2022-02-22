using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace BiomesCore.ThingComponents
{
    public class CompProperties_RuinedWithoutWater : CompProperties_TemperatureRuinable
    {
        public CompProperties_RuinedWithoutWater()
        {
            compClass = typeof(CompRuinedWithoutWater);
        }
    }
}