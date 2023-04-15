using Verse;
using RimWorld;
using UnityEngine;

namespace BiomesCore
{
    public class CompProperties_PlantReleaseGas : CompProperties
    {
        public GasType gasType;
        public float cellsToFill;
        public EffecterDef effecterReleasing;
        public float growthProgress = 1f;

        public CompProperties_PlantReleaseGas() => compClass = typeof(CompPlantReleaseGas);
    }

    public class CompPlantReleaseGas : ThingComp
    {
        public CompProperties_PlantReleaseGas Props => (CompProperties_PlantReleaseGas)props;

        private Effecter effecter;

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            this.effecter?.Cleanup();
            this.effecter = (Effecter)null;
        }

        public override void CompTickLong()
        {
            base.CompTickLong();

            if (parent.Map == null || !(parent is Plant plant))
                return;
            if (plant.Growth >= Props.growthProgress && !plant.Dying)
            {
                float radius = Mathf.Round(plant.Growth * this.Props.cellsToFill);
                GasUtility.AddGas(this.parent.PositionHeld, this.parent.MapHeld, this.Props.gasType, radius);
                if (this.Props.effecterReleasing != null)
                {
                    if (this.effecter == null)
                        this.effecter = this.Props.effecterReleasing.Spawn((Plant)this.parent, this.parent.MapHeld);
                    this.effecter.EffectTick((TargetInfo)(Plant)this.parent, (TargetInfo)(Plant)this.parent);
                }
            }
        }
    }
}