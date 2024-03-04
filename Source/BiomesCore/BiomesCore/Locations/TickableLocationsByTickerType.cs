using System.Collections.Generic;
using Verse;

namespace BiomesCore.Locations
{
	/// <summary>
	/// Utility class which holds locations of a specific tickerType, and optimizes the handling of their tick calls.
	/// </summary>
	public class TickableLocationsByTickerType
	{
		/// <summary>
		/// Indexes each group of locations by their hash tick.
		/// </summary>
		private Dictionary<int, List<Location>> locationsByHashTick =
			new Dictionary<int, List<Location>>();

		/// <summary>
		/// See TickerTypeInterval.
		/// </summary>
		private readonly int tickLength;

		/// <summary>
		/// True for all tickerTypes except Never.
		/// </summary>
		private readonly bool shouldTick;

		public TickableLocationsByTickerType(TickerType tickerType)
		{
			if (tickerType == TickerType.Never)
			{
				// tickLength is used to place instances in the dictionary using their hash, so this must be larger than zero.
				tickLength = 1;
				shouldTick = false;
			}
			else
			{
				tickLength = TickerTypeInterval.Get(tickerType);
				shouldTick = true;
			}
		}

		public void Add(Location instance)
		{
			int modHash = instance.GetUniqueID() % tickLength;
			if (!locationsByHashTick.ContainsKey(modHash))
			{
				locationsByHashTick[modHash] = new List<Location>();
			}

			locationsByHashTick[modHash].Add(instance);
		}

		private Location GetOrRemove(Map map, IntVec3 position, bool remove)
		{
			int uniqueID = Location.GetUniqueID(map, position);

			if (!locationsByHashTick.TryGetValue(uniqueID % tickLength, out List<Location> instanceList))
			{
				return null;
			}

			for (int index = 0; index < instanceList.Count; ++index)
			{
				Location current = instanceList[index];
				if (current.GetUniqueID() != uniqueID)
				{
					continue;
				}

				if (remove)
				{
					instanceList.RemoveAt(index);
				}

				return current;
			}

			return null;
		}

		public Location Remove(Map map, IntVec3 position)
		{
			return GetOrRemove(map, position, true);
		}

		public void Tick(int gameTick)
		{
			if (!shouldTick || !locationsByHashTick.TryGetValue(gameTick % tickLength,
				    out List<Location> instanceList))
			{
				return;
			}

			for (int index = 0; index < instanceList.Count; ++index)
			{
				instanceList[index].Tick(gameTick);
			}
		}

		public Location Get(Map map, IntVec3 position)
		{
			return GetOrRemove(map, position, false);
		}
	}
}