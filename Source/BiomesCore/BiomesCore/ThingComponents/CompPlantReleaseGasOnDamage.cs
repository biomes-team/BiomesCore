using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesCore
{
    public class CompProperties_PlantReleaseGasOnDamage : CompProperties
    {
        public GasType gasType;
        public float cellsToFill;
        public EffecterDef effecterReleasing;
        public float growthProgress = 1f;

        public CompProperties_PlantReleaseGasOnDamage() =>
            compClass = typeof(CompPlantReleaseGasOnDamage);
    }


    public class CompPlantReleaseGasOnDamage : ThingComp
    {
        public CompProperties_PlantReleaseGasOnDamage Props =>
            (CompProperties_PlantReleaseGasOnDamage)props;

        private Effecter effecter;

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);

            if (parent.Map == null || !(parent is Plant plant))
                return;
            if (!(plant.Growth >= Props.growthProgress) || plant.Dying) return;

            var radius = Mathf.Round(plant.Growth * Props.cellsToFill);
            GasUtility.AddGas(parent.PositionHeld, parent.MapHeld, Props.gasType, radius);
            if (Props.effecterReleasing != null)
            {
                effecter ??= Props.effecterReleasing.Spawn((Plant)parent, parent.MapHeld);
                effecter.EffectTick((TargetInfo)(Plant)parent, (TargetInfo)(Plant)parent);
            }
        }
    }
}