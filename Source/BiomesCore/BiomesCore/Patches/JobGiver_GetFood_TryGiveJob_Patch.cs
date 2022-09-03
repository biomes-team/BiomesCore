using BiomesCore.DefModExtensions;
using HarmonyLib;
using System.Linq;
using Verse;
using RimWorld;
using Verse.AI;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(JobGiver_GetFood), "TryGiveJob")]
    public static class JobGiver_GetFood_TryGiveJob_Patch
    {
        public static bool Prefix(ref Job __result, Pawn pawn, HungerCategory ___minCategory, float ___maxLevelPercentage)
        {
            Need_Food food = pawn.needs.food;
            if (food == null || (int)food.CurCategory < (int)___minCategory || food.CurLevelPercentage > ___maxLevelPercentage)
            {
                return true;
            }
            
            var extension = pawn.def.GetModExtension<Biomes_AnimalControl>();
            if (extension != null)
            {
                if (extension.isBloodDrinkingAnimal)
                {
                    var nearestPawn = pawn.Map.mapPawns.AllPawnsSpawned
                        .Where(x => x.def != pawn.def 
                                    && x.Position.DistanceTo(pawn.Position) <= 100
                                    && x.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss) is null
                                    && pawn.CanReserveAndReach(x, PathEndMode.Touch, Danger.Deadly))
                        .OrderBy(x => x.Position.DistanceTo(pawn.Position)).FirstOrDefault();
                    
                    if (nearestPawn != null)
                    {
                        __result = JobMaker.MakeJob(BiomesCoreDefOf.BC_BloodDrinking, nearestPawn);
                        return false;
                    }
                }

                if (extension.isBottomFeeder)
                {
                    var bottomFeeder = pawn.GetComp<CompBottomFeeder>();
                    if (bottomFeeder != null)
                    {
                        var nearestCell = IntVec3.Invalid;
                        pawn.Map.floodFiller.FloodFill(pawn.Position, c => true, c =>
                        {
                            if (!c.GetTerrain(pawn.Map).HasTag(bottomFeeder.Props.feedingTerrainTag)) return false;
                            if (!pawn.CanReserveAndReach(c, PathEndMode.OnCell, Danger.Deadly)) return false;
                            nearestCell = c;
                            return true;
                        });
                        
                        if (nearestCell.IsValid)
                        {
                            __result = JobMaker.MakeJob(BiomesCoreDefOf.BC_BottomFeeder, nearestCell);
                            return false;
                        }
                    }
                }
                
                if (extension.isCustomThingEater)
                {
                    var customThingEater = pawn.GetComp<CompCustomThingEater>();
                    if (customThingEater != null)
                    {
                        var nearestCell = IntVec3.Invalid;
                        var acceptableThings = customThingEater.Props.thingsToNutrition.Keys;
                        pawn.Map.floodFiller.FloodFill(pawn.Position, c => true, c =>
                        {
                            if (!c.GetThingList(pawn.Map).Any(t => acceptableThings.Contains(t.def))) return false;
                            if (!pawn.CanReserveAndReach(c, PathEndMode.OnCell, Danger.Deadly)) return false;
                            nearestCell = c;
                            return true;
                        });
                        
                        if (nearestCell.IsValid)
                        {
                            var thing = nearestCell.GetThingList(pawn.Map).First(t => acceptableThings.Contains(t.def));
                            __result = JobMaker.MakeJob(BiomesCoreDefOf.BC_EatCustomThing, thing);
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
