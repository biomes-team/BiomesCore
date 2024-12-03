using RimWorld;
using Verse;

namespace BiomesCore
{
    public class HediffCompProperties_CorpseSpawner : HediffCompProperties
    {
        public ThingDef bloodDef;

        public IntRange bloodCountRange = new IntRange(3, 6);

        public int bloodRadius = 2;

        public IntRange pawnCountRange = new IntRange(1, 1);

        public PawnKindDef pawn;

        public bool forceParentFaction = false;

        public HediffCompProperties_CorpseSpawner()
        {
            compClass = typeof(HediffComp_CorpseSpawner);
        }

        public override void ResolveReferences(HediffDef parent)
        {
            if (bloodDef == null)
            {
                bloodDef = ThingDefOf.Filth_Blood;
            }
        }
    }
}
