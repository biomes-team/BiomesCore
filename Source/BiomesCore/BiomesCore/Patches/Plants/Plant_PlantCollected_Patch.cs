using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches.Plants
{
    [HarmonyPatch(typeof(Plant), nameof(Plant.PlantCollected))]
    public class Plant_PlantCollected_Patch
    {
        public static void Postfix(Pawn by, Plant __instance)
        {
            if (__instance.def.HasModExtension<PlantHarvestMemoryExtension>())
            {
                var plantHarvestMemoryExtension =
                    __instance.def.GetModExtension<PlantHarvestMemoryExtension>();
                by?.needs?.mood?.thoughts?.memories.TryGainMemory(plantHarvestMemoryExtension
                    .memory);
            }
        }
    }
}