using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace BiomesCore
{
    public class TerrainComp_BurnItems : TerrainComp
    {
        public TerrainCompProperties_BurnItems Props { get { return (TerrainCompProperties_BurnItems)props; } }
        public override void CompTick()
        {
            base.CompTick();
            if (Find.TickManager.TicksGame % 60 == this.HashCodeToMod(60))
            {
                DamageThings(this.parent.Position);
                foreach (var cell in GenAdj.CellsAdjacent8Way(new TargetInfo(this.parent.Position, this.parent.Map)).InRandomOrder())
                {
                    var terrain = cell.GetTerrain(this.parent.Map);
                    if (terrain != this.parent.def)
                    {
                        if (Rand.Chance(0.3f))
                        {
                            DamageThings(cell);
                            break;
                        }
                    }
                }
            }
        }

        private void DamageThings(IntVec3 position)
        {
            var allThings = position.GetThingList(parent.Map);
            for (var i = allThings.Count - 1; i >= 0; i--)
            {
                var t = allThings[i];
                if (t.def.BaseFlammability > 0)
                {
                    var fireSize = Rand.Range(0.3f, 1f);
                    Fire obj = (Fire)ThingMaker.MakeThing(ThingDefOf.Fire);
                    obj.fireSize = fireSize;
                    if (t.CanEverAttachFire() && !t.HasAttachment(ThingDefOf.Fire))
                    {
                        obj.AttachTo(t);
                    }
                    GenSpawn.Spawn(obj, t.Position, t.Map, Rot4.North);
                    Pawn pawn = t as Pawn;
                    if (pawn != null)
                    {
                        pawn.jobs.StopAll();
                        pawn.records.Increment(RecordDefOf.TimesOnFire);
                        if (pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Bottom, BodyPartDepth.Outside).Where(x => x.coverageAbs > 0).TryRandomElement(out var part))
                        {
                            pawn.TakeDamage(new DamageInfo(DamageDefOf.Burn, Rand.Range(3, 20), hitPart: part));
                        }
                    }
                    else
                    {
                        t.TakeDamage(new DamageInfo(DamageDefOf.Burn, Rand.Range(3, 20)));
                    }
                }
            }
        }
    }
}
