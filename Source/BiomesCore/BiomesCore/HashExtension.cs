using Verse;

namespace BiomesCore
{
	public static class HashExtension
	{
		public static bool IsHashIntervalTick(this ThingComp thingComp, int interval)
		{
			return thingComp.parent.IsHashIntervalTick(interval);
		}

	}
}