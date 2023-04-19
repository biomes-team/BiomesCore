using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace BiomesCore
{
    public class CompProperties_PlantProximityExplosive : CompProperties
    {
        public ThingDef proxiTarget = null;
        public ThingRequestGroup proxiGroup;
        public ThingDef postExplosionSpawnThingDef;
        public ThingDef preExplosionSpawnThingDef;
        public DamageDef explosiveDamageType;
        public DamageDef requiredDamageTypeToExplode;
        public GasType postExplosionGasType;
        public EffecterDef explosionEffect;
        public SoundDef explosionSound;
        public List<DamageDef> detonateOnDamageTaken;
        public float growthProgress = 1f;
        public float proxiRadius;
        public float minBodySize;
        public float explosiveRadius = 1.9f;
        public float growthAfterExplosion = 15f;
        public float armorPenetrationBase = -1f;
        public float postExplosionSpawnChance;
        public float preExplosionSpawnChance;
        public float chanceToStartFire;
        public float propagationSpeed = 1f;
        public int damageAmountBase = -1;
        public int postExplosionSpawnThingCount = 1;
        public int preExplosionSpawnThingCount = 1;
        public bool explodeOnKilled;
        public bool destroyedThroughDetonation;
        public bool applyDamageToExplosionCellsNeighbors;
        public bool damageFalloff;
        public bool doVisualEffects = true;

        public CompProperties_PlantProximityExplosive() => compClass = typeof(CompPlantProximityExplosive);

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            if (this.explosiveDamageType != null)
                return;
            this.explosiveDamageType = DamageDefOf.Bomb;
        }
    }

    public class CompPlantProximityExplosive : ThingComp
    {
        private List<Thing> thingsIgnoredByExplosion;
        private Thing instigator;
        private Effecter effecter;

        public CompProperties_PlantProximityExplosive Props => (CompProperties_PlantProximityExplosive)this.props;

        public override void CompTickLong()
        {
            var thingRequest = Props.proxiTarget != null
                ? ThingRequest.ForDef(Props.proxiTarget)
                : ThingRequest.ForGroup(Props.proxiGroup);

            if (parent.Map == null || !(parent is Plant plant))
                return;
            if (!this.parent.Spawned || GenClosest.ClosestThingReachable(this.parent.Position, this.parent.Map, thingRequest, PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors), Props.proxiRadius, thing => thing is Pawn pawn ? pawn.BodySize >= Props.minBodySize : true) == null)
                return;
            if (plant.Growth >= Props.growthProgress && !plant.Dying)
            {
                this.Detonate();
            }
        }

        public void AddThingsIgnoredByExplosion(List<Thing> things)
        {
            if (this.thingsIgnoredByExplosion == null)
                this.thingsIgnoredByExplosion = new List<Thing>();
            this.thingsIgnoredByExplosion.AddRange((IEnumerable<Thing>)things);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look<Thing>(ref this.instigator, "instigator");
            Scribe_Collections.Look<Thing>(ref this.thingsIgnoredByExplosion, "thingsIgnoredByExplosion", LookMode.Reference);
            //Scribe_Values.Look<bool>(ref this.destroyedThroughDetonation, "destroyedThroughDetonation");
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (mode != DestroyMode.KillFinalize || !this.Props.explodeOnKilled)
                return;
            this.Detonate();
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            absorbed = false;
            if (dinfo.Def.ExternalViolenceFor((Thing)this.parent) && (double)dinfo.Amount >= (double)this.parent.HitPoints && this.CanExplodeFromDamageType(dinfo.Def))
            {
                if (this.parent.MapHeld == null)
                    return;
                this.instigator = dinfo.Instigator;
                this.Detonate();
                if (!this.parent.Destroyed)
                    return;
                absorbed = true;
            }
            else
            {
                if (this.Props.detonateOnDamageTaken == null || !this.Props.detonateOnDamageTaken.Contains(dinfo.Def))
                    return;
                this.Detonate();
            }
        }

        protected void Detonate()
        {
            float radius = Props.explosiveRadius * Props.growthProgress;
            if ((double)radius <= 0.0)
                return;
            if (parent.Map == null || !(parent is Plant plant))
                return;
            if (Props.explosionEffect != null)
            {
                if (this.effecter == null)
                    this.effecter = this.Props.explosionEffect.Spawn((Plant)this.parent, this.parent.MapHeld);
                this.effecter.EffectTick((TargetInfo)(Plant)this.parent, (TargetInfo)(Plant)this.parent);
                this.effecter.Cleanup();
            }
            Thing instigator = this.instigator == null || this.instigator.HostileTo(this.parent.Faction) && this.parent.Faction != Faction.OfPlayer ? (Thing)this.parent : this.instigator;
            GenExplosion.DoExplosion(this.parent.PositionHeld, this.parent.MapHeld, radius, Props.explosiveDamageType, instigator, Props.damageAmountBase, Props.armorPenetrationBase, Props.explosionSound, postExplosionSpawnThingDef: Props.postExplosionSpawnThingDef, postExplosionSpawnChance: Props.postExplosionSpawnChance, postExplosionSpawnThingCount: Props.postExplosionSpawnThingCount, postExplosionGasType: this.Props.postExplosionGasType, applyDamageToExplosionCellsNeighbors: Props.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef: Props.preExplosionSpawnThingDef, preExplosionSpawnChance: Props.preExplosionSpawnChance, preExplosionSpawnThingCount: Props.preExplosionSpawnThingCount, chanceToStartFire: Props.chanceToStartFire, damageFalloff: Props.damageFalloff, ignoredThings: this.thingsIgnoredByExplosion, doVisualEffects: Props.doVisualEffects, propagationSpeed: Props.propagationSpeed);
            if (Props.destroyedThroughDetonation)
            {
                plant.Destroy();
            }
            else
            {
                plant.Growth = Props.growthAfterExplosion;
            }
        }

        private bool CanExplodeFromDamageType(DamageDef damage) => this.Props.requiredDamageTypeToExplode == null || this.Props.requiredDamageTypeToExplode == damage;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            CompPlantProximityExplosive CompPlantProximityExplosive = this;
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action commandAction = new Command_Action();
                commandAction.defaultLabel = "DEV: Explode";
                commandAction.action = new Action(CompPlantProximityExplosive.Detonate);
                yield return (Gizmo)commandAction;
            }
        }
    }
}