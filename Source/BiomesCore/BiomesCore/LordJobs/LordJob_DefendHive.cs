using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI.Group;
using Verse;

namespace BiomesCore.LordJobs
{
    public class LordJob_DefendHive : LordJob
    {
        private IntVec3 point;
        private float? wanderRadius;
        private float? defendRadius;
        private bool isCaravanSendable;
        private bool addFleeToil;

        public override bool IsCaravanSendable => this.isCaravanSendable;

        public override bool AddFleeToil => this.addFleeToil;

        public LordJob_DefendHive()
        {
        }

        public LordJob_DefendHive(
          IntVec3 point,
          float? wanderRadius = null,
          float? defendRadius = null,
          bool isCaravanSendable = false,
          bool addFleeToil = true)
        {
            this.point = point;
            this.wanderRadius = wanderRadius;
            this.defendRadius = defendRadius;
            this.isCaravanSendable = isCaravanSendable;
            this.addFleeToil = addFleeToil;
        }

        public override StateGraph CreateGraph()
        {
            StateGraph graph = new StateGraph();
            IntVec3 point = this.point;
            float? wanderRadius1 = this.wanderRadius;
            float? defendRadius = this.defendRadius;
            float? wanderRadius2 = wanderRadius1;
            graph.AddToil((LordToil)new LordToil_DefendPoint(point, defendRadius, wanderRadius2));
            return graph;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look<IntVec3>(ref this.point, "point");
            Scribe_Values.Look<float?>(ref this.wanderRadius, "wanderRadius");
            Scribe_Values.Look<float?>(ref this.defendRadius, "defendRadius");
            Scribe_Values.Look<bool>(ref this.isCaravanSendable, "isCaravanSendable");
            Scribe_Values.Look<bool>(ref this.addFleeToil, "addFleeToil");
        }
    }
}
