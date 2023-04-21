using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BiomesCore.DefModExtensions
{
	public class BiomesMap : DefModExtension
	{
		public bool plantTaggingSystemEnabled = true;

		public bool isIsland = false;
		public List<IslandShape> islandShapes = new List<IslandShape>();
		public bool addIslandHills = true;
		public bool allowBeach = true;

		public bool isValley = false;
		public List<ValleyShape> valleyShapes = new List<ValleyShape>();

		public bool isCavern = false;
		public List<CavernShape> cavernShapes = new List<CavernShape>();

		public bool isOasis = false;
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
	}

	public enum IslandShape
    {
		Smooth,
		Rough,
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
