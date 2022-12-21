using BiomesCore.DefModExtensions;
using RimWorld;
using Verse;

namespace BiomesCore
{
    public class CompProperties_ShootGrowth : CompProperties
    {
        public ThingDef shootGrowthThingDef;
        public float growthProgress = 1f; //add later to give more controll

        public CompProperties_ShootGrowth() => compClass = typeof(CompShootGrowth);
    }
    public class CompShootGrowth : ThingComp
    {
        public CompProperties_ShootGrowth Props => (CompProperties_ShootGrowth)props;

        public override void CompTickLong()
        {
            base.CompTickLong();

            if (parent.Map == null || !(parent is Plant oldPlant)) return;
            if (oldPlant.Growth >= Props.growthProgress) //change from == to >= add progess point
            {
                Thing newThing = ThingMaker.MakeThing(Props.shootGrowthThingDef);
                GenSpawn.Spawn(newThing, parent.Position, parent.Map);
                //oldPlant.Destroy();
            }
        }
    }
}
namespace BiomesCore
{
    public class CompProperties_ClassShootGrowthxxx : CompProperties
    {
        public ThingDef shootGrowthThingDef = null;
        public float growthProgress = 1f;
        //public bool destroyBasePlant = false;

        public CompProperties_ClassShootGrowthxxx() => this.compClass = typeof(Plant_ClassShootGrowthxxx);
    }
    public class Comp_ClassShootGrowthxxx : ThingComp
    {
        public CompProperties_ClassShootGrowthxxx Props => (CompProperties_ClassShootGrowthxxx)this.props;
    }
    public class Plant_ClassShootGrowthxxx : Plant
    {
        public override void TickLong()
        {
            base.TickLong();

            if (!Spawned) return;

            var props = def.GetCompProperties<CompProperties_ClassShootGrowthxxx>();
            //var props = this.GetComp<Comp_ShootGrowth>()?.Props;

            if (props != null && growthInt >= props.growthProgress)
            {
                Thing newPlant = ThingMaker.MakeThing(props.shootGrowthThingDef);
                GenSpawn.Spawn(newPlant, this.Position, this.Map);
                this.Destroy();
            }
        }
        protected override bool Resting => IsResting;
        bool IsResting
        {
            get
            {
                if (def.HasModExtension<Biomes_PlantControl>())
                {
                    Biomes_PlantControl ext = def.GetModExtension<Biomes_PlantControl>();
                    if (ext.needsRest)
                    {
                        if (!(GenLocalDate.DayPercent(this) < ext.growingHours.min))
                        {
                            return GenLocalDate.DayPercent(this) > ext.growingHours.max;
                        }
                        return true;
                    }
                    return false;
                }
                // returns the default Resting value without having to copy code
                return base.Resting;
            }
        }
    }
}