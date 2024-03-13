using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BiomesCore.ThingComponents
{
	public class CompProperties_DefensiveReaction : CompProperties
	{
		public float minPain = 0f;
		public int duration = 500;

		public string graphicSuffix;
		public bool useMultiGraphic;

		public List<HediffDef> hediffs;

		public CompProperties_DefensiveReaction() => compClass = typeof(CompDefensiveReaction);
	}

	public class CompDefensiveReaction : CompDynamicPawnGraphic
	{
		public CompProperties_DefensiveReaction Props => (CompProperties_DefensiveReaction) props;

		public Pawn Pawn => parent as Pawn;

		private List<Hediff> _appliedHediffs;

		// private GraphicData _cachedDefenseGraphic;
		// private PawnKindLifeStage _cachedForLifeStage;

		public override bool Active() => _remainingTicksActive > 0;

		// Assumes that PawnGraphicSet has already set its nakedGraphic value.
		public override GraphicData Graphic()
		{
			// ToDo: https://github.com/biomes-team/BiomesCore/issues/14
			/*
			var stage = Pawn.ageTracker.CurKindLifeStage;
			var original = Pawn.drawer.renderer.graphics.nakedGraphic.data;

			if (_cachedDefenseGraphic == null || _cachedForLifeStage != stage)
			{
				_cachedForLifeStage = stage;
				_cachedDefenseGraphic = new GraphicData();
				_cachedDefenseGraphic.CopyFrom(original);
				_cachedDefenseGraphic.texPath += Props.graphicSuffix;
				if (!Props.useMultiGraphic) _cachedDefenseGraphic.graphicClass = typeof(Graphic_Single);
			}

			return _cachedDefenseGraphic;
			*/
			return null;
		}

		private int _remainingTicksActive;

		public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			var pawn = parent as Pawn;
			var pawnIsSpawned = pawn != null && pawn.Spawned && !pawn.Dead;
			var pawnNotManhunter = !pawnIsSpawned || !pawn.InMentalState ||
			                       (pawn.MentalStateDef != MentalStateDefOf.Manhunter &&
			                        pawn.MentalStateDef != MentalStateDefOf.ManhunterPermanent);
			if (pawnIsSpawned && pawnNotManhunter &&
			    dinfo.Instigator != null && dinfo.Def.ExternalViolenceFor(pawn) &&
			    pawn.health.hediffSet.PainTotal >= Props.minPain)
			{
				BeginDefensiveReaction();
			}
		}

		public override void CompTick()
		{
			if (_remainingTicksActive > 0)
			{
				if (_remainingTicksActive == 1)
				{
					EndDefensiveReaction();
				}
				else
				{
					_remainingTicksActive--;
				}
			}
		}

		private void BeginDefensiveReaction()
		{
			var alreadyActive = Active();
			_remainingTicksActive = Props.duration;
			if (alreadyActive) return;

			ForceGraphicUpdateNow();

			if (Props.hediffs != null)
			{
				_appliedHediffs = new List<Hediff>();

				foreach (var hediff in Props.hediffs)
				{
					_appliedHediffs.Add(Pawn.health.AddHediff(hediff));
				}
			}
		}

		private void EndDefensiveReaction()
		{
			if (!Active()) return;

			if (_appliedHediffs != null)
			{
				foreach (var hediff in _appliedHediffs.Where(h => Pawn.health.hediffSet.hediffs.Contains(h)))
				{
					Pawn.health.RemoveHediff(hediff);
				}
			}

			_remainingTicksActive = 0;
			_appliedHediffs = null;

			ForceGraphicUpdateNow();
		}

		public override void PostExposeData()
		{
			Scribe_Values.Look(ref _remainingTicksActive, "remainingTicksActive");
		}
	}
}