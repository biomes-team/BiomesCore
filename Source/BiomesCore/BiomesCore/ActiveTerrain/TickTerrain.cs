using System;
using System.Collections.Generic;
using Verse;

namespace BiomesCore.ActiveTerrain
{
	public class TickTerrain
	{
		private Dictionary<int, HashSet<TerrainInstance>> terrainsByHashTick =
			new Dictionary<int, HashSet<TerrainInstance>>();

		private int tickLength;

		public TickTerrain(int length)
		{
			tickLength = length;
		}

		public void Add(int cellHashCode, TerrainInstance instance)
		{
			int modHash = Math.Abs(cellHashCode) % tickLength;
			if (!terrainsByHashTick.ContainsKey(modHash))
			{
				terrainsByHashTick[modHash] = new HashSet<TerrainInstance>();
			}

			terrainsByHashTick[modHash].Add(instance);
		}

		public void Remove(int cellHashCode, TerrainInstance instance)
		{
			int modHash = Math.Abs(cellHashCode) % tickLength;
			if (terrainsByHashTick.TryGetValue(modHash, out HashSet<TerrainInstance> set))
			{
				set.Remove(instance);
			}
		}

		public void Tick(int gameTick)
		{
			if (!terrainsByHashTick.ContainsKey(gameTick % tickLength))
			{
				return;
			}

			foreach (TerrainInstance terrainInstance in terrainsByHashTick[gameTick % tickLength])
			{
				terrainInstance.Tick();
			}
		}
	}
}