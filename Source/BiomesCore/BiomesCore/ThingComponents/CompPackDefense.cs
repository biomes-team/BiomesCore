using RimWorld;
using Verse;
using Verse.AI;

namespace BiomesCore
{
    public class CompPackDefense : ThingComp
    {
        private static CompProperties_PackDefense _cacheValue;
        private static ThingDef _cacheKey;

        public static CompProperties_PackDefense GetProps(ThingDef def)
        {
            if (_cacheKey == def) return _cacheValue;
            _cacheValue = def.GetCompProperties<CompProperties_PackDefense>();
            _cacheKey = def;
            return _cacheValue;
        }
        
        public CompProperties_PackDefense Props => (CompProperties_PackDefense)props;

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            var instigatorPawn = dinfo.Instigator as Pawn;

            if (parent is Pawn pawn && 
                pawn.Spawned && !pawn.Dead && 
                dinfo.Instigator != null &&
                dinfo.Def.ExternalViolenceFor(pawn) &&
                (instigatorPawn != null || dinfo.Instigator is Building_Turret) &&
                (pawn.Faction == null || pawn.Faction != dinfo.Instigator.Faction) &&
                (pawn.CurJob == null || pawn.CurJob.def != JobDefOf.PredatorHunt || 
                 dinfo.Instigator != ((JobDriver_PredatorHunt) pawn.jobs.curDriver).Prey) &&
                Rand.Chance(GetOnDamageChance(pawn.Position.DistanceTo(dinfo.Instigator.Position), dinfo.Instigator)))
            {
                foreach (var packmate in CompPackHunter.FindPackmates(pawn, Props.joinDefenseRange, true))
                {
                    if (packmate.health.hediffSet.PainTotal <= Props.joinMaxPain)
                    {
                        var job = JobMaker.MakeJob(JobDefOf.AttackMelee, dinfo.Instigator);
                        job.maxNumMeleeAttacks = Props.maxNumMeleeAttacks;
                        job.expiryInterval = Props.expiryInterval + Rand.Range(0, 100);
                        job.attackDoorIfTargetLost = Props.attackDoorIfTargetLost;
                        job.canBashFences = Props.canBashFences;
                        packmate.jobs.StartJob(job, JobCondition.InterruptOptional);
                    }
                }
            }
        }
        
        public float GetOnDamageChance(float distance, Thing instigator)
        {
            float onDamageChance = Props.baseChance * GenMath.LerpDoubleClamped(1f, Props.attackerMaxDistance, 3f, 1f, distance);
            if (instigator is Pawn) onDamageChance *= 1f - instigator.GetStatValue(StatDefOf.HuntingStealth);
            return onDamageChance;
        }
    }
    
    public class CompProperties_PackDefense : CompProperties
    {
        public float baseChance = 0.5f;
        public float attackerMaxDistance = 30f;
        public float predatorConfidencePenalty = 100f;
        public float joinDefenseRange = 20f;
        public float joinMaxPain = 0.3f;
        public int maxNumMeleeAttacks = 1;
        public int expiryInterval = 500;
        public bool attackDoorIfTargetLost = true;
        public bool canBashFences = false;

        public CompProperties_PackDefense() => compClass = typeof(CompPackDefense);
    }
}