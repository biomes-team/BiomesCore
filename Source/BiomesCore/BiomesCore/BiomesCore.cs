using HarmonyLib;
using System;
using BiomesCore.ModSettings;
using BiomesCore.Patches;
using UnityEngine;
using Verse;

namespace BiomesCore
{
	public class BiomesCore : Mod
	{
		public static Harmony HarmonyInstance;
		public const string Id = "rimworld.biomes.core";
		public const string Name = "Biomes! Core";
		private static readonly Version Version = typeof(BiomesCore).Assembly.GetName().Version;

		public BiomesCore(ModContentPack content) : base(content)
		{
			// Regular harmony patches.
			HarmonyInstance = new Harmony(Id);
			HarmonyInstance.PatchAll();
			// Conditional Harmony patches. Mostly intended for mod compatibility.
			PawnGroupKindWorker_Trader_GenerateCarriers.ModCompatibility();

			LongEventHandler.ExecuteWhenFinished(InitializeWhenLoadingFinished);
		}

		private void InitializeWhenLoadingFinished()
		{
			GetSettings<Settings>();
			ExtraStatInfo.Initialize();
			Log("Initialized");
		}

		/// <summary>
		/// Name of the mod in the settings list.
		/// </summary>
		/// <returns>Name of the mod in the settings list.</returns>
		public override string SettingsCategory()
		{
			return SettingsWindow.SettingsCategory();
		}

		/// <summary>
		/// Contents of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		public override void DoSettingsWindowContents(Rect inRect)
		{
			SettingsWindow.DoWindowContents(inRect);
			base.DoSettingsWindowContents(inRect);
		}

		public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));
		public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));

		private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";
	}
}