using Verse;

namespace BiomesCore.ThingComponents
{
    public abstract class CompDynamicPawnGraphic : ThingComp
    {
        public abstract bool Active();

        public abstract GraphicData Graphic();

        protected void ForceGraphicUpdateNow()
        {
            if (parent is Pawn pawn)
            {
                pawn.drawer.renderer.graphics.nakedGraphic = null;
            }
        }
    }
}
