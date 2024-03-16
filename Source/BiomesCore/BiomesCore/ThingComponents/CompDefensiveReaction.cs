using System.Collections.Generic;
using System.Linq;
using BiomesCore.ThingComponents;
using RimWorld;
using Verse;

namespace BiomesCore
{
	public class CompProperties_DefensiveReaction : CompProperties_DynamicAnimalGraphic
	{
		public float minPain = 0f;

		public int duration = 500;

		public List<HediffDef> hediffs;

		public CompProperties_DefensiveReaction() => compClass = typeof(CompDefensiveReaction);
	}

	public class CompDefensiveReaction : CompDynamicAnimalGraphic
	{
		private CompProperties_DefensiveReaction Props => (CompProperties_DefensiveReaction) props;

		private List<Hediff> _appliedHediffs;

		public override bool Active() => _remainingTicksActive > 0;

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
					_appliedHediffs.Add(_pawn.health.AddHediff(hediff));
				}
			}
		}

		private void EndDefensiveReaction()
		{
			if (!Active()) return;

			if (_appliedHediffs != null)
			{
				foreach (var hediff in _appliedHediffs.Where(h => _pawn.health.hediffSet.hediffs.Contains(h)))
				{
					_pawn.health.RemoveHediff(hediff);
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