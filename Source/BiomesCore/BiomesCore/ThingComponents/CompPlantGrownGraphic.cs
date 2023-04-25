using System.Collections.Generic;
using BiomesCore.ThingComponents;
using RimWorld;
using Verse;

namespace BiomesCore.ThingComponents
{
	public class CompProperties_CompPlantGrownGraphic : CompProperties_CompPlantGraphic
	{
		public string grownGraphicPath;
		[Unsaved] public Graphic grownGraphic;

		public CompProperties_CompPlantGrownGraphic() => compClass = typeof(CompPlantGrownGraphic);

		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			foreach (var line in base.ConfigErrors(parentDef))
			{
				yield return line;
			}

			if (grownGraphicPath == null)
			{
				yield return $"{GetType().Name} must define a grownGraphicPath.";
			}
		}

		protected override void Initialize(ThingDef parentDef)
		{
			var graphic = parentDef.graphicData.graphicClass;
			var shader = parentDef.graphic.Shader;
			var size = parentDef.graphicData.drawSize;
			var color = parentDef.graphicData.color;
			var colorTwo = parentDef.graphicData.colorTwo;
			grownGraphic = GraphicDatabase.Get(graphic, grownGraphicPath, shader, size, color, colorTwo);
		}
	}
}

public class CompPlantGrownGraphic : CompPlantGraphic
{
	private CompProperties_CompPlantGrownGraphic Props => (CompProperties_CompPlantGrownGraphic) props;

	public CompPlantGrownGraphic()
	{
	}

	public override bool Active()
	{
		return parent is Plant plant && plant.Growth >= 1.0F;
	}

	public override Graphic Graphic()
	{
		return Props.grownGraphic;
	}
}