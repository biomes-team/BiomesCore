using RimWorld;
using Verse;

namespace BiomesCore
{
    public class CompProperties_ShootGrowth : CompProperties
    {
        public ThingDef shootGrowthThingDef = null;
        public float growthProgress = 1f;
        //public bool destroyBasePlant = false;

        public CompProperties_ShootGrowth() => this.compClass = typeof(Plant_ShootGrowth);
    }
    public class Comp_ShootGrowth : ThingComp
    {
        public CompProperties_ShootGrowth Props => (CompProperties_ShootGrowth)this.props;
    }
    public class Plant_ShootGrowth : Plant
    {
        public override void TickLong()
        {
            base.TickLong();

            if (!Spawned) return;

            var props = def.GetCompProperties<CompProperties_ShootGrowth>();
            //var props = this.GetComp<Comp_ShootGrowth>()?.Props;

            if (props != null && Growth >= props.growthProgress)
            {
                Thing newPlant = ThingMaker.MakeThing(props.shootGrowthThingDef);
                GenSpawn.Spawn(newPlant, this.Position, this.Map);
                this.Destroy();
            }
            
        }
    } 
}