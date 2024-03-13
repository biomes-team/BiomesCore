using RimWorld;
using Verse;

namespace BiomesCore.Locations
{
	/// <summary>
	/// This location is created by other locations which need to burn things around them. It will be removed after all
	/// burners have also been removed.
	/// </summary>
	public class BurnThingsLocation : Location
	{
		private int burnAroundInstances;

		public BurnThingsLocation(Map map, IntVec3 position)
		{
			Map = map;
			Position = position;
			burnAroundInstances = 1;
		}

		public override void Tick(int gameTick)
		{
			TerrainDef terrainDef = Position.GetTerrain(Map);
			if (terrainDef.passability == Traversability.Impassable)
			{
				// Impassable tiles are assumed to either be empty of things which can burn (magma) or not capable of sustaining
				// a fire (water).
				return;
			}

			Map.snowGrid.AddDepth(Position, -0.06f);
			float chance = FireUtility.ChanceToStartFireIn(Position, Map) / 4.0F;
			if (Rand.Chance(chance))
			{
				FireUtility.TryStartFireIn(Position, Map, 0.1F, null);
			}
			else if (chance > 0.0F)
			{
				FleckMaker.ThrowMicroSparks(Position.ToVector3Shifted(), Map);
			}
		}

		public void AddBurner()
		{
			++burnAroundInstances;
		}

		public bool RemoveBurner()
		{
			--burnAroundInstances;
			return burnAroundInstances <= 0;
		}
	}
}