using BiomesCore.DefModExtensions;
using BiomesCore.Locations;
using BiomesCore.ModSettings;
using RimWorld;
using Verse;

namespace BMT
{
	/// <summary>
	/// Special class used for certain Biomes! plants. It extends plant functionality with some features that would be too
	/// performance-heavy if they were implemented as comps or Harmony patches. Enables the following features:
	///
	/// * Enables the use of custom textures with the CustomGraphicsPlantDef DefModExtension.
	///
	/// * The plant can be sown and will grow indoors outside of the vanilla temperature range if it has a
	///   Biomes_PlantControl.optimalTemperature and the current temperature is inside of that range.
	///
	/// * If Biomes_PlantControl.needsRest is set to false, the plant will never rest.
	/// </summary>
	public class BiomesPlant : Plant
	{
		private CustomGraphicsPlantDef customGraphicsDef;

		private FloatRange? optimalTemperature;

		private bool? needsRest;

		private FloatRange? growingHours;

		private BiomeGraphics customBiomeGraphic;

		private void InitializeBiomeGraphics(BiomeGraphics entry)
		{
			if (entry.graphic != null)
			{
				return;
			}

			var graphic = def.graphicData.graphicClass;
			var shader = def.graphic.Shader;
			var size = def.graphicData.drawSize;
			var color = def.graphicData.color;
			var colorTwo = def.graphicData.colorTwo;

			if (entry.graphicPath != null)
			{
				entry.graphic = GraphicDatabase.Get(graphic, entry.graphicPath, shader, size, color, colorTwo);
			}

			if (entry.sowingGraphicPath != null)
			{
				entry.sowingGraphic = GraphicDatabase.Get(graphic, entry.sowingGraphicPath, shader, size, color, colorTwo);
			}

			if (entry.leaflessGraphicPath != null)
			{
				entry.leaflessGraphic =
					GraphicDatabase.Get(graphic, entry.leaflessGraphicPath, shader, size, color, colorTwo);
			}

			if (entry.immatureGraphicPath != null)
			{
				entry.immatureGraphic =
					GraphicDatabase.Get(graphic, entry.immatureGraphicPath, shader, size, color, colorTwo);
			}

			if (ModsConfig.BiotechActive && entry.pollutedGraphicPath != null)
			{
				entry.pollutedGraphic =
					GraphicDatabase.Get(graphic, entry.pollutedGraphicPath, shader, size, color, colorTwo);
			}
		}

		private void SetCustomBiomeGraphic(Map map)
		{
			if (customGraphicsDef == null || customGraphicsDef.graphicsPerBiome.NullOrEmpty())
			{
				return;
			}

			var biomeDef = map.Biome;
			foreach (var entry in customGraphicsDef.graphicsPerBiome)
			{
				if (entry.biomes.Contains(biomeDef))
				{
					customBiomeGraphic = entry;
					LongEventHandler.ExecuteWhenFinished(() => InitializeBiomeGraphics(entry));
					break;
				}
			}
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			var controlDef = def.GetModExtension<Biomes_PlantControl>();
			if (controlDef != null)
			{
				optimalTemperature = controlDef.optimalTemperature;
				needsRest = controlDef.needsRest;
				growingHours = controlDef.growingHours;
			}

			base.SpawnSetup(map, respawningAfterLoad);

			customGraphicsDef = def.GetModExtension<CustomGraphicsPlantDef>();
			SetCustomBiomeGraphic(map);
		}

		/// <summary>
		/// Overrides plant graphics according to the CustomGraphicsPlantDef instance.
		/// This feature is in a derived class to avoid Harmony patching vanilla plants, as that would have a huge cost.
		/// </summary>
		public override Graphic Graphic
		{
			get
			{
				if (customGraphicsDef == null)
				{
					return base.Graphic;
				}

				if (customBiomeGraphic != null)
				{
					if (LifeStage == PlantLifeStage.Sowing)
					{
						return customBiomeGraphic.sowingGraphic;
					}

					if (def.plant.pollutedGraphic != null && PositionHeld.IsPolluted(MapHeld))
					{
						return customBiomeGraphic.pollutedGraphic;
					}

					if (def.plant.leaflessGraphic != null && LeaflessNow && (!sown || !HarvestableNow))
					{
						return customBiomeGraphic.leaflessGraphic;
					}

					if (customBiomeGraphic.immatureGraphic != null && customGraphicsDef.forceUseImmature)
					{
						return Growth < 1.0F ? customBiomeGraphic.immatureGraphic : customBiomeGraphic.graphic;
					}

					return customBiomeGraphic.immatureGraphic != null && !HarvestableNow
						? customBiomeGraphic.immatureGraphic
						: customBiomeGraphic.graphic;
				}

				if (def.plant.immatureGraphic != null && customGraphicsDef.forceUseImmature)
				{
					return Growth < 1.0F ? def.plant.immatureGraphic : def.graphic;
				}

				return base.Graphic;
			}
		}

