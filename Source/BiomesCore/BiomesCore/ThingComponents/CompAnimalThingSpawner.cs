﻿using RimWorld;
using System.Collections.Generic;
using Verse;

namespace BiomesCore.ThingComponents
{
	public class CompProperties_AnimalThingSpawner : Verse.CompProperties
	{
		public ThingDef thingToSpawn;

		public int spawnCount = 1;

		public IntRange spawnIntervalRange = new IntRange(100, 100);

		public string saveKeysPrefix;

		public CompProperties_AnimalThingSpawner()
		{
			compClass = typeof(CompAnimalThingSpawner);
		}
	}


	class CompAnimalThingSpawner : ThingComp
	{
		private int ticksUntilSpawn;

		private CompProperties_AnimalThingSpawner PropsSpawner => (CompProperties_AnimalThingSpawner) props;

		private Pawn parentPawn;

		private CompActivity compActivity;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			parentPawn = parent as Pawn;
			if (parentPawn == null)
			{
				Log.Warning($"CompAnimalThingSpawner is only allowed in pawns, but it has been assigned to a {parent}");
			}

			compActivity = parentPawn?.GetComp<CompActivity>();

			if (!respawningAfterLoad)
			{
				ResetCountdown();
			}
		}

		public override void CompTick()
		{
			TickInterval(1);
		}

		public override void CompTickRare()
		{
			TickInterval(250);
		}

		private bool ThingSpawningDormant()
		{
			return !parent.Spawned || parent.Position.Fogged(parent.Map) || parentPawn.Downed ||
			       (compActivity != null && compActivity.IsDormant);
		}

		private void TickInterval(int interval)
		{
			if (ThingSpawningDormant())
			{
				return;
			}

			ticksUntilSpawn -= interval;
			CheckShouldSpawn();
		}

		private void CheckShouldSpawn()
		{
			if (ticksUntilSpawn <= 0)
			{
				ResetCountdown();
				TryDoSpawn();
			}
		}

		public bool TryDoSpawn()
		{
			if (!parent.Spawned)
			{
				return false;
			}

			if (TryFindSpawnCell(parent, PropsSpawner.thingToSpawn, PropsSpawner.spawnCount, out var result))
			{
				Thing thing = ThingMaker.MakeThing(PropsSpawner.thingToSpawn);
				if (thing == null)
				{
					Log.Error("Could not spawn anything for " + parent);
					return false;
				}

				thing.stackCount = PropsSpawner.spawnCount;

				GenPlace.TryPlaceThing(thing, result, parent.Map, ThingPlaceMode.Direct, out var lastResultingThing);


				if (parentPawn != null && parentPawn.Faction?.IsPlayer != true)
				{
					if (!parent.Map.areaManager.Home[parentPawn.Position])
					{
						lastResultingThing.SetForbidden(value: true);
					}
				}

				return true;
			}

			return false;
		}

		public static bool TryFindSpawnCell(Thing parent, ThingDef thingToSpawn, int spawnCount, out IntVec3 result)
		{
			foreach (IntVec3 item in GenAdj.CellsAdjacent8Way(parent).InRandomOrder())
			{
				if (!item.Walkable(parent.Map))
				{
					continue;
				}

				Building edifice = item.GetEdifice(parent.Map);
				if (edifice != null && thingToSpawn.IsEdifice())
				{
					continue;
				}

				Building_Door building_Door = edifice as Building_Door;
				if ((building_Door != null && !building_Door.FreePassage) ||
				    (parent.def.passability != Traversability.Impassable &&
				     !GenSight.LineOfSight(parent.Position, item, parent.Map)))
				{
					continue;
				}

				bool flag = false;
				List<Thing> thingList = item.GetThingList(parent.Map);
				foreach (var thing in thingList)
				{
					if (thing.def.category == ThingCategory.Item &&
					    (thing.def != thingToSpawn || thing.stackCount > thingToSpawn.stackLimit - spawnCount))
					{
						flag = true;
						break;
					}
				}

				if (!flag)
				{
					result = item;
					return true;
				}
			}

			result = IntVec3.Invalid;
			return false;
		}

		private void ResetCountdown()
		{
			ticksUntilSpawn = PropsSpawner.spawnIntervalRange.RandomInRange;
		}

		public override string CompInspectStringExtra()
		{
			return ThingSpawningDormant()
				? null
				: "BMT_AnimalThingSpawner".Translate(PropsSpawner.thingToSpawn.label, ticksUntilSpawn.ToStringTicksToPeriod());
		}

		public override void PostExposeData()
		{
			string text = (PropsSpawner.saveKeysPrefix.NullOrEmpty() ? null : (PropsSpawner.saveKeysPrefix + "_"));
			Scribe_Values.Look(ref ticksUntilSpawn, text + "ticksUntilSpawn", 0);
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Prefs.DevMode)
			{
				Command_Action command_Action = new Command_Action();
				command_Action.defaultLabel = "DEBUG: Spawn " + PropsSpawner.thingToSpawn.label;
				command_Action.icon = TexCommand.DesirePower;
				command_Action.action = delegate
				{
					ticksUntilSpawn = -1;
					CheckShouldSpawn();
				};
				yield return command_Action;
			}
		}
	}
}