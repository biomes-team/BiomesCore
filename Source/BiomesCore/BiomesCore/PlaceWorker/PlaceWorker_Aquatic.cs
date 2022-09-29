using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCore
{
    public class PlaceWorker_Aquatic : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(
          BuildableDef checkingDef,
          IntVec3 loc,
          Rot4 rot,
          Map map,
          Thing thingToIgnore = null,
          Thing thing = null)
        {
            foreach (IntVec3 aquaticCell in GenAdj.CellsOccupiedBy(loc, rot, checkingDef.Size))
            {
                if (!map.terrainGrid.TerrainAt(aquaticCell).affordances.Contains(TerrainAffordanceDefOf.Bridgeable))
                    return new AcceptanceReport((string)"TerrainCannotSupport_TerrainAffordance".Translate((NamedArgument)(Def)checkingDef, (NamedArgument)(Def)TerrainAffordanceDefOf.Bridgeable));
            }
            return true;
        }
    }
}
