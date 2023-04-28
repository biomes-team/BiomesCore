using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesCore
{
    public class CompPowerPlantLava : CompPowerPlant
    {
        private float pumpPosition;

        private bool cacheDirty = true;

        private bool lavaUsable;

        private bool lavaDoubleUsed;

        protected override float DesiredPowerOutput
        {
            get
            {
                if (cacheDirty)
                {
                    RebuildCache();
                }
                if (!lavaUsable)
                {
                    return 0f;
                }
                if (lavaDoubleUsed)
                {
                    return base.DesiredPowerOutput * 0.3f;
                }
                return base.DesiredPowerOutput;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            pumpPosition = Rand.Range(0f, 15f);
            RebuildCache();
            ForceOthersToRebuildCache(parent.Map);
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            ForceOthersToRebuildCache(map);
        }

        private void ClearCache()
        {
            cacheDirty = true;
        }

        private void RebuildCache()
        {
            lavaUsable = true;
            foreach (IntVec3 item in LavaCells())
            {
                if (item.InBounds(parent.Map) && !parent.Map.terrainGrid.TerrainAt(item).affordances.Contains(BiomesCoreDefOf.Lava))
                {
                    lavaUsable = false;
                    break;
                }
            }
            lavaDoubleUsed = false;
            IEnumerable<Building> enumerable = parent.Map.listerBuildings.AllBuildingsColonistOfDef(BiomesCoreDefOf.BMT_LavaGenerator);
            foreach (IntVec3 item2 in LavaUseCells())
            {
                if (!item2.InBounds(parent.Map))
                {
                    continue;
                }
                foreach (Building item3 in enumerable)
                {
                    if (item3 != parent && item3.GetComp<CompPowerPlantLava>().LavaUseRect().Contains(item2))
                    {
                        lavaDoubleUsed = true;
                        break;
                    }
                }
            }
            if (!lavaUsable)
            {
                return;
            }
            Vector3 zero = Vector3.zero;
            foreach (IntVec3 item4 in LavaCells())
            {
            }
            cacheDirty = false;
        }

        private void ForceOthersToRebuildCache(Map map)
        {
            foreach (Building item in map.listerBuildings.AllBuildingsColonistOfDef(BiomesCoreDefOf.BMT_LavaGenerator))
            {
                item.GetComp<CompPowerPlantLava>().ClearCache();
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (base.PowerOutput > 0.01f)
            {
                pumpPosition = (pumpPosition + 1f / 150f * (float)Math.PI * 2f) % ((float)Math.PI * 2f);
            }
        }

        public IEnumerable<IntVec3> LavaCells()
        {
            return LavaCells(parent.Position, parent.Rotation);
        }

        public static IEnumerable<IntVec3> LavaCells(IntVec3 loc, Rot4 rot)
        {
            IntVec3 perpOffset = rot.Rotated(RotationDirection.Counterclockwise).FacingCell;
            yield return loc + rot.FacingCell * 2 - perpOffset;
        }

        public CellRect LavaUseRect()
        {
            return LavaUseRect(parent.Position, parent.Rotation);
        }

        public static CellRect LavaUseRect(IntVec3 loc, Rot4 rot)
       
        {
            int width = (rot.IsHorizontal ? 13 : 13);
            int height = (rot.IsHorizontal ? 13 : 13);
            return CellRect.CenteredOn(loc + rot.FacingCell, width, height);
        }

        public IEnumerable<IntVec3> LavaUseCells()
        {
            return LavaUseCells(parent.Position, parent.Rotation);
        }

        public static IEnumerable<IntVec3> LavaUseCells(IntVec3 loc, Rot4 rot)
        {
            foreach (IntVec3 item in LavaUseRect(loc, rot))
            {
                yield return item;
            }
        }

        public IEnumerable<IntVec3> GroundCells()
        {
            return GroundCells(parent.Position, parent.Rotation);
        }

        public static IEnumerable<IntVec3> GroundCells(IntVec3 loc, Rot4 rot)
        {
            IntVec3 perpOffset = rot.Rotated(RotationDirection.Counterclockwise).FacingCell;
            yield return loc - rot.FacingCell;
            yield return loc - rot.FacingCell - perpOffset;
            yield return loc - rot.FacingCell + perpOffset;
            yield return loc;
            yield return loc - perpOffset;
            yield return loc + perpOffset;
            yield return loc + rot.FacingCell;
            yield return loc + rot.FacingCell - perpOffset;
            yield return loc + rot.FacingCell + perpOffset;
        }

        public override string CompInspectStringExtra()
        {
            string text = base.CompInspectStringExtra();
            if (lavaUsable && lavaDoubleUsed)
            {
                text += "\n" + "LavaGeneratoor_lavaUsedTwice".Translate();
            }
            return text;
        }
    }
}
