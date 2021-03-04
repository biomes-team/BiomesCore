using BiomesCore.DefModExtensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BMT_Derived_Classes
{
    class BMT_Plant : Plant
    {
        protected override bool Resting => needsRest;
        bool needsRest
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
                if (!(GenLocalDate.DayPercent(this) < 0.25f))
                {
                    return GenLocalDate.DayPercent(this) > 0.8f;
                }
                return true;
            }
        }
    }
}
