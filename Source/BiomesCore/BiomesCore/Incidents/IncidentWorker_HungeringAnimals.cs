using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesCore.Incidents
{
	/// <summary>
	/// Extend this class to choose valid animals for the specified incident parameters.
	/// </summary>
	public abstract class IncidentWorker_HungeringAnimals : IncidentWorker
	{
		private const int AnimalsStayDurationMin = 45000;
		private const int AnimalsStayDurationMax = 90000;

		protected abstract PawnKindDef HungeringAnimalDef(IncidentParms parms);


		// See ManhunterPackIncidentUtility.GetAnimalsCount.
		// This version can return zero, to avoid the event from triggering when there are not enough points.
		public static int GetAnimalsCount(PawnKindDef animalKind, float points)
		{
			if (animalKind == null)
			{
				return 0;
			}

			return Mathf.Clamp(Mathf.RoundToInt(points / animalKind.combatPower), 0, 100);
		}

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (!base.CanFireNowSub(parms))
			{
				return false;
			}

			var target = (Map) parms.target;
			var pawnKindDef = HungeringAnimalDef(parms);
			if (pawnKindDef == null)
			{
				return false;
			}

			return GetAnimalsCount(pawnKindDef, parms.points) > 0 &&
			       RCellFinder.TryFindRandomPawnEntryCell(out _, target, CellFinder.EdgeRoadChance_Animal);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			var target = (Map) parms.target;
			var kind = HungeringAnimalDef(parms);
			var count = GetAnimalsCount(kind, parms.points);
			if (count == 0)
			{
				return false;
			}

			var spawnPoint = parms.spawnCenter;
			if (!spawnPoint.IsValid &&
			    !RCellFinder.TryFindRandomPawnEntryCell(out spawnPoint, target, CellFinder.EdgeRoadChance_Animal))
			{
				return false;
			}

			var animals = AggressiveAnimalIncidentUtility.GenerateAnimals(kind, target.Tile, 0, count);
			var rot = Rot4.FromAngleFlat((target.Center - spawnPoint).AngleFlat);

			for (int index = 0; index < animals.Count; ++index)
			{
				var newPawn = animals[index];
				var location = CellFinder.RandomClosewalkCellNear(spawnPoint, target, 10);
				var spawnedPawn = GenSpawn.Spawn(newPawn, location, target, rot);
				QuestUtility.AddQuestTag(spawnedPawn, parms.questTag);

				newPawn.mindState.exitMapAfterTick =
					Find.TickManager.TicksGame + Rand.Range(AnimalsStayDurationMin, AnimalsStayDurationMax);
				newPawn.health.AddHediff(BiomesCoreDefOf.BMT_HungeringHediff);
				newPawn.mindState.mentalStateHandler.TryStartMentalState(BiomesCoreDefOf.BMT_Hungering);
			}

			SendStandardLetter("BMT_LetterLabelHungeringPackArrived".Translate(),
				"BMT_LetterTextHungeringPackArrived".Translate((NamedArgument) kind.GetLabelPlural()), LetterDefOf.ThreatBig,
				parms, (Thing) animals[0]);

			Find.TickManager.slower.SignalForceNormalSpeedShort();
			return true;
		}
	}
}