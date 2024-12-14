using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesCore
{
    public class CompProperties_PlantReleaseGasOnDeath : CompProperties
    {
        public GasType gasType;
        public float cellsToFill;
        public EffecterDef effecterReleasing;
        public float growthProgress = 1f;

        public CompProperties_PlantReleaseGasOnDeath() =>
            compClass = typeof(CompPlantReleaseGasOnDeath);
    }

    public class CompPlantReleaseGasOnDeath : ThingComp
    {
        public CompProperties_PlantReleaseGasOnDeath Props =>
            (CompProperties_PlantReleaseGasOnDeath)props;

        private Effecter effecter;

        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            base.Notify_Killed(prevMap, dinfo);

            if (parent.Map == null || !(parent is Plant plant))
                return;
            if (!(plant.Growth >= Props.growthProgress)) return;

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