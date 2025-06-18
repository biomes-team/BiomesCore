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
                damType: Props.explosiveDamageType, instigator: parent, damAmount: Props.damageAmountBase,
                armorPenetration: Props.armorPenetrationBase, explosionSound: Props.explosionSound, null, null, null,
                Props.postExplosionSpawnThingDef, postExplosionSpawnChance: Props.postExplosionSpawnChance,
                postExplosionSpawnThingCount: Props.postExplosionSpawnThingCount, postExplosionGasType: Props.postExplosionGasType, 0f, 0,
                applyDamageToExplosionCellsNeighbors: Props.applyDamageToExplosionCellsNeighbors,
                preExplosionSpawnThingDef: Props.preExplosionSpawnThingDef, preExplosionSpawnChance: Props.preExplosionSpawnChance,
                preExplosionSpawnThingCount: Props.preExplosionSpawnThingCount, chanceToStartFire: Props.chanceToStartFire,
                damageFalloff: Props.damageFalloff, null, null, null, doVisualEffects: Props.doVisualEffects,
                propagationSpeed: Props.propagationSpeed, 0f, doSoundEffects: Props.doSoundEffects
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