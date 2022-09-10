using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BiomesCore
{
    public class CompPackHunter : ThingComp
    {
        public CompProperties_PackHunter Props => (CompProperties_PackHunter)props;

        public static IEnumerable<Pawn> FindPackmates(Thing thing, float range, bool includeSelf = false)
        {
            if (!(thing is Pawn pawn)) yield break;

            var room = pawn.GetDistrict();

            foreach (var mate in pawn.Map.mapPawns.AllPawnsSpawned.Where(mate =>
                         (pawn != mate || includeSelf) &&
                         mate.def == pawn.def &&
                         mate.Faction == pawn.Faction &&
                         !mate.InMentalState &&
                         mate.Position.InHorDistOf(pawn.Position, range) &&
                         mate.GetDistrict() == room))
            {
                yield return mate;
            }
        }
    }
    
    public class CompProperties_PackHunter : CompProperties
    {
        public float joinHuntRange;
        public float joinMaxPain = 0.3f;

        public CompProperties_PackHunter() => compClass = typeof(CompPackHunter);
    }
}