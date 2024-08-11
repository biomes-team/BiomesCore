using Verse;

namespace BiomesCore.ThingComponents
{
    public class CompSpawnGasOnTakeDamage : ThingComp
    {
        public CompProperties_SpawnGasOnTakeDamage Props => (CompProperties_SpawnGasOnTakeDamage)props;

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (parent.Spawned && Rand.Chance(Props.chance))
            {
                GasUtility.AddGas(parent.Position, parent.Map, Props.gasType,
                    Props.amount);
            }
        }
    }

    public class CompProperties_SpawnGasOnTakeDamage : CompProperties
    {
        public CompProperties_SpawnGasOnTakeDamage()
        {
            compClass = typeof(CompSpawnGasOnTakeDamage);
        }

        public GasType gasType = GasType.ToxGas;

        public float chance = 1f;
        public int amount = 255;
    }
}