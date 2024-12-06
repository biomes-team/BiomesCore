using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace BiomesCore
{

	public class CompProperties_HiveSpawner : CompProperties
	{
		public List<string> spawnablePawnKinds;

		public SoundDef spawnSound;

		public float defendRadius = 7f;
		public float wanderRadius = 6f;

		public int initialPawnCount;

		public int maxPawnCount = 20;

		public PawnKindDef pawnKindSpawnAfterKill;

		public FloatRange pawnSpawnIntervalDays = new FloatRange(0.5f, 2f);

		public int pawnSpawnRadius = 2;

		public string nextSpawnInspectStringKey;

		public bool spawnAsPlayerFaction = false;

		public FactionDef faction;

		public CompProperties_HiveSpawner()
		{
			compClass = typeof(CompHiveSpawner);
		}
	}

	public class CompHiveSpawner : ThingComp
	{
		public int nextPawnSpawnTick = -1;

		public bool aggressive = true;

		public bool canSpawnPawns = true;

		private CompProperties_HiveSpawner Props => (CompProperties_HiveSpawner)props;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad && nextPawnSpawnTick == -1)
			{
				SpawnInitialPawns();
			}
		}

		private void SpawnInitialPawns()
		{
			for (int i = 0; i < Props.initialPawnCount; i++)
			{
				if (!TrySpawnPawn(out var _))
				{
					break;
				}
			}
			CalculateNextPawnSpawnTick();
		}

		private void CalculateNextPawnSpawnTick()
		{
			CalculateNextPawnSpawnTick(Props.pawnSpawnIntervalDays.RandomInRange * 60000f);
		}

		public void CalculateNextPawnSpawnTick(float delayTicks)
		{
			nextPawnSpawnTick = Find.TickManager.TicksGame + (int)((double)delayTicks / (1.0 * (double)Find.Storyteller.difficulty.enemyReproductionRateFactor));
		}

		private bool TrySpawnPawn(out Pawn pawn)
		{
			int num = 0;
			foreach (string item in Props.spawnablePawnKinds.Distinct())
			{
				string text = item;
				num += parent.Map.listerThings.ThingsOfDef(ThingDef.Named(item)).Count;
			}
			if (num < Props.maxPawnCount)
			{
				PawnKindDef named = DefDatabase<PawnKindDef>.GetNamed(Props.spawnablePawnKinds.RandomElement(), errorOnFail: false);
				if (named != null)
				{
					Faction faction = ((Props.faction != null && FactionUtility.DefaultFactionFrom(Props.faction) != null) ? FactionUtility.DefaultFactionFrom(Props.faction) : null);
					PawnGenerationRequest request = new PawnGenerationRequest(named, faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: true, allowDowned: false, canGeneratePawnRelations: false, mustBeCapableOfViolence: true, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: false, allowPregnant: true, allowFood: true, allowAddictions: false);
					Pawn pawnToCreate = PawnGenerator.GeneratePawn(request);
					GenSpawn.Spawn(pawnToCreate, CellFinder.RandomClosewalkCellNear(parent.Position, parent.Map, Props.pawnSpawnRadius), parent.Map);
					if (parent.Map != null)
					{
						Lord lord = null;
						if (parent.Map.mapPawns.SpawnedPawnsInFaction(faction).Any((Pawn p) => p != pawnToCreate))
						{
							lord = ((Pawn)GenClosest.ClosestThing_Global(parent.Position, parent.Map.mapPawns.SpawnedPawnsInFaction(faction), 30f, (Thing p) => p != pawnToCreate && ((Pawn)p).GetLord() != null)).GetLord();
						}
						if (lord == null)
						{
							lord = LordMaker.MakeNewLord(faction, new LordJob_DefendPoint(parent.Position, Props.wanderRadius, defendRadius: Props.defendRadius), parent.Map);
						}
						lord.AddPawn(pawnToCreate);
					}
					pawn = pawnToCreate;
					if (Props.spawnSound != null)
					{
						Props.spawnSound.PlayOneShot(parent);
					}
					return true;
				}
				pawn = null;
				return false;
			}
			canSpawnPawns = false;
			pawn = null;
			return false;
		}

		public override void CompTick()
		{
			if (parent.Spawned && nextPawnSpawnTick == -1)
			{
				SpawnInitialPawns();
			}
			if (parent.Spawned && Find.TickManager.TicksGame >= nextPawnSpawnTick)
			{
				if (TrySpawnPawn(out var pawn) && pawn.caller != null)
				{
					pawn.caller.DoCall();
				}
				CalculateNextPawnSpawnTick();
			}
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Prefs.DevMode)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEBUG: Spawn pawn",
					icon = TexCommand.ReleaseAnimals,
					action = delegate
					{
						TrySpawnPawn(out var _);
					}
				};
			}
		}

		public override string CompInspectStringExtra()
		{
			if (!canSpawnPawns)
			{
				return "DormantHiveNotReproducing".Translate();
			}
			return canSpawnPawns ? ((string)("HiveReproducesIn".Translate() + ": " + (nextPawnSpawnTick - Find.TickManager.TicksGame).ToStringTicksToPeriod())) : null;
		}

		public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
		{
			if (Props.pawnKindSpawnAfterKill != null)
			{
				Faction faction = ((Props.faction != null && FactionUtility.DefaultFactionFrom(Props.faction) != null) ? FactionUtility.DefaultFactionFrom(Props.faction) : null);
				PawnGenerationRequest request = new PawnGenerationRequest(Props.pawnKindSpawnAfterKill, faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: true, allowDowned: false, canGeneratePawnRelations: false, mustBeCapableOfViolence: true, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: false, allowPregnant: true, allowFood: true, allowAddictions: false);
				Pawn pawnToCreate = PawnGenerator.GeneratePawn(request);
				GenSpawn.Spawn(pawnToCreate, parent.Position, prevMap);
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref nextPawnSpawnTick, "nextPawnSpawnTick", 0);
			Scribe_Values.Look(ref aggressive, "aggressive", defaultValue: false);
			Scribe_Values.Look(ref canSpawnPawns, "canSpawnPawns", defaultValue: false);
		}
	}

}
