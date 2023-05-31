using BiomesCore.DefModExtensions;
using HarmonyLib;
using Verse;
using Verse.Noise;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(RockNoises), "Init")]
    internal static class RockNoises_Init
    {
        static void Postfix()
        {
            foreach (var rockNoise in RockNoises.rockNoises)
            {
                var config = rockNoise.rockDef.GetModExtension<RockNoiseConfig>();
                if (config != null)
                {
                    if (rockNoise.noise is Perlin original)
                    {
                        var perlin = new Perlin(
                            config.frequency, 
                            original.Lacunarity, 
                            original.Persistence, 
                            original.OctaveCount, 
                            original.Seed, 
                            original.Quality
                        );

                        rockNoise.noise = new Add(perlin, new Const(config.offset));
                    }
                }
            }
        }
    }
}
