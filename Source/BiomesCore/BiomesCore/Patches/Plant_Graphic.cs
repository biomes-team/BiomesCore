using Verse;
using RimWorld;
using HarmonyLib;

namespace BiomesCore.Patches
{
	[HarmonyPatch]
	internal static class Plant_Graphic
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Plant), nameof(Plant.Graphic), MethodType.Getter)]
		internal static bool ChangePlantGraphics(ref Graphic __result, Plant __instance)
		{
			var comp = __instance.GetComp<CompPlantGraphicPerBiome>();
			if (comp != null)
			{
				BiomeDef biomeDef = __instance.Map.LocalBiome(__instance.Position);
				Graphic newGraphic = comp.PerBiomeGraphic(biomeDef, __instance);
				if (newGraphic != null)
				{
					__result = newGraphic;
					return false;
				}
			}

			return true;
		}
	}
}