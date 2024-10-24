using Verse;

namespace BiomesCore
{
    public class CompProperties_PermanentManhunter : CompProperties
    {
        public bool sendLetter = true; // Should only use if there's only one spawned

        public bool bigThreat = false;

        // These point to the related languages stuff. {0} corresponds with the pawn's label with the first letter capitalized
        public string letterLabel = "BMT_PermanentManhunterSpawned";

        public string letterDesc = "BMT_PermanentManhunterSpawnedDesc";

        public CompProperties_PermanentManhunter()
        {
            compClass = typeof(Comp_PermanentManhunter);
        }
    }
}
