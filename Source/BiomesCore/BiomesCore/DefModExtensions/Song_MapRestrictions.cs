using RimWorld;
using System.Collections.Generic;
using Verse;

namespace BiomesCore.DefModExtensions
{
    class Song_MapRestrictions : DefModExtension
    {
        public List<string> biomeDefNameRestrictions = new List<string>();
        public List<string> weatherDefNameRestrictions = new List<string>();
        public List<string> gameConditionDefNameRestrictions = new List<string>();

        public IEnumerable<BiomeDef> BiomeDefRestrictions()
        {
            foreach(string name in biomeDefNameRestrictions)
            {
                BiomeDef restriction = DefDatabase<BiomeDef>.GetNamed(name, false);
                if (restriction != null)
                {
                    yield return restriction;
                }
            }
        }
        public IEnumerable<WeatherDef> WeatherDefRestrictions()
        {
            foreach (string name in weatherDefNameRestrictions)
            {
                WeatherDef restriction = DefDatabase<WeatherDef>.GetNamed(name, false);
                if (restriction != null)
                {
                    yield return restriction;
                }
            }
        }

        public IEnumerable<GameConditionDef> GameConditionDefRestrictions()
        {
            foreach (string name in gameConditionDefNameRestrictions)
            {
                GameConditionDef restriction = DefDatabase<GameConditionDef>.GetNamed(name, false);
                if (restriction != null)
                {
                    yield return restriction;
                }
            }
        }
    }
}
