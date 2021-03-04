using BiomesCore.DefModExtensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCore.DerivedClasses
{
    public class BMT_Plant : Plant
    {
        protected override bool Resting => isResting;
        bool isResting
        {
            get
            {
                if (def.HasModExtension<Biomes_PlantControl>())
                {
                    Biomes_PlantControl ext = def.GetModExtension<Biomes_PlantControl>();
                    if (ext.needsRest)
                    {
                        if (!(GenLocalDate.DayPercent(this) < ext.growingHours.min))
                        {
                            return GenLocalDate.DayPercent(this) > ext.growingHours.max;
                        }
                        return true;
                    }
                    return false;
                }
                // returns the default Resting value without having to copy code
                return base.Resting;
            }
        }
    }
}
