using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BiomesCore.DefModExtensions
{
	public class BiomesMap : DefModExtension
	{
		public bool plantTaggingSystemEnabled = true;

		// ISLANDS
		public bool isIsland = false;
		public List<IslandShape> islandShapes = new List<IslandShape>();
		public bool addIslandHills = true;
		public bool allowBeach = true;

        public FloatRange islandSizeMapPct = new FloatRange(.45f, .55f);
        public float islandCtrVarPct = .15f;
		public TerrainDef islandDeepTerrain = TerrainDefOf.WaterOceanDeep;
        public TerrainDef islandShallowTerrain = TerrainDefOf.WaterOceanShallow;
        public TerrainDef islandBeachTerrain = TerrainDefOf.Sand;
        public float islandBeachPct = .2f;
        public FloatRange islandNoiseRange0_10 = new FloatRange(4, 5);
        public FloatRange islandOceanDepth0_10 = new FloatRange(4, 5);


		// MISC
        public bool isValley = false;
		public List<ValleyShape> valleyShapes = new List<ValleyShape>();

		public bool isCavern = false;
		public List<CavernShape> cavernShapes = new List<CavernShape>();

        //OASIS
        public TerrainDef oasisOuterTerrain = TerrainDefOf.Soil;
        public TerrainDef oasisShoreTerrain = TerrainDefOf.SoilRich;
        public TerrainDef oasisShallowTerrain = TerrainDefOf.WaterShallow;
		public TerrainDef oasisDeepTerrain = TerrainDefOf.WaterDeep;

		public float oasisShallowPct = .5f;
		public float oasisShorePct = .2f;
		public float oasisOuterPct = .2f;
		public FloatRange oasisNoiseRange0_10 = new FloatRange(4, 5);

        // The center of the oasis will be within x % of the center of the map.
        // Placement within allowed range is random.
        // .50 allows the oasis to be centered on map edge. 0 requires the oasis to be centered on map center.
        public float oasisCtrVarPct = .15f;

		// The % of map width that the oasis should take up.
		// For non-square map sizes, is based on width only.
		public FloatRange oasisSizeMapPct = new FloatRange(.45f, .55f);


        //public bool hasRuins = true;
        public bool hasScatterables = true;
		public float minHillEncroachment = 1;
		public float maxHillEncroachment = 1;
		public float minHillEdgeMultiplier = 1;
		public float maxHillEdgeMultiplier = 1;

		/// <summary>
		/// Adds these pack animals as possible choices for all factions visiting the biome. The BiomeDef's
		/// allowedPackAnimals list can be used to control which animals are allowed as carriers.
		/// </summary>
		public List<PawnGenOption> extraCarriers;
		
		/// <summary>
		/// If this value is 1, it is equally probable to choose an original carrier or an extra carrier. Increasing it will
		/// make extra carriers proportionally more probable.
		/// </summary>
		public float extraCarriersRelativeWeight;

		public bool alwaysGrowthSeason;

		public float seasonalTemperatureShift = 1f;

		/// <summary>
		/// These weathers will never happen in the biome.
		/// </summary>
		public List<WeatherDef> disallowedWeathers;

		/// <summary>
		/// If enabled, the BiomeDef.texture will be drawn using the Ocean material. Useful for islands.
		/// </summary>
		public bool useOceanMaterial;

		/// <summary>
		/// Texture to be drawn as a transparent overlay on top of the normal biome texture.
		/// </summary>
		public string overlayTexturePath;
	}

	public enum IslandShape
    {
		Smooth,
		Round,
		Crescent,
		Pair,
		Cluster,
		Broken
	}

	public enum ValleyShape
	{
		Linear
	}

	public enum CavernShape
    {
		Vanilla,
		TunnelNetwork,
		Smooth,
		Tubes,
		LargeChambers,
		SmallChambers
    }
}
