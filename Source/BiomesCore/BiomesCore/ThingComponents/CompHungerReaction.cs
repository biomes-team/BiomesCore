using BiomesCore.ThingComponents;
using Verse;

namespace BiomesCore
{
    public class CompHungerReaction : CompDynamicAnimalGraphic
    {
        private CompProperties_HungerReaction Props => (CompProperties_HungerReaction)props;

        public override bool Active()
        {
            if (parent is Pawn pawn && pawn.needs?.food != null)
                return pawn.needs.food.CurLevelPercentage <= Props.maxFood;
            return false;
        }

        private bool prevActive = false;

        public override void CompTick()
        {
            if (parent.IsHashIntervalTick(60) && parent is Pawn pawn && pawn.needs?.food != null)
                if (pawn.needs.food.CurLevelPercentage <= Props.maxFood)
                {
                    if (!prevActive)
                    {
                        if (!Props.hediffs.NullOrEmpty())
                            foreach (var hediff in Props.hediffs)
                                pawn.health.AddHediff(hediff);
                        prevActive = true;
                        ForceGraphicUpdateNow();
                    }
                }
                else
                {
                    if (prevActive)
                    {
                        if (!Props.hediffs.NullOrEmpty())
                        {
                            foreach (HediffDef hediff in Props.hediffs)
                            {
                                Hediff target = pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
                                if (target != null)
                                    pawn.health.RemoveHediff(target);
                            }
                        }
                        prevActive = false;
                        ForceGraphicUpdateNow();
                    }
                }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref prevActive, "prevActive", false);
        }
    }
}
