using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BiomesCore
{
    public class CompPackHunter : ThingComp
    {
        public CompProperties_PackHunter Props => (CompProperties_PackHunter)props;

        public IEnumerable<Pawn> FindPackmates()
        {
            if (!(parent is Pawn pawn)) yield break;

            var room = pawn.GetDistrict();

            foreach (var mate in pawn.Map.mapPawns.AllPawnsSpawned.Where(mate =>
                         pawn != mate &&
                         mate.def == pawn.def &&
                         mate.Faction == pawn.Faction &&
                         mate.Position.InHorDistOf(pawn.Position, Props.joinHuntRange) &&
                         mate.GetDistrict() == room))
            {
                yield return mate;
            }
        }
    }
    
    public class CompProperties_PackHunter : CompProperties
    {
        public float joinHuntRange;

        public CompProperties_PackHunter() => compClass = typeof(CompPackHunter);
    }
}