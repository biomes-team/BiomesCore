using BiomesCore.DefModExtensions;
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
					if (ext.needsRest)
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
		//public override bool DyingBecauseExposedToLight
		//{
		//	get
		//	{
		//		if (def.HasModExtension<Biomes_PlantControl>())
		//		{
		//			Biomes_PlantControl ext = def.GetModExtension<Biomes_PlantControl>();
		//			if (base.Map.glowGrid.GameGlowAt(base.Position, ignoreCavePlants: true) < ext.lightRange.min)
		//				return false;
		//			if (base.Map.glowGrid.GameGlowAt(base.Position, ignoreCavePlants: true) > ext.lightRange.max)
		//				return false;
		//			return true;
		//		}
		//		return base.DyingBecauseExposedToLight;
		//	}
		//}
	}
}
