using System;
using System.Collections.Generic;
using Verse;

namespace BiomesCore
{
    public class CompCustomThingEater : ThingComp
    {
        // This value is intentionally kept unsaved. It is only used to avoid making food lookups for every critter
        // at the same time.
        private bool canLookForFood;

        private const int TickInterval = 600;

        public override void CompTick()
        {
            canLookForFood = canLookForFood || this.IsHashIntervalTick(TickInterval);
        }

        public bool TryLookForFood()
        {
            var result = canLookForFood;
            canLookForFood = false;
            return result;
        }

        public CompProperties_CustomThingEater Props => (CompProperties_CustomThingEater)props;
    }
    
    public class CompProperties_CustomThingEater : CompProperties
    {
        /// <summary>
        /// List of strings following the format ThingDef~NutritionValue.
        /// </summary>
        public List<string> thingsToNutritionMapper = new List<string>();

        /// <summary>
        /// This creature can eat all filth. Eating filth provides this nutrition.
        /// thingsToNutritionMapper can be used to override specific filth nutrition values.
        /// </summary>
        public float filthNutrition;

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

            foreach (ThingDef filthDef in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (filthDef.filth != null)
                {
                    thingsToNutrition[filthDef] = filthNutrition;
                }
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