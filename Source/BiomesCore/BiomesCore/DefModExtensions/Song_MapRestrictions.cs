using RimWorld;
using System.Collections.Generic;
using Verse;

namespace BiomesCore.DefModExtensions
{
    class Song_MapRestrictions : DefModExtension
    {
        //Caches to avoid repeated searching.
        protected Dictionary<string, BiomeDef> _biomeDefByNameCache = new Dictionary<string, BiomeDef>();
        protected Dictionary<string, WeatherDef> _weatherDefByNameCache = new Dictionary<string, WeatherDef>();
        protected Dictionary<string, GameConditionDef> _gameConditionDefByNameCache = new Dictionary<string, GameConditionDef>();

        //Values in the actual ModExtension exposed to XML.
        public FloatRange? dangerRange = null;
        public List<string> biomeDefNameRestrictions = new List<string>();
        public List<string> weatherDefNameRestrictions = new List<string>();
        public List<string> gameConditionDefNameRestrictions = new List<string>();

        public IEnumerable<BiomeDef> BiomeDefRestrictions()
        {
            foreach(string name in biomeDefNameRestrictions)
            {
                BiomeDef def = null;
                if (_biomeDefByNameCache.ContainsKey(name)) //If it's cached..
                    def = _biomeDefByNameCache[name]; //Get it from the cache..
                else //If it isn't cached..
                    def = _biomeDefByNameCache[name] = DefDatabase<BiomeDef>.GetNamed(name, false); //Search for it and cache the result.
                if (def != null) //If it's present..
                    yield return def; //Return it.
            }
        }

        public IEnumerable<WeatherDef> WeatherDefRestrictions()
        {
            foreach (string name in weatherDefNameRestrictions)
            {
                WeatherDef def = null;
                if (_weatherDefByNameCache.ContainsKey(name)) //If it's cached..
                    def = _weatherDefByNameCache[name]; //Get it from the cache..
                else //If it isn't cached..
                    def = _weatherDefByNameCache[name] = DefDatabase<WeatherDef>.GetNamed(name, false); //Search for it and cache the result.
                if (def != null) //If it's present..
                    yield return def; //Return it.
            }
        }

        public IEnumerable<GameConditionDef> GameConditionDefRestrictions()
        {
            foreach (string name in gameConditionDefNameRestrictions)
            {
                GameConditionDef def = null;
                if (_gameConditionDefByNameCache.ContainsKey(name)) //If it's cached..
                    def = _gameConditionDefByNameCache[name]; //Get it from the cache..
                else //If it isn't cached..
                    def = _gameConditionDefByNameCache[name] = DefDatabase<GameConditionDef>.GetNamed(name, false); //Search for it and cache the result.
                if (def != null) //If it's present..
                    yield return def; //Return it.
            }
        }
    }
}
