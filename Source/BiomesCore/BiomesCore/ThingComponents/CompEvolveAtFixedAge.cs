using System;
using RimWorld;
using Verse;
using Verse.Sound;

namespace BiomesCore
{
	public class CompEvolveAtFixedAge : ThingComp
	{
		public CompProperties_EvolveAtFixedAge Props => (CompProperties_EvolveAtFixedAge) props;

		public override void CompTickRare()
		{
			base.CompTickRare();

			if (parent.Map == null || !(parent is Pawn oldPawn)) return;

			if (oldPawn.ageTracker.AgeBiologicalTicks >= Props.ageInDays * 60000L)
			{
				var newThing = CreateNewThing(oldPawn);

				Props.evolveSound?.PlayOneShot(new TargetInfo(parent.Position, parent.Map));

				if (Props.filthDef != null)
					for (int i = 0; i < Props.filthAmount; i++)
					{
						var parms = TraverseParms.For(TraverseMode.NoPassClosedDoors);
						if (CellFinder.TryFindRandomReachableNearbyCell(parent.Position, parent.Map, 2,
							    parms, null, null, out var c))
						{
							FilthMaker.TryMakeFilth(c, parent.Map, Props.filthDef);
						}
					}

				if (newThing != null) GenSpawn.Spawn(newThing, parent.Position, parent.Map);
				if (!parent.Destroyed) parent.Destroy();
			}
		}

		private Thing CreateNewThing(Pawn oldPawn)
		{
			PawnKindDef newPawnKindDef = Props.evolveIntoPawnKindDef;
			if (newPawnKindDef != null)
			{
				Gender? oldGender = oldPawn.gender;
				if ((newPawnKindDef.race.race.hasGenders) == (oldGender == Gender.None))
				{
					// Do not force a fixed gender if the previous gender is inconsistent with the new hasGenders value.
					oldGender = null;
				}

				PawnGenerationRequest request = new PawnGenerationRequest(newPawnKindDef)
				{
					Faction = oldPawn.Faction,
					FixedGender = oldGender,
					FixedBiologicalAge = Props.carryOverAge ? oldPawn.ageTracker.AgeBiologicalYearsFloat : 0f,
					FixedChronologicalAge = Props.carryOverAge ? oldPawn.ageTracker.AgeChronologicalYearsFloat : 0f
				};

				var newPawn = PawnGenerator.GeneratePawn(request);

				if (newPawn.playerSettings != null && oldPawn.playerSettings != null)
					newPawn.playerSettings.AreaRestrictionInPawnCurrentMap =
						oldPawn.playerSettings.AreaRestrictionInPawnCurrentMap;

				return newPawn;
			}

			if (Props.evolveIntoThingDef != null)
			{
				return ThingMaker.MakeThing(Props.evolveIntoThingDef);
			}

			return null;
		}

		public override string CompInspectStringExtra()
		{
			var key = Props.inspectionStringKey;
			if (key.NullOrEmpty() || !(parent is Pawn oldPawn)) return null;
			int remainingTicks = Math.Max(0, (int) (Props.ageInDays * 60000L - oldPawn.ageTracker.AgeBiologicalTicks));
			return key.Translate(remainingTicks.ToStringTicksToPeriod(false, false, false));
		}
	}

	public class CompProperties_EvolveAtFixedAge : CompProperties
	{
		public int ageInDays;
		public ThingDef evolveIntoThingDef;
		public PawnKindDef evolveIntoPawnKindDef;
		public SoundDef evolveSound;
		public int filthAmount;
		public ThingDef filthDef;
		public bool carryOverAge;
		public string inspectionStringKey;

		public CompProperties_EvolveAtFixedAge() => compClass = typeof(CompEvolveAtFixedAge);
	}
}