using Verse;
using RimWorld;

namespace BiomesCore
{
    public class CompDefaultOffGlower : CompGlower
    {
        public override void PostSpawnSetup(bool respawnAfterLoad)
        {
            if (ShouldBeLitNow)
            {
                UpdateLit(parent.Map);
                parent.Map.glowGrid.DeRegisterGlower(this);
            }
            else
            {
                UpdateLit(parent.Map);
            }
        }
    }
}