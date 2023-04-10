using System.Collections.Generic;
using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace BiomesCore
{
    public class CompProperties_PlantPolluteOverTime : CompProperties
    {
        public int cellsToPollutePerDay;
        public float growthProgress = 1f;

        public CompProperties_PlantPolluteOverTime() => this.compClass = typeof(CompPlantPolluteOverTime);
    }

    public class CompPlantPolluteOverTime : ThingComp
    {
        private CompProperties_PlantPolluteOverTime Props => (CompProperties_PlantPolluteOverTime)this.props;

        private int TicksToPolluteCell => 30 / this.Props.cellsToPollutePerDay;

        public override void CompTickLong()
        {
            if (!this.parent.Spawned || !this.parent.IsHashIntervalTick(this.TicksToPolluteCell))
                return;
            if (parent.Map == null || !(parent is Plant plant)) return;
            if (plant.Growth >= Props.growthProgress)
            {
                this.Pollute();
            }
                
        }

        private void Pollute()
        {
            if (!ModsConfig.BiotechActive)
                return;
            int num = GenRadial.NumCellsInRadius(GenRadial.MaxRadialPatternRadius - 1f);
            for (int index = 0; index < num; ++index)
            {
                IntVec3 intVec3 = this.parent.Position + GenRadial.RadialPattern[index];
                if (!intVec3.IsPolluted(this.parent.Map) && intVec3.CanPollute(this.parent.Map))
                {
                    intVec3.Pollute(this.parent.Map);
                    this.parent.Map.effecterMaintainer.AddEffecterToMaintain(BiomesCoreDefOf.CellPollution.Spawn(intVec3, this.parent.Map, Vector3.zero), intVec3, 45);
                    break;
                }
            }
        }

        public override string CompInspectStringExtra() => (string)("TilePollution".Translate() + ": " + "CellsPerDay".Translate((NamedArgument)this.Props.cellsToPollutePerDay));

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            CompPlantPolluteOverTime compPlantPolluteOverTime = this;
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action commandAction = new Command_Action();
                commandAction.defaultLabel = "DEV: Pollute";
                commandAction.action = new Action(compPlantPolluteOverTime.Pollute);
                yield return (Gizmo)commandAction;
            }
        }
    }
}