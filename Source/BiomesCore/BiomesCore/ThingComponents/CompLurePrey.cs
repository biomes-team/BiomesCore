using Verse;

namespace BiomesCore
{
    public class CompLurePrey : ThingComp
    {
        private static CompProperties_LurePrey _cacheValue;
        private static ThingDef _cacheKey;

        public static CompProperties_LurePrey GetProps(ThingDef def)
        {
            if (_cacheKey == def) return _cacheValue;
            _cacheValue = def.GetCompProperties<CompProperties_LurePrey>();
            _cacheKey = def;
            return _cacheValue;
        }

        public CompProperties_LurePrey Props => (CompProperties_LurePrey)props;
    }
    
    public class CompProperties_LurePrey : CompProperties
    {
        public int preyApproachTimeout = 5000;
        public float attackOnDistance = 3;

        public CompProperties_LurePrey() => compClass = typeof(CompLurePrey);
    }
}