using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BiomesCore.DefModExtensions
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

	/// <summary>
	/// Defines custom graphics for a plant. The plant must use the BMT.BiomesPlant thingClass.
	/// </summary>
	public class CustomGraphicsPlantDef : DefModExtension
	{
		/// <summary>
		/// When this variable is enabled, immatureGraphics will be used when the plant is not fully grown.
		/// </summary>
		public bool forceUseImmature;

		/// <summary>
		/// Define alternate graphics for one or more biomes.
		/// </summary>
		public List<BiomeGraphics> graphicsPerBiome;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var line in base.ConfigErrors())
			{
				yield return line;
			}

			if (graphicsPerBiome.NullOrEmpty())
			{
				yield break;
			}

			HashSet<BiomeDef> seenBiomes = new HashSet<BiomeDef>();
			for (int biomeIndex = 0; biomeIndex < graphicsPerBiome.Count; ++biomeIndex)
			{
				var entry = graphicsPerBiome[biomeIndex];
				var repeatedBiomes = seenBiomes.Intersect(entry.biomes).ToList();
				if (repeatedBiomes.Any())
				{
					yield return
						$"{GetType().Name}[{biomeIndex}] contains repeated biome entries {string.Join(", ", repeatedBiomes)}";
				}

				if (entry.Invalid())
				{
					yield return $"{GetType().Name}[{biomeIndex}] must contain at least one graphic path.";
				}

				foreach (var entryBiome in entry.biomes)
				{
					seenBiomes.Add(entryBiome);
				}
			}
		}
	}
}