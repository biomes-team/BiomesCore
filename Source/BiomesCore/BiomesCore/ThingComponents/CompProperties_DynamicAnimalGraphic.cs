using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BiomesCore.ThingComponents
{
	public class CompProperties_DynamicAnimalGraphic : CompProperties
	{
		public GraphicData graphicData;

		public List<GraphicData> alternateGraphics;

		private IEnumerable<string> GraphicDataConfigErrors(string label, GraphicData data)
		{
			if (data.texPath.NullOrEmpty())
			{
				yield return $"{GetType().Name}: {label} has a null or empty texPath.";
				yield break;
			}

			if (data.drawSize != Vector2.one)
			{
				yield return
					$"{GetType().Name}: {label} with texPath {data.texPath} must not define a drawSize; it will be overriden by the pawn's current drawSize.";
			}
		}

		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			foreach (var line in base.ConfigErrors(parentDef))
			{
				yield return line;
			}

			if (graphicData == null)
			{
				yield return $"{GetType().Name} must have a graphicData.";
				yield break;
			}

			if (parentDef.race == null || !parentDef.race.Animal)
			{
				yield return $"{GetType().Name} can only be applied to animal pawns.";
				yield break;
			}

			foreach (string graphicDataError in GraphicDataConfigErrors(nameof(graphicData), graphicData))
			{
				yield return graphicDataError;
			}

			if (alternateGraphics.NullOrEmpty())
			{
				yield break;
			}

			for (int alternateIndex = 0; alternateIndex < alternateGraphics.Count; ++alternateIndex)
			{
				string label = $"{nameof(alternateGraphics)}[{alternateIndex}]";
				foreach (string alternateLine in GraphicDataConfigErrors(label, alternateGraphics[alternateIndex]))
				{
					yield return alternateLine;
				}
			}
		}

		public override void PostLoadSpecial(ThingDef parentDef)
		{
			LongEventHandler.ExecuteWhenFinished(() =>
			{
				graphicData.graphicClass ??= typeof(Graphic_Multi);
				graphicData.ExplicitlyInitCachedGraphic();

				if (!alternateGraphics.NullOrEmpty())
				{
					foreach (GraphicData alternateData in alternateGraphics)
					{
						alternateData.graphicClass ??= typeof(Graphic_Multi);
						alternateData.ExplicitlyInitCachedGraphic();
					}
				}
			});
		}
	}
}