using System;
using Verse;

namespace BiomesCore.Locations
{
	/// <summary>
	/// As stated in Location.cs, the location system is stateless and lacks persistence. This class is responsible of
	/// regenerating all locations on game load, and for managing their life cycle through the game.
	/// </summary>
	public class LocationGrid : MapComponent
	{
		// Currently there is only one terrain location which has a rare ticker type. This can be expanded as needed.
		private TickableLocationsByTickerType rareTickTerrainLocations = new TickableLocationsByTickerType(TickerType.Rare);

		// Burn things locations are stored separately, as other locations need to retrieve and instantiate them.
		private TickableLocationsByTickerType burnThingsInLocation = new TickableLocationsByTickerType(TickerType.Rare);

		private bool mapGenerated = false;

		public LocationGrid(Map map) : base(map)
		{
		}

		public void TerrainChanged(IntVec3 position, TerrainDef previousTerrainDef)
		{
			if (!mapGenerated)
			{
				// Terrain changes prior to the map being fully generated are dropped.
				return;
			}

			TerrainLocationDefExtension previousDefExtension = GetExtension(previousTerrainDef);
			if (previousDefExtension != null)
			{
				// ToDo: This assumes that all terrains have a rare ticker type, which may not be the case in the future.
				if (rareTickTerrainLocations.Remove(map, position) is TerrainLocation previousInstance)
				{
					previousInstance.TerrainRemoved(this);
				}
			}

			TerrainLocationDefExtension terrainLocationDefExtension = GetExtension(position.GetTerrain(map));
			// ToDo: This assumes that all terrains have a rare ticker type, which may not be the case in the future.
			if (terrainLocationDefExtension != null)
			{
				TerrainLocation terrain =
					(TerrainLocation) Activator.CreateInstance(terrainLocationDefExtension.TerrainLocationType());
				terrain.Initialize(terrainLocationDefExtension, map, position);
				// ToDo: This assumes that all terrains have a rare ticker type, which may not be the case in the future.
				rareTickTerrainLocations.Add(terrain);
				terrain.TerrainRegistered(this);
			}
		}

		/// <summary>
		/// Executed when map generation or loading have finished.
		/// </summary>
		public override void FinalizeInit()
		{
			mapGenerated = true;
			foreach (IntVec3 cell in map.AllCells)
			{
				TerrainChanged(cell, null);
			}
		}

		private static TerrainLocationDefExtension GetExtension(TerrainDef terrainDef)
		{
			if (terrainDef?.modExtensions == null)
			{
				return null;
			}

			foreach (DefModExtension defExtension in terrainDef.modExtensions)
			{
				if (defExtension is TerrainLocationDefExtension terrainLocationDefExtension)
				{
					return terrainLocationDefExtension;
				}
			}

			return null;
		}

		public override void MapComponentTick()
		{
			int gameTick = Find.TickManager.TicksGame;
			// ToDo: This assumes that all terrains have a rare ticker type, which is currently the case.
			rareTickTerrainLocations.Tick(gameTick);
			burnThingsInLocation.Tick(gameTick);
		}

		public void AddThingBurnerAt(IntVec3 position)
		{
			// Custom version of GenAdj.CellsAdjacent8Way which ignores cells outside of the map.
			foreach (IntVec3 adjacentOffset in GenAdj.AdjacentCells)
			{
				IntVec3 adjacentCell = position + adjacentOffset;
				if (!adjacentCell.InBounds(map))
				{
					continue;
				}

				if (burnThingsInLocation.Get(map, adjacentCell) is BurnThingsLocation burnThingsInstance)
				{
					burnThingsInstance.AddBurner();
				}
				else
				{
					burnThingsInLocation.Add(new BurnThingsLocation(map, adjacentCell));
				}
			}
		}

		public void RemoveThingBurnerAt(IntVec3 position)
		{
			// Custom version of GenAdj.CellsAdjacent8Way which ignores cells outside of the map.
			foreach (IntVec3 adjacentOffset in GenAdj.AdjacentCells)
			{
				IntVec3 adjacentCell = position + adjacentOffset;
				if (!adjacentCell.InBounds(map))
				{
					continue;
				}

				if (burnThingsInLocation.Get(map, adjacentCell) is BurnThingsLocation burnThingsInstance)
				{
					bool allRemoved = burnThingsInstance.RemoveBurner();
					if (allRemoved)
					{
						burnThingsInLocation.Remove(map, adjacentCell);
					}
				}
				else
				{
					burnThingsInLocation.Add(new BurnThingsLocation(map, adjacentCell));
				}
			}
		}
	}
}