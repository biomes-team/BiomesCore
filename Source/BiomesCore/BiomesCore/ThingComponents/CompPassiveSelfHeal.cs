using System;
using System.Collections.Generic;
using Verse;

namespace BiomesCore
{
	public class CompPassiveSelfHeal : ThingComp
	{
		public CompProperties_PassiveSelfHeal Props => (CompProperties_PassiveSelfHeal) props;

		private int _cooldownTicks;

		public override void CompTick()
		{
			_cooldownTicks--;

			if (_cooldownTicks <= 0)
			{
				_cooldownTicks = Props.cooldown;

				if (!(parent is Pawn self)) return;
				List<Hediff_Injury> injuries = new List<Hediff_Injury>();
				self.health.hediffSet.GetHediffs(ref injuries,
					hediffInjury => (hediffInjury.CanHealNaturally() || hediffInjury.CanHealFromTending()) &&
					                (!Props.onlyHealBleeding || hediffInjury.Bleeding));

				if (injuries.TryRandomElement(out Hediff_Injury injury))
				{
					injury.Heal(Props.healFactor);
				}
			}
		}
	}

	public class CompProperties_PassiveSelfHeal : CompProperties
	{
		public int cooldown;
		public int healFactor;
		public bool onlyHealBleeding;

		public CompProperties_PassiveSelfHeal() => compClass = typeof(CompPassiveSelfHeal);
	}
}