using System.Collections.Generic;
using System.Linq;
using BiomesCore.ThingComponents;
using RimWorld;
using Verse;

namespace BiomesCore.ThingComponents
{
	public class BiomeGraphics
	{
		public List<BiomeDef> biomes;

		public string graphicPath;
		public string sowingGraphicPath;
		public string leaflessGraphicPath;
		public string immatureGraphicPath;
		public string pollutedGraphicPath;

		[Unsaved] public Graphic graphic;
		[Unsaved] public Graphic sowingGraphic;
		[Unsaved] public Graphic leaflessGraphic;
		[Unsaved] public Graphic immatureGraphic;
		[Unsaved] public Graphic pollutedGraphic;

		public bool Invalid()
		{
			return graphicPath == null && sowingGraphicPath == null && leaflessGraphicPath == null &&
			       immatureGraphicPath == null && pollutedGraphicPath == null;
		}
	}

	public class CompProperties_CompPlantGraphicPerBiome : CompProperties
	{
		public List<BiomeGraphics> graphicsPerBiome;

		public CompProperties_CompPlantGraphicPerBiome() => compClass = typeof(CompPlantGraphicPerBiome);

		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			foreach (var line in base.ConfigErrors(parentDef))
			{
				yield return line;
			}

			if (!parentDef.IsPlant)
			{
				yield return $"{GetType().Name} can only be applied to plants.";
			}

			if (graphicsPerBiome.NullOrEmpty())
			{
				yield return $"{GetType().Name} must define a graphicsPerBiome with at least one entry.";
			}

			HashSet<BiomeDef> seenBiomes = new HashSet<BiomeDef>();
			for (int index = 0; index < graphicsPerBiome.Count; ++index)
			{
				var entry = graphicsPerBiome[index];
				var repeatedBiomes = seenBiomes.Intersect(entry.biomes).ToList();
				if (repeatedBiomes.Any())
				{
					yield return
						$"{GetType().Name}[{index}] contains repeated biome entries {string.Join(", ", repeatedBiomes)}";
				}

				if (entry.Invalid())
				{
					yield return $"{GetType().Name}[{index}] must contain at least one graphic path.";
				}

				foreach (var entryBiome in entry.biomes)
				{
					seenBiomes.Add(entryBiome);
				}
			}
		}

		private void Initialize(ThingDef parentDef)
		{
			var graphic = parentDef.graphicData.graphicClass;
			var shader = parentDef.graphic.Shader;
			var size = parentDef.graphicData.drawSize;
			var color = parentDef.graphicData.color;
			var colorTwo = parentDef.graphicData.colorTwo;

			foreach (var entry in graphicsPerBiome)
			{
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
		}


		public override void PostLoadSpecial(ThingDef parentDef)
		{
			LongEventHandler.ExecuteWhenFinished(() => Initialize(parentDef));
		}
	}
}

public class CompPlantGraphicPerBiome : ThingComp
{
	private CompProperties_CompPlantGraphicPerBiome Props => (CompProperties_CompPlantGraphicPerBiome) props;

	public CompPlantGraphicPerBiome()
	{
	}

	private static Graphic PerEntryGraphic(BiomeGraphics entry, Plant plant)
	{
		if (plant.LifeStage == PlantLifeStage.Sowing)
		{
			return entry.sowingGraphic;
		}

		if (plant.def.plant.pollutedGraphic != null && plant.PositionHeld.IsPolluted(plant.MapHeld))
		{
			return entry.pollutedGraphic;
		}

		if (plant.def.plant.leaflessGraphic != null && plant.LeaflessNow && (!plant.sown || !plant.HarvestableNow))
		{
			return entry.leaflessGraphic;
		}

		return plant.def.plant.immatureGraphic != null && !plant.HarvestableNow ? entry.immatureGraphic : entry.graphic;
	}

	public Graphic PerBiomeGraphic(BiomeDef biome, Plant plant)
	{
		foreach (var entry in Props.graphicsPerBiome)
		{
			if (entry.biomes.Contains(biome))
			{
				return PerEntryGraphic(entry, plant);
			}
		}

		return null;
	}
}