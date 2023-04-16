using System.Collections.Generic;
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

	public class CompSleepGraphic : CompDynamicPawnGraphic
	{
		public CompProperties_CompSleepGraphic Props => (CompProperties_CompSleepGraphic) props;

		[Unsaved] private bool wasAwake;

		public CompSleepGraphic()
		{
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			wasAwake = parent is Pawn pawn && pawn.Awake();
		}

		public override void CompTick()
		{
			base.CompTick();
			if (parent is Pawn pawn && wasAwake != pawn.Awake())
			{
				ForceGraphicUpdateNow();
				wasAwake = !wasAwake;
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