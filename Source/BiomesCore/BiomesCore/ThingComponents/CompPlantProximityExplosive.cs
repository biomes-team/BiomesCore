using RimWorld;
using System.Collections.Generic;
using RimWorld.Planet;
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
            if (explosiveDamageType != null)
                return;
            explosiveDamageType = DamageDefOf.Bomb;
        }
    }

    public class CompPlantProximityExplosive : ThingComp
    {
        private List<Thing> thingsIgnoredByExplosion;
        private Thing instigator;
        private Effecter effecter;
        private bool plantedByMapFaction;

        private static int _lastExplodedTick = -1;
        private static bool _lastExplodedPlantedByMapFaction;

        public CompProperties_PlantProximityExplosive Props => (CompProperties_PlantProximityExplosive)props;
        
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (respawningAfterLoad || Current.ProgramState != ProgramState.Playing || parent.Map.ParentFaction == null) return;

            // propagate plantedByMapFaction to new shrooms produced in explosions
            if (_lastExplodedTick > 0 && Find.TickManager.TicksGame - _lastExplodedTick < 200)
            {
                plantedByMapFaction = _lastExplodedPlantedByMapFaction;
                return;
            }

            // try to find pawn from the map faction that is planting the shroom
            var nearbyMapFactionPawn = GenClosest.ClosestThingReachable(parent.Position, parent.Map, 
                ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, 
                TraverseParms.For(TraverseMode.NoPassClosedDoors), Props.proxiRadius, 
                thing => thing.Faction == parent.Map.ParentFaction);
            
            plantedByMapFaction = nearbyMapFactionPawn != null;
        }

        public override void CompTick()
        {
            if (!parent.IsHashIntervalTick(100)) return;

            var thingRequest = Props.proxiTarget != null
                ? ThingRequest.ForDef(Props.proxiTarget)
                : ThingRequest.ForGroup(Props.proxiGroup);

            if (parent.Map == null || !(parent is Plant plant))
                return;
            if (plant.Growth < Props.growthProgress || plant.Dying)
                return;
            if (!parent.Spawned || GenClosest.ClosestThingReachable(parent.Position, parent.Map, thingRequest, PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors), Props.proxiRadius, CanBeTriggeredBy) == null)
                return;
            
            Detonate();
        }

        private bool CanBeTriggeredBy(Thing thing)
        {
            if (thing is Pawn pawn)
            {
                if (pawn.BodySize < Props.minBodySize) return false;
                
                if (plantedByMapFaction)
                {
                    var mapFaction = parent.Map.ParentFaction;
                    if (pawn.Faction != null && !pawn.Faction.HostileTo(mapFaction)) return false;
                    if (pawn.Faction == null && pawn.RaceProps.Animal && !pawn.InAggroMentalState) return false;
                    if (pawn.guest is { Released: true }) return false;
                    if (!pawn.IsPrisoner && pawn.HostFaction == mapFaction) return false;
                    if (pawn.RaceProps.Humanlike && pawn.IsFormingCaravan()) return false;
                    if (pawn.IsPrisoner && pawn.guest.ShouldWaitInsteadOfEscaping && mapFaction == pawn.HostFaction) return false;
                    if (pawn.Faction == null && pawn.RaceProps.Humanlike) return false;
                }
                else
                {
                    if (pawn.RaceProps.Animal && !pawn.InAggroMentalState) return false;
                }
            }
            
            return true;
        }

        public void AddThingsIgnoredByExplosion(List<Thing> things)
        {
            thingsIgnoredByExplosion ??= new List<Thing>();
            thingsIgnoredByExplosion.AddRange(things);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref plantedByMapFaction, "plantedByMapFaction");
            Scribe_References.Look(ref instigator, "instigator");
            Scribe_Collections.Look(ref thingsIgnoredByExplosion, "thingsIgnoredByExplosion", LookMode.Reference);
            //Scribe_Values.Look<bool>(ref this.destroyedThroughDetonation, "destroyedThroughDetonation");
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (mode != DestroyMode.KillFinalize || !Props.explodeOnKilled)
                return;
            Detonate();
        }

        public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            absorbed = false;
            if (dinfo.Def.ExternalViolenceFor(parent) && dinfo.Amount >= (double)parent.HitPoints && CanExplodeFromDamageType(dinfo.Def))
            {
                if (parent.MapHeld == null)
                    return;
                instigator = dinfo.Instigator;
                Detonate();
                if (!parent.Destroyed)
                    return;
                absorbed = true;
            }
            else
            {
                if (Props.detonateOnDamageTaken == null || !Props.detonateOnDamageTaken.Contains(dinfo.Def))
                    return;
                Detonate();
            }
        }

        protected void Detonate()
        {
            float radius = Props.explosiveRadius * Props.growthProgress;
            if (radius <= 0.0)
                return;
            if (parent.Map == null || !(parent is Plant plant))
                return;
            
            if (Props.explosionEffect != null)
            {
                effecter ??= Props.explosionEffect.Spawn((Plant) parent, parent.MapHeld);
                effecter.EffectTick((TargetInfo)(Plant)parent, (TargetInfo)(Plant)parent);
                effecter.Cleanup();
            }
            
            var explosionInstigator = instigator == null || instigator.HostileTo(parent.Faction) && parent.Faction != Faction.OfPlayer ? parent : instigator;
            GenExplosion.DoExplosion(parent.PositionHeld, parent.MapHeld, radius, Props.explosiveDamageType, explosionInstigator, Props.damageAmountBase, Props.armorPenetrationBase, Props.explosionSound, postExplosionSpawnThingDef: Props.postExplosionSpawnThingDef, postExplosionSpawnChance: Props.postExplosionSpawnChance, postExplosionSpawnThingCount: Props.postExplosionSpawnThingCount, postExplosionGasType: Props.postExplosionGasType, applyDamageToExplosionCellsNeighbors: Props.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef: Props.preExplosionSpawnThingDef, preExplosionSpawnChance: Props.preExplosionSpawnChance, preExplosionSpawnThingCount: Props.preExplosionSpawnThingCount, chanceToStartFire: Props.chanceToStartFire, damageFalloff: Props.damageFalloff, ignoredThings: thingsIgnoredByExplosion, doVisualEffects: Props.doVisualEffects, propagationSpeed: Props.propagationSpeed);

            _lastExplodedTick = Find.TickManager.TicksGame;
            _lastExplodedPlantedByMapFaction = plantedByMapFaction;
            
            if (Props.destroyedThroughDetonation)
            {
                plant.Destroy();
            }
            else
            {
                plant.Growth = Props.growthAfterExplosion;
            }
        }

        private bool CanExplodeFromDamageType(DamageDef damage) => Props.requiredDamageTypeToExplode == null || Props.requiredDamageTypeToExplode == damage;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            var compPlantProximityExplosive = this;
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = plantedByMapFaction ? "DEV: Explode (PMF)" : "DEV: Explode",
                    action = compPlantProximityExplosive.Detonate
                };
            }
        }
    }
}
