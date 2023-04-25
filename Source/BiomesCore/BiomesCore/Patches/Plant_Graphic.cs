using BiomesCore.ThingComponents;
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
			foreach (var comp in __instance.AllComps)
			{
				if (comp is CompPlantGraphic plantGraphicComp && plantGraphicComp.Active())
				{
					var data = plantGraphicComp.Graphic();
					if (data != null)
					{
						__result = data;
						return false;
					}
				}
			}

			return true;
		}
	}
}