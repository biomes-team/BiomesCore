using RimWorld;
using Verse;
using Verse.AI;

namespace BiomesCore
{
    [DefOf]
    public static class BiomesCoreDefOf
    {
        public static ThingDef BMT_LavaGenerator;

        public static DutyDef BMT_WanderAroundPoint;
        
        public static GameConditionDef Earthquake;
        
        public static IncidentDef CaveIn;

        public static JobDef BC_BloodDrinking;
        public static JobDef BC_BottomFeeder;
        public static JobDef BC_EatCustomThing;
        public static JobDef BC_HarvestAnimalProduct;
        public static JobDef BC_HermaphroditicMate;
        //public static JobDef BC_Cough;

        //public static TaleDef BMT_Coughed;

        [MayRequireBiotech]
        public static EffecterDef CellPollution;
        //public static EffecterDef BMT_CoughBlood;

        public static SoundDef EarthquakeSound;
        //public static SoundDef BMT_Cough;

        //public static HediffDef BMT_Sputum;

        public static RoofDef BMT_RockRoofStable;

        public static TerrainAffordanceDef Lava;

        static BiomesCoreDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BiomesCoreDefOf));
        }
    }
}
