using RimWorld;
using Verse;

namespace BiomesCore
{
    [DefOf]
    public static class BiomesCoreDefOf
    {
        public static RoofDef BMT_RockRoofStable;
        public static IncidentDef CaveIn;
        public static GameConditionDef Earthquake;
        public static JobDef BC_BloodDrinking;
        public static JobDef BC_BottomFeeder;
        public static JobDef BC_EatCustomThing;
        public static JobDef BC_HarvestAnimalProduct;

        static BiomesCoreDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BiomesCoreDefOf));
        }
    }
}
