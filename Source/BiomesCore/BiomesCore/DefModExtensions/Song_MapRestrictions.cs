using RimWorld;
using System.Collections.Generic;
using Verse;

namespace BiomesCore.DefModExtensions
{
    class Song_MapRestrictions : DefModExtension
    {
        public List<string> biomeDefNameRestrictions = new List<string>();

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
    }
}
