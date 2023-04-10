using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BiomesCore
{
    public class CompProperties_PlantGasOnDestroy : CompProperties
    {
        public GasType gasType;
        public float cellsToFill;
        public EffecterDef effecterReleasing;
        public float growthProgress = 1f;

        public CompProperties_PlantGasOnDestroy() => compClass = typeof(CompPlantGasOnDestroy);
    }
    public class CompPlantGasOnDestroy : ThingComp
    {
        public CompProperties_PlantGasOnDestroy Props => (CompProperties_PlantGasOnDestroy)props;

        private Effecter effecter;

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            this.effecter?.Cleanup();
            this.effecter = (Effecter)null;

            if (previousMap == null || !(parent is Plant plant)) return;
            if (plant.Growth >= Props.growthProgress)
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