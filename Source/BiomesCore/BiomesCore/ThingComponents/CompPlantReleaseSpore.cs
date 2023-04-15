using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BiomesCore
{
    public class CompProperties_PlantReleaseSpore : CompProperties
    {
        public string moteDef = null;
        public HediffDef hediffDef = null;
        public BodyPartDef partsToAffect = null;
        public int countToAffect = 1;
        public float baseSeverity = 1f;
        public float cellsToFill = 2f;
        public float growthProgress = 1f;

        public CompProperties_PlantReleaseSpore() => compClass = typeof(CompPlantReleaseSpore);
    }

    public class CompPlantReleaseSpore : ThingComp
    {
        public CompProperties_PlantReleaseSpore Props => (CompProperties_PlantReleaseSpore)props;

        public override void CompTickLong()
        {
            if (parent.Map == null || !(parent is Plant plant))
                return;
            if (plant.Growth >= Props.growthProgress && !plant.Dying)
            {
                ThrowPoisonSmoke();
                List<Pawn> allPawnsSpawned = plant.Map.mapPawns.AllPawnsSpawned;
                for (int pawnIndex = 0; pawnIndex < allPawnsSpawned.Count; pawnIndex++)
                {
                    Pawn pawn = allPawnsSpawned[pawnIndex];
                    if (!pawn.Position.InHorDistOf(plant.Position, Props.cellsToFill * Props.growthProgress) || !pawn.RaceProps.IsFlesh)
                    {
                        continue;
                    }
                    {
                        float sevOffset = Props.baseSeverity * plant.Growth * (1 - pawn.GetStatValue(StatDefOf.ToxicEnvironmentResistance));
                        if (sevOffset != 0f)
                        {
                            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
                            if (firstHediffOfDef != null)
                            {
                                firstHediffOfDef.Severity += sevOffset;
                            }
                            else if (sevOffset > 0f)
                            {
                                for (int i = 0; i < Props.countToAffect; i++)
                                {
                                    IEnumerable<BodyPartRecord> allParts = pawn.health.hediffSet.GetNotMissingParts();
                                    BodyPartRecord partRecord = allParts.FirstOrDefault(part => part.def == Props.partsToAffect);
                                    firstHediffOfDef = HediffMaker.MakeHediff(Props.hediffDef, pawn, partRecord);
                                    firstHediffOfDef.Severity = sevOffset;
                                    pawn.health.AddHediff(firstHediffOfDef);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public void ThrowPoisonSmoke()
        {
            if (parent.Map == null || !(parent is Plant plant))
                return;
            Vector3 spawnPosition = plant.Position.ToVector3Shifted() + Vector3Utility.RandomHorizontalOffset(3f);
            if (spawnPosition.ShouldSpawnMotesAt(plant.Map) && !plant.Map.moteCounter.SaturatedLowPriority)
            {   
                MoteThrown moteThrown = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamedSilentFail(Props.moteDef)) as MoteThrown;
                moteThrown.Scale = (Props.cellsToFill * Props.growthProgress) + 1f;
                moteThrown.rotationRate = Rand.Range(-4, 4);
                moteThrown.exactPosition = spawnPosition;
                moteThrown.SetVelocity(Rand.Range(-20, 20), 0f);
                GenSpawn.Spawn(moteThrown, spawnPosition.ToIntVec3(), plant.Map);
            }
        }
    }
}