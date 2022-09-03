using System.Linq;
using Verse;

namespace BiomesCore
{
    public class CompPassiveSelfHeal : ThingComp
    {
        public CompProperties_PassiveSelfHeal Props => (CompProperties_PassiveSelfHeal)props;

        private int _cooldownTicks;

        public override void CompTick()
        {
            _cooldownTicks--;

            if (_cooldownTicks <= 0)
            {
                _cooldownTicks = Props.cooldown;

                if (!(parent is Pawn self)) return;

                var tendable = self.health.hediffSet.GetInjuriesTendable()
                    .Where(t => !Props.onlyHealBleeding || t.Bleeding)
                    .ToList();

                if (tendable.Any())
                {
                    var injury = tendable.RandomElement();
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