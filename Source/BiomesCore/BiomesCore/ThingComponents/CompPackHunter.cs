using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BiomesCore
{
    public class CompPackHunter : ThingComp
    {
        private static CompProperties_PackHunter _cacheValue;
        private static ThingDef _cacheKey;

        public static CompProperties_PackHunter GetProps(ThingDef def)
        {
            if (_cacheKey == def) return _cacheValue;
            _cacheValue = def.GetCompProperties<CompProperties_PackHunter>();
            _cacheKey = def;
            return _cacheValue;
        }
        
        private static int _packSizeCacheValue;
        private static Pawn _packSizeCacheKey;
        private static long _packSizeCacheTimestamp;

        public static int GetPackSize(Pawn pawn, float range)
        {
            if (_packSizeCacheKey == pawn && _packSizeCacheTimestamp >= Find.TickManager?.TicksGame - 1000)
                return _packSizeCacheValue;

            _packSizeCacheValue = FindPackmates(pawn, range, true).Count();
            _packSizeCacheTimestamp = Find.TickManager?.TicksGame ?? 0;
            _packSizeCacheKey = pawn;
            return _packSizeCacheValue;
        }
        
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
        
        public static Pawn FindAnyPackmate(Thing thing, float range)
        {
            return FindPackmates(thing, range).FirstOrDefault();
        }
    }
    
    public class CompProperties_PackHunter : CompProperties
    {
        public float joinHuntRange;
        public float joinMaxPain = 0.3f;
        public float packConfidenceBonusPerMember = 0.25f;

        public CompProperties_PackHunter() => compClass = typeof(CompPackHunter);
    }
}