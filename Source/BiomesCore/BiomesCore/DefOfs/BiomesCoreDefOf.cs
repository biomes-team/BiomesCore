using RimWorld;
using Verse;
using Verse.AI;

namespace BiomesCore
{
	[DefOf]
	public static class BiomesCoreDefOf
	{
		public static ThingDef BMT_LavaGenerator;

		public static ConceptDef BMT_HungeringAnimalsConcept;

		public static DutyDef BMT_WanderAroundPoint;

		public static GameConditionDef BMT_Earthquake;

		public static HediffDef BMT_HungeringHediff;
		public static IncidentDef BMT_CaveIn;

		public static JobDef BMT_BloodDrinking;
		public static JobDef BMT_BottomFeeder;
		public static JobDef BMT_DevourHungering;
		public static JobDef BMT_EatCustomThing;
		public static JobDef BMT_HarvestAnimalProduct;
		public static JobDef BMT_HermaphroditicMate;
		//public static JobDef BC_Cough;

		public static MentalStateDef BMT_Hungering;

		//public static TaleDef BMT_Coughed;

		[MayRequireBiotech] public static EffecterDef CellPollution;
		//public static EffecterDef BMT_CoughBlood;

		public static SoundDef BMT_EarthquakeSound;
		//public static SoundDef BMT_Cough;

		//public static HediffDef BMT_Sputum;

		public static RoofDef BMT_RockRoofStable;

		public static TerrainAffordanceDef BMT_TerrainAffordance_Lava;

		static BiomesCoreDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(BiomesCoreDefOf));
		}
	}
}