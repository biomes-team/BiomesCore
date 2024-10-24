using RimWorld;
using Verse;

namespace BiomesCore
{
    public class Comp_PermanentManhunter : ThingComp
    {
        CompProperties_PermanentManhunter Props => (CompProperties_PermanentManhunter)props;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!respawningAfterLoad && parent is Pawn pawn)
            {
                pawn.mindState?.mentalStateHandler?.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
                if (Props.sendLetter)
                    Find.LetterStack.ReceiveLetter(Props.letterLabel.Translate(pawn.LabelCap).CapitalizeFirst(),
                            Props.letterDesc.Translate(pawn.LabelCap).CapitalizeFirst(), Props.bigThreat ? LetterDefOf.ThreatSmall : LetterDefOf.ThreatBig, pawn);
            }
        }
    }
}
