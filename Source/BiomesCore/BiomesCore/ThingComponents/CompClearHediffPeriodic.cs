using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCore
{
    public class CompClearHediffPeriodic : ThingComp
    {
        CompProperties_ClearHediffPeriodic Props => (CompProperties_ClearHediffPeriodic)props;

        public Pawn Wearer
        {
            get
            {
                return !(this.ParentHolder is Pawn_ApparelTracker parentHolder) ? (Pawn)null : parentHolder.pawn;
            }
        }

        int ticks = 0;

        const int TicksInDay = 60000;

        float maxTicks => TicksInDay * Props.daysCooldown;
        public override void CompTickRare()
        {
            if(Wearer == null) {return; }

            ticks += 250;

            float days = (float)ticks / maxTicks;

            if(days >= maxTicks)
            {
                ticks = 0;
                ClearHediff();
            }
            
        }

        void ClearHediff()
        {
            Hediff hediff = Wearer.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
            if(hediff != null)
            {
                Wearer.health.RemoveHediff(hediff);
            }
        }

    }

    public class CompProperties_ClearHediffPeriodic : CompProperties
    {
        public float daysCooldown;

        public HediffDef hediffDef;

        public CompProperties_ClearHediffPeriodic() => compClass = typeof(CompClearHediffPeriodic);
    }
}
