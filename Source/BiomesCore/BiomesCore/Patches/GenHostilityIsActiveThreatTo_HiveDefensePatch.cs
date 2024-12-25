using BiomesCore.LordJobs;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;
using Verse.AI.Group;
using Verse;
using Verse.AI;

namespace BiomesCore.Patches
{


    [HarmonyPatch(typeof(GenHostility), nameof(GenHostility.IsActiveThreatTo))]
    public class GenHostilityIsActiveThreatTo_HiveDefensePatch
    {

        static bool Prefix(IAttackTarget target, ref bool __result)
        {
            if (target.Thing is Pawn thing)
            {
                Lord lord = thing.GetLord();
                if (lord != null && lord.LordJob is LordJob_DefendHive && (thing.mindState.duty == null || thing.mindState.duty.def != DutyDefOf.AssaultColony))
                {
                    __result = true;
                    return false;
                }

            }

            return true;
        }
    }

}
