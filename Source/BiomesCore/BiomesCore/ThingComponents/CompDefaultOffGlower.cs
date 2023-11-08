using Verse;
using RimWorld;

namespace BiomesCore
{
    public class CompDefaultOffGlower : CompGlower
    {
        public CompProperties_DefaultOffGlower NewProps => (CompProperties_DefaultOffGlower)props;

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

    public class CompProperties_DefaultOffGlower : CompProperties_Glower
    {
        public CompProperties_DefaultOffGlower()
        {
            compClass = typeof(CompDefaultOffGlower);
        }
    }
}