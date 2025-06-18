using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BiomesCore
{
	public class IncidentWorker_PlantSprout : IncidentWorker
	{
		private const int MinRoomCells = 64;

		private const int SpawnRadius = 6;

		public IncidentWorker_PlantSprout()
		{
		}

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = parms.target as Map;
			PlantSproutIncidentDef incidentDef = def as PlantSproutIncidentDef;

			return map != null && incidentDef != null && base.CanFireNowSub(parms) &&
				   TryFindRootCell(map, out IntVec3 cell) &&
                   (incidentDef.ignoreSeason || (incidentDef.plant != null && PlantUtility.GrowthSeasonNow(cell, parms.target as Map, incidentDef.plant)));
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = parms.target as Map;
			PlantSproutIncidentDef incidentDef = def as PlantSproutIncidentDef;
			if (incidentDef == null || !TryFindRootCell(map, out var cell))
			{
				return false;
			}

			int randomInRange = incidentDef.amount.RandomInRange;
			Thing anyPlant = null;
			for (int i = 0; i < randomInRange; i++)
			{
				if (!CellFinder.TryRandomClosewalkCellNear(cell, map, 6, out var result, x => CanSpawnAt(x, map)))
				{
					break;
				}
				result.GetPlant(map)?.Destroy();
				Thing thing2 = GenSpawn.Spawn(incidentDef.plant, result, map);
				if (anyPlant == null && thing2 != null)
				{
					anyPlant = thing2;
				}
			}

			if (anyPlant != null)
			{
				SendStandardLetter(parms, anyPlant);
			}

			return true;
		}

		private bool TryFindRootCell(Map map, out IntVec3 cell)
		{
			return CellFinderLoose.TryFindRandomNotEdgeCellWith(10, (IntVec3 x) => CanSpawnAt(x, map) && x.GetRoom(map).CellCount >= 64, map, out cell);
		}

		private bool CanSpawnAt(IntVec3 c, Map map)
		{
			PlantSproutIncidentDef incidentDef = def as PlantSproutIncidentDef;
			if (incidentDef == null || !c.Standable(map) || c.Fogged(map) ||
			    map.fertilityGrid.FertilityAt(c) < incidentDef.plant.plant.fertilityMin ||
			    (!incidentDef.allowIndoors && !c.GetRoom(map).PsychologicallyOutdoors) || c.GetEdifice(map) != null ||
			    (incidentDef.ignoreSeason && !PlantUtility.GrowthSeasonNow(map, incidentDef.plant)))
			{
				return false;
			}

			Plant plant = c.GetPlant(map);
			if (plant != null && plant.def.plant.growDays > 10f)
			{
				return false;
			}
			List<Thing> thingList = c.GetThingList(map);
			foreach (var t in thingList)
			{
				if (t.def == incidentDef.plant)
				{
					return false;
				}
			}

			return true;
		}
	}

}
