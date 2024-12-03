using RimWorld;
using Verse;

namespace BiomesCore
{
    public class HediffComp_CorpseSpawner : HediffComp
    {
        public HediffCompProperties_CorpseSpawner Props => (HediffCompProperties_CorpseSpawner)props;
        public override void Notify_PawnKilled()
        {
            int splatterCount = Props.bloodCountRange.RandomInRange;

            for (int i = 0; i < splatterCount; i++)
            {
                FilthMaker.TryMakeFilth(CellFinder.RandomClosewalkCellNear(Pawn.Position, Pawn.Map, Props.bloodRadius), Pawn.Map, Props.bloodDef, Pawn.LabelIndefinite());
            }

            Faction faction = Props.forceParentFaction == true ? Pawn.Faction : null;

            PawnGenerationRequest request = new PawnGenerationRequest(Props.pawn, faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, allowDead: false, allowDowned: true, canGeneratePawnRelations: false, developmentalStages: DevelopmentalStage.Newborn);
            request.DontGivePreArrivalPathway = true;

            int pawnCount = Props.pawnCountRange.RandomInRange;
            for (int i = 0; i < pawnCount; i++)
            {
                Pawn newPawn = PawnGenerator.GeneratePawn(request);

                PawnUtility.TrySpawnHatchedOrBornPawn(newPawn, Pawn);
            }
        }
    }
}
