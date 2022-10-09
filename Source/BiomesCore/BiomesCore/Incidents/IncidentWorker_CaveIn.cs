using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace BiomesCore
{
    public class IncidentWorker_CaveIn : IncidentWorker
    {
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			IntVec3 cell;
			return TryFindCell(out cell, map);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			if (!TryFindCell(out var cell, map))
			{
				return false;
			}
			List<Thing> list = ThingSetMakerDefOf.Meteorite.root.Generate();
			SkyfallerMaker.SpawnSkyfaller(ThingDefOf.MeteoriteIncoming, list, cell, map);
			LetterDef baseLetterDef = (list[0].def.building.isResourceRock ? LetterDefOf.PositiveEvent : LetterDefOf.NeutralEvent);
			string text = string.Format(def.letterText, list[0].def.label).CapitalizeFirst();
			SendStandardLetter(def.letterLabel + ": " + list[0].def.LabelCap, text, baseLetterDef, parms, new TargetInfo(cell, map));
			return true;
		}

		private bool TryFindCell(out IntVec3 cell, Map map)
		{
			int maxMineables = ThingSetMaker_Meteorite.MineablesCountRange.max;
			return TryFindCaveInCell(ThingDefOf.MeteoriteIncoming, map, out cell, 10, default(IntVec3), -1, allowRoofedCells: true, allowCellsWithItems: false, allowCellsWithBuildings: false, colonyReachable: false, avoidColonistsIfExplosive: true, alwaysAvoidColonists: true, delegate (IntVec3 x)
			{
				int num = Mathf.CeilToInt(Mathf.Sqrt(maxMineables)) + 2;
				CellRect cellRect = CellRect.CenteredOn(x, num, num);
				int num2 = 0;
				foreach (IntVec3 item in cellRect)
				{
					if (item.InBounds(map) && item.Standable(map))
					{
						num2++;
					}
				}
				return num2 >= maxMineables;
			});
		}

		private static bool TryFindCaveInCell(ThingDef rockChunks, Map map, out IntVec3 cell, int minDistToEdge = 10, IntVec3 nearLoc = default(IntVec3), int nearLocMaxDist = -1, bool allowRoofedCells = true, bool allowCellsWithItems = false, bool allowCellsWithBuildings = false, bool colonyReachable = false, bool avoidColonistsIfExplosive = true, bool alwaysAvoidColonists = false, Predicate<IntVec3> extraValidator = null)
		{
			bool avoidColonists = (avoidColonistsIfExplosive && rockChunks.skyfaller.CausesExplosion) || alwaysAvoidColonists;
			Predicate<IntVec3> validator = delegate (IntVec3 x)
			{
				foreach (IntVec3 item in GenAdj.OccupiedRect(x, Rot4.North, rockChunks.size))
				{
					if (!item.InBounds(map) || item.Fogged(map) || !item.Standable(map))// || (item.Roofed(map) && item.GetRoof(map).isThickRoof))
					{
						return false;
					}
					//if (!allowRoofedCells && item.Roofed(map))
					//{
					//	return false;
					//}
					if (!allowCellsWithItems && item.GetFirstItem(map) != null)
					{
						return false;
					}
					if (!allowCellsWithBuildings && item.GetFirstBuilding(map) != null)
					{
						return false;
					}
					if (item.GetFirstSkyfaller(map) != null)
					{
						return false;
					}
					foreach (Thing thing in item.GetThingList(map))
					{
						if (thing.def.preventSkyfallersLandingOn)
						{
							return false;
						}
					}
				}
				if (avoidColonists && SkyfallerUtility.CanPossiblyFallOnColonist(rockChunks, x, map))
				{
					return false;
				}
				if (minDistToEdge > 0 && x.DistanceToEdge(map) < minDistToEdge)
				{
					return false;
				}
				if (colonyReachable && !map.reachability.CanReachColony(x))
				{
					return false;
				}
				return (extraValidator == null || extraValidator(x)) ? true : false;
			};
			if (nearLocMaxDist > 0)
			{
				return CellFinder.TryFindRandomCellNear(nearLoc, map, nearLocMaxDist, validator, out cell);
			}
			return CellFinderLoose.TryFindRandomNotEdgeCellWith(minDistToEdge, validator, map, out cell);
		}
	}
}