		/// <summary>
		/// True if the plant has a Biomes_PlantControl.optimalTemperature and the current temperature is optimal.
		/// </summary>
		/// <returns>True if the plant is using the extension and if it should be growing now.</returns>
		private bool ExtendedGrowthSeasonNow()
		{
			return optimalTemperature != null && optimalTemperature.Value.Includes(Position.GetTemperature(Map));
		}

		/// <summary>
		/// Returns true if the plant is in growth season, regardless of if it is using the vanilla check or the extended
		/// check.
		/// </summary>
		/// <returns>True if the plant should be growing now.</returns>
		private bool GrowthSeasonNow()
		{
			return ExtendedGrowthSeasonNow() || PlantUtility.GrowthSeasonNow(Map, def);
		}

		/// <summary>
		/// Identical to the parent function, except for using the custom GrowthSeasonNow implementation.
		/// </summary>
		public override float GrowthRate => Blighted || Spawned && !GrowthSeasonNow()
			? 0.0f
			: GrowthRateFactor_Fertility * GrowthRateFactor_Temperature * GrowthRateFactor_Light *
			  GrowthRateFactor_NoxiousHaze;

		/// <summary>
		/// Prevent plants from becoming leafless and/or dying while within their extended temperature range.
		/// </summary>
		protected override float LeaflessTemperatureThresh
		{
			get
			{
				var vanillaValue = base.LeaflessTemperatureThresh;
				if (optimalTemperature == null || vanillaValue < optimalTemperature.Value.min) return vanillaValue;
				return optimalTemperature.Value.min - Rand.RangeSeeded(0, 8f, this.thingIDNumber ^ 838051265);
			}
		}

		public override void TickLong()
		{
			base.TickLong();
			if (Destroyed)
			{
				return;
			}

			// Check if Plant.TickLong would not execute its growth code but the BiomesPlant needs it due to having an
			// optimal temperature range coming from the extension.
			if (PlantUtility.GrowthSeasonNow(Map, def) || !ExtendedGrowthSeasonNow())
			{
				return;
			}

			// Duplication of the Growth code in Plant.TickLong.
			var wasNotMature = LifeStage != PlantLifeStage.Mature;
			growthInt += GrowthPerTick * TickerTypeInterval.LongTickerInterval;
			if (growthInt > 1.0F)
			{
				growthInt = 1.0F;
			}

			if (wasNotMature && LifeStage == PlantLifeStage.Mature && CurrentlyCultivated())
			{
				Map.mapDrawer.MapMeshDirty(Position, MapMeshFlagDefOf.Things);
			}
		}

		public override string GetInspectString()
		{
			string inspectString = base.GetInspectString();

			if (!PlantUtility.GrowthSeasonNow(Map, def) && ExtendedGrowthSeasonNow())
			{
				// Remove the now incorrect string in base.
				inspectString = inspectString.Replace("OutOfIdealTemperatureRangeNotGrowing".Translate(), "");
			}

			return inspectString;
		}

		protected override bool Resting => IsResting();

		private bool IsResting()
		{
			if (needsRest.HasValue && !needsRest.Value)
			{
				return false;
			}

			if (growingHours.HasValue)
			{
				if (Settings.Values.SetCustomGrowingHoursToAll)
				{
					return false;
				}

				float date = GenLocalDate.DayPercent(this);

				if (growingHours.Value.min > growingHours.Value.max)
				{
					return date < growingHours.Value.min && date > growingHours.Value.max;
				}

				return date < growingHours.Value.min || date > growingHours.Value.max;
			}

			return base.Resting;
		}
	}
}
