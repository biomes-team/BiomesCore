using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace BiomesCore
{
    public class IslandMap : DefModExtension
    {
        public bool isIsland = false;
        public bool hasHilliness = true;
        //public bool hasRuins = true;
        public bool hasScatterables = true;
    }

    public class ValleyMap : DefModExtension
    {
        public float minHillEncroachment = 1;
        public float maxHillEncroachment = 1;
        public float minHillEdgeMultiplier = 1;
        public float maxHillEdgeMultiplier = 1;
        public bool isOasis = false;
    }

}