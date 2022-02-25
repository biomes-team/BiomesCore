using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace BiomesCore
{
    public class PlantSproutIncidentDef : IncidentDef
    {
        public ThingDef plant;
        public IntRange amount = new IntRange(10, 20);
        public bool ignoreSeason;
    }
}