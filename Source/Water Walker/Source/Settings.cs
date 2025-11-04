using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace WaterWalker
{
    // Persistent settings storage
    public class WaterWalkerModSettings : ModSettings
    {
        // default on
        public static bool deepWaterStandable = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref deepWaterStandable, "deepWaterStandable", true);
        }
    }

    // Runtime manager that applies the setting to the terrain defs
    [StaticConstructorOnStartup]
    public static class WaterWalkerSettingsManager
    {
        private static readonly string[] deepTerrainNames = new[] { "WaterDeep", "WaterOceanDeep" };

        static WaterWalkerSettingsManager()
        {
            // Apply the default (or saved) setting as early as possible.
            ApplySettings();
        }

        // Call this whenever the setting changes (or at startup)
        public static void ApplySettings()
        {
            bool makeStandable = WaterWalkerModSettings.deepWaterStandable;

            foreach (var defName in deepTerrainNames)
            {
                try
                {
                    TerrainDef terr = DefDatabase<TerrainDef>.GetNamedSilentFail(defName);
                    if (terr == null) continue;

                    // Set to Standable when enabled, otherwise to Impassable.
                    terr.passability = makeStandable ? Traversability.Standable : Traversability.Impassable;
                }
                catch (Exception ex)
                {
                    Log.Error($"[WaterWalker] Failed to set passability for terrain '{defName}': {ex}");
                }
            }
        }
    }

    // The Mod class that draws the settings window
    public class WaterWalkerMod : Mod
    {
        private WaterWalkerModSettings settings;

        public WaterWalkerMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<WaterWalkerModSettings>();
            // Ensure runtime manager applies saved value
            WaterWalkerSettingsManager.ApplySettings();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            // Warning label about map reload
            GUI.contentColor = Color.yellow;
            listing.Label("Changing this requires reloading the map to take effect.");
            GUI.contentColor = Color.white;

            listing.Gap(12f);

            // Checkbox that applies immediately
            bool prevValue = WaterWalkerModSettings.deepWaterStandable;
            listing.CheckboxLabeled("Make deep water standable", ref WaterWalkerModSettings.deepWaterStandable,
                "When enabled, WaterDeep and WaterOceanDeep will be set to Standable. When disabled, they will be Impassable.");

            // Apply immediately if changed
            if (WaterWalkerModSettings.deepWaterStandable != prevValue)
            {
                settings.Write(); // save to XML
                WaterWalkerSettingsManager.ApplySettings(); // apply changes
            }

            listing.End();
        }


        public override string SettingsCategory()
        {
            return "WaterWalker";
        }
    }
}
