using Verse;
using RimWorld;
using HarmonyLib;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(Plant), "Graphic", MethodType.Getter)]
    internal static class Plant_Graphic
    {
        internal static Graphic Postfix(Graphic originalResult, Plant __instance)
        {
            var extension = __instance.def.GetModExtension<DefModExtensions.Plant_GraphicPerBiome>();
            if (extension != null)
            {
                BiomeDef biome = __instance.Map.LocalBiome(__instance.Position);
                if (__instance.LifeStage == PlantLifeStage.Sowing)
                {
                    return extension.SowingGraphicPerBiome(biome);
                }
                if (__instance.def.plant.leaflessGraphic != null && __instance.LeaflessNow && (!__instance.sown || !__instance.HarvestableNow))
                {
                    return extension.LeaflessGraphicPerBiome(biome);
                }
                if (__instance.def.plant.immatureGraphic != null && !__instance.HarvestableNow)
                {
                    return extension.ImmatureGraphicPerBiome(biome);
                }
                return extension.GraphicForBiome(biome);
            }
            return originalResult;
        }
    }
}