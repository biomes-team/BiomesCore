using RimWorld;
using Verse;

namespace BiomesCore
{
    public class DeathActionWorker_CorpseThingSpawner : DeathActionWorker
    {
        public override void PawnDied(Corpse corpse)
        {
            CorpseThingSpawner comp = corpse.InnerPawn.TryGetComp<CorpseThingSpawner>();
            if (comp != null)
            {
                FilthMaker.TryMakeFilth(corpse.Position, corpse.Map, DefDatabase<ThingDef>.GetNamedSilentFail(comp.Props.filthDef), comp.Props.filthAmount, FilthSourceFlags.None);
                Thing thing = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamedSilentFail(comp.Props.thingDef), null);
                thing.stackCount = comp.Props.resourceAmount;
                GenPlace.TryPlaceThing(thing, corpse.Position, corpse.Map, 0, null, null, default);
                corpse.Destroy(0);
            }
        }
    }
    public class CompProperties_CorpseThingSpawner : CompProperties
    {
        public string filthDef = null;
        public string thingDef = null;
        public int filthAmount = 0;
        public int resourceAmount = 1;
        public CompProperties_CorpseThingSpawner() => this.compClass = typeof(CorpseThingSpawner);
    }
    public class CorpseThingSpawner : ThingComp
    {
        public CompProperties_CorpseThingSpawner Props => (CompProperties_CorpseThingSpawner)this.props;
    }
}