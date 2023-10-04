using System.Collections.Generic;
using RimWorld;
using System.Linq;
using Verse;

namespace BiomesCore
{
    public class TerrainComp_BurnItems : TerrainComp
    {
        /// <summary>
        /// Lazily initialized cache of surrounding tiles affected by this comp.
        /// </summary>
        private List<IntVec3> affectedTiles;

        public TerrainCompProperties_BurnItems Props { get { return (TerrainCompProperties_BurnItems)props; } }

        private void InitializeAffectedTiles()
        {
            if (affectedTiles != null)
            {
                return;
            }
            affectedTiles = new List<IntVec3>();

            // Custom version of GenAdj.CellsAdjacent8Way which ignores cells outside of the map.
            foreach (var adjacentOffset in GenAdj.AdjacentCells)
            {
                IntVec3 adjacentCell = parent.Position + adjacentOffset;
                if (adjacentCell.x >= 0 && adjacentCell.x < parent.Map.Size.x && adjacentCell.z >= 0 &&
                    adjacentCell.z < parent.Map.Size.z)
                {
                    affectedTiles.Add(adjacentCell);
                }
            }
        }
        
        public override void CompTick()
        {
            base.CompTick();
            if (this.IsHashIntervalTick(60))
            {
                DamageThings(parent.Position);

                InitializeAffectedTiles();
                if (affectedTiles.Count != 0)
                {
                    int index = Rand.Range(0, affectedTiles.Count - 1);
                    DamageThings(affectedTiles[index]);
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
                    if (t is Pawn pawn)
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
