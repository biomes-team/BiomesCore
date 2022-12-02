using RimWorld;
using Verse;

namespace BiomesCore
{
    public class CompProperties_HarvestAnimalProduct : CompProperties
    {
        public int harvestIntervalDays;
        public int baseResourceAmount = 1;
        public ThingDef thingDef;
        public Gender fixedGender = Gender.None;
        public string resourceGrowthReportString = "ThingGrowth";

        public CompProperties_HarvestAnimalProduct() => this.compClass = typeof(CompHarvestAnimalProduct);
    }

    public class CompHarvestAnimalProduct : CompHasGatherableBodyResource
    {

        protected override int GatherResourcesIntervalDays => this.Props.harvestIntervalDays;

        protected override int ResourceAmount => parent is Pawn pawn ? (int)GenMath.RoundRandom(pawn.ageTracker.CurLifeStage.bodySizeFactor * this.Props.baseResourceAmount) : 0;

        protected override ThingDef ResourceDef => this.Props.thingDef;

        public override string CompInspectStringExtra() => !this.Active ? (string)null : (string)(this.Props.resourceGrowthReportString.Translate() + ": " + this.Fullness.ToStringPercent());

        protected override string SaveKey => this.Props.resourceGrowthReportString;

        public CompProperties_HarvestAnimalProduct Props => (CompProperties_HarvestAnimalProduct)this.props;

        protected override bool Active
        {
            get
            {
                if (!base.Active)
                    return false;
                return parent is Pawn pawn && (Props.fixedGender == Gender.None || Props.fixedGender == pawn.gender);
            }
        }
    }
}