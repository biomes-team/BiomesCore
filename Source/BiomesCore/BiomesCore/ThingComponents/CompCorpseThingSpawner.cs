using RimWorld;
using Verse;

namespace BiomesCore
{
    public class CompProperties_CorpseThingSpawner : CompProperties
    {
        public string filthDef = null;
        public string thingDef = null;
        public int filthAmount = 0;
        public int baseResourceAmount = 1;
        public CompProperties_CorpseThingSpawner() => this.compClass = typeof(CompCorpseThingSpawner);
    }
    public class CompCorpseThingSpawner : ThingComp
    {
        public CompProperties_CorpseThingSpawner Props => (CompProperties_CorpseThingSpawner)this.props;
    }
    public class DeathActionWorker_CorpseThingSpawner : DeathActionWorker
    {
        public override void PawnDied(Corpse corpse)
        {
            CompCorpseThingSpawner comp = corpse.InnerPawn.TryGetComp<CompCorpseThingSpawner>();
            if (comp != null)
            {
                FilthMaker.TryMakeFilth(corpse.Position, corpse.Map, DefDatabase<ThingDef>.GetNamedSilentFail(comp.Props.filthDef), comp.Props.filthAmount, FilthSourceFlags.None);
                Thing thing = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamedSilentFail(comp.Props.thingDef), null);
                thing.stackCount = (int)GenMath.RoundRandom(comp.Props.baseResourceAmount * corpse.InnerPawn.BodySize);
                GenPlace.TryPlaceThing(thing, corpse.Position, corpse.Map, 0, null, null, default);
                corpse.DeSpawn();
            }
        }
    }
}