using BiomesCore.DefModExtensions;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public override int SeedPart
        {
            get
            {
                return 1182952823;
            }
        }

        public override void Generate(Map map, GenStepParams parms)
        {
            if (!map.Biome.HasModExtension<BiomesMap>())
            {
                return;
            }
            if (!map.Biome.GetModExtension<BiomesMap>().isIsland)
            {
                return;
            }
            if (!map.Biome.GetModExtension<BiomesMap>().addIslandHills)
            {
                return;
            }
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
            MapGenFloatGrid fertility = MapGenerator.Fertility;

            foreach (IntVec3 current in map.AllCells)
            {
                elevation[current] = (1 + islandHillTuning * Mathf.Min(fertility[current], islandHillCenter)) * moduleBase.GetValue(current) - 0.5f;
            }
        }

    }
}