using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace BiomesCore.MapGeneration
{
    public static class BiomesMapGenUtil
    {
        // This class exists to hold values between different, related gensteps
        //OASIS
        private static IntVec3 oasisCenter = new IntVec3();
        private static float oasisBaseSize = 30;

        public static void SetOasisCenter(IntVec3 center)
        {
            oasisCenter = center;
        }

        public static IntVec3 GetOasisCenter()
        {
            return oasisCenter;
        }

        public static void SetOasisBaseSize(float size)
        {
            oasisBaseSize = size;
        }

        public static float GetOasisBaseSize()
        {
            return oasisBaseSize;
        }



        //ISLANDS
        //OASIS
        private static IntVec3 islandCenter = new IntVec3();
        private static float islandBaseSize = 30;
        private static MapGenFloatGrid islandGrid;
        public static void SetIslandCenter(IntVec3 center)
        {
            islandCenter = center;
        }
        public static IntVec3 GetIslandCenter()
        {
            return islandCenter;
        }

        public static void SetIslandBaseSize(float size)
        {
            islandBaseSize = size;
        }
        public static float GetIslandBaseSize()
        {
            return islandBaseSize;
        }

        public static void SetIslandFloatGrid(MapGenFloatGrid grid)
        {
            islandGrid = grid;
        }
        public static MapGenFloatGrid GetIslandFloatGrid()
        {
            return islandGrid;
        }
        public static void ClearIslandFloatGrid(MapGenFloatGrid grid)
        {
            islandGrid.Clear();

        }

        //GENERAL UTILITY
        public static float DistanceBetweenPoints(IntVec3 point1, IntVec3 point2)
        {
            float dist = 0;
            double xDist = Math.Pow(point1.x - point2.x, 2);
            double zDist = Math.Pow(point1.z - point2.z, 2);
            dist = (float)Math.Sqrt(xDist + zDist);

            return dist;
        }

    }
}
