using Verse;
using Verse.AI.Group;

namespace BiomesCore
{
    public class DeathActionWorker_ReleaseGasCloud : DeathActionWorker
    {
        public override bool DangerousInMelee => true;
        public override void PawnDied(Corpse corpse, Lord prevLord)
        {
            Pawn deceasedPawn = corpse.InnerPawn;
            ModExtension_ReleaseGasCloud modExtension = deceasedPawn.def.GetModExtension<ModExtension_ReleaseGasCloud>();
            if (modExtension == null)
            {
                Log.Error($"ModExtension_ReleaseGasCloud is needed for DeathActionWorker_ReleaseGasCloud to work on pawn {deceasedPawn.def.defName}");
                return;
            }
            if (!corpse.DestroyedOrNull())
            {
                GasUtility.AddGas(corpse.Position, corpse.Map, modExtension.gasType, modExtension.amountOfGasFloat);
            }
        }
    }
}