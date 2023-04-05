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
        /// <summary>
        /// List of strings following the format ThingDef~NutritionValue.
        /// </summary>
        public List<string> thingsToNutritionMapper = new List<string>();

        /// <summary>
        /// This creature can eat all filth which can be washed away by rain. Eating filth provides this nutrition.
        /// thingsToNutritionMapper can be used to override specific filth nutrition values.
        /// </summary>
        public float filthNutrition;

        /// <summary>
        /// Lazily initialized set of all filth that creatures with a filthNutrition value can eat.
        /// </summary>
        private static HashSet<ThingDef> _filth;

        [Unsaved]
        public Dictionary<ThingDef, float> thingsToNutrition = new Dictionary<ThingDef, float>();

        public CompProperties_CustomThingEater() => compClass = typeof(CompCustomThingEater);

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (var line in base.ConfigErrors(parentDef))
            {
                yield return line;
            }

            if (thingsToNutritionMapper.Count == 0 && filthNutrition <= 0.0F)
            {
                yield return "CompProperties_CustomThingEater must define some custom things to eat";
            }
        }

        private void CalculateFilthNutrition()
        {
            if (filthNutrition <= 0.0F)
            {
                return;
            }

            if (_filth == null)
            {
                _filth = new HashSet<ThingDef>();
                foreach (ThingDef filthDef in DefDatabase<ThingDef>.AllDefs)
                {
                    if (filthDef.filth == null || !filthDef.filth.rainWashes)
                    {
                        continue;
                    }

                    _filth.Add(filthDef);
                }
            }

            foreach (ThingDef filthDef in _filth)
            {
                thingsToNutrition[filthDef] = filthNutrition;
            }
        }
        
        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);

            CalculateFilthNutrition();

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