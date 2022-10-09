using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BiomesCore
{
    public class TerrainCompProperties_Healer : TerrainCompProperties
    {
        public float amountToHeal;
        public TerrainCompProperties_Healer()
        {
            compClass = typeof(TerrainComp_Healer);
        }
    }
    
    public class TerrainComp_Healer : TerrainComp
    {
        public TerrainCompProperties_Healer Props => (TerrainCompProperties_Healer)props;

        [ThreadStatic] // for good ol RimThreaded support
        private static List<Hediff_Injury> tmpHediffInjuries;

        public override void CompTick()
        {
            base.CompTick();
            foreach (var pawn in this.parent.Position.GetThingList(this.parent.Map).OfType<Pawn>())
            {
                var injury = FirstInjuryToThreat(pawn);
                if (injury != null)
                {
                    injury.Heal(Props.amountToHeal);
                    pawn.health.Notify_HediffChanged(injury);
                }
            }
        }

        public Hediff FirstInjuryToThreat(Pawn pawn)
        {
            tmpHediffInjuries ??= new List<Hediff_Injury>();
            pawn.health.hediffSet.GetHediffs(ref tmpHediffInjuries);
            var minorInjuries = new List<Hediff>();
            var permanentInjuries = new List<Hediff>();
            foreach (var injury in tmpHediffInjuries)
            {
                var comp = injury.TryGetComp<HediffComp_GetsPermanent>();
                if (comp is null || !comp.IsPermanent)
                {
                    minorInjuries.Add(injury);
                }
                else if (comp.IsPermanent)
                {
                    permanentInjuries.Add(injury);
                }
            }
            if (minorInjuries.Any())
            {
                return minorInjuries.MinBy(x => x.BleedRate);
            }
            if (permanentInjuries.Any())
            {
                return permanentInjuries.MinBy(x => x.Part.def.GetMaxHealth(pawn) - pawn.health.hediffSet.GetPartHealth(x.Part));
            }
            return null;
        }
    }
}
