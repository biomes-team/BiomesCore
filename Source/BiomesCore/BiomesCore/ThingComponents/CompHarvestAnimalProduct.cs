using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BiomesCore
{
    public class CompProperties_HarvestAnimalProduct : CompProperties
    {
        public int harvestIntervalDays = 1;
        public int baseResourceAmount = 1;
        public ThingDef thingDef;
        public Gender fixedGender = Gender.None;
        public string resourceGrowthReportString = null;

        public CompProperties_HarvestAnimalProduct() => compClass = typeof(CompHarvestAnimalProduct);

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (var error in base.ConfigErrors(parentDef))
            {
                yield return error;
            }

            if (resourceGrowthReportString == null)
            {
                yield return "CompProperties_HarvestAnimalProduct must set a resourceGrowthReportString.";
            }

            if (thingDef == null)
            {
                yield return "CompProperties_HarvestAnimalProduct must set a thingDef to be produced.";
            }
        }
    }

    public class CompHarvestAnimalProduct : CompHasGatherableBodyResource
    {

        protected override int GatherResourcesIntervalDays => this.Props.harvestIntervalDays;

        protected override int ResourceAmount => parent is Pawn pawn ?
            GenMath.RoundRandom(pawn.ageTracker.CurLifeStage.bodySizeFactor * Props.baseResourceAmount) : 0;

        protected override ThingDef ResourceDef => Props.thingDef;

        public override string CompInspectStringExtra() => !Active ? null : Props.resourceGrowthReportString.Translate() + ": " + Fullness.ToStringPercent();

        protected override string SaveKey => Props.resourceGrowthReportString;

        public CompProperties_HarvestAnimalProduct Props => (CompProperties_HarvestAnimalProduct)props;

        protected override bool Active =>base.Active && parent is Pawn pawn &&
                                         (Props.fixedGender == Gender.None || Props.fixedGender == pawn.gender);

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = $"Finish production of {ResourceDef.label}",
                    icon = TexCommand.ForbidOff,
                    action = () =>fullness = 1F
                };
            }
        }
    }
}