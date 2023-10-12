using System;

namespace BiomesCore
{
    using Verse;

    /// <summary>
    /// Extensions which must be updated periodically. Can only be applied to ActiveTerrainDef.
    /// </summary>
    public abstract class DefExtensionActive : DefModExtension
    {
        public abstract void DoWork(TerrainDef def);
        public abstract void DoWork(ThingDef def);
    }

    public class DefExtension_ShaderSpeedMult : DefExtensionActive
    {
        private float timeMult = 1;

        public override void DoWork(TerrainDef def) => 
            def.waterDepthMaterial.SetFloat("_GameSeconds", Find.TickManager.TicksGame * this.timeMult);

        public override void DoWork(ThingDef def) => throw new NotImplementedException();
    }
}
