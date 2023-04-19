using System;
using System.Collections.Generic;
using Verse;

namespace BiomesCore
{
    public class CompProperties_CleanFilthAround : CompProperties
    {
        public float radius;
        public int tickRate = 2500;
        public CompProperties_CleanFilthAround() => this.compClass = typeof(CompCleanFilthAround);
    }

    public class CompCleanFilthAround : ThingComp
    {
        public CompProperties_CleanFilthAround Props => this.props as CompProperties_CleanFilthAround;

        public override void CompTick()
        {
            base.CompTick();
            if (this.parent.IsHashIntervalTick(this.Props.tickRate))
            {
                ClearFilth();
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            if (this.parent.IsHashIntervalTick(this.Props.tickRate))
            {
                ClearFilth();
            }
        }

        public override void CompTickLong()
        {
            base.CompTickLong();
            if (this.parent.IsHashIntervalTick(this.Props.tickRate))
            {
                ClearFilth();
            }
        }

        private void ClearFilth()
        {
            List<Thing> filth = this.parent.Map.listerThings.ThingsInGroup(ThingRequestGroup.Filth);
            for (int index = 0; index < filth.Count; ++index)
            {
                if (filth[index].Position.InHorDistOf(this.parent.Position, this.Props.radius))
                {
                    filth[index].Destroy();
                    break;
                }
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            CompCleanFilthAround CompCleanFilthAround = this;
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action commandAction = new Command_Action();
                commandAction.defaultLabel = "DEV: Clean";
                commandAction.action = new Action(CompCleanFilthAround.ClearFilth);
                yield return (Gizmo)commandAction;
            }
        }
    }
}