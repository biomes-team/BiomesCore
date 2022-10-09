using Verse;

namespace BiomesCore
{
    /// <summary>
    /// This terrain comp will push heat endlessly with no regard for ambient temperature.
    /// </summary>
    public class TerrainComp_HeatPush : TerrainComp
    {
        public TerrainCompProperties_HeatPush Props { get { return (TerrainCompProperties_HeatPush)props; } }

        protected virtual bool ShouldPushHeat { get { return true; } }

        protected virtual float PushAmount { get { return Props.pushAmount; } }

        public override void CompTick()
        {
            base.CompTick();
            if (Find.TickManager.TicksGame % 60 == this.HashCodeToMod(60) && ShouldPushHeat)
            {
                GenTemperature.PushHeat(parent.Position, parent.Map, PushAmount);
            }
        }
    }
}
