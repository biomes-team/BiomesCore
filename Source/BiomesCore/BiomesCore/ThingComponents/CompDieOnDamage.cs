using Verse;

namespace BiomesCore.ThingComponents
{
    public class CompDieOnDamage : ThingComp
    {
        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
            if (Rand.Chance(Props.procChance))
            {
                parent.Kill();
            }
        }

        public CompProperties_DieOnDamage Props => props as CompProperties_DieOnDamage;
    }

    public class CompProperties_DieOnDamage : CompProperties
    {
        public float procChance = 1f;
        
        public CompProperties_DieOnDamage()
        {
            compClass = typeof(CompDieOnDamage);
        }
    }
}