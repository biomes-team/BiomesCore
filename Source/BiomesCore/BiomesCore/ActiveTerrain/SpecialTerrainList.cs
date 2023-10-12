using System;
using System.Collections.Generic;
using BiomesCore.ActiveTerrain;
using Verse;

namespace BiomesCore
{
	internal struct DefExtensionActiveEntry
	{
		public ActiveTerrainDef def;
		public DefExtensionActive extension;

		public DefExtensionActiveEntry(ActiveTerrainDef newDef, DefExtensionActive newExtension)
		{
			def = newDef;
			extension = newExtension;
		}
	}

	public class SpecialTerrainList : MapComponent
	{
		/// <summary>
		/// Keeps track of every terrain instance in the map, associating it to its position. This dictionary is stored
		/// into savegames.
		/// </summary>
		private Dictionary<IntVec3, TerrainInstance> terrains = new Dictionary<IntVec3, TerrainInstance>();

		/// <summary>
		/// Tracks DefExtensionActive which are associated to some ActiveTerrainDef in the map.
		/// This is kept updated when new terrains appear, but it does not track terrain removals.
		/// This collection is not persisted into savegames and is reconstructed when the map finalizes its init phase.
		/// </summary>
		private HashSet<DefExtensionActiveEntry> defExtensionsActive = new HashSet<DefExtensionActiveEntry>();

		// Data structures used to optimize tick triggering.
		private HashSet<TerrainInstance> tickTerrains = new HashSet<TerrainInstance>();

		// See TickList.TickInterval for these two values.
		private TickTerrain rareTickTerrains = new TickTerrain(250);
		private TickTerrain longTickTerrains = new TickTerrain(2000);

		public SpecialTerrainList(Map map) : base(map)
		{
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref terrains, "terrains", LookMode.Value, LookMode.Deep);
		}

		/// <summary>
		/// Calls Update() on terrain instances and DefExtensionActive instances. Intended for graphical updates.
		/// </summary>
		public override void MapComponentUpdate()
		{
			base.MapComponentUpdate();
			foreach (var terr in terrains)
			{
				terr.Value.Update();
			}

			foreach (DefExtensionActiveEntry entry in defExtensionsActive)
			{
				entry.extension.DoWork(entry.def);
			}
		}

		public override void MapComponentTick()
		{
			base.MapComponentTick();

			foreach (TerrainInstance terrainInstance in tickTerrains)
			{
				terrainInstance.Tick();
			}

			int gameTick = Find.TickManager.TicksGame;
			rareTickTerrains.Tick(gameTick);
			longTickTerrains.Tick(gameTick);
		}

		public override void FinalizeInit()
		{
			base.FinalizeInit();
			RefreshAllCurrentTerrain();
			foreach (TerrainInstance terrainInstance in terrains.Values)
			{
				terrainInstance.PostLoad();
			}
		}

		/// <summary>
		/// Registers terrain currently present to terrain list, called on init
		/// </summary>
		private void RefreshAllCurrentTerrain()
		{
			foreach (IntVec3 cell in map)
			{
				TerrainDef terrain = map.terrainGrid.TerrainAt(cell);
				if (terrain is ActiveTerrainDef special)
				{
					RegisterAt(special, cell);
				}
			}
		}

		/// <summary>
		/// Register the presence of an ActiveTerrainDef at a specific cell. Instantiate the terrain.
		/// </summary>
		/// <param name="activeTerrainDef">Definition being registered.</param>
		/// <param name="cell">Position of the terrain.</param>
		public void RegisterAt(ActiveTerrainDef activeTerrainDef, IntVec3 cell)
		{
			if (terrains.ContainsKey(cell))
			{
				Notify_RemovedTerrainAt(cell);
			}

			var newTerr = activeTerrainDef.MakeTerrainInstance(map, cell);
			newTerr.Init();
			terrains.Add(cell, newTerr);

			int hash = cell.FastHashCode();
			switch (activeTerrainDef.tickerType)
			{
				case TickerType.Normal:
					tickTerrains.Add(newTerr);
					break;
				case TickerType.Rare:
					rareTickTerrains.Add(hash, newTerr);
					break;
				case TickerType.Long:
					longTickTerrains.Add(hash, newTerr);
					break;
				case TickerType.Never:
				default:
					break;
			}

			if (activeTerrainDef.modExtensions == null)
			{
				return;
			}

			foreach (DefModExtension extension in activeTerrainDef.modExtensions)
			{
				if (extension is DefExtensionActive activeExtension)
				{
					defExtensionsActive.Add(new DefExtensionActiveEntry(activeTerrainDef, activeExtension));
				}
			}
		}

		public void Notify_RemovedTerrainAt(IntVec3 c)
		{
			if (!terrains.ContainsKey(c))
			{
				// This can happen if this cell contains an ActiveTerrainDef with the disableActiveTerrainDef flag set to true.
				return;
			}

			TerrainInstance terrainInstance = terrains[c];
			terrains.Remove(c);

			int hash = c.FastHashCode();
			switch (terrainInstance.def.tickerType)
			{
				case TickerType.Normal:
					tickTerrains.Remove(terrainInstance);
					break;
				case TickerType.Rare:
					rareTickTerrains.Remove(hash, terrainInstance);
					break;
				case TickerType.Long:
					longTickTerrains.Remove(hash, terrainInstance);
					break;
				case TickerType.Never:
				default:
					break;
			}

			terrainInstance.PostRemove();
		}
	}
}