using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace BiomesCore_BiomeControl
{
    public class BiomesMap : DefModExtension
    {
        public bool isIsland = false;
        public bool hasHilliness = true;
        //public bool hasRuins = true;
        public bool hasScatterables = true;
        public float minHillEncroachment = 1;
        public float maxHillEncroachment = 1;
        public float minHillEdgeMultiplier = 1;
        public float maxHillEdgeMultiplier = 1;
        public bool isOasis = false;
    }

    public class BiomesWorldMap : DefModExtension
    {
        public bool allowOnWater = false;
        public bool allowOnLand = false;
        public float minTemperature = 0;
        public float maxTemperature = 0;
        public float minElevation = 0;
        public float maxElevation = 0;
        public float minHilliness = 0;
        public float maxHilliness = 0;
        public float minRainfall = 0;
        public float maxRainfall = 0;

    }


    //public class UniversalBiomeWorker
    //{
    //    public float GetScore(Tile tile, int tileID, Map map)
    //    {
    //        BiomesWorldMap control = map.Biome.GetModExtension<BiomesWorldMap>();
    //        if (!map.Biome.HasModExtension<BiomesWorldMap>())
    //        {
    //            return 0f;
    //        }
    //        if (control.allowOnWater == false && tile.WaterCovered)
    //        {
    //            return 0f;
    //        }
    //        if (control.allowOnLand == false && !tile.WaterCovered)
    //        {
    //            return 0f;
    //        }
    //        if (tile.temperature < control.minTemperature || tile.temperature > control.maxTemperature)
    //        {
    //            return 0f;
    //        }
    //        return 0f;
    //    }
    //}
}