using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace BiomesCore.Patches.WorldMap
{
	/// <summary>
	/// Implements the BiomesMap.useOceanMaterial extension property.
	/// </summary>
	[HarmonyPatch(typeof(BiomeDef), nameof(BiomeDef.DrawMaterial), MethodType.Getter)]
	public class BiomeDef_DrawMaterial_Patch
	{
		public static bool Prefix(BiomeDef __instance, ref Material ___cachedMat)
		{
			if (___cachedMat == null)
			{
				BiomesMap extension = __instance.GetModExtension<BiomesMap>();
				if (extension != null && extension.useOceanMaterial)
				{
					___cachedMat = WorldMaterials.WorldOcean;
					___cachedMat.mainTexture = (Texture)ContentFinder<Texture2D>.Get(__instance.texture);
				}
			}

			return true;
		}
	}
}