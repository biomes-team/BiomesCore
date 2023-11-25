using BiomesCore.DefModExtensions;
using BiomesCore.ModSettings;
using RimWorld;
using Verse;

namespace BMT
{
	/// <summary>
	/// Special class used for certain Biomes! plants. It extends plant functionality with some features that would be too
	/// performance-heavy if they were implemented as comps.
	///
	/// * Enables the use of custom textures with the CustomGraphicsPlantDef DefModExtension.
	/// * The plant can be sown and will grow indoors outside of the vanilla temperature range if it has a
	///   Biomes_PlantControl.optimalTemperature and the current temperature is inside of that range.
	/// </summary>
	public class BiomesPlant : Plant
	{
		private CustomGraphicsPlantDef customGraphicsDef;
		private FloatRange optimalTemperature = new FloatRange(float.MinValue, float.MinValue);

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
			base.SpawnSetup(map, respawningAfterLoad);
			customGraphicsDef = def.GetModExtension<CustomGraphicsPlantDef>();
			SetCustomBiomeGraphic(map);

			var controlDef = def.GetModExtension<Biomes_PlantControl>();
			if (controlDef != null)
			{
				optimalTemperature = controlDef.optimalTemperature;
			}
		}

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

		private bool GrowthSeasonNow()
		{
			return optimalTemperature.Includes(Position.GetTemperature(Map)) || PlantUtility.GrowthSeasonNow(Position, Map);
		}

		public override float GrowthRate => Blighted || Spawned && !GrowthSeasonNow()
			? 0.0f
			: GrowthRateFactor_Fertility * GrowthRateFactor_Temperature * GrowthRateFactor_Light *
			GrowthRateFactor_NoxiousHaze;

		public override void TickLong()
		{
			base.TickLong();
			if (Destroyed)
			{
				return;
			}

			// Check Plant.TickLong would not execute its growth code but BiomesPlant needs it.
			if (PlantUtility.GrowthSeasonNow(Position, Map) || !GrowthSeasonNow())
			{
				return;
			}

			// Duplication of the Growth code in base.TickLong.
			var wasNotMature = LifeStage != PlantLifeStage.Mature;
			growthInt += GrowthPerTick * 2000f;
			if (growthInt > 1.0F)
			{
				growthInt = 1.0F;
			}

			if (wasNotMature && LifeStage == PlantLifeStage.Mature && CurrentlyCultivated())
			{
				Map.mapDrawer.MapMeshDirty(Position, MapMeshFlag.Things);
			}
		}

		public override string GetInspectString()
		{
			var inspectString = base.GetInspectString();

			if (!PlantUtility.GrowthSeasonNow(Position, Map) && GrowthSeasonNow())
			{
				// Remove the now incorrect string in base.
				inspectString = inspectString.Replace("OutOfIdealTemperatureRangeNotGrowing".Translate(), "");
			}

			return inspectString;
		}

		protected override bool Resting => IsResting();

		private bool IsResting()
		{
			var extension = def.GetModExtension<Biomes_PlantControl>();
			if (extension == null)
			{
				return base.Resting;
			}

			if (!extension.needsRest || Settings.Values.SetCustomGrowingHoursToAll)
			{
				return false;
			}

			var date = GenLocalDate.DayPercent(this);
			return date >= extension.growingHours.min && date <= extension.growingHours.max;
		}
	}
}