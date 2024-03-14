using RimWorld;
using Verse;

namespace BiomesCore
{
    public class JobDriver_HarvestAnimalProduct : JobDriver_GatherAnimalBodyResources
    {
        protected override float WorkTotal => 400f;

        protected override CompHasGatherableBodyResource GetComp(
          Pawn animal)
        {
            return (CompHasGatherableBodyResource)animal.TryGetComp<CompHarvestAnimalProduct>();
        }
    }

    public class WorkGiver_HarvestAnimalProduct : WorkGiver_GatherAnimalBodyResources
    {
        protected override JobDef JobDef => BiomesCoreDefOf.BMT_HarvestAnimalProduct;

        protected override CompHasGatherableBodyResource GetComp(
          Pawn animal)
        {
            return (CompHasGatherableBodyResource)animal.TryGetComp<CompHarvestAnimalProduct>();
        }
    }
}