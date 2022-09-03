using RimWorld;

namespace BiomesCore.ThingComponents
{
    public class CompProperties_RuinedWithoutWater : CompProperties_TemperatureRuinable
    {
        public CompProperties_RuinedWithoutWater()
        {
            compClass = typeof(CompRuinedWithoutWater);
        }
    }
}