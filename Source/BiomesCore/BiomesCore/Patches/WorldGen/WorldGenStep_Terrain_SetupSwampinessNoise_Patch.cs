using BiomesCore.Planet;
using HarmonyLib;
using RimWorld.Planet;
using Verse.Noise;

namespace BiomesCore.Patches.WorldGen
{
	/// <summary>
	/// Initialize the Biomes! additional world generation data.
	/// This is done after the SetupSwampinessNoise because that is the last vanilla noise used during world gen.
	/// </summary>
	[HarmonyPatch(typeof(WorldGenStep_Terrain), "SetupSwampinessNoise")]
	public static class WorldGenStep_Terrain_SetupSwampinessNoise_Patch
	{
		public static void Postfix(ModuleBase ___noiseElevation)
		{
			WorldGenInfoHandler.Setup(___noiseElevation);
		}
	}
}