using System.Collections.Generic;
using BiomesCore.Patches;
using RimWorld;
using Verse;

namespace BiomesCore.ThingComponents
{
	public class CompProperties_CompSleepGraphic : CompProperties
	{
		/// <summary>
		/// Alternate graphic data that will be used for sleeping.
		/// </summary>
		public List<GraphicData> sleepGraphicData = new List<GraphicData>();

		/// <summary>
		/// If this is set to true, the alternate graphic will only be used if the pawn is sleeping under a roof.
		/// </summary>
		public bool roofedOnly;

		/// <summary>
		/// Force the use of the south texture while sleeping. Conflicts with animal pack textures.
		/// </summary>
		public bool faceSouth;

		public CompProperties_CompSleepGraphic() => compClass = typeof(CompSleepGraphic);

		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			foreach (var line in base.ConfigErrors(parentDef))
			{
				yield return line;
			}

			if (sleepGraphicData.NullOrEmpty())
			{
				yield return $"{GetType().Name} must have a sleepGraphicData.";
				yield break;
			}

			if (parentDef.race == null)
			{
				yield return $"{GetType().Name} can only be applied to pawns.";
				yield break;
			}

			if (parentDef.race.lifeStageAges.Count != sleepGraphicData.Count)
			{
				yield return
					$"{GetType().Name} {parentDef.defName} has {parentDef.race.lifeStageAges.Count} life stages but only {sleepGraphicData.Count} sleepGraphicData instances were provided.";
			}

			foreach (var data in sleepGraphicData)
			{
				if (data.texPath.NullOrEmpty())
				{
					yield return $"{GetType().Name}: sleepGraphicData has a null or empty texPath.";
				}
			}
		}

		public override void PostLoadSpecial(ThingDef parentDef)
		{
			LongEventHandler.ExecuteWhenFinished(() =>
			{
				foreach (var data in sleepGraphicData)
				{
					data.graphicClass ??= typeof(Graphic_Multi);
					data.ExplicitlyInitCachedGraphic();
				}
			});
		}
	}

	// ToDo: https://github.com/biomes-team/BiomesCore/issues/14
	public class CompSleepGraphic : CompDynamicPawnGraphic
	{
		public CompProperties_CompSleepGraphic Props => (CompProperties_CompSleepGraphic) props;

		// [Unsaved] private bool wasAwake;

		public CompSleepGraphic()
		{
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!(parent is Pawn pawn))
			{
				return;
			}

			/*
			wasAwake = pawn.Awake();
			if (wasAwake)
			{
				SleepingPawnAngle.OverrideAngle.Remove(pawn);
				SleepingPawnLayFacing.OverrideDirection.Remove(pawn);
			}
			else
			{
				SleepingPawnLayFacing.OverrideDirection.Add(pawn);
				if (Props.faceSouth)
				{
					SleepingPawnAngle.OverrideAngle[pawn] = 0.0F;
				}
			}
			*/
		}

		public override void CompTick()
		{
			base.CompTick();
			if (!(parent is Pawn pawn))
			{
				return;
			}

			/*
			var isAwake = pawn.Awake();
			if (wasAwake == isAwake)
			{
				return;
			}

			// Force an update of the PawnGraphicSet.
			pawn.drawer.renderer.graphics.nakedGraphic = null;
			wasAwake = !wasAwake;
			if (isAwake)
			{
				SleepingPawnAngle.OverrideAngle.Remove(pawn);
				SleepingPawnLayFacing.OverrideDirection.Remove(pawn);
			}
			else
			{
				SleepingPawnLayFacing.OverrideDirection.Add(pawn);
				if (Props.faceSouth)
				{
					SleepingPawnAngle.OverrideAngle[pawn] = 0.0F;
				}
			}
			*/
		}

		public override void PostDeSpawn(Map map)
		{
			base.PostDeSpawn(map);
			if (parent is Pawn pawn)
			{
				/*
				SleepingPawnAngle.OverrideAngle.Remove(pawn);
				SleepingPawnLayFacing.OverrideDirection.Remove(pawn);
				*/
			}
		}

		public override bool Active()
		{
			return parent is Pawn pawn && !pawn.Dead && !pawn.Awake() &&
			       (!Props.roofedOnly || pawn.Position.Roofed(pawn.Map));
		}

		public override GraphicData Graphic()
		{
			if (parent is Pawn pawn)
			{
				var lifeStage = pawn.ageTracker.CurLifeStageIndex;
				return Active() && lifeStage < Props.sleepGraphicData.Count ? Props.sleepGraphicData[lifeStage] : null;
			}

			return null;
		}
	}
}