using System.Collections.Generic;
using Verse;

namespace BiomesCore.DefModExtensions
{
    public class Biomes_AnimalControl : DefModExtension
    {
        public List<string> biomesAlternateGraphics = new List<string>();
        
        
        /// <summary>
        /// The creature will eat the custom things described below even when it is fed.
        /// </summary>
        public bool eatWhenFed;

        public bool isBloodDrinkingAnimal;
        public bool isCustomThingEater;
        public bool isBottomFeeder;

        /// <summary>
        /// Unless set to true, the animal will not be considered insectoid even if they have insectoid fleshtype or are
        /// an insect.
        /// * The animal will not spawn as an enemy in ancient complexes.
        /// * The animal will not be stimulated by pollution.
        /// * The animal can be chosen as a venerated animal.
        /// </summary>
        public bool isInsectoid;
    }
}
