using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCore.DefModExtensions
{
    public class AdditionalHarvestDrops : DefModExtension
    {
        public List<ThingDef> defs = new List<ThingDef>();
        public List<float> yields = new List<float>();
    }
}
