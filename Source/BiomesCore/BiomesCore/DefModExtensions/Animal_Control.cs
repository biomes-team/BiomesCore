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
    }
}
