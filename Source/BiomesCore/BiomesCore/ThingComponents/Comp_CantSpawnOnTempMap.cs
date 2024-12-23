using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCore
{
    public class CompProperties_CantSpawnOnTempMap : CompProperties
    {
        public CompProperties_CantSpawnOnTempMap() => this.compClass = typeof(Comp_CantSpawnOnTempMap);
    }

    public class Comp_CantSpawnOnTempMap: ThingComp
    {

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (parent.Map.IsTempIncidentMap)
            {
                this.parent.Destroy(DestroyMode.Vanish);
            }
        }
    }
}
