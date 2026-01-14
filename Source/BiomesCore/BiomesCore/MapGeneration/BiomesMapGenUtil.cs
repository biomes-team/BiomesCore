using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCore.MapGeneration
{
    public static class BiomesMapGenUtil
    {
        // This class exists to hold values between different, related gensteps
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
