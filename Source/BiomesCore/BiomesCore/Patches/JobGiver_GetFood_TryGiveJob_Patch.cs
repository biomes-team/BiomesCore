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
            var extension = pawn.def.GetModExtension<Biomes_AnimalControl>();
            if (extension == null)
            {
                return true;
            }

            Need_Food food = pawn.needs.food;
            if (food == null || !extension.eatWhenFed &&
                ((int)food.CurCategory < (int)___minCategory || food.CurLevelPercentage > ___maxLevelPercentage))
            {
                return true;
            }

            bool desperate = pawn.needs.food.CurCategory == HungerCategory.Starving;

            if (extension.isBloodDrinkingAnimal)
            {
                var nearestPawn = pawn.Map.mapPawns.AllPawnsSpawned
                    .Where(x => x.def != pawn.def 
                                && x.Position.DistanceTo(pawn.Position) <= 100
                                // && (!x.Position.IsForbidden(pawn) || desperate)
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
                        if (c.IsForbidden(pawn) && !desperate)
                        {
                            return false;
                        }
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
                    Thing nearestCustomThing = CustomThingEater(pawn, customThingEater, desperate);

                    if (nearestCustomThing != null)
                    {
                        __result = JobMaker.MakeJob(BiomesCoreDefOf.BC_EatCustomThing, nearestCustomThing);
                        return false;
                    }
                }
            }

            return true;
        }

        private static Thing CustomThingEater(Pawn pawn, CompCustomThingEater extension, bool desperate)
        {
            if (!extension.TryLookForFood())
            {
                return null;
            }

            Thing nearestCustomThing = null;
            bool canEatAnyFilth = extension.Props.filthNutrition > 0.0F;
            // Look-up optimized for filth.
            if (canEatAnyFilth)
            {
                var position = pawn.Position;
                int currentDistance = int.MaxValue;
                var filthList = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Filth);
                foreach (var filth in filthList)
                {
                    var filthPos = filth.Position;
                    if (filthPos.IsForbidden(pawn) && !desperate ||
                        !pawn.CanReserveAndReach(filth, PathEndMode.OnCell, Danger.Deadly))
                    {
                        continue;
                    }

                    int distance = (filth.Position - position).LengthManhattan;
                    if (currentDistance > distance)
                    {
                        nearestCustomThing = filth;
                        currentDistance = distance;
                    }
                }
            }

            if (nearestCustomThing != null || extension.Props.thingsToNutritionMapper.Count == 0)
            {
                return nearestCustomThing;
            }

            // Generic look-up for any other items. It is more costly in performance so it should be used as sparingly
            // as possible.
            var acceptableThings = extension.Props.thingsToNutrition.Keys;
            pawn.Map.floodFiller.FloodFill(pawn.Position, c => true, c =>
            {
                if (c.IsForbidden(pawn) && !desperate ||
                    !pawn.CanReach(c, PathEndMode.OnCell, Danger.Deadly))
                {
                    return false;
                }

                foreach (var thing in c.GetThingList(pawn.Map))
                {
                    if (acceptableThings.Contains(thing.def) && pawn.CanReserve(thing))
                    {
                        nearestCustomThing = thing;
                        return true;
                    }
                }

                return false;
            });

            return nearestCustomThing;
        }
    }
}
