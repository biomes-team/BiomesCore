using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;

namespace BiomesCore
{

    public class HediffCompProperties_ClearHediffPeriodic : HediffCompProperties
    {
        public float daysCooldown;

        public HediffDef hediffDef;

        public HediffCompProperties_ClearHediffPeriodic() => compClass = typeof(HediffCompClearHediffPeriodic);
    }
    public class HediffCompClearHediffPeriodic : HediffComp
    {
        int ticks = 0;

        const int TicksInDay = 60000;

        float maxTicks => TicksInDay * Props.daysCooldown;

        HediffCompProperties_ClearHediffPeriodic Props => (HediffCompProperties_ClearHediffPeriodic)props;

        void ClearHediff()
        {
            Hediff hediff = Pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
            if (hediff != null)
            {
                Pawn.health.RemoveHediff(hediff);
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            ticks++;

            if (ticks >= maxTicks)
            {
                ticks = 0;
                ClearHediff();
            }
        }

    }
}
