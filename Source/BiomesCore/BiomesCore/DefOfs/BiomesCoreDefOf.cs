using RimWorld;
using Verse;
using Verse.AI;

namespace BiomesCore
{
    [DefOf]
    public static class BiomesCoreDefOf
    {
        public static DutyDef BMT_WanderAroundPoint;
        
        public static GameConditionDef Earthquake;
        
        public static IncidentDef CaveIn;

        public static JobDef BC_BloodDrinking;
        public static JobDef BC_BottomFeeder;
        public static JobDef BC_EatCustomThing;
        public static JobDef BC_HarvestAnimalProduct;
        public static JobDef BC_HermaphroditicMate;

        public static RoofDef BMT_RockRoofStable;
        static BiomesCoreDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BiomesCoreDefOf));
        }
    }
}
