using Verse;
using RimWorld;
using UnityEngine;

namespace BiomesCore
{
    public class IncidentWorker_Earthquake : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Find.CurrentMap.gameConditionManager.RegisterCondition(new GameCondition_Earthquake(Rand.Value * 6, Mathf.RoundToInt(1 + Rand.Value * 3 * 2500)));
            //SendStandardLetter(parms, null);
            return true;
        }
    }
}