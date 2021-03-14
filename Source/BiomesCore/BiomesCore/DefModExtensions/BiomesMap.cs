using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCore.DefModExtensions
{
	public class BiomesMap : DefModExtension
	{
		public bool isIsland = false;
		public List<IslandShape> islandShapes = new List<IslandShape>();

		public bool isCavern = false;
		public bool addIslandHills = true;
		public bool allowBeach = true;
		//public bool hasRuins = true;
		public bool hasScatterables = true;
		public bool isValley = false;
		public float minHillEncroachment = 1;
		public float maxHillEncroachment = 1;
		public float minHillEdgeMultiplier = 1;
		public float maxHillEdgeMultiplier = 1;
		public bool isOasis = false;
	}

	public enum IslandShape
    {
		Smooth,
		Rough,
		Crescent,
		Pair
    }
}
