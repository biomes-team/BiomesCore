using System;
using Verse;

namespace BiomesCore.Locations
{
	/// <summary>
	/// Utility class for accessing the duration in ticks of each tick interval type.
	/// </summary>
	public static class TickerTypeInterval
	{
		// See TickList.TickInterval.
		public const int NeverTickerInterval = 0;
		public const int NormalTickerInterval = 1;
		public const int RareTickerInterval = 250;
		public const int LongTickerInterval = 2000;

		public static int Get(TickerType tickerType)
		{
			switch (tickerType)
			{
				case TickerType.Never:
					return NeverTickerInterval;
				case TickerType.Normal:
					return NormalTickerInterval;
				case TickerType.Rare:
					return RareTickerInterval;
				case TickerType.Long:
					return LongTickerInterval;
				default:
					throw new ArgumentOutOfRangeException(nameof(tickerType), tickerType, null);
			}
		}
	}
}