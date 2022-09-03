using System.Collections.Generic;
using Verse;

namespace BiomesCore
{
    public class CompCustomThingEater : ThingComp
    {
        public CompProperties_CustomThingEater Props => (CompProperties_CustomThingEater)props;
    }
    
    public class CompProperties_CustomThingEater : CompProperties
    {
        public List<string> thingsToNutritionMapper;

        [Unsaved]
        public Dictionary<ThingDef, float> thingsToNutrition = new Dictionary<ThingDef, float>();

        public CompProperties_CustomThingEater() => compClass = typeof(CompCustomThingEater);
        
        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);

            thingsToNutrition.Clear();
            foreach (var str in thingsToNutritionMapper)
            {
                var parts = str.Split('~');
                var thingDef = DefDatabase<ThingDef>.GetNamedSilentFail(parts[0]);
                if (thingDef != null)
                {
                    var nutrition = parts.Length > 1 ? float.Parse(parts[1]) : 1f;
                    thingsToNutrition[thingDef] = nutrition;
                }
            }
        }
    }
}