using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesCore
{
    public class PlaceWorker_LavaGenerator : PlaceWorker
    {
        public static List<Thing> lavaGenerator = new List<Thing>();

        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            foreach (IntVec3 item in CompPowerPlantLava.GroundCells(loc, rot))
            {
                if (!map.terrainGrid.TerrainAt(item).affordances.Contains(TerrainAffordanceDefOf.Heavy))
                {
                    return new AcceptanceReport("TerrainCannotSupport_TerrainAffordance".Translate(checkingDef, TerrainAffordanceDefOf.Heavy));
                }
            }
            if (!LavaCellsPresent(loc, rot, map))
            {
                return new AcceptanceReport("MustBeOnLava".Translate());
            }
            return true;
        }

        private bool LavaCellsPresent(IntVec3 loc, Rot4 rot, Map map)
        {
            foreach (IntVec3 item in CompPowerPlantLava.LavaCells(loc, rot))
            {
                if (!map.terrainGrid.TerrainAt(item).affordances.Contains(BiomesCoreDefOf.Lava))
                {
                    return false;
                }
            }
            return true;
        }

        public override void DrawGhost(ThingDef def, IntVec3 loc, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            GenDraw.DrawFieldEdges(CompPowerPlantLava.GroundCells(loc, rot).ToList(), Color.white);
            Color color = (LavaCellsPresent(loc, rot, Find.CurrentMap) ? Designator_Place.CanPlaceColor.ToOpaque() : Designator_Place.CannotPlaceColor.ToOpaque());
            GenDraw.DrawFieldEdges(CompPowerPlantLava.LavaCells(loc, rot).ToList(), color);
            bool flag = false;
            CellRect cellRect = CompPowerPlantLava.LavaUseRect(loc, rot);
            lavaGenerator.AddRange(Find.CurrentMap.listerBuildings.AllBuildingsColonistOfDef(BiomesCoreDefOf.BMT_LavaGenerator).Cast<Thing>());
            lavaGenerator.AddRange(from t in Find.CurrentMap.listerThings.ThingsInGroup(ThingRequestGroup.Blueprint)
                                where t.def.entityDefToBuild == BiomesCoreDefOf.BMT_LavaGenerator
                                   select t);
            lavaGenerator.AddRange(from t in Find.CurrentMap.listerThings.ThingsInGroup(ThingRequestGroup.BuildingFrame)
                                where t.def.entityDefToBuild == BiomesCoreDefOf.BMT_LavaGenerator
                                   select t);
            foreach (Thing lavaGenerator in lavaGenerator)
            {
                GenDraw.DrawFieldEdges(CompPowerPlantLava.LavaUseCells(lavaGenerator.Position, lavaGenerator.Rotation).ToList(), new Color(0.2f, 0.2f, 1f));
                if (cellRect.Overlaps(CompPowerPlantLava.LavaUseRect(lavaGenerator.Position, lavaGenerator.Rotation)))
                {
                    flag = true;
                }
            }
            lavaGenerator.Clear();
            Color color2 = (flag ? new Color(1f, 0.6f, 0f) : Designator_Place.CanPlaceColor.ToOpaque());
            if (!flag || Time.realtimeSinceStartup % 0.4f < 0.2f)
            {
                GenDraw.DrawFieldEdges(CompPowerPlantLava.LavaUseCells(loc, rot).ToList(), color2);
            }
        }

        public override IEnumerable<TerrainAffordanceDef> DisplayAffordances()
        {
            yield return TerrainAffordanceDefOf.Heavy;
            yield return BiomesCoreDefOf.Lava;
        }
    }
}