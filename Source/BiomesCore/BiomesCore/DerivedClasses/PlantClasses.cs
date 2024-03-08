using BiomesCore.DefModExtensions;
using BiomesCore.ModSettings;
using RimWorld;

namespace BiomesCore.DerivedClasses
{
	/// Avoid using this class. Use BiomesPlant instead.
	public class BMT_Plant : Plant
	{
		protected override bool Resting => IsResting;
		bool IsResting
		{
			get
			{
				if (def.HasModExtension<Biomes_PlantControl>())
				{
					Biomes_PlantControl ext = def.GetModExtension<Biomes_PlantControl>();
					if (ext.needsRest && !Settings.Values.SetCustomGrowingHoursToAll)
					{
						if (!(GenLocalDate.DayPercent(this) < ext.growingHours.min))
						{
							return GenLocalDate.DayPercent(this) > ext.growingHours.max;
						}
						return true;
					}
					return false;
				}
				// returns the default Resting value without having to copy code
				return base.Resting;
			}
		}

	}
}
