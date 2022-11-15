using RimWorld;
using System.Collections.Generic;
using System;
using UnityEngine;
using Verse;

namespace BiomesCore
{
    public class IncidentWorker_GelatinousGuardiansEggDrop : IncidentWorker
    {
        private static readonly Pair<int, float>[] CountChance = new Pair<int, float>[4]
        {
      new Pair<int, float>(1, 1f),
      new Pair<int, float>(2, 0.95f),
      new Pair<int, float>(3, 0.7f),
      new Pair<int, float>(4, 0.4f)
        };
        private int RandomCountToDrop
        {
            get
            {
                float timePassedFactor = Mathf.Clamp(GenMath.LerpDouble(0.0f, 1.2f, 1f, 0.1f, (float)Find.TickManager.TicksGame / 3600000f), 0.1f, 1f);
                return ((IEnumerable<Pair<int, float>>)IncidentWorker_GelatinousGuardiansEggDrop.CountChance).RandomElementByWeight<Pair<int, float>>((Func<Pair<int, float>, float>)(x => x.First == 1 ? x.Second : x.Second * timePassedFactor)).First;
            }
        }
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
                return false;
            Map target = (Map)parms.target;
            return this.TryFindGelatinousRaptorEggDropCell(target.Center, target, 999999, out IntVec3 _);
        }
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map target = (Map)parms.target;
            IntVec3 pos;
            if (!this.TryFindGelatinousRaptorEggDropCell(target.Center, target, 999999, out pos))
                return false;
            this.SpawnGelatinousRaptorEgg(pos, target, this.RandomCountToDrop);
            Messages.Message((string)"BMT_MessageGelatinousRaptorEggDrop".Translate(), (LookTargets)new TargetInfo(pos, target), MessageTypeDefOf.NeutralEvent);
            return true;
        }
        private void SpawnGelatinousRaptorEgg(IntVec3 firstEggPos, Map map, int count)
        {
            this.GelatinousRaptorEgg(firstEggPos, map);
            for (int index = 0; index < count - 1; ++index)
            {
                IntVec3 pos;
                if (this.TryFindGelatinousRaptorEggDropCell(firstEggPos, map, 5, out pos))
                    this.GelatinousRaptorEgg(pos, map);
                    FilthMaker.TryMakeFilth(pos, map, BiomesCoreDefOf.Filth_GelatinousSlime);
            }
        }
        private void GelatinousRaptorEgg(IntVec3 pos, Map map) => SkyfallerMaker.SpawnSkyfaller(BiomesCoreDefOf.BMT_GelatinousRaptorEggIncoming, BiomesCoreDefOf.BMT_GelatinousRaptorEgg, pos, map);
        private bool TryFindGelatinousRaptorEggDropCell(IntVec3 nearLoc, Map map, int maxDist, out IntVec3 pos) => CellFinderLoose.TryFindSkyfallerCell(BiomesCoreDefOf.BMT_GelatinousRaptorEggIncoming, map, out pos, nearLoc: nearLoc, nearLocMaxDist: maxDist);
    }
}