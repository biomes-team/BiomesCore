using Verse;

namespace BiomesCore
{
    public class CompProperties_BottomFeeder : CompProperties
    {
        public int tickInterval = 250;
        public string consumingFoodReportString = "Eating food";
        public EffecterDef effecterDef;
        public string feedingTerrainTag;
        public bool shouldGlow = false; // For this to work, you need to give them the comp "CompProperties_DefaultOffGlower", too.
        public CompProperties_BottomFeeder() => this.compClass = typeof(CompBottomFeeder);
    }
    public class CompBottomFeeder : ThingComp
    {
        public CompProperties_BottomFeeder Props => (CompProperties_BottomFeeder)this.props;

    }
}
