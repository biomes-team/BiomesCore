using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NocturnalAnimals
{

	[StaticConstructorOnStartup]
	internal class NocturnalAnimalsMod : Mod
	{
		/// <summary>
		///     The instance of the settings to be read by the mod
		/// </summary>
		public static NocturnalAnimalsMod instance;

		private static readonly Vector2 iconSize = new Vector2(58f, 58f);

		private static readonly Vector2 buttonSize = new Vector2(120f, 25f);

		private static readonly Vector2 searchSize = new Vector2(200f, 25f);

		private static Listing_Standard listing_Standard;

		private static string currentVersion;

		private static Vector2 scrollPosition;

		private static string searchText = "";

		private static readonly Color alternateBackground = new Color(0.2f, 0.2f, 0.2f, 0.5f);

		/// <summary>
		///     The private settings
		/// </summary>
		private NocturnalAnimalsSettings settings;

		/// <summary>
		///     Constructor
		/// </summary>
		/// <param name="content"></param>
		public NocturnalAnimalsMod(ModContentPack content) : base(content)
		{
			instance = this;
			if (instance.Settings.AnimalSleepType == null)
			{
				instance.Settings.AnimalSleepType = new Dictionary<string, int>();
			}

		}

		/// <summary>
		///     The instance-settings for the mod
		/// </summary>
		internal NocturnalAnimalsSettings Settings
		{
			get
			{
				if (settings == null)
				{
					settings = GetSettings<NocturnalAnimalsSettings>();
				}

				return settings;
			}
			set => settings = value;
		}

		/// <summary>
		///     The title for the mod-settings
		/// </summary>
		/// <returns></returns>
		public override string SettingsCategory()
		{
			return "[XND] Nocturnal Animals";
		}

		private static void DrawButton(Action action, string text, Vector2 pos)
		{
			var rect = new Rect(pos.x, pos.y, buttonSize.x, buttonSize.y);
			if (!Widgets.ButtonText(rect, text, true, false, Color.white))
			{
				return;
			}

			SoundDefOf.Designate_DragStandard_Changed.PlayOneShotOnCamera();
			action();
		}

		/// <summary>
		///     The settings-window
		/// </summary>
		/// <param name="rect"></param>
		public override void DoSettingsWindowContents(Rect rect)
		{
			base.DoSettingsWindowContents(rect);

			listing_Standard = new Listing_Standard();
			listing_Standard.Begin(rect);
			Text.Font = GameFont.Medium;
			var headerLabel = listing_Standard.Label("NocturnalAnimals.BodyClocks".Translate());

			if (instance.Settings.AnimalSleepType == null)
			{
				instance.Settings.AnimalSleepType = new Dictionary<string, int>();
			}

			if (instance.Settings.AnimalSleepType.Any())
			{
				DrawButton(() =>
					{
						Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
							"NocturnalAnimals.reset.confirm".Translate(),
							delegate { instance.Settings.ResetManualValues(); }));
					}, "NocturnalAnimals.reset.button".Translate(),
					new Vector2(headerLabel.position.x + headerLabel.width - buttonSize.x,
						headerLabel.position.y));
			}

			Text.Font = GameFont.Small;
			listing_Standard.CheckboxLabeled("NocturnalAnimals.logging.label".Translate(), ref Settings.VerboseLogging,
				"NocturnalAnimals.logging.tooltip".Translate());
			if (currentVersion != null)
			{
				listing_Standard.Gap();
				GUI.contentColor = Color.gray;
				listing_Standard.Label("NocturnalAnimals.version.label".Translate(currentVersion));
				GUI.contentColor = Color.white;
			}

			searchText =
				Widgets.TextField(
					new Rect(headerLabel.position + new Vector2((rect.width / 2) - (searchSize.x / 2), 0),
						searchSize),
					searchText);
			TooltipHandler.TipRegion(new Rect(
				headerLabel.position + new Vector2((rect.width / 2) - (searchSize.x / 2), 0),
				searchSize), "NocturnalAnimals.search".Translate());

			listing_Standard.End();

			var allAnimals = NocturnalAnimals.AllAnimals;
			if (!string.IsNullOrEmpty(searchText))
			{
				allAnimals = NocturnalAnimals.AllAnimals.Where(def =>
						def.label.ToLower().Contains(searchText.ToLower()) || def.modContentPack?.Name.ToLower()
							.Contains(searchText.ToLower()) == true)
					.ToList();
			}

			var borderRect = rect;
			borderRect.y += headerLabel.y + 90;
			borderRect.height -= headerLabel.y + 90;
			var scrollContentRect = rect;
			scrollContentRect.height = allAnimals.Count * 61f;
			scrollContentRect.width -= 20;
			scrollContentRect.x = 0;
			scrollContentRect.y = 0;


			var scrollListing = new Listing_Standard();
			Widgets.BeginScrollView(borderRect, ref scrollPosition, scrollContentRect);
			scrollListing.Begin(scrollContentRect);
			var alternate = false;
			foreach (var animal in allAnimals)
			{
				var modInfo = animal.modContentPack?.Name;
				var rowRect = scrollListing.GetRect(60);
				alternate = !alternate;
				if (alternate)
				{
					Widgets.DrawBoxSolid(rowRect.ExpandedBy(10, 0), alternateBackground);
				}

				var raceLabel = $"{animal.label.CapitalizeFirst()} ({animal.defName})";
				DrawIcon(animal,
					new Rect(rowRect.position, iconSize));
				var selectorRect = new Rect(rowRect.position + new Vector2(iconSize.x, 0),
					rowRect.size - new Vector2(iconSize.x, 0));
				var section = selectorRect.width / 4;
				var sectionVertical = selectorRect.height / 7 * 3;
				var selectedValue = 0;
				if (instance.Settings.AnimalSleepType.ContainsKey(animal.defName))
				{
					selectedValue = instance.Settings.AnimalSleepType[animal.defName];
				}

				Widgets.Label(new Rect(selectorRect.position, new Vector2(selectorRect.width / 2, sectionVertical)),
					raceLabel);
				var anchor = Text.Anchor;
				Text.Anchor = TextAnchor.MiddleRight;
				Widgets.Label(
					new Rect(selectorRect.position + new Vector2(selectorRect.width / 2, 0),
						new Vector2(selectorRect.width / 2, sectionVertical)), modInfo);
				Text.Anchor = anchor;

				if (RadioButtonLabeledLeft(
						new Rect(selectorRect.position + new Vector2(0, sectionVertical),
							new Vector2(section, sectionVertical)),
						"NocturnalAnimals.BodyClock_Diurnal".Translate(),
						selectedValue == 0, "NocturnalAnimals.BodyClock_Diurnal_Description".Translate()))
				{
					instance.Settings.AnimalSleepType[animal.defName] = 0;
				}

				if (RadioButtonLabeledLeft(
						new Rect(selectorRect.position + new Vector2(section, sectionVertical),
							new Vector2(section, sectionVertical)),
						"NocturnalAnimals.BodyClock_Nocturnal".Translate(),
						selectedValue == 1, "NocturnalAnimals.BodyClock_Nocturnal_Description".Translate()))
				{
					instance.Settings.AnimalSleepType[animal.defName] = 1;
				}

				if (RadioButtonLabeledLeft(
						new Rect(selectorRect.position + new Vector2(section * 2, sectionVertical),
							new Vector2(section, sectionVertical)),
						"NocturnalAnimals.BodyClock_Crepuscular".Translate(),
						selectedValue == 2, "NocturnalAnimals.BodyClock_Crepuscular_Description".Translate()))
				{
					instance.Settings.AnimalSleepType[animal.defName] = 2;
				}

				if (RadioButtonLabeledLeft(
						new Rect(selectorRect.position + new Vector2(section * 3, sectionVertical),
							new Vector2(section, sectionVertical)),
						"NocturnalAnimals.BodyClock_Cathemeral".Translate(),
						selectedValue == 3, "NocturnalAnimals.BodyClock_Cathemeral_Description".Translate()))
				{
					instance.Settings.AnimalSleepType[animal.defName] = 3;
				}
			}

			scrollListing.End();
			Widgets.EndScrollView();
		}

		public static bool RadioButtonLabeledLeft(Rect rect, string labelText, bool chosen, string tooltip)
		{
			var anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(new Rect(rect.position + new Vector2(24f, 0), rect.size - new Vector2(24f, 0)), labelText);
			TooltipHandler.TipRegion(rect, tooltip);
			Text.Anchor = anchor;
			return Widgets.RadioButton(rect.x, rect.y + (rect.height / 2f) - 12f, chosen);
		}

		private void DrawIcon(ThingDef animal, Rect rect)
		{
			var pawnKind = DefDatabase<PawnKindDef>.GetNamedSilentFail(animal.defName);

			var texture2D = pawnKind?.lifeStages?.Last()?.bodyGraphicData?.Graphic?.MatSingle?.mainTexture;

			if (texture2D == null)
			{
				return;
			}

			var toolTip = $"{pawnKind.LabelCap}\n{pawnKind.race?.description}";
			if (texture2D.width != texture2D.height)
			{
				var ratio = (float)texture2D.width / texture2D.height;

				if (ratio < 1)
				{
					rect.x += (rect.width - (rect.width * ratio)) / 2;
					rect.width *= ratio;
				}
				else
				{
					rect.y += (rect.height - (rect.height / ratio)) / 2;
					rect.height /= ratio;
				}
			}

			GUI.DrawTexture(rect, texture2D);
			TooltipHandler.TipRegion(rect, toolTip);
		}
	}
}