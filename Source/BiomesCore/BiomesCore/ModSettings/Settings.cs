using Verse;

namespace BiomesCore.ModSettings
{
	/// <summary>
	/// Contains data for all settings values. The default values of this object are the initial default settings for the mod.
	/// </summary>
	public class SettingValues
	{
		/// <summary>
		/// Some of the plants in Biomes! have a custom growing hours interval. Mods that modify the growing hours of
		/// regular plants might fail to work with these plants.  If this setting is set to true, Biomes! plants with custom
		/// growing hours will grow for the entire day period instead.
		/// </summary>
		public bool SetCustomGrowingHoursToAll = false;
	}

	/// <summary>
	/// Allows the rest of the mod to access a SettingValues instance. Handles resetting, save and load.
	/// </summary>
	public class Settings : Verse.ModSettings
	{
		/// <summary>
		/// Single instance of the setting values of this mod. Uses static for performance reasons.
		/// </summary>
		public static SettingValues Values = new SettingValues();

		/// <summary>
		/// Set all settings to their default values.
		/// </summary>
		public static void Reset()
		{
			Values = new SettingValues();
		}

		/// <summary>
		/// Save and load preferences.
		/// </summary>
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref Values.SetCustomGrowingHoursToAll, nameof(Values.SetCustomGrowingHoursToAll),
				defaultValue: false);
		}
	}
}