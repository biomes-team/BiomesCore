using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace BiomesCore.Jobs
{
	/// <summary>
	/// If any food can be found nearby, eat it.
	/// If no food can be found, travel to a random point near a human.
	/// When moving to eat or just wandering, smash any building in its path.
	/// </summary>
	public class JobGiver_Hungering : ThinkNode_JobGiver
	{
		private const float MaxHorDist = 20F;

		private static bool EdibleThing(Thing thing, Pawn eater)
		{
			if (!eater.RaceProps.CanEverEat(thing))
			{
				return false;
			}

			var ingestibleDef = FoodUtility.GetFinalIngestibleDef(thing);
			return eater.RaceProps.CanEverEat(thing) && FoodUtility.GetNutrition(eater, thing, ingestibleDef) > 0.0F &&
				!(thing is Pawn) && !(thing is Plant);
		}

		private static Thing EdibleThingInCell(IntVec3 processCell, Pawn eater)
		{
			var thingList = eater.Map.thingGrid.ThingsListAtFast(processCell);

			for (var index = 0; index < thingList.Count; ++index)
			{
				var thing = thingList[index];
				if (EdibleThing(thing, eater))
				{
					return thing;
				}
			}

			return null;
		}

		private static Thing ClosestEdibleThing(Pawn eater)
		{
			var map = eater.Map;
			foreach (var cell in GenRadial.RadialCellsAround(eater.Position, MaxHorDist, true))
			{
				if (cell.InBounds(map) && cell.Standable(map))
				{
					var edibleThing = EdibleThingInCell(cell, eater);
					if (edibleThing != null)
					{
						return edibleThing;
					}
				}
			}

			return null;
		}

		private static Pawn RandomHumanlike(Pawn eater)
		{
			var allPawns = eater.Map.mapPawns.AllPawnsSpawned;
			var targetPawns = new List<Pawn>();
			var eaterCell = eater.Position;
			for (int index = 0; index < allPawns.Count; ++index)
			{
				var pawn = allPawns[index];
				// Only choose human-like pawns that are not too close. No need to search for food in the same place twice.
				if (pawn.RaceProps.Humanlike && !eaterCell.InHorDistOf(pawn.Position, MaxHorDist))
				{
					targetPawns.Add(pawn);
				}
			}

			return targetPawns.Count > 0 ? targetPawns.RandomElement() : null;
		}

		private static IntVec3 LocationNearHumanlike(Pawn eater, Pawn chosenTarget)
		{
			if (chosenTarget == null)
			{
				// If no human-like pawns have been found, choose a random spot just outside of the colony. 
				return RCellFinder.TryFindRandomSpotJustOutsideColony(eater, out IntVec3 randomSpot)
					? randomSpot
					: IntVec3.Invalid;
			}

			var map = eater.Map;
			var eaterPathGrid = map.pathing.For(eater).pathGrid;
			bool Validator(IntVec3 cell) => eaterPathGrid.WalkableFast(cell);
			CellFinder.TryFindRandomCellNear(chosenTarget.Position, map, 5, Validator,
				out IntVec3 destinationCell);

			return destinationCell;
		}

		private static Job DiggingJob(Pawn eater, IntVec3 destinationCell)
		{
			if (destinationCell == IntVec3.Invalid)
			{
				return null;
			}

			const TraverseMode traverseMode = TraverseMode.PassAllDestroyableThingsNotWater;
			var parms = TraverseParms.For(eater, Danger.Deadly, traverseMode, true, false, true);
			using var path = eater.Map.pathFinder.FindPath(eater.Position, destinationCell, parms);
			Job job = null;

			if (!path.Found)
			{
				return null;
			}

			var blocker = path.FirstBlockingBuilding(out var cellBefore, eater);
			if (blocker != null)
			{
				job = DigUtility.PassBlockerJob(eater, blocker, cellBefore, false, false);
			}

			return job;
		}

		private static Job GetHungeringJob(Pawn eater)
		{
			// Try to find anything edible nearby.
			var foodSource = ClosestEdibleThing(eater);


			var destinationCell = IntVec3.Invalid;
			Pawn chosenPawn = null;
			// If a food source was found, set the destination cell to its position.
			// If no food source was found, set the destination cell to a location near a random human-like pawn.
			if (foodSource == null)
			{
				chosenPawn = RandomHumanlike(eater);
				destinationCell = LocationNearHumanlike(eater, chosenPawn);
			}
			else
			{
				destinationCell = foodSource.Position;
			}

			// Check if the creature needs to dig to reach their chosen destination.
			var diggingJob = DiggingJob(eater, destinationCell);
			if (diggingJob != null)
			{
				return diggingJob;
			}

			if (foodSource == null)
			{
				return JobMaker.MakeJob(JobDefOf.Goto, destinationCell);
			}

			var devourJob = JobMaker.MakeJob(BiomesCoreDefOf.BMT_DevourHungering, foodSource);
			devourJob.count = foodSource.stackCount;
			return devourJob;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			var job = GetHungeringJob(pawn);
			if (job != null)
			{
				job.canBashDoors = true;
				job.canBashFences = true;
				job.overeat = true;
				job.expiryInterval = 999999;
			}

			return job;
		}
	}
}