using System;
using RimWorld;
using Verse;
using Verse.Sound;

namespace BiomesCore
{
    public class CompProperties_EvolveAtFixedAge : CompProperties
    {
        public int ageInDays;
        public ThingDef evolveIntoThingDef;
        public PawnKindDef evolveIntoPawnKindDef;
        public SoundDef evolveSound;
        public int filthAmount;
        public ThingDef filthDef;
        public bool carryOverAge;
        public string inspectionStringKey;

        public CompProperties_EvolveAtFixedAge() => compClass = typeof(CompEvolveAtFixedAge);
    }

    public class CompEvolveAtFixedAge : ThingComp
    {
        public CompProperties_EvolveAtFixedAge Props => (CompProperties_EvolveAtFixedAge) props;
        
        public override void CompTickRare()
        {
            base.CompTickRare();

            if (parent.Map == null || !(parent is Pawn oldPawn)) return;
            
            if (oldPawn.ageTracker.AgeBiologicalTicks >= Props.ageInDays * 60000L)
            {
                var newThing = CreateNewThing(oldPawn);

                Props.evolveSound?.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
                
                if (Props.filthDef != null) for (int i = 0; i < Props.filthAmount; i++)
                {
                    var parms = TraverseParms.For(TraverseMode.NoPassClosedDoors);
                    if (CellFinder.TryFindRandomReachableCellNear(parent.Position, parent.Map, 2, parms, null, null, out var c))
                    {
                        FilthMaker.TryMakeFilth(c, parent.Map, Props.filthDef);
                    }
                }
                
                if (newThing != null) GenSpawn.Spawn(newThing, parent.Position, parent.Map);
                if (!parent.Destroyed) parent.Destroy();
            }
        }

        private Thing CreateNewThing(Pawn oldPawn)
        {
            if (Props.evolveIntoPawnKindDef != null)
            {
                var request = new PawnGenerationRequest(Props.evolveIntoPawnKindDef)
                {
                    Faction = oldPawn.Faction,
                    FixedGender = oldPawn.gender,
                    FixedBiologicalAge = Props.carryOverAge ? oldPawn.ageTracker.AgeBiologicalYearsFloat : 0f,
                    FixedChronologicalAge = Props.carryOverAge ? oldPawn.ageTracker.AgeChronologicalYearsFloat : 0f
                };

                var newPawn = PawnGenerator.GeneratePawn(request);
                return newPawn;
            }
            
            if (Props.evolveIntoThingDef != null)
            {
                return ThingMaker.MakeThing(Props.evolveIntoThingDef);
            }
            
            return null;
        }

        public override string CompInspectStringExtra()
        {
            var key = Props.inspectionStringKey;
            if (key.NullOrEmpty() || !(parent is Pawn oldPawn)) return null;
            int remainingTicks = Math.Max(0, (int)(Props.ageInDays * 60000L - oldPawn.ageTracker.AgeBiologicalTicks));
            return key.Translate(remainingTicks.ToStringTicksToPeriod(false, false, false));
        }
    }
}