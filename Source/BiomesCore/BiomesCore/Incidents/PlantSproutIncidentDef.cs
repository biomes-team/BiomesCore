using Verse;
using RimWorld;

namespace BiomesCore
{
    public class PlantSproutIncidentDef : IncidentDef
    {
        public ThingDef plant;
        public IntRange amount = new IntRange(10, 20);
        public bool ignoreSeason;
    }
}