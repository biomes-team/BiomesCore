using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace BiomesCore
{
    public enum TempControlType : byte
    {
        None = 0,
        Heater = 1,
        Cooler = 2,
        Both = 3
    }

    public static class ActiveTerrainUtility
    {
        public static TempControlType AnalyzeType(this CompTempControl tempControl)
        {
            float f = tempControl.Props.energyPerSecond;
            return f > 0 ? TempControlType.Heater : f < 0 ? TempControlType.Cooler : TempControlType.None;
        }

        //Terrain instance

        public static TerrainInstance MakeTerrainInstance(this ActiveTerrainDef tDef, Map map, IntVec3 loc)
        {
            var terr = (TerrainInstance)Activator.CreateInstance(tDef.terrainInstanceClass);
            terr.def = tDef;
            terr.Map = map;
            terr.Position = loc;
            return terr;
        }
    }
}
