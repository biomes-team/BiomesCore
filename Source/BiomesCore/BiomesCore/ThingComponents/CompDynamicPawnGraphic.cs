using Verse;

namespace BiomesCore.ThingComponents
{
	public abstract class CompDynamicPawnGraphic : ThingComp
	{
		public abstract bool Active();

		public abstract GraphicData Graphic();

		// ToDo: https://github.com/biomes-team/BiomesCore/issues/14
		protected void ForceGraphicUpdateNow()
		{
			if (parent is Pawn pawn)
			{
				// pawn.drawer.renderer.graphics.nakedGraphic = null;
			}
		}
	}
}