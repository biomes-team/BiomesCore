using RimWorld;
using Verse;

namespace BiomesCore
{
    public class CompMakeFilthTrail : ThingComp
    {
        public CompProperties_MakeFilthTrail Props => (CompProperties_MakeFilthTrail)props;

        private int _cooldownTicks;
        private IntVec3 _lastPos = IntVec3.Invalid;

        public override void CompTick()
        {
            if (parent.Map == null || parent.Position == _lastPos) return;
            
            _cooldownTicks--;
            _lastPos = parent.Position;

            if (_cooldownTicks <= 0)
            {
                _cooldownTicks = Props.cooldown;

                FilthMaker.TryMakeFilth(parent.Position, parent.Map, Props.filthDef);
            }
        }
    }
    
    public class CompProperties_MakeFilthTrail : CompProperties
    {
        public int cooldown;
        public ThingDef filthDef;

        public CompProperties_MakeFilthTrail() => compClass = typeof(CompMakeFilthTrail);
    }
}