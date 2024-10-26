using System.Collections.Generic;
using BiomesCore.ThingComponents;
using Verse;

namespace BiomesCore
{
    public class CompProperties_HungerReaction : CompProperties_DynamicAnimalGraphic
    {
        public float maxFood = 0.3f;

        public List<HediffDef> hediffs;

        public CompProperties_HungerReaction()
        {
            compClass = typeof(CompHungerReaction);
        }
    }
}
