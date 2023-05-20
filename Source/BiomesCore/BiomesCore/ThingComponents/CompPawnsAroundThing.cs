using System.Collections.Generic;
using BiomesCore.LordJobs;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace BiomesCore.ThingComponents
{
	/// <summary>
	/// Things with this comp will spawn a specific pawn type. The pawn will wander around the thing until it is destroyed.
	/// </summary>
	public class CompProperties_CompAnimalsAroundThing : CompProperties
	{
		public PawnKindDef pawnDef;

		public IntRange pawnCount;

		public FloatRange pawnSpawnIntervalDays;

		public float wanderRadius = 10.0F;

		public SoundDef spawnSound;

		public CompProperties_CompAnimalsAroundThing() => compClass = typeof(CompAnimalsAroundThing);

		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			foreach (var line in base.ConfigErrors(parentDef))
			{
				yield return line;
			}

			if (pawnDef == null)
			{
				yield return "CompProperties_CompAnimalsAroundThing must have a valid pawnDef";
			}

			if (pawnCount.min < 0 || pawnCount.max < pawnCount.min)
			{
				yield return "CompProperties_CompAnimalsAroundThing has an invalid pawnCount interval";
			}

			if (pawnSpawnIntervalDays.min <= 0.0F || pawnSpawnIntervalDays.max < pawnSpawnIntervalDays.min)
			{
				yield return "CompProperties_CompAnimalsAroundThing has an invalid pawnCount pawnSpawnIntervalDays";
			}
		}
	}

	public class CompAnimalsAroundThing : ThingComp
	{
		private const int SpawnRadius = 2;

		private int nextSpawnTick = -1;

		private List<Pawn> pawnsAround = new List<Pawn>();

		private CompCanBeDormant dormancyCompCached;

		private CompProperties_CompAnimalsAroundThing Props => (CompProperties_CompAnimalsAroundThing) props;

		private Lord lord;

		private CompCanBeDormant DormancyComp => dormancyCompCached ??= parent.TryGetComp<CompCanBeDormant>();

		private bool Dormant => DormancyComp != null && !DormancyComp.Awake;

		public CompAnimalsAroundThing()
		{
		}

		private bool TrySpawnPawn(bool doCall)
		{
			nextSpawnTick =
				(int) (Find.TickManager.TicksGame + Props.pawnSpawnIntervalDays.RandomInRange * GenDate.TicksPerDay);

			var pawnDef = Props.pawnDef;
			PawnGenerationRequest request = new PawnGenerationRequest(Props.pawnDef);
			int index = pawnDef.lifeStages.Count - 1;
			request.FixedBiologicalAge = pawnDef.race.race.lifeStageAges[index].minAge;
			Pawn pawn = PawnGenerator.GeneratePawn(request);
			if (pawn == null)
			{
				return false;
			}

			Thing pawnThing = GenSpawn.Spawn(pawn,
				CellFinder.RandomClosewalkCellNear(parent.Position, parent.Map, SpawnRadius), parent.Map);
			if (pawnThing == null)
			{
				return false;
			}

			Props.spawnSound?.PlayOneShot(parent);

			// The lord might get destroyed at any time in which it has no pawns.
			if (lord == null || !lord.AnyActivePawn)
			{
				lord = LordMaker.MakeNewLord(null, new WanderAroundThing(parent), parent.Map);
			}

			lord.AddPawn(pawn);
			pawnsAround.Add(pawn);

			if (doCall && pawn.caller != null)
			{
				pawn.caller.DoCall();
			}

			return true;
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			while (pawnsAround.Count < Props.pawnCount.min && TrySpawnPawn(false))
			{
			}
		}

		public override void CompTick()
		{
			if (Dormant || !parent.Spawned || parent.Fogged() || Find.TickManager.TicksGame < nextSpawnTick)
			{
				return;
			}

			for (int index = pawnsAround.Count - 1; index >= 0; --index)
			{
				if (!pawnsAround[index].Spawned)
				{
					pawnsAround.RemoveAt(index);
				}
			}

			if (pawnsAround.Count < Props.pawnCount.max)
			{
				TrySpawnPawn(true);
			}
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = $"Spawn {Props.pawnDef.LabelCap}",
					icon = TexCommand.ReleaseAnimals,
					action = () => TrySpawnPawn(true)
				};
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref nextSpawnTick, "nextSpawnTick");
			Scribe_Collections.Look(ref pawnsAround, "pawnsAround", LookMode.Reference);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				pawnsAround.RemoveAll(x => x == null);
			}
		}
	}
}