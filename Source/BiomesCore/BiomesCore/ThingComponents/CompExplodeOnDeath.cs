using RimWorld;
using Verse;

namespace BiomesCore.ThingComponents
{
    public class CompExplodeOnDeath : ThingComp
    {
        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            GenExplosion.DoExplosion(
                parent.Position, prevMap, Props.explosiveRadius,
                Props.explosiveDamageType, parent, Props.damageAmountBase,
                Props.armorPenetrationBase, Props.explosionSound, null, null, null,
                Props.postExplosionSpawnThingDef, Props.postExplosionSpawnChance,
                Props.postExplosionSpawnThingCount, Props.postExplosionGasType,
                Props.applyDamageToExplosionCellsNeighbors,
                Props.preExplosionSpawnThingDef, Props.preExplosionSpawnChance,
                Props.preExplosionSpawnThingCount, Props.chanceToStartFire,
                Props.damageFalloff, null, null, null, Props.doVisualEffects,
                Props.propagationSpeed, 0f, Props.doSoundEffects
            );
            base.Notify_Killed(prevMap, dinfo);

        }

        public CompProperties_ExplodeOnDeath Props =>
            (CompProperties_ExplodeOnDeath)props;
    }

    public class CompProperties_ExplodeOnDeath : CompProperties_Explosive
    {
        public CompProperties_ExplodeOnDeath()
        {
            compClass = typeof(CompExplodeOnDeath);
        }
    }
}