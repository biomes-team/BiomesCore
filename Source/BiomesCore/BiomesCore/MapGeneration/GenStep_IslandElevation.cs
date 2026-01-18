using BiomesCore.DefModExtensions;
using RimWorld;
using RimWorld.Planet;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{ 
    // based on GenStep_ElevationFertility
    // this adds island-appropriate elevation grid to island maps, which are below sea level

    public class GenStep_IslandElevation : GenStep
    {

        private float islandHillCenter = 16f;
        private float islandHillTuning = 0.1f;

        public override int SeedPart => 584864096;

        public override void Generate(Map map, GenStepParams parms)
        {
            //if (!map.Biome.HasModExtension<BiomesMap>())
            //{
            //    return;
            //}
            //if (!map.Biome.GetModExtension<BiomesMap>().isIsland)
            //{
            //    return;
            //}
            //if (!map.Biome.GetModExtension<BiomesMap>().addIslandHills)
            //{
            //    return;
            //}
            SetElevationGrid(map);
        }


        private void SetElevationGrid(Map map)
        {
            float freq = 0.025f;
            float lacun = 2.0f;

            //if(ModsConfig.IsActive("zylle.MapDesigner"))
            //{
            //    freq = 1.2f * LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().hillSize;
            //    lacun = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().hillSmoothness;
            //}

            MapGenFloatGrid islandGrid = BiomesMapGenUtil.GetIslandFloatGrid();
            float islandSize = BiomesMapGenUtil.GetIslandBaseSize();


            ModuleBase moduleBase = new Perlin(freq, lacun, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);

            moduleBase = new ScaleBias(0.5, 0.5, moduleBase);
            NoiseDebugUI.StoreNoiseRender(moduleBase, "elev base");
            float elevScaling = 1f;
            switch (map.TileInfo.hilliness)
            {
                case Hilliness.Flat:
                    elevScaling = MapGenTuning.ElevationFactorFlat;
                    break;
                case Hilliness.SmallHills:
                    elevScaling = MapGenTuning.ElevationFactorSmallHills;
                    break;
                case Hilliness.LargeHills:
                    elevScaling = MapGenTuning.ElevationFactorLargeHills;
                    break;
                case Hilliness.Mountainous:
                    elevScaling = MapGenTuning.ElevationFactorMountains;
                    break;
                case Hilliness.Impassable:
                    elevScaling = MapGenTuning.ElevationFactorImpassableMountains;
                    break;
            }

            // Exagerrate the difference between different hilliness. This is necessary because islands don't fill the whole map, and mountainous islands do not have a solid side.
            elevScaling *= elevScaling;

            moduleBase = new Multiply(moduleBase, new Const((double)elevScaling));
            NoiseDebugUI.StoreNoiseRender(moduleBase, "elev world-factored");
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            //MapGenFloatGrid fertility = MapGenerator.Fertility;
            float curDist = 0f;
            float multiplier = 1f;
            islandSize /= 2f;
            float modifier = 0.2f * islandSize;

            foreach (IntVec3 current in map.AllCells)
            {
                //elevation[current] = (1 + islandHillTuning * Mathf.Min(fertility[current], islandHillCenter)) * moduleBase.GetValue(current) - 0.5f;
                curDist = islandGrid[current];
                if (curDist < islandSize - modifier)
                {
                    multiplier = 2f;
                }
                else if (curDist > islandSize + modifier)
                {
                    multiplier = 0f;
                }
                else
                {
                    float distanceTo = curDist - (islandSize - modifier);
                    distanceTo = distanceTo / (modifier * 2);
                    multiplier = 2f - distanceTo;
                }

                elevation[current] = (1f + islandHillTuning) * multiplier * moduleBase.GetValue(current) - 0.5f;
            }
        }

    }
}