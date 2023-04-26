using BiomesCore.DefModExtensions;
using RimWorld;
using Verse;

namespace BMT
{
	public class BiomesPlant : Plant
	{
		private CustomGraphicsPlantDef defExtension;

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
			if (defExtension.graphicsPerBiome.NullOrEmpty())
			{
				return;
			}

			var biomeDef = map.Biome;
			foreach (var entry in defExtension.graphicsPerBiome)
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
			defExtension = def.GetModExtension<CustomGraphicsPlantDef>();
			if (defExtension == null)
			{
				var errorStr =
					$"{def.defName} is a CustomGraphicsPlant but it lacks the required CustomGraphicsPlantDef extension.";
				Log.ErrorOnce(errorStr, errorStr.GetHashCode());
			}
			else
			{
				SetCustomBiomeGraphic(map);
			}
		}

		private bool UseImmatureGraphic()
		{
			var str0 =
				$"Plant {def.defName} UseImmatureGraphic: {!defExtension.forceUseImmature} || {defExtension.forceUseImmature} && {Growth < 1.0F}";
			Log.ErrorOnce(str0, str0.GetHashCode());
			return (!defExtension.forceUseImmature && !HarvestableNow) || (defExtension.forceUseImmature && Growth < 1.0F);
		}

		public override Graphic Graphic
		{
			get
			{
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

					if (customBiomeGraphic.immatureGraphic != null && defExtension.forceUseImmature)
					{
						return Growth < 1.0F ? customBiomeGraphic.immatureGraphic : customBiomeGraphic.graphic;
					}

					return customBiomeGraphic.immatureGraphic != null && !HarvestableNow
						? customBiomeGraphic.immatureGraphic
						: customBiomeGraphic.graphic;
				}

				if (def.plant.immatureGraphic != null && defExtension.forceUseImmature)
				{
					return Growth < 1.0F ? def.plant.immatureGraphic : def.graphic;
				}

				return base.Graphic;
			}
		}
	}
}